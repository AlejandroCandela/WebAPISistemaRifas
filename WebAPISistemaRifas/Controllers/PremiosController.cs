using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPISistemaRifas.DTOs;

namespace WebAPISistemaRifas.Controllers
{
    [ApiController]
    [Route("RegistroPremiosDeRifa/")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Administrador")]
    public class PremiosController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IMapper mapper;

        public PremiosController(ApplicationDBContext context,
           IMapper mapper)
        {
            this.mapper = mapper;
            this.dBContext = context;
        }

        [HttpPut("ModificarUnPremio/{id:int}")]
        public async Task<ActionResult> put(int id, CreacionPremioDTO creacionPremiosDTO) 
        {
            var exist = await dBContext.Premios.AnyAsync(x => x.id == id);
            if (!exist)
            {
                return NotFound();
            }

            var premios = mapper.Map<Premios>(creacionPremiosDTO);
            premios.id = id;

            dBContext.Update(premios);
            await dBContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
