namespace LogManagement.Event.Parameters
{
    public interface IBooleanBase
    {
        bool Evaluate(IContext context);
        string GetSyntax(IContext context);
    }

    public abstract class BooleanBase : IBooleanBase
    {
        public abstract bool Evaluate(IContext context);
        public abstract string GetSyntax(IContext context);
    }
}
