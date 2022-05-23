using System.ComponentModel.DataAnnotations;

namespace WebAPISistemaRifas.Validaciones
{
    public class ValidacionNumeroCarta : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var valor = Convert.ToInt32(value);
            if (!(valor >= 1) || !(valor <= 54))
            {
                return new ValidationResult("El numero ingresado no se encuentra dentro del rango de los numeros de la loteria");
            }

            return ValidationResult.Success;
        }
    }
}
