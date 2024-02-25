using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.DTO.Budget
{
    public class BudgetDto
    {
        public string BudgetName { get; set; }
        public decimal BudgetLimit { get; set; }
        public decimal? ActualSpend { get; set; }
        public decimal? ForecastedSpend { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Threshold { get; set; }
        public string TimeUnit { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}
