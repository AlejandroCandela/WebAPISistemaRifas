using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WebAPISistemaRifas.Entidades;
using WebAPISistemaRifas.Validaciones;

namespace WebAPISistemaRifas
{
    public class Rifa
    {

        public int id { get; set; }
        [Required]
        [ValidacionNombreRifa]
        public string Nombre { get; set; }
        [Required]
        public DateTime Fecha_apertura { get; set; }
        [Required]
        public DateTime Fecha_cierre { get; set; }
        [Required]
        public DateTime Fecha_rifa { get; set; }
        public List<ParticipantesCartasRifa>? ParticipantesCartasRifa { get; set; }
        public List<Premios>? premios{ get; set; }
        public string UserId { get; set; }
        public IdentityUser usuario { get; set; }
    }
}
