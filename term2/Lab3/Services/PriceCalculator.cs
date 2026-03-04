using System.Collections.Generic;
using System.Linq;

namespace Lab2
{
    public static class PriceCalculator
    {
        public static decimal GetComputerPrice(Computer c) => c.Price();

        public static decimal GetLaboratoryTotal(IEnumerable<Computer> computers)
            => computers.Sum(c => c.Price());

        public static decimal GetAveragePrice(IEnumerable<Computer> computers)
        {
            var list = computers.ToList();
            return list.Count == 0 ? 0m : GetLaboratoryTotal(list) / list.Count;
        }
    }
}
