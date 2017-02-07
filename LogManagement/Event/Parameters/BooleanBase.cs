namespace LogManagement.Event.Parameters
{
    public interface IEventBoolean
    {
        bool Evaluate(IContext context);
        string GetSyntax(IContext context);
    }

    public abstract class BooleanBase : IEventBoolean
    {
        public abstract bool Evaluate(IContext context);
        public abstract string GetSyntax(IContext context);
    }
}
