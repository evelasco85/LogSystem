using LogManagement.Dynamic.Event;

namespace LogManagement.Dynamic.Event.Parameters
{
    public interface IData
    {
        string Name { get; }
        object GetData(IContext context);
        string GetSyntax(IContext context);
    }

    public abstract class Data : IData
    {
        public abstract string Name { get; }
        public abstract object GetData(IContext context);
        public abstract string GetSyntax(IContext context);
    }
}
