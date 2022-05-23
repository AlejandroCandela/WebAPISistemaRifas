using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPISistemaRifas.DTOs;
using WebAPISistemaRifas.Seguridad;

namespace WebAPISistemaRifas.Controllers
{
    [ApiController]
    [Route("CuentasCasino")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Administrador")]
    public class UsuariosController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ILogger<UsuariosController> logger;

        public UsuariosController(UserManager<IdentityUser> userManager,
            IConfiguration configuration, SignInManager<IdentityUser> signInManager, ILogger<UsuariosController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.logger = logger;
        }
        [HttpPost("RegistroDeUsuarios")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            logger.LogInformation("Entrada de datos");
            logger.LogInformation(credencialesUsuario.ToString());
            var user = new IdentityUser
            {
                UserName = credencialesUsuario.Nombres,
                Email = credencialesUsuario.email
            };
            logger.LogInformation("Creacion de entidad usuario");
            logger.LogInformation(user.ToString());
            var result = await userManager.CreateAsync(user, credencialesUsuario.contraseña);
            if (result.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario) 
        {
            var result = await signInManager.PasswordSignInAsync(credencialesUsuario.email,
                credencialesUsuario.contraseña, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login Incorrecto");
            }
        }

        [HttpPost("DeclaracionEmpleados")]
        public async Task<ActionResult> HacerAdmin(EditarCoordinadores editar) 
        {
            var usuario = await userManager.FindByNameAsync(editar.Nombres );
            await userManager.AddClaimAsync(usuario, new Claim("Administrador", "1"));
            return NoContent();
        }

        [HttpPost("BajaDeEmpleados")]
        public async Task<ActionResult> QuitarAdmin(EditarCoordinadores editar)
        {
            var usuario = await userManager.FindByNameAsync(editar.Nombres );
            await userManager.RemoveClaimAsync(usuario, new Claim("Administrador","1"));
            return NoContent();
        }

        [HttpGet("RenovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> Renovar()
        {
            var nombreclaim = HttpContext.User.Claims.Where(claim => claim.Type == "Nombre").FirstOrDefault();
            var emailclaim = HttpContext.User.Claims.Where(claim => claim.Type == "Email" && claim.Type == "Email").FirstOrDefault();
            var nombre = nombreclaim.Value;
            var email = emailclaim.Value;
            var credenciales = new CredencialesUsuario()
            {
                email = email,
                Nombres = nombreclaim.Value,
            };

            return await ConstruirToken(credenciales);

        }
        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>
            {
                new Claim("Nombre", credencialesUsuario.Nombres),
                new Claim("Email", credencialesUsuario.email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var FechaExpiracion = DateTime.UtcNow.AddMinutes(60);
            var token = new JwtSecurityToken(issuer: null,audience: null, claims: claims,
                expires: FechaExpiracion, signingCredentials: creds);
            return new RespuestaAutenticacion() 
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                FechaExpiracion = FechaExpiracion
            };
        }


    }
}
