using System;
using System.Collections.Generic;

namespace BackEndDeveloperAssesment.Models
{
    public partial class MWithdrawals
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int? AccountId { get; set; }
        public decimal? Amount { get; set; }
    }
}
