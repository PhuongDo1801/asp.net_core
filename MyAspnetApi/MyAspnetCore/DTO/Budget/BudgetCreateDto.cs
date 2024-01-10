using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.DTO.Budget
{
    public class BudgetCreateDto
    {
        //public string AccountId { get; set; }
        public string BudgetName { get; set; }
        public decimal BudgetLimit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Email { get; set; }
        public int limitPercent { get; set; }
    }
}
