namespace LogManagement.Event.Parameters
{
    public interface IBooleanBase
    {
        string Operator { get; }
        bool Evaluate(IContext context);
        string GetSyntax(IContext context);
    }

    public abstract class BooleanBase : IBooleanBase
    {
        public abstract string Operator { get; }
        public abstract bool Evaluate(IContext context);
        public abstract string GetSyntax(IContext context);
    }
}
