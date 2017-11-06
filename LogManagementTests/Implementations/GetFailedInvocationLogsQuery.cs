﻿using System;
using System.Collections.Generic;
using System.Linq;
using LogManagement;
using LogManagement.Models;

namespace LogManagementTests.Implementations
{
    public class GetFailedInvocationLogsQuery : BaseLogQueryObject<LogEntryKVP, GetFailedInvocationLogsQuery.Criteria>
    {
        private IList<LogEntryKVP> _inMemoryLogEntries;

        public GetFailedInvocationLogsQuery(IList<LogEntryKVP> inMemoryLogEntries)
        {
            _inMemoryLogEntries = inMemoryLogEntries;
        }

        public class Criteria
        {
        }

        public override IEnumerable<LogEntryKVP> PerformSearchOperation(Criteria searchInput)
        {
            return _inMemoryLogEntries
                .Where(log => (log.Key == "Status") && (log.Value == Status.Failure.ToString()));
        }
    }
}
