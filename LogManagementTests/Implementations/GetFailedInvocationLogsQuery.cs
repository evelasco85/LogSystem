using System;
using System.Collections.Generic;
using System.Linq;
using LogManagement;
using LogManagement.Models;

namespace LogManagementTests.Implementations
{
    public class GetFailedInvocationLogsQuery : BaseLogQueryObject<ILogEntry, GetFailedInvocationLogsQuery.Criteria>
    {
        private IList<ILogEntry> _inMemoryLogEntries;

        public GetFailedInvocationLogsQuery(IList<ILogEntry> inMemoryLogEntries)
        {
            _inMemoryLogEntries = inMemoryLogEntries;
        }

        public class Criteria
        {
        }

        public override IEnumerable<ILogEntry> PerformSearchOperation(Criteria searchInput)
        {
            return _inMemoryLogEntries
                .Where(log => log.Status == Status.Failure);
        }
    }
}
