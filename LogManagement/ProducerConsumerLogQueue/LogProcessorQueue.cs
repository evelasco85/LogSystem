using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace LogManagement.ProducerConsumerLogQueue
{
    public class LogProcessorQueue<TLogEntity> : ProducerConsumerLogQueue<TLogEntity>
    {
        bool _disposed = false;
        SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);
        private ILogMonitor<TLogEntity> _logMonitor;
        private ILogInserter<TLogEntity> _logInserter;

        public LogProcessorQueue(ILogInserter<TLogEntity> logInserter,
            ILogMonitor<TLogEntity> logMonitor
            )
        {
            _logInserter = logInserter;
            _logMonitor = logMonitor;
        }

        public override void ProcessLog(TLogEntity log)
        {
            if (_logInserter != null) _logInserter.Insert(log);

            //-->> Normalize log persistency table here (if necessary) prior to analysis of log entries
            if (_logMonitor != null) _logMonitor.Evaluate(log);
            //-->> Clear log persistency table here (if necessary)
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _handle.Dispose();
            }

            _disposed = true;
            
            base.Dispose(disposing);
        }
    }
}
