using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPISistemaRifas.Entidades;

namespace WebAPISistemaRifas.DTOs
{
    [ApiController]
    [Route("SistemaDeInscripciones/")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Administrador")]
    public class ParticipanteController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IMapper mapper;

        public ParticipanteController(ApplicationDBContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.dBContext = context;
        }

        [HttpGet("ObtenerCliente/{id:int}", Name = "ObtenerPartcipante")]
        public async Task<ActionResult<ParticipanteDTO>> Get(int id) 
        {
            var participante = await dBContext.Participantes.FirstOrDefaultAsync(x => x.Id == id);
            var usuario = await dBContext.Users.FirstAsync(x => x.Id == participante.UserId);
            if (usuario == null) 
            {
                return NotFound("El ususario relacionado al particpante no fue encontrado");
            }
            if (participante == null) 
            {
                return BadRequest("El participante no fue encontrado");
            }
            var participanteDTO =  mapper.Map<ParticipanteDTO>(participante);
            /*participanteDTO.Nombres = usuario.UserName;
            participanteDTO.email = usuario.Email;*/
            return participanteDTO;
        }

        [HttpPost("DarDeAltaParticipante")]
        public async Task<ActionResult> Post(CreacionParticipanteDTO creacionParticipanteDTO) 
        {
            var existPar = await dBContext.Participantes.AnyAsync(x=> x.tarjeta_credito == creacionParticipanteDTO.tarjeta_credito);
            if (existPar)
            {
                return BadRequest("El participante ya existe");
            }
            /*var existUser = await dBContext.Users.AnyAsync(x => x.Id == id.ToString());
            if (existUser) 
            {
                return BadRequest("El usuario no puede tener mas que un solo participante");
            }*/
            var nuevoElemento = mapper.Map<Participantes>(creacionParticipanteDTO);
            dBContext.Add(nuevoElemento);
            var nuevaVista = mapper.Map<ParticipanteDTO>(nuevoElemento);
            await dBContext.SaveChangesAsync();
            return CreatedAtRoute("ObtenerPartcipante", new { id = nuevoElemento.Id }, nuevaVista);
        }

        [HttpGet("ObtenerInscripcionesParticipante/{id:int}",Name ="ObtenerParticipacion")]
        public async Task<ActionResult> GetParticipacion(int id) 
        {
            var inscripciones = await dBContext.ParticipantesCartasRifa.Where(x => x.IdParticipantes == id.ToString()).ToListAsync();
            var salida = new ParticipanteCartasRifaDTOSalida();
            foreach (var i in inscripciones)
            { 
                salida.cartasDTO.Add(Convert.ToInt32(i.IdCartas));
                salida.rifaDTO.Add(Convert.ToInt32(i.IdRifa));
            }
            return Ok(salida);
        }

        [HttpPost("inscripcionRifa/{idRifa:int}/Participante/{idParticipante:int}/Carta/{idCarta:int}")]
        public async Task<ActionResult> PostInscripcion(int idRifa, int idParticipante, int idCarta) 
        {
            var elementoRifa = await dBContext.Rifas.AnyAsync(x=>x.id == idRifa);
            if (!elementoRifa) 
            {
                return NotFound("No se encontro la rifa undicada");
            }

            var elementoParticipante = await dBContext.Participantes.AnyAsync(x => x.Id == idParticipante);
            if (!elementoParticipante) 
            {
                return NotFound("No se encontro el participante indicado");
            }

            var elementoCartas = await dBContext.Cartas.AnyAsync(x => x.id == idCarta);
            if (!elementoCartas)
            {
                return NotFound("No el id de la carta no es indicado");
            }

            try{
                var elemento = await dBContext.ParticipantesCartasRifa.SingleAsync(x => x.IdRifa == idRifa.ToString() && x.IdParticipantes == idParticipante.ToString());
                return BadRequest("El participante ya se encuentra participando en esta rifa");
            }
            catch (Exception ex) {
                var cantidadDePraticipantes = await dBContext.ParticipantesCartasRifa.Where(x => x.IdRifa == idRifa.ToString()).ToListAsync();
               
                if (!(cantidadDePraticipantes.Count()<=54)) 
                {
                    return BadRequest("La rifa ya esta llena");
                }

                var validacionCarta = await dBContext.ParticipantesCartasRifa.AnyAsync(x => x.IdRifa == idRifa.ToString() && x.IdCartas == idCarta.ToString());
                if (validacionCarta) 
                {
                    return BadRequest("La carta que se ingreso ya esta en juego en la rifa indicada");
                }
                var carta = await dBContext.Cartas.FirstAsync(x=> x.id == idCarta);
                var rifa = await dBContext.Rifas.FirstAsync(x => x.id == idRifa);
                var participante = await dBContext.Participantes.FirstAsync(x => x.Id == idParticipante);
                var nuevoElemento = new ParticipantesCartasRifa() {
                                                                IdCartas = carta.id.ToString(),
                                                                IdParticipantes = participante.Id.ToString(),
                                                                IdRifa = rifa.id.ToString(),
                                                                Cartas = carta,  
                                                                Participantes = participante,
                                                                Rifa = rifa,
                                                              };
                dBContext.Add(nuevoElemento);
                var nuevaVista = mapper.Map<ParticipantesCartasRifaDTO>(nuevoElemento);
                await dBContext.SaveChangesAsync();
                return CreatedAtRoute("ObtenerParticipacion", new { id = Convert.ToInt32(nuevaVista.ParticipanteDTO)}, nuevaVista);
            }

            
        }

        [HttpPut("ModificarParticipante/{id:int}")]
        public async Task<ActionResult> Put(CreacionParticipanteDTO creacionParticipanteDTO, int id) 
        {
            var exist = await dBContext.Participantes.AnyAsync(x=> x.Id == id);
            if (!exist) {
                return BadRequest("El elemento no existe");
            }
            var participante = mapper.Map<Rifa>(creacionParticipanteDTO);
            participante.id = id;
            dBContext.Update(participante);
            await dBContext.SaveChangesAsync();
            return Ok();
        }
    }
}
