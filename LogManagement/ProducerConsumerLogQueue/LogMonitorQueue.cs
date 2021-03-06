﻿using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace LogManagement.ProducerConsumerLogQueue
{
    public class LogMonitorQueue<TLogEntity> : ProducerConsumerLogQueue<TLogEntity>
    {
        bool _disposed = false;
        SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);
        private ILogMonitor<TLogEntity> _logMonitor;

        public LogMonitorQueue(
            ILogMonitor<TLogEntity> logMonitor
            )
        {
            _logMonitor = logMonitor;
        }

        public override void ProcessLog(TLogEntity log)
        {
            if (_logMonitor != null) _logMonitor.Evaluate(log);
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
