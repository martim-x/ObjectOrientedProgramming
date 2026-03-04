using System.ComponentModel.DataAnnotations;

namespace Lab2
{
    public class GPU
    {
        [Required(ErrorMessage = "Укажите производителя видеокарты.")]
        [RegularExpression(@"^(NVIDIA|AMD|Intel)$",
            ErrorMessage = "Производитель GPU: NVIDIA, AMD или Intel.")]
        public string Manufacturer { get; set; } = "";

        public string Series { get; set; } = "";

        [Required(ErrorMessage = "Укажите модель видеокарты.")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Модель GPU: от 2 до 50 символов.")]
        public string Model { get; set; } = "";

        [Range(100.0, 5000.0, ErrorMessage = "Частота GPU: от 100 до 5000 МГц.")]
        public double Frequency { get; set; }

        [Range(1, 80, ErrorMessage = "Объём памяти GPU: от 1 до 80 ГБ.")]
        public int MemoryGB { get; set; }

        /// <summary>
        /// Базовая рыночная цена карты.
        /// Задаётся через BasePrice — MainForm.GpuSpec[model].basePrice.
        /// Frequency больше не участвует в цене (устранена проблема Frequency*200=360 000 ₽).
        /// </summary>
        public decimal BasePrice { get; set; }

        public decimal Price() => BasePrice > 0 ? BasePrice : 5_000m + MemoryGB * 1_500m;

        public override string ToString() =>
            $"{Manufacturer} {Series} {Model} ({MemoryGB}ГБ / {Frequency}МГц)";
    }
}
