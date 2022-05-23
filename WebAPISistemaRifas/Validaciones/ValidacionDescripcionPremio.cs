using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebAPISistemaRifas.Validaciones
{
    public class ValidacionDescripcionPremio : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult("Porfavor ingrese algun nombre para el premio");
            }

            var elementos = value.ToString().Split(' ');

            if (!elementos[0].All(char.IsDigit))
            {
                return new ValidationResult("Ingrese la cantidad del objeto a ser regalado");
            }

            for (int i= 1; i<elementos.Length; i++)
            {
                if (!Regex.IsMatch(elementos[i],@"[a-z]"))
                {
                    return new ValidationResult("Ingrese de forma correcta el objeto a ser regalado");
                }
            }

            return ValidationResult.Success;
        }
    }
}
