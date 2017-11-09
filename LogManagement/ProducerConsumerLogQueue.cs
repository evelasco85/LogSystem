﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace LogManagement
{
    public class ProducerConsumerLogQueue<TLogEntity> : IDisposable
    {
        public delegate void QueuedItemProcessedDelegate();

        private EventWaitHandle _waitHandle;
        private Thread _worker;
        private readonly object _lockerObject;
        private Queue<TLogEntity> _logTasks;
        private ILogMonitor<TLogEntity> _logMonitor;

        private QueuedItemProcessedDelegate _processCompleteAction;

        public ProducerConsumerLogQueue(ILogMonitor<TLogEntity> logMonitor)
            :this(logMonitor, null)
        {
        }

        public ProducerConsumerLogQueue(ILogMonitor<TLogEntity> logMonitor, QueuedItemProcessedDelegate processCompleteAction)
        {
            _waitHandle = new AutoResetEvent(false);
            _worker = new Thread(Work);
            _lockerObject = new object();
            _logTasks = new Queue<TLogEntity>();
            _logMonitor = logMonitor;

            _processCompleteAction = processCompleteAction;

            _worker.Start();
        }

        public void EnqueueLog(TLogEntity log)
        {
            lock (_lockerObject) _logTasks.Enqueue(log);

            _waitHandle.Set();
        }

        public void Dispose()
        {
            EnqueueLog(default(TLogEntity));     // Signal the consumer to exit.
            _worker.Join();         // Wait for the consumer's thread to finish.
            _waitHandle.Close();            // Release any OS resources.
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

                    if (_processCompleteAction != null) _processCompleteAction();
                }
                else
                    _waitHandle.WaitOne();         // No more tasks, block current thread - wait for a signal
            }
        }
    }
}