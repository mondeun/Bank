using System.Collections.Generic;
using Bank.Exceptions;
using Bank.Interfaces;

namespace Bank
{
    public class Bank
    {
        private IAuditLogger _auditlogger;
        private Dictionary<string, Account> _accounts = new Dictionary<string, Account>();


        public Bank(IAuditLogger auditlogger)
        {
            _auditlogger = auditlogger;
        }


        /// <summary>
        /// Create a new bank account
        /// </summary>
        /// <param name="account"></param>
        public void CreateAccount(Account account)
        {
            if (_accounts.ContainsKey(account.Number))
                throw new DuplicateAccount();


            int num = 0;
            if (int.TryParse(account.Number, out num) == false)
            {
                _auditlogger.AddMessage("Warn12: CreateAccount, Invalid account number received");
                _auditlogger.AddMessage("Error45: Alert, internal error, should not happen!");
                throw new InvalidAccount();
            }

            _accounts.Add(account.Number, account);

            _auditlogger.AddMessage("New account, Account number " + account.Number + ", Name=" + account.Name);
        }
 

        /// <summary>
        /// Get a specific account details
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public Account GetAccount(string number)
        {
            return _accounts[number];
        }


        /// <summary>
        /// Return the full auditlog
        /// </summary>
        /// <returns></returns>
        public List<string> GetAuditLog()
        {
            return _auditlogger.GetLog();
        }
    }
}