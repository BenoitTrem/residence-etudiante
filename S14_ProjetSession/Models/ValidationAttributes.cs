using System;
using System.ComponentModel.DataAnnotations;

namespace S14_ProjetSession.Models
{
    public class DatePasDansLeFuturAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return true;
            if (value is DateTime dateTime)
            {
                return dateTime <= DateTime.Now;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} ne peut pas être une date dans le futur.";
        }
    }

    public class AgeMinimumAttribute : ValidationAttribute
    {
        private readonly int _ageMin;
        public AgeMinimumAttribute(int ageMin)
        {
            _ageMin = ageMin;
        }

        public override bool IsValid(object? value)
        {
            if (value == null) return true;
            if (value is DateTime dateTime)
            {
                return dateTime <= DateTime.Now.AddYears(-_ageMin);
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} doit correspondre à un âge d'au moins {_ageMin} ans.";
        }
    }
}
