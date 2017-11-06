using System.Collections.Generic;
using System.Linq;
using LogManagement;

namespace LogManagementTests.Implementations
{
    public class GetFailedValidationDescriptionLogsQuery : BaseLogQueryObject<LogEntryKVP, GetFailedValidationDescriptionLogsQuery.Criteria>
    {
        private IList<LogEntryKVP> _inMemoryLogEntries;

        public GetFailedValidationDescriptionLogsQuery(IList<LogEntryKVP> inMemoryLogEntries)
        {
            _inMemoryLogEntries = inMemoryLogEntries;
        }

        public class Criteria
        {
        }

        public override IEnumerable<LogEntryKVP> PerformSearchOperation(Criteria searchInput)
        {
            return _inMemoryLogEntries
                .Where(log => (log.Key == "Description") && (log.Value == "Validation has been invoked but was failed"));
        }
    }
}
