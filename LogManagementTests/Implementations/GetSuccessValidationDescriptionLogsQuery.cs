using System.Collections.Generic;
using System.Linq;
using LogManagement;
using LogManagement.Models;

namespace LogManagementTests.Implementations
{
    public class GetSuccessValidationDescriptionLogsQuery : BaseLogQueryObject<ILogEntry, GetSuccessValidationDescriptionLogsQuery.Criteria>
    {
        public const string DESCRIPTION = "Validation has been invoked successfully";

        private IList<ILogEntry> _inMemoryLogEntries;

        public GetSuccessValidationDescriptionLogsQuery(IList<ILogEntry> inMemoryLogEntries)
        {
            _inMemoryLogEntries = inMemoryLogEntries;
        }

        public class Criteria
        {
        }

        public override IEnumerable<ILogEntry> PerformSearchOperation(Criteria searchInput)
        {
            string key = "Description";
            return _inMemoryLogEntries
                .Where(log => (log.Parameters.ContainsKey(key)) && (log.Parameters[key].ToString() == DESCRIPTION));
        }
    }
}
