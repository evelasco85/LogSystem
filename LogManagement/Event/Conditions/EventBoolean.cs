namespace LogManagement.Event.Conditions
{
    public interface IEventBoolean
    {
        bool Evaluate(IEventContext context);
    }

    public abstract class EventBoolean : IEventBoolean
    {
        public abstract bool Evaluate(IEventContext context);
    }
}
