using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Entities
{
    public class Budgets
    {
        public string AccountId { get; set; }
        public string BudgetName { get; set; }
        public decimal BudgetLimit { get; set; }
        public string Currency { get; set; }
        //public TimeUnit TimeUnit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> CostFilters { get; set; }
        public bool IncludeSupport { get; set; }
        public bool IncludeTax { get; set; }
        public bool IncludeSubscription { get; set; }
        public bool IncludeOtherSubscription { get; set; }
        public bool IncludeUpfront { get; set; }
        public bool IncludeRecurring { get; set; }
        public bool IncludeDiscount { get; set; }
        public bool IncludeCredit { get; set; }
        public bool IncludeRefund { get; set; }
        public bool UseBlended { get; set; }
        //public List<NotificationWithSubscriber> NotificationsWithSubscribers { get; set; }
    }
}
