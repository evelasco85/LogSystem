using System;

namespace LogManagement.Models
{
    public enum Status
    {
        Success,
        Failure
    }

    public enum Priority
    {
        /// <summary>
        /// Technical facing
        /// </summary>
        Debug,

        /// <summary>
        /// Displays expected or business as usual logs
        /// </summary>
        Info,

        /// <summary>
        /// Shows deviation from expectation
        /// </summary>
        Notice,

        /// <summary>
        /// Shows recurrent deviations or threshold exceeding limits
        /// </summary>
        Warning,

        /// <summary>
        /// Shows business logic error
        /// </summary>
        Error,

        /// <summary>
        /// Technical error or exception-handling
        /// </summary>
        Critical,

        /// <summary>
        /// Notification for constant attack, non-showstopper exceptions, or fail-safe invocations
        /// </summary>
        Alert,

        /// <summary>
        /// Notify showstopper or breaches
        /// </summary>
        Emergency
    }

    public interface ILogEntry
    {
        DateTime Occurence { get; set; }
        string System { get; set; }
        string Application { get; set; }
        string Component { get; set; }
        string Event { get; set; }
        Status Status { get; set; }
        string User { get; set; }
        string SessionId { get; set; }
        string TransactionId { get; set; }
        string Description { get; set; }
        string Reason { get; set; }
        string Source { get; set; }
        string Destination { get; set; }
        string RuleId { get; set; }
    }

    public class LogEntry : ILogEntry
    {
        public DateTime Occurence { get; set; }
        public string System { get; set; }
        public string Application { get; set; }
        public string Component { get; set; }
        public string Event { get; set; }
        public Status Status { get; set; }
        public string User { get; set; }
        public string SessionId { get; set; }
        public string TransactionId { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string RuleId { get; set; }
    }
}
