using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lab2.Attributes;

namespace Lab2
{
    public class Computer : IValidatableObject
    {
        [Required(ErrorMessage = "Укажите тип компьютера.")]
        [RegularExpression(
            @"^(Рабочая станция|Сервер|Ноутбук|Моноблок|Мини-ПК|МИНИ-ПК)$",
            ErrorMessage = "Недопустимый тип компьютера.")]
        public string Type { get; set; } = "";

        public Processor CPU { get; set; } = new();
        public GPU VideoCard { get; set; } = new();

        [Range(4, 512, ErrorMessage = "Объём ОЗУ должен быть от 4 до 512 ГБ.")]
        public int RAMsizeGB { get; set; }

        [Required(ErrorMessage = "Укажите тип ОЗУ.")]
        [RegularExpression(@"^DDR[345]$",
            ErrorMessage = "Тип ОЗУ должен быть: DDR3, DDR4 или DDR5.")]
        public string RAMtype { get; set; } = "";

        [Range(128, 32768, ErrorMessage = "Объём диска: от 128 до 32 768 ГБ.")]
        public int HDDsizeGB { get; set; }

        [Required(ErrorMessage = "Укажите тип диска.")]
        [RegularExpression(@"^(SSD|HDD)$",
            ErrorMessage = "Тип диска: SSD или HDD.")]
        public string HDDtype { get; set; } = "";

        [ValidPurchaseDate(30)]
        public DateTime PurchaseDate { get; set; } = DateTime.Today;

        public bool HasMonitor { get; set; }
        public bool HasKeyboard { get; set; }
        public bool HasMouse { get; set; }

        // IValidatableObject — кросс-полевые проверки
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CPU is null)
                yield return new ValidationResult("Процессор не задан.", new[] { nameof(CPU) });

            if (VideoCard is null)
                yield return new ValidationResult("Видеокарта не задана.", new[] { nameof(VideoCard) });

            // Бизнес-правило: серверы не могут иметь встроенный HDD < 500 ГБ
            if (Type == "Сервер" && HDDsizeGB < 500)
                yield return new ValidationResult(
                    "Для сервера объём диска должен быть не менее 500 ГБ.",
                    new[] { nameof(HDDsizeGB) });
        }

        /// <summary>
        /// Итоговая стоимость = (CPU + GPU + ОЗУ + Диск + Периферия) × коэффициент типа.
        ///
        /// Формула v2 (сбалансированная):
        ///   CPU  — Processor.Price()         диапазон ~21 000–65 000 ₽  (~25–30%)
        ///   GPU  — GPU.BasePrice             диапазон ~28 000–160 000 ₽ (~55–65%)
        ///   RAM  — GB × (DDR3:50|DDR4:80|DDR5:140)  диапазон ~400–9 000 ₽   (~1–4%)
        ///   Диск — SSD:GB×8 | HDD:GB×2      диапазон ~500–32 000 ₽  (~3–12%)
        ///   Деление на 3 убрано — было искусственным масштабированием.
        /// </summary>
        public decimal Price()
        {
            decimal p = CPU.Price() + VideoCard.Price();

            decimal ramPerGB = RAMtype switch
            {
                "DDR3" => 50m,
                "DDR4" => 80m,
                "DDR5" => 140m,
                _      => 60m
            };
            p += RAMsizeGB * ramPerGB;
            p += HDDtype == "SSD" ? HDDsizeGB * 8m : HDDsizeGB * 2m;

            if (HasMonitor)  p += 9_000m;
            if (HasKeyboard) p += 1_500m;
            if (HasMouse)    p += 1_000m;

            p *= Type switch
            {
                "Рабочая станция" => 1.3m,
                "Сервер"          => 1.6m,
                "Ноутбук"         => 1.4m,
                "Моноблок"        => 1.2m,
                "МИНИ-ПК"         => 1.1m,
                "Мини-ПК"         => 1.1m,
                _                 => 1.0m
            };

            return Math.Round(p, 2);
        }

        public override string ToString() =>
            $"[{Type}] {CPU.Model} | {VideoCard.Model} | {RAMsizeGB}ГБ {RAMtype} | {HDDsizeGB}ГБ {HDDtype}";
    }
}
