using System;
using System.Collections.Generic;
using LogManagement.Managers;

namespace LogManagement.Models
{
    public interface IStaticLogEntryWrapper_Emitter : ILogCreator
    {
        IStaticLogEntryWrapper_Emitter AddParameters(string parameterName, object parameterValue);
        void EmitLog(Priority priority, Status status);
        void EmitLog(bool retainParameters, Priority priority, Status status);
        void EmitLog(LogOutputType outputType, Priority priority, Status status);
        void EmitLog(bool retainParameters, LogOutputType outputType, Priority priority, Status status);
        ILogEntry CreateLogEntry(LogOutputType outputType, Priority priority, Status status);
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
        Dictionary<string, object> _parameterList = new Dictionary<string, object>();

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

        public ILogEntry CreateLogEntry(LogOutputType outputType, Priority priority, Status status)
        {
            ILogEntry entry = _manager.CreateLogEntry(outputType, priority);

            entry.System = System;
            entry.Application = Application;
            entry.Component = Component;

            entry.Event = Event;

            entry.Status = status;

            return entry;
        }

        public ILogEntry CreateLogEntry(Priority priority)
        {
            return CreateLogEntry(LogOutputType.All, priority, Status.None);
        }

        public ILogEntry CreateLogEntry(LogOutputType outputType, Priority priority)
        {
            return CreateLogEntry(outputType, priority, Status.None);
        }

        public void EmitLog(ILogEntry log)
        {
            _manager.EmitLog(log);
        }

        public void EmitLog(bool retainParameters, LogOutputType outputType, Priority priority, Status status)
        {
            ILogEntry entry = CreateLogEntry(outputType, priority, status);

            entry.Status = status;
            entry.Parameters = CloneParameters(_parameterList);

            EmitLog(entry);

            if (!retainParameters) _parameterList.Clear();
        }

        IDictionary<string, object> CloneParameters(Dictionary<string, object> originalDictionary)
        {
            Dictionary<string, object> newDictionary = new Dictionary<string, object>(
                originalDictionary.Count,
                originalDictionary.Comparer
            );

            foreach (KeyValuePair<string, object> entry in originalDictionary)
            {
                newDictionary.Add(entry.Key, entry.Value);
            }

            return newDictionary;
        }

        public void EmitLog(LogOutputType outputType, Priority priority, Status status)
        {
            EmitLog(false, outputType, priority, status);
        }

        public void EmitLog(bool retainParameters, Priority priority, Status status)
        {
            EmitLog(retainParameters, LogOutputType.All, priority, status);
        }

        public void EmitLog(Priority priority, Status status)
        {
            EmitLog(false, LogOutputType.All, priority, status);
        }

        public IStaticLogEntryWrapper_Emitter AddParameters(string parameterName, object parameterValue)
        {
            _parameterList.Add(parameterName, parameterValue);

            return this;
        }
    }
}
