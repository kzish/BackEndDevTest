using System;
using System.Collections.Generic;

namespace BackEndDeveloperAssesment.Models
{
    public partial class MAccount
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public string AccountType { get; set; }
        public string AccountNumber { get; set; }
    }
}
