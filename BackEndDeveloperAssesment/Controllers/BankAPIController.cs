using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using BackEndDeveloperAssesment.Models;

namespace WalletApi.Controllers
{

    /// <summary>
    /// core banking api
    /// requires oAuth token for a secure banking api
    /// </summary>
    [Route("BankingApi")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class BankAPIController : Controller
    {
        dbContext db = new dbContext();
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            db.Dispose();
        }

        [HttpPost("CreateAccount")]
        public async Task<JsonResult> CreateAccount([FromBody]Account account)
        {
            try
            {
                //Opening An account can only be opened through a minimum deposit of ZWL1,000.00
                MAccount mAccount = new MAccount();
                mAccount.AccountName = account.AccountName;
                mAccount.DateCreated = DateTime.Now;
                mAccount.Balance = account.Deposit;
                mAccount.Currency = account.Currency;
                mAccount.AccountType = account.AccountType;
                mAccount.AccountNumber = account.AccountNumber;

                var account_exists = db.MAccount.Where(i => i.AccountNumber == account.AccountNumber).Any();
                if(account_exists)
                {
                    return Json(new
                    {
                        res = "err",
                        msg = $"Account with account number '{account.AccountNumber}' exists"
                    });
                }

                if(mAccount.AccountType.ToLower()!="savings" && mAccount.AccountType.ToLower() != "current")
                {
                    return Json(new
                    {
                        res = "err",
                        msg = "Account type must be savings or current"
                    });
                }
                if (mAccount.Balance < 1000 && mAccount.AccountType.ToLower() == "savings")
                {
                    return Json(new {
                        res = "err",
                        msg = "minimum account deposit for savings account is 1000"
                    });
                }

                db.MAccount.Add(mAccount);
                await db.SaveChangesAsync();

                return Json(new
                {
                    res = "ok",
                    msg = "Account Created Successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    res = "err",
                    msg = ex.Message
                });
            }
        }

        [HttpPost("MakeDeposit")]
        public async Task<JsonResult> MakeDeposit(string account_number, decimal amount)
        {
            try
            {
                //Deposit savings and current
                //Account balance must be increased by the amount deposited
                //Each transaction must be saved to the Transaction History

                MAccount mAccount = db.MAccount.Where(i => i.AccountNumber == account_number).FirstOrDefault();
                if (mAccount == null)
                {
                    return Json(new
                    {
                        res = "err",
                        msg = $"Account with Account number {account_number} does not exist"
                    });
                }

                //using transactions to ensure data integrity
                using (var transaction = db.Database.BeginTransaction())
                {
                    mAccount.Balance = mAccount.Balance + amount;
                    //
                    MDeposits mDeposits = new MDeposits();
                    mDeposits.Date = DateTime.Now;
                    mDeposits.AccountId = mAccount.Id;
                    mDeposits.Amount = amount;
                    db.MDeposits.Add(mDeposits);
                    //
                    await db.SaveChangesAsync();
                    transaction.Commit();
                }

                return Json(new
                {
                    res = "ok",
                    msg = "Deposit Stored Successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    res = "err",
                    msg = ex.Message
                });
            }
        }

        [HttpPost("MakeWithDrawal")]
        public async Task<JsonResult> MakeWithDrawal(string account_number, decimal amount)
        {
            try
            {
                //Withdraw savings
                //The account needs to have a minimum balance of ZWL1,000.00 at all times
                //Account balance must be decreased by the amount draw
                //Each transaction must be saved to the Transaction History

                //Withdraw current
                //The account can have an overdraft limit of ZWL100,000.00
                //The overdraft must be calculated as a negative number
                //A person cannot withdraw more than the(Current Balance +Overdraft) of the
                //account e.g.If the person has ZWL50,000 in their account, they will be allowed
                //to withdraw a maximum of ZWL150,000
                //Account balance must be decreased by the amount drawn
                //Each transaction must be saved to the Transaction History

                MAccount mAccount = db.MAccount.Where(i => i.AccountNumber == account_number).FirstOrDefault();
                if (mAccount == null)
                {
                    return Json(new
                    {
                        res = "err",
                        msg = $"Account with Account number {account_number} does not exist"
                    });
                }

                if((mAccount.Balance - amount) < 1000 && mAccount.AccountType=="savings")
                {
                    return Json(new
                    {
                        res = "err",
                        msg = $"You have insufficient amount to make this withdrawal"
                    });
                }

                if ((mAccount.Balance - amount) < -100000 && mAccount.AccountType == "current")
                {
                    return Json(new
                    {
                        res = "err",
                        msg = $"Current account overdraft of more than 100,000"
                    });
                }



                using (var transaction = db.Database.BeginTransaction())
                {
                    mAccount.Balance = mAccount.Balance - amount;
                    //
                    MWithdrawals mWithdrawals = new MWithdrawals();
                    mWithdrawals.Date = DateTime.Now;
                    mWithdrawals.AccountId = mAccount.Id;
                    mWithdrawals.Amount = amount;
                    db.MWithdrawals.Add(mWithdrawals);
                    //
                    await db.SaveChangesAsync();
                    transaction.Commit();
                }

                return Json(new
                {
                    res = "ok",
                    msg = "WithDrawal Completed Successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    res = "err",
                    msg = ex.Message
                });
            }
        }


        [HttpPost("MakeTransfer")]
        public async Task<JsonResult> MakeTransfer(string source_account_number, string destination_account_number, string reference, decimal amount)
        {
            try
            {
                MAccount mAccountSource = db.MAccount.Where(i => i.AccountNumber == source_account_number).FirstOrDefault();
                MAccount mAccountDestination = db.MAccount.Where(i => i.AccountNumber == destination_account_number).FirstOrDefault();

                if (mAccountSource == null)
                {
                    return Json(new
                    {
                        res = "err",
                        msg = $"Source Account does not exist"
                    });
                }
                if (mAccountDestination == null)
                {
                    return Json(new
                    {
                        res = "err",
                        msg = $"Destination Account does not exist"
                    });
                }

                //
                if(mAccountSource.AccountType=="savings" && ((mAccountSource.Balance - amount) < 1000))
                {
                    return Json(new
                    {
                        res = "err",
                        msg = $"Source Account (savings) has insufficient funds to make this transfer"
                    });
                }
                if (mAccountSource.AccountType == "current" && ((mAccountSource.Balance - amount) < -100000))
                {
                    return Json(new
                    {
                        res = "err",
                        msg = $"Source Account (current) has insufficient funds to make this transfer"
                    });
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    mAccountSource.Balance = mAccountSource.Balance - amount;
                    mAccountDestination.Balance = mAccountDestination.Balance + amount;
                    //
                    MTransactions mTransactions = new MTransactions();
                    mTransactions.Date = DateTime.Now;
                    mTransactions.Amount = amount;
                    mTransactions.SourceAccountNumber = source_account_number;
                    mTransactions.DestinationAccountNumber = destination_account_number;
                    mTransactions.Reference = reference;
                    //
                    db.MTransactions.Add(mTransactions);
                    //
                    await db.SaveChangesAsync();
                    transaction.Commit();
                }

                return Json(new
                {
                    res = "ok",
                    msg = "Transfer Completed Successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    res = "err",
                    msg = ex.Message
                });
            }
        }

        [HttpPost("CheckBalance")]
        public JsonResult CheckBalance(string source_account_number)
        {
            try
            {
                MAccount mAccount = db.MAccount.Where(i => i.AccountNumber == source_account_number).FirstOrDefault();

                return Json(new
                {
                    res = "ok",
                    msg = $"Account Balance: {mAccount.Balance.ToString("0.00")}, Account type: {mAccount.AccountType}"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    res = "err",
                    msg = ex.Message
                });
            }
        }


        [HttpPost("GetTransferHistory")]
        public JsonResult GetTransferHistory(string account_number)
        {
            try
            {
                var transfers = db.MTransactions.AsQueryable();
                //
                transfers = transfers.Where(i => i.DestinationAccountNumber == account_number || i.SourceAccountNumber == account_number);
               
                return Json(new
                {
                    res = "ok",
                    msg = transfers.ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    res = "err",
                    msg = ex.Message
                });
            }
        }

        [HttpPost("GetTransactionHistory")]
        public JsonResult GetTransactionHistory(string account_number)
        {
            try
            {
                var withdrawals = db.MWithdrawals.AsQueryable();
                var deposits = db.MDeposits.AsQueryable();
                MAccount mAccount = db.MAccount.Where(i => i.AccountNumber == account_number).FirstOrDefault();
                if (mAccount == null)
                {
                    return Json(new
                    {
                        res = "err",
                        msg = $"Account with account number {account_number} does not exist"
                    });
                }
                //
                withdrawals = withdrawals.Where(i => i.AccountId == mAccount.Id);
                deposits = deposits.Where(i => i.AccountId == mAccount.Id);


                return Json(new
                {
                    res = "ok",
                    withdrawals = withdrawals.ToList(),
                    deposits = deposits.ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    res = "err",
                    msg = ex.Message
                });
            }
        }

    }
}
