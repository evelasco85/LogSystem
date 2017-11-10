using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace LogManagement.ProducerConsumerLogQueue
{
    public interface IProducerConsumerLogQueue<TLogEntity> : IDisposable
    {
        bool IsEmpty { get; }
        void EnqueueLog(TLogEntity log);
    }

    public abstract class ProducerConsumerLogQueue<TLogEntity> : IProducerConsumerLogQueue<TLogEntity>
    {
        private EventWaitHandle _waitHandle;
        private Thread _worker;
        private readonly object _lockerObject;
        private Queue<TLogEntity> _logTasks;
        private bool _isEmpty = true;
        private bool _isClose = false;

        private bool _disposed = false;
        private SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

        public ProducerConsumerLogQueue()
        {
            _waitHandle = new AutoResetEvent(false);
            _worker = new Thread(Work);
            _lockerObject = new object();
            _logTasks = new Queue<TLogEntity>();

            _worker.Start();
        }
        
        public bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public virtual void EnqueueLog(TLogEntity log)
        {
            if (_isClose) return;

            lock (_lockerObject)
            {
                _logTasks.Enqueue(log);
            }

            _isEmpty = false;
            _waitHandle.Set();
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
                    ProcessLog(log);
                }
                else
                {
                    _isEmpty = true;

                    _waitHandle.WaitOne(); // No more tasks, block current thread - wait for a signal
                }
            }
        }

        public abstract void ProcessLog(TLogEntity log);

        void Close()
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
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _handle.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            Close();

            _disposed = true;
        }

        ~ProducerConsumerLogQueue()
        {
            Dispose(false);
        }
    }
}
