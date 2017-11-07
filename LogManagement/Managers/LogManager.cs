using System;
using System.Collections.Generic;
using LogManagement.Models;

namespace LogManagement.Dynamic.Managers
{
    public interface ILogCreator
    {
        void EmitLog(ILogEntry log);
        ILogEntry CreateLogEntry(Priority priority);
        ILogEntry CreateLogEntry(LogOutputType outputType, Priority priority);
    }

    public interface ILogEmitter
    {
        OnLogEmit LogEmit { get; set; }
    }

    public interface ILogManager : ILogEmitter, ILogCreator
    {
        RetrieveUserDelegate RetrieveUser { get; set; }
        RetrieveSessionIdDelegate RetrieveSessionId { get; set; }
        RetrieveBusinessTransactionIdDelegate RetrieveBusinessTransactionId { get; set; }
    }

    public delegate string RetrieveUserDelegate();
    public delegate string RetrieveSessionIdDelegate();
    public delegate string RetrieveBusinessTransactionIdDelegate();

    public delegate void OnLogEmit(ILogEntry log);

    public class LogManager : ILogManager
    {
        static ILogManager s_instance = new LogManager();

        RetrieveUserDelegate _retrieveUser;
        RetrieveSessionIdDelegate _retrieveSessionId;
        RetrieveBusinessTransactionIdDelegate _retrieveBusinessTransactionId;

        private OnLogEmit _onLogEmit;

        private LogManager() {}

        public RetrieveUserDelegate RetrieveUser
        {
            get { return _retrieveUser; }
            set { _retrieveUser = value; }
        }

        public RetrieveSessionIdDelegate RetrieveSessionId
        {
            get { return _retrieveSessionId; }
            set { _retrieveSessionId = value; }
        }

        public RetrieveBusinessTransactionIdDelegate RetrieveBusinessTransactionId
        {
            get { return _retrieveBusinessTransactionId; }
            set { _retrieveBusinessTransactionId = value; }
        }

        public OnLogEmit LogEmit
        {
            get { return _onLogEmit; }
            set { _onLogEmit = value; }
        }

        public static ILogManager GetInstance()
        {
            return s_instance;
        }

        public ILogEntry CreateLogEntry(LogOutputType outputType, Priority priority)
        {
            if (_retrieveUser == null)
                throw new NotImplementedException(String.Format("Implementation for member '{0}' is required",
                    "RetrieveUser"));

            if (_retrieveSessionId == null)
                throw new NotImplementedException(String.Format("Implementation for member '{0}' is required",
                    "RetrieveSessionId"));

            if (_retrieveBusinessTransactionId == null)
                throw new NotImplementedException(String.Format("Implementation for member '{0}' is required",
                    "RetrieveBusinessTransactionId"));

            ILogEntry log = new LogEntry(outputType,
                TimeZoneInfo.Local,
                DateTime.Now,
                _retrieveUser(),
                _retrieveSessionId(),
                _retrieveBusinessTransactionId(),
                priority
            )
            {
                Status = Status.None,
                Parameters = new List<Tuple<string, object>>()
            };

            return log;
        }

        public ILogEntry CreateLogEntry(Priority priority)
        {
            return CreateLogEntry(LogOutputType.All, priority);
        }

        public void EmitLog(ILogEntry log)
        {
            if (_onLogEmit != null)
                _onLogEmit(log);
        }
    }
}
