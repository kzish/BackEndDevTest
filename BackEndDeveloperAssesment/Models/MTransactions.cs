using System;
using System.Collections.Generic;

namespace BackEndDeveloperAssesment.Models
{
    public partial class MTransactions
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber { get; set; }
        public string Reference { get; set; }
    }
}
