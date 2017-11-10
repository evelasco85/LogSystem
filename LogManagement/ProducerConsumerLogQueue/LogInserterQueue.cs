using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace LogManagement.ProducerConsumerLogQueue
{
    public class LogInserterQueue<TLogEntity> : ProducerConsumerLogQueue<TLogEntity>
    {
        bool _disposed = false;
        SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);
        private ILogInserter<TLogEntity> _logInserter;

        public LogInserterQueue(ILogInserter<TLogEntity> logInserter)
        {
            _logInserter = logInserter;
        }

        public override void ProcessLog(TLogEntity log)
        {
            if (_logInserter != null) _logInserter.Insert(log);
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
