using System.ComponentModel.DataAnnotations;
using WebAPISistemaRifas.Validaciones;

namespace WebAPISistemaRifas.DTOs
{
    public class CreacionPremioDTO : IValidatableObject
    {
        [ValidacionDescripcionPremio]
        public string descripcion { get; set; }
        public int nivel { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (nivel == 0)
            {
                yield return new ValidationResult("Por favor ingrese algun numero",
                new String[] { nameof(nivel) });
            }

            if (!(nivel >= 1) || !(nivel <= 6))
            {
                yield return new ValidationResult("Ingrese algun nivel valido",
                new String[] { nameof(nivel) });
            }

        }
    }
}
