using LogManagement.Managers;

namespace LogManagement.Models
{
    public interface IStaticLogEntryWrapper
    {
        string System { set; }
        string Application { set; }
        string Component { set; }
        string Event { set; }

        ILogEntry CreateLogEntry(Priority priority);
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
          ) : this(manager, system, string.Empty, string.Empty, string.Empty)
        {
        }

        public StaticLogEntryWrapper(ILogManager manager,
          string system,
          string application
          ) : this(manager, system, application, string.Empty, string.Empty)
        {
        }

        public StaticLogEntryWrapper(ILogManager manager,
           string system,
           string application,
           string component
           ) : this(manager, system, application, component, string.Empty)
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

        public ILogEntry CreateLogEntry(Priority priority)
        {
            ILogEntry entry = _manager.CreateLogEntry(priority);

            entry.System = System;
            entry.Application = Application;
            entry.Component = Component;
            entry.Event = Event;

            return entry;
        }
    }
}
