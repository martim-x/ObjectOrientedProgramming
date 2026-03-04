using System.ComponentModel.DataAnnotations;

namespace Lab2
{
    public class Processor
    {
        [Required(ErrorMessage = "Укажите производителя процессора.")]
        [RegularExpression(@"^(Intel|AMD)$",
            ErrorMessage = "Производитель процессора: Intel или AMD.")]
        public string Manufacturer { get; set; } = "";

        public string Series { get; set; } = "";

        [Required(ErrorMessage = "Укажите модель процессора.")]
        [StringLength(60, MinimumLength = 2,
            ErrorMessage = "Модель процессора: от 2 до 60 символов.")]
        public string Model { get; set; } = "";

        [Range(1, 256, ErrorMessage = "Количество ядер: от 1 до 256.")]
        public int Cores { get; set; }

        [Range(0.5, 10.0, ErrorMessage = "Базовая частота: от 0.5 до 10.0 ГГц.")]
        public double Frequency { get; set; }

        [Range(0.5, 10.0, ErrorMessage = "Максимальная частота: от 0.5 до 10.0 ГГц.")]
        public double MaxFrequency { get; set; }

        [Range(32, 64, ErrorMessage = "Разрядность архитектуры: 32 или 64 бит.")]
        public int ArchBits { get; set; } = 64;

        [Range(0, 65536, ErrorMessage = "Кэш L1: от 0 до 65 536 КБ.")]
        public int CacheL1KB { get; set; }

        [Range(0, 65536, ErrorMessage = "Кэш L2: от 0 до 65 536 КБ.")]
        public int CacheL2KB { get; set; }

        [Range(0, 1024, ErrorMessage = "Кэш L3: от 0 до 1 024 МБ.")]
        public int CacheL3MB { get; set; }

        /// <summary>
        /// База 8000 + частота×800 + L3×300 + 1000 (за 64бит).
        /// Диапазон: ~21 000 (6C/gen5/Intel) — ~65 000 (16C/gen10/AMD).
        /// Деление на 10 убрано — было в Computer.Price(), теперь CPU имеет реальный вес.
        /// </summary>
        public decimal Price()
        {
            decimal p = 8_000m;
            p += (decimal)(MaxFrequency * 800);
            p += CacheL3MB * 300m;
            p += ArchBits == 64 ? 1_000m : 0m;
            return p;
        }

        public override string ToString() =>
            $"{Manufacturer} {Series} {Model} ({Cores}C / {MaxFrequency:F1}GHz)";
    }
}
