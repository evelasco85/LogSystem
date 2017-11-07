using System.Collections.Generic;
using System.Linq;
using LogManagement;
using LogManagement.Models;

namespace LogManagementTests.Implementations
{
    public class GetFailedValidationDescriptionLogsQuery : BaseLogQueryObject<ILogEntry, GetFailedValidationDescriptionLogsQuery.Criteria>
    {
        private IList<ILogEntry> _inMemoryLogEntries;

        public GetFailedValidationDescriptionLogsQuery(IList<ILogEntry> inMemoryLogEntries)
        {
            _inMemoryLogEntries = inMemoryLogEntries;
        }

        public class Criteria
        {
        }

        public override IEnumerable<ILogEntry> PerformSearchOperation(Criteria searchInput)
        {
            return _inMemoryLogEntries
                .Where(log => (log.Description == "Validation has been invoked but was failed"));
        }
    }
}
