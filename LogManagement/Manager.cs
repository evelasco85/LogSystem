namespace LogManagement
{
    public interface IManager
    {
        IBuilder CreateLogBuilder(string sessionId, string businessTransactionId);
    }
}
