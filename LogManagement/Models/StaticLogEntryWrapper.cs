using LogManagement.Dynamic.Managers;
using System;
using System.Collections.Generic;

namespace LogManagement.Models
{
    public interface IStaticLogEntryWrapper_Emitter : ILogCreator
    {
        void EmitLog(Priority priority, Status status, String description);
        void EmitLog(Priority priority, Status status, String description, Tuple<string, object> parameter);
        void EmitLog(Priority priority, Status status, String description, IList<Tuple<string, object>> parameters);
        ILogEntry CreateLogEntry(Priority priority, Status status, String description);
    }

    public interface IStaticLogEntryWrapper : IStaticLogEntryWrapper_Emitter
    {
        string System { set; }
        string Application { set; }
        string Component { set; }
        string Event { set; }

        IStaticLogEntryWrapper SetSystem(String system);
        IStaticLogEntryWrapper SetApplication(String application);
        IStaticLogEntryWrapper SetComponent(String component);
        IStaticLogEntryWrapper_Emitter SetEvent(String @event);
    }

    public class StaticLogEntryWrapper : IStaticLogEntryWrapper
    {
        private ILogManager _manager;
        private string _logCreatorId;

        public string System { private get; set; }
        public string Application { private get; set; }
        public string Component { private get; set; }
        public string Event { private get; set; }

        public StaticLogEntryWrapper(string logCreatorId, ILogManager manager) :
            this(logCreatorId, manager, string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        public StaticLogEntryWrapper(string logCreatorId, ILogManager manager,
          string system
          )
            : this(logCreatorId, manager, system, string.Empty, string.Empty, string.Empty)
        {
        }

        public StaticLogEntryWrapper(string logCreatorId, ILogManager manager,
          string system,
          string application
          )
            : this(logCreatorId, manager, system, application, string.Empty, string.Empty)
        {
        }

        public StaticLogEntryWrapper(string logCreatorId, ILogManager manager,
           string system,
           string application,
           string component
           )
            : this(logCreatorId, manager, system, application, component, string.Empty)
        {
        }

        public StaticLogEntryWrapper(string logCreatorId, ILogManager manager,
            string system,
            string application,
            string component,
            string @event
            )
        {
            _logCreatorId = logCreatorId;
            _manager = manager;

            System = system;
            Application = application;
            Component = component;
            Event = @event;
        }

        public IStaticLogEntryWrapper SetSystem(String system)
        {
            System = system;

            return this;
        }

        public IStaticLogEntryWrapper SetApplication(String application)
        {
            Application = application;

            return this;
        }

        public IStaticLogEntryWrapper SetComponent(String component)
        {
            Component = component;

            return this;
        }

        public IStaticLogEntryWrapper_Emitter SetEvent(String @event)
        {
            Event = @event;

            return this;
        }

        public ILogEntry CreateLogEntry(Priority priority)
        {
            return CreateLogEntry(_logCreatorId, priority);
        }

        public ILogEntry CreateLogEntry(string logCreatorId, Priority priority)
        {
            ILogEntry entry = _manager.CreateLogEntry(logCreatorId, priority);

            entry.System = System;
            entry.Application = Application;
            entry.Component = Component;
            entry.Event = Event;

            return entry;
        }

        public ILogEntry CreateLogEntry(Priority priority, Status status, String description)
        {
            ILogEntry entry = CreateLogEntry(priority);

            entry.Status = status;
            entry.Description = description;

            return entry;
        }

        public void EmitLog(ILogEntry log)
        {
            _manager.EmitLog(log);
        }

        public void EmitLog(Priority priority, Status status, String description)
        {
            ILogEntry entry = CreateLogEntry(priority, status, description);

            entry.Status = status;

            EmitLog(entry);
        }

        public void EmitLog(Priority priority, Status status, String description, Tuple<string, object> parameter)
        {
            EmitLog(priority, status, description, new List<Tuple<string, object>>{parameter});
        }

        public void EmitLog(Priority priority, Status status, String description, IList<Tuple<string, object>> parameters)
        {
            ILogEntry entry = CreateLogEntry(priority, status, description);

            entry.Status = status;
            entry.Parameters = parameters;

            EmitLog(entry);
        }
    }
}
