using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PL
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, "Field cannot be empty");
            }
            return ValidationResult.ValidResult;
        }
    }
    public class PositiveNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, "Field cannot be empty");
            }

            if (!int.TryParse(value.ToString(), out int number) || number <= 0)
            {
                return new ValidationResult(false, "the field must be a positive number");
            }

            return ValidationResult.ValidResult;
        }
    }

}
