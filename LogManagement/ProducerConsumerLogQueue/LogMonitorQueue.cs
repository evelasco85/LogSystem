using System.Collections.Generic;
using System.Threading;

namespace LogManagement.ProducerConsumerLogQueue
{
    public class LogMonitorQueue<TLogEntity> : IProducerConsumerLogQueue<TLogEntity>
    {
        private EventWaitHandle _waitHandle;
        private Thread _worker;
        private readonly object _lockerObject;
        private Queue<TLogEntity> _logTasks;
        private ILogMonitor<TLogEntity> _logMonitor;
        private bool _isEmpty = true;
        private bool _isClose = false;

        public LogMonitorQueue(ILogMonitor<TLogEntity> logMonitor)
        {
            _waitHandle = new AutoResetEvent(false);
            _worker = new Thread(Work);
            _lockerObject = new object();
            _logTasks = new Queue<TLogEntity>();
            _logMonitor = logMonitor;

            _worker.Start();
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public void EnqueueLog(TLogEntity log)
        {
            if(_isClose) return;
            
            lock (_lockerObject)
            {
                _logTasks.Enqueue(log);
            }

            _isEmpty = false;
            _waitHandle.Set();
        }

        public void Close()
        {
            if (!_isClose)
            {
                EnqueueLog(default(TLogEntity));     // Signal the consumer to exit.
                _worker.Join();         // Wait for the consumer's thread to finish.
                _waitHandle.Close();            // Release any OS resources.

                _isClose = true;
            }
        }

        public void Dispose()
        {
            Close();
        }

        void Work()     //Run on separate thread
        {
            while (true)
            {
                TLogEntity log = default(TLogEntity);

                lock (_lockerObject)
                    if (_logTasks.Count > 0)
                    {
                        log = _logTasks.Dequeue();

                        if (log == null) return;
                    }

                if (log != null)
                {
                    if (_logMonitor != null) _logMonitor.Evaluate(log);
                }
                else
                {
                    _isEmpty = true;

                    _waitHandle.WaitOne(); // No more tasks, block current thread - wait for a signal
                }
            }
        }
    }
}
