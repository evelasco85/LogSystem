namespace LogManagement.Event.Parameters
{
    public interface IEventBoolean
    {
        bool Evaluate(IEventContext context);
        string GetSyntax(IEventContext context);
    }

    public abstract class EventBoolean : IEventBoolean
    {
        public abstract bool Evaluate(IEventContext context);
        public abstract string GetSyntax(IEventContext context);
    }
}
