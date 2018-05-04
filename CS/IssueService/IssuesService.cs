using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PagedAsyncSourceSkipTokenSample {
    public static class IssuesService {
        #region helpers
        static object SyncObject = new object();
        static Lazy<IssueData[]> AllIssues = new Lazy<IssueData[]>(() => {
            var date = DateTime.Today;
            var rnd = new Random(0);
            return Enumerable.Range(0, 100000)
                .Select(i => {
                    date = date.AddSeconds(-rnd.Next(20 * 60));
                    return new IssueData(
                        subject: OutlookDataGenerator.GetSubject(),
                        user: OutlookDataGenerator.GetFrom(),
                        created: date,
                        votes: rnd.Next(100),
                        priority: OutlookDataGenerator.GetPriority());
                }).ToArray();
        });
        #endregion

        class SkipToken {
            public SkipToken(DateTime createdValue) {
                CreatedValue = createdValue;
            }
            public DateTime CreatedValue { get; private set; }
        }

        public async static Task<IssuesFetchResult> GetIssuesAsync(object skipToken, int pageSize, IssueSortOrder sortOrder) {
            await Task.Delay(300);
            IEnumerable<IssueData> issues = AllIssues.Value;

            var createdValue = skipToken != null ? ((SkipToken)skipToken).CreatedValue : default(DateTime?);
            if(sortOrder == IssueSortOrder.CreatedAscending) {
                issues = issues
                    .OrderBy(x => x.Created)
                    .Where(x => createdValue != null ? x.Created > createdValue : true);
            } else {
                issues = issues
                    .OrderByDescending(x => x.Created)
                    .Where(x => createdValue != null ? x.Created < createdValue : true);
            }

            var rows = issues.Take(pageSize).ToArray();
            var nextSkipToken = rows.Any() ? new SkipToken(rows.Last().Created) : null;
            return new IssuesFetchResult(rows, nextSkipToken);
        }
    }
}
