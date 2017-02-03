namespace LogManagement.Builders
{
    public interface ILogBuilder
    {
        
    }

    public class LogBuilder : ILogBuilder
    {
        private string _sessionId;
        private string _businessTransactionId;

        public LogBuilder(string sessionId, string businessTransactionId)
        {
            _sessionId = sessionId;
            _businessTransactionId = businessTransactionId;
        }
    }
}
