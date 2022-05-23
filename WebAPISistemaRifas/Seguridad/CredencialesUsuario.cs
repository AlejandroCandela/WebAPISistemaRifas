using System.ComponentModel.DataAnnotations;

namespace WebAPISistemaRifas.Seguridad
{
    public class CredencialesUsuario
    {
        [Required]
        public string Nombres { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string contraseña { get; set; }
        
    }
}
