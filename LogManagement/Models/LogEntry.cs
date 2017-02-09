using System;

namespace LogManagement.Models
{
    public enum Status
    {
        None = 0,
        Success = 1,
        Failure = 2
    }

    public enum Priority
    {
        /// <summary>
        /// Technical facing
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Displays expected or business as usual logs
        /// </summary>
        Info = 2,

        /// <summary>
        /// Shows deviation from expectation
        /// </summary>
        Notice = 3,

        /// <summary>
        /// Shows recurrent deviations or threshold exceeding limits
        /// </summary>
        Warning = 4,

        /// <summary>
        /// Shows business logic error
        /// </summary>
        Error = 5,

        /// <summary>
        /// Technical error or exception-handling
        /// </summary>
        Critical = 6,

        /// <summary>
        /// Notification for constant attack, non-showstopper exceptions, or fail-safe invocations
        /// </summary>
        Alert = 7,

        /// <summary>
        /// Notify showstopper or breaches
        /// </summary>
        Emergency = 8
    }

    public interface ILogEntry
    {
        TimeZoneInfo TimeZone { get; }
        DateTime Occurence { get; }
        string User { get; }
        string SessionId { get; }
        string TransactionId { get; }

        string System { get; set; }
        string Application { get; set; }
        string Component { get; set; }
        string Event { get; set; }

        Priority Priority { get; }
        Status Status { get; set; }
        string Description { get; set; }
        
        string Source { get; set; }
        string Destination { get; set; }

        string RuleId { get; set; }
        string Reason { get; set; }
    }

    public class LogEntry : ILogEntry
    {
        TimeZoneInfo _timeZoneInfo;
        private DateTime _occurence;
        string _user;
         string _sessionId;
         string _transactionId;
        private Priority _priority;

         public TimeZoneInfo TimeZone { get { return _timeZoneInfo; } }
         public DateTime Occurence { get { return _occurence; } }

         public string User { get { return _user; } }
         public string SessionId { get { return _sessionId; } }
         public string TransactionId { get { return _transactionId; } }

        public string System { get; set; }
        public string Application { get; set; }
        public string Component { get; set; }
        public string Event { get; set; }

        public Priority Priority { get { return _priority; } }
        public Status Status { get; set; }
        public string Description { get; set; }
        
        public string Source { get; set; }
        public string Destination { get; set; }

        public string RuleId { get; set; }
        public string Reason { get; set; }

        public LogEntry(TimeZoneInfo timeZoneInfo, DateTime occurence,
            string user, string sessionId, string transactionId, Priority priority)
        {
            _timeZoneInfo = timeZoneInfo;
            _occurence = occurence;
            _user = user;
            _sessionId = sessionId;
            _transactionId = transactionId;
            _priority = priority;
        }
    }
}
