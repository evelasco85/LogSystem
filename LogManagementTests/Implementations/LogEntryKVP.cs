using LogManagement.Models;

namespace LogManagementTests.Implementations
{
    public class LogEntryKVP
    {
        public string TransactionId { get; set; }
        public Priority Priority { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
