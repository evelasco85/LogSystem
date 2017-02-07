namespace LogManagement.Event.Parameters
{
    public interface IEventData
    {
        object GetData(IEventContext context);
        string GetSyntax(IEventContext context);
    }

    public abstract class EventData : IEventData
    {
        public abstract object GetData(IEventContext context);
        public abstract string GetSyntax(IEventContext context);
    }
}
