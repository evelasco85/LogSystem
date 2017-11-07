using System;
using System.Collections.Generic;

namespace LogManagement.Models
{
    public static class LogOutputTypeValidator
    {
        public static bool HasFlag(this LogOutputType thisEnum, LogOutputType checkFlag)
        {
            return (thisEnum & checkFlag) == checkFlag;
        }
    }

    [Flags]
    public enum LogOutputType
    {
        Default = 1,        //Timezone, occurence, priority, status, and description
        IDs = 2,            //User, session id, and transaction id
        Module = 4,         //System, application, and component
        Event = 6,          //Event
        Params = 16,         //Parameters
        All = Default | IDs | Module | Event | Params,
    }

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
        string Id { get; }
        LogOutputType OutputType { get; set; }
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
        
        IDictionary<string, object> Parameters { get; set; }
    }

    public class LogEntry : ILogEntry
    {
        private string _id;
        private LogOutputType _outputType;
        private TimeZoneInfo _timeZoneInfo;
        private DateTime _occurence;
        private string _user;
        private string _sessionId;
        private string _transactionId;
        private Priority _priority;

        public string Id { get { return _id; } }

        public LogOutputType OutputType
        {
            get { return _outputType; }
            set { _outputType = value; }
        }

        public TimeZoneInfo TimeZone
        {
            get { return _timeZoneInfo; }
        }

        public DateTime Occurence
        {
            get { return _occurence; }
        }

        public string User
        {
            get { return _user; }
        }

        public string SessionId
        {
            get { return _sessionId; }
        }

        public string TransactionId
        {
            get { return _transactionId; }
        }

        public string System { get; set; }
        public string Application { get; set; }
        public string Component { get; set; }
        public string Event { get; set; }

        public Priority Priority
        {
            get { return _priority; }
        }

        public Status Status { get; set; }

        public IDictionary<string, object> Parameters { get; set; }

        public LogEntry(LogOutputType outputType,
            TimeZoneInfo timeZoneInfo, DateTime occurence,
            string user, string sessionId, string transactionId,
            Priority priority)
        {
            _id = Guid.NewGuid().ToString();

            _outputType = outputType;
            _timeZoneInfo = timeZoneInfo;
            _occurence = occurence;
            _user = user;
            _sessionId = sessionId;
            _transactionId = transactionId;
            _priority = priority;
        }
    }
}
