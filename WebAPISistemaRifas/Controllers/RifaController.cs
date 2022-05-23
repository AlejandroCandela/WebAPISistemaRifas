using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPISistemaRifas.Entidades;

namespace WebAPISistemaRifas.DTOs
{
    [ApiController]
    [Route("API/Rifas")]
    public class RifaController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;
        private readonly ILogger<RifaController> logger;
        private readonly IMapper mapper;
        //private readonly UserManager<IdentityUser> userManager;
        public RifaController(ApplicationDBContext dbcontext,
            ILogger<RifaController> logger, IMapper mapper//,
            /*UserManager<IdentityUser> userManager*/)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.dBContext = dbcontext;
            //this.userManager = userManager;
        }
        
        [HttpGet("ObtenerPremios/Rifa/{id:int}",Name = "GetRifa")]
        //[AllowAnonymous]
        public async Task<ActionResult<RifaDTOconPremios>> Get(int id)
        {
            var rifa = await dBContext.Rifas
                .Include(p => p.premios)
                .FirstOrDefaultAsync(x => x.id == id);
            logger.LogInformation("Se extrago el elemento de la base de datos");
            logger.LogInformation(rifa.ToString());
            if (rifa == null)
            {
                logger.LogInformation("No habia nada");
                return NotFound("La rifa no fue encontrada");
            }
            logger.LogInformation("Paso el if");
            return mapper.Map<RifaDTOconPremios>(rifa);
        }

        [HttpGet("ObtenerCartasDisponibles/Rifa/{id:int}")]
        public async Task<ActionResult<RifaDTOconCartas>> GetCartas(int id)
        {
            var rifa = await dBContext.Rifas
                .Include(x => x.ParticipantesCartasRifa)
                .ThenInclude(x => x.Cartas)
                .FirstOrDefaultAsync(x => x.id == id);
            logger.LogInformation("informacion extraida");
            logger.LogInformation(rifa.ToString());

            if (rifa == null)
            {
                logger.LogInformation("No habia nada");
                return NotFound("La rifa no fue encontrada");
            }

            var baraja = await dBContext.Cartas
                .ExceptBy(rifa.ParticipantesCartasRifa.Select(c =>c.Cartas)
                , x=>x).ToListAsync();

            if (baraja.Count()!=54) 
            {
                return BadRequest("Las cartas no han sido introducidas");
            }

            var rifaFaltantes = new Rifa()
            {
                id = rifa.id,
                Nombre = rifa.Nombre,
                Fecha_apertura = rifa.Fecha_apertura,
                Fecha_cierre = rifa.Fecha_cierre,
                Fecha_rifa = rifa.Fecha_rifa,
                premios = rifa.premios,
                ParticipantesCartasRifa = new List<ParticipantesCartasRifa>(){}
             };

            foreach (var i in baraja) 
            {
                rifaFaltantes.ParticipantesCartasRifa.Add(new ParticipantesCartasRifa {
                                                            IdCartas = i.id.ToString(),
                                                            IdRifa = rifaFaltantes.id.ToString(),
                                                            Cartas = i,
                                                            Rifa = rifaFaltantes 
                                                          });
            }

            logger.LogInformation("Inforacion recreada");
            logger.LogInformation(rifaFaltantes.ToString());
            return mapper.Map<RifaDTOconCartas>(rifaFaltantes);
        }

        [HttpPatch("/ModificacionesRifa/{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<RifaPatchDTO> json)
        {
            if (json == null) { return BadRequest(); }
            logger.LogInformation("Si habia algo en en json patch");
            logger.LogInformation(json.ToString());

            var Rifa = await dBContext.Rifas.AnyAsync(x => x.id == id);
            logger.LogInformation("Se extrago algo en la base de datos");
            logger.LogInformation(Rifa.ToString());

            if (!Rifa) { return BadRequest(); }
            var rifaDTO = mapper.Map<RifaPatchDTO>(Rifa);
            logger.LogInformation("Se mapeo el elemento");
            logger.LogInformation(rifaDTO.ToString());
            json.ApplyTo(rifaDTO);

            var IsValid = TryValidateModel(rifaDTO);
            if (!IsValid)
            {
                return BadRequest(ModelState);
            }
            logger.LogInformation("Se logro realizar el procedimiento de validacion");

            mapper.Map(rifaDTO, Rifa);
            logger.LogInformation("Se logro realizar el mapeo");

            await dBContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("/CreacionDeRifa")]
        public async Task<ActionResult> Post(CreacionRifaDTO rifaCreacionDTO) 
        {
            //var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            //var email = emailClaim.Value;
            //var usuario = await userManager.FindByEmailAsync(email);
            //var usuarioId = usuario.Id;

            var existe = await dBContext.Rifas.AnyAsync(x => x.Nombre == rifaCreacionDTO.Nombre);
            logger.LogInformation("Se extrago algo de la base de datos");
            logger.LogInformation(existe.ToString());
            if (existe)
            {
                return BadRequest("Ya se cuenta con una rifa con este nombre");
            }

            if (rifaCreacionDTO.premios.Count != 6) 
            {
                return BadRequest("Los premios no cuentan con la cantidad estipulada para la rifa");
            }
            for(var i = 0; i < 5; i++)
            {
                for (var j = i+1; j < 6; j++) 
                {
                    if (rifaCreacionDTO.premios[i].nivel == rifaCreacionDTO.premios[j].nivel) 
                    {
                        return BadRequest("Los niveles de los premios estan repetidos");
                    }
                }
            }

            var nuevoElemento = mapper.Map<Rifa>(rifaCreacionDTO);
            logger.LogInformation("Se realizo el mapeo");
            logger.LogInformation(nuevoElemento.ToString());
            dBContext.Add(nuevoElemento);
            foreach (var r in nuevoElemento.premios) 
            {
                dBContext.Add(r);
            }
            logger.LogInformation("Se agrego el elemento a la base de datos");
            await dBContext.SaveChangesAsync();
            var nuevaVista = mapper.Map<RifaDTOconPremios>(nuevoElemento);
            logger.LogInformation("Empezamos con el envio de la confirmacion de la creacion");
            logger.LogInformation(nuevaVista.ToString());
            return CreatedAtRoute("GetRifa", new { id = nuevoElemento.id },nuevaVista);
        }

        [HttpPost("/AsignacionDeCartas")]
        public async Task<ActionResult> PostCartas(CreacionCartasDTO cartasDTO) 
        {
            var exist = await dBContext.Cartas.AnyAsync(x => x.numero == cartasDTO.numero);
            if (exist) 
            {
                return BadRequest("Ya existe esa carta");
            }
            var nuevoElemento = mapper.Map<Cartas>(cartasDTO);
            dBContext.Add(nuevoElemento);
            logger.LogInformation("Se agrego el elemento a la base de datos");
            await dBContext.SaveChangesAsync();
            var nuevaVista = mapper.Map<CartasDTO>(nuevoElemento);
            logger.LogInformation("Empezamos con el envio de la confirmacion de la creacion");
            logger.LogInformation(nuevaVista.ToString());
            return CreatedAtRoute("GetRifa", new { id = nuevoElemento.id }, nuevaVista);
        }

        [HttpDelete("/EliminacionDeRifa/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await dBContext.Rifas.AnyAsync(x => x.id == id);
            if (!existe)
            {
                return NotFound("El Recurso no fue encontrado.");
            }
            var rifaBorrar = await dBContext.Rifas.Include(x => x.premios).FirstAsync(x => x.id == id);
            dBContext.Remove(rifaBorrar);

            await dBContext.SaveChangesAsync();
            return Ok();
        }
    }
}
