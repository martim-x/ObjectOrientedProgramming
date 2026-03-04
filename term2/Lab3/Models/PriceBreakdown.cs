namespace Lab2
{
    public class PriceBreakdown
    {
        public decimal Cpu { get; init; }
        public decimal Gpu { get; init; }
        public decimal Ram { get; init; }
        public decimal Disk { get; init; }
        public decimal Peripherals { get; init; }

        public decimal Subtotal { get; init; }
        public decimal TypeMultiplier { get; init; }
        public decimal Total { get; init; }
    }
}
