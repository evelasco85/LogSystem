namespace LogManagement.Event.Parameters
{
    public interface IData
    {
        object GetData(IContext context);
        string GetSyntax(IContext context);
    }

    public abstract class Data : IData
    {
        public abstract object GetData(IContext context);
        public abstract string GetSyntax(IContext context);
    }
}
