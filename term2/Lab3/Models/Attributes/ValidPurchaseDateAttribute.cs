using System;
using System.ComponentModel.DataAnnotations;

namespace Lab2.Attributes
{
    /// <summary>
    /// Кастомный атрибут валидации: дата покупки не в будущем
    /// и не старше maxYearsBack лет (логично для компьютерной техники).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidPurchaseDateAttribute : ValidationAttribute
    {
        private readonly int _maxYearsBack;

        public ValidPurchaseDateAttribute(int maxYearsBack = 30)
        {
            _maxYearsBack = maxYearsBack;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
        {
            if (value is not DateTime date)
                return new ValidationResult("Некорректный тип даты.");

            if (date.Date > DateTime.Today)
                return new ValidationResult(
                    "Дата покупки не может быть в будущем.",
                    new[] { ctx.MemberName ?? "" });

            if (date < DateTime.Today.AddYears(-_maxYearsBack))
                return new ValidationResult(
                    $"Дата покупки не может быть старше {_maxYearsBack} лет.",
                    new[] { ctx.MemberName ?? "" });

            return ValidationResult.Success;
        }
    }
}
