﻿using System.ComponentModel.DataAnnotations;
using WebAPISistemaRifas.Validaciones;

namespace WebAPISistemaRifas.DTOs
{
    public class RifaPatchDTO
    {
        [Required]
        [ValidacionNombreRifa]
        public string Nombre { get; set; }
        [Required]
        public DateTime Fecha_apertura { get; set; }
        [Required]
        public DateTime Fecha_cierre { get; set; }
        [Required]
        public DateTime Fecha_rifa { get; set; }
    }
}
