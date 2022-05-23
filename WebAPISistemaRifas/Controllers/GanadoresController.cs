using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPISistemaRifas.DTOs;
using WebAPISistemaRifas.Entidades;
using WebAPISistemaRifas.Services;

namespace WebAPISistemaRifas.Controllers
{
    [ApiController]
    [Route("ResultadosDeRifa/")]
    public class GanadoresController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IWebHostEnvironment env;
        private readonly string archivoGanadores = "Registrosdellamada.txt"; 

        public GanadoresController(ApplicationDBContext context, IWebHostEnvironment env)
        {
            this.dBContext = context;
            this.env = env;
        }

        [HttpGet("CalculoDeGanadorEnRifa/{id:int}")]
        public async Task<ActionResult<List<ParticipanteDTO>>> post(int id) 
        {
            var rifa = dBContext.Rifas
                .Include(p => p.premios)
                .Include(rpc => rpc.ParticipantesCartasRifa)
                .ThenInclude(p => p.Participantes)
                .FirstOrDefault(x => x.id == id);
            if (rifa == null)
            {
                return NotFound("No se encontro la rifa solicitada");
            }

            if (rifa.premios.Count != 6) 
            {
                return NotFound("No se cuentan con los premios suficientes");
            }

            if (rifa.ParticipantesCartasRifa.Count >= 54) 
            {
                return NotFound("No se cuentan con la cantidad suficientes de participantes");
            }

            var mazo = await dBContext.Cartas.ToListAsync();
            mazo = SeleccionDeCartas(mazo);
            var lista = new List<ParticipanteDTO>();
            foreach (var c in mazo.Take(6)) 
            {
                var participanteGanador = rifa.ParticipantesCartasRifa.FirstOrDefault(x => x.IdRifa == id.ToString() && x.IdCartas == c.id.ToString()).IdParticipantes;
                var participante = await dBContext.Participantes.FirstAsync(x => x.Id.ToString() == participanteGanador);
                lista.Add(new ParticipanteDTO { num_telefono = participante.num_telefono, tarjeta_credito = participante.tarjeta_credito });

            }

            foreach (var e in lista)
            {
                var ruta = $@"{env.ContentRootPath}\wwwroot\{archivoGanadores}";
                using (StreamWriter writer = new StreamWriter(ruta, append: true))
                { writer.WriteLine($@"Llamada:{e.num_telefono}"); }
            }
            return lista;
        }

        private List<Cartas> SeleccionDeCartas(List<Cartas> CartasLoteria)
        {
            Random random = new Random();
            List<Cartas> lista = new List<Cartas>();
            for (int i = 0; i < CartasLoteria.Count; i++)
            {
                int indice = random.Next(0, CartasLoteria.Count);
                lista.Add(CartasLoteria[indice]);
                CartasLoteria.Remove(CartasLoteria[indice]);
            }
            return lista;
        }
    }
}
