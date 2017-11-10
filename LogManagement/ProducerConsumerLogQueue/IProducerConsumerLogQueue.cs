using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogManagement.ProducerConsumerLogQueue
{
    public interface IProducerConsumerLogQueue<TLogEntity> : IDisposable
    {
        bool IsEmpty { get; }
        void EnqueueLog(TLogEntity log);
        void Close();
    }
}
