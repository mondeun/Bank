using System.Collections.Generic;

namespace Bank.Interfaces
{
    public interface IAuditLogger
    {
        void AddMessage(string message);
        List<string> GetLog();
    }
}