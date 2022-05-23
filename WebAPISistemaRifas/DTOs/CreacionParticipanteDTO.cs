using System.ComponentModel.DataAnnotations;

namespace WebAPISistemaRifas.DTOs
{
    public class CreacionParticipanteDTO :IValidatableObject
    {
        [Required]
        public string num_telefono { get; set; }
        [Required]
        [CreditCard]
        public string tarjeta_credito { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(num_telefono))
            {
                if (!(num_telefono[2] == '-' || num_telefono[3] == '-'))
                {
                    yield return new ValidationResult("El numero de telefono debe dividir la Lada y el demas contenido",
                        new String[] { nameof(num_telefono) });
                }
                
                var elementos = num_telefono.Split('-');

                switch (elementos[0])
                {
                    case "81":
                    case "55":
                    case "33":
                        if (elementos[1].Length != 8)
                        {
                            yield return new ValidationResult("Longuitud no valida",
                            new String[] { nameof(num_telefono) });
                        }
                        break;
                    case "222":
                    case "782":
                    case "667":
                        if (elementos[1].Length != 7)
                        {
                            yield return new ValidationResult("Longuitud no valida",
                            new String[] { nameof(num_telefono) });
                        }
                        break;
                    default:
                        yield return new ValidationResult("Extension Lada no valida",
                        new String[] { nameof(num_telefono) });
                        break;
                }
            }
        }
    }
}
