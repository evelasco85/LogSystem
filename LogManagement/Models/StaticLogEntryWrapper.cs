using LogManagement.Managers;
using System;
using System.Collections.Generic;

namespace LogManagement.Models
{
    public interface IStaticLogEntryWrapper_Emitter
    {
        void EmitLog(Priority priority, String description, IList<Tuple<string, object>> parameters);
        void EmitLog(Priority priority, Status status);
        void EmitLog(Priority priority, Status status, IList<Tuple<string, object>> parameters);
    }

    public interface IStaticLogEntryWrapper : ILogCreator, IStaticLogEntryWrapper_Emitter
    {
        string System { set; }
        string Application { set; }
        string Component { set; }
        string Event { set; }

        IStaticLogEntryWrapper SetSystem(String system);
        IStaticLogEntryWrapper SetApplication(String application);
        IStaticLogEntryWrapper SetComponent(String component);
        IStaticLogEntryWrapper_Emitter SetEvent(String @event);

        ILogEntry CreateLogEntry(Priority priority, String description);
    }

    public class StaticLogEntryWrapper : IStaticLogEntryWrapper
    {
        ILogManager _manager;

        public string System { private get; set; }
        public string Application { private get; set; }
        public string Component { private get; set; }
        public string Event { private get; set; }

        public StaticLogEntryWrapper(ILogManager manager) :
            this(manager, string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        public StaticLogEntryWrapper(ILogManager manager,
          string system
          )
            : this(manager, system, string.Empty, string.Empty, string.Empty)
        {
        }

        public StaticLogEntryWrapper(ILogManager manager,
          string system,
          string application
          )
            : this(manager, system, application, string.Empty, string.Empty)
        {
        }

        public StaticLogEntryWrapper(ILogManager manager,
           string system,
           string application,
           string component
           )
            : this(manager, system, application, component, string.Empty)
        {
        }

        public StaticLogEntryWrapper(ILogManager manager,
            string system,
            string application,
            string component,
            string @event
            )
        {
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
            ILogEntry entry = _manager.CreateLogEntry(priority);

            entry.System = System;
            entry.Application = Application;
            entry.Component = Component;
            entry.Event = Event;

            return entry;
        }

        public ILogEntry CreateLogEntry(Priority priority, String description)
        {
            ILogEntry entry = CreateLogEntry(priority);

            entry.Description = description;

            return entry;
        }

        public void EmitLog(ILogEntry log)
        {
            _manager.EmitLog(log);
        }

        public void EmitLog(Priority priority, String description)
        {
            ILogEntry entry = CreateLogEntry(priority, description);

            EmitLog(entry);
        }

        public void EmitLog(Priority priority, Status status)
        {
            ILogEntry entry = CreateLogEntry(priority, "");

            entry.Status = status;

            EmitLog(entry);
        }

        public void EmitLog(Priority priority, Status status, IList<Tuple<string, object>> parameters)
        {
            ILogEntry entry = CreateLogEntry(priority, "");

            entry.Status = status;
            entry.Parameters = parameters;

            EmitLog(entry);
        }

        public void EmitLog(Priority priority, String description, IList<Tuple<string, object>> parameters)
        {
            ILogEntry entry = CreateLogEntry(priority, description);

            entry.Parameters = parameters;

            EmitLog(entry);
        }
    }
}
