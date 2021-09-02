using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndDeveloperAssesment.Models
{
    public class Account
    {
      public string AccountName { get; set; }
      public decimal Deposit { get; set; }
      public string Currency { get; set; } = "USD";
      public string AccountType { get; set; } = "savings";
      public string AccountNumber { get; set; }
    }
}
