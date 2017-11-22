using System;
using System.Collections.Generic;
using System.Linq;
using LogManagement;
using LogManagement.Models;

namespace LogManagementTests.Implementations
{
    public class GetFailedLoginAttemptsQuery : BaseLogQueryObject<ILogEntry, GetFailedLoginAttemptsQuery.Criteria>
    {
        public const String INVALID_LOGIN = "Invalid Login";

        private IList<ILogEntry> _inMemoryLogEntries;

        public GetFailedLoginAttemptsQuery(IList<ILogEntry> inMemoryLogEntries)
        {
            _inMemoryLogEntries = inMemoryLogEntries;
        }

        public class Criteria
        {
        }

        public override IEnumerable<ILogEntry> PerformSearchOperation(Criteria searchInput)
        {
            return _inMemoryLogEntries
                .Where(log => (log.Event == INVALID_LOGIN));
        }
    }
}
