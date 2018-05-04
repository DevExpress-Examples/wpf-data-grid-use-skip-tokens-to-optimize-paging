namespace PagedAsyncSourceSkipTokenSample {
    public class IssuesFetchResult {
        public IssuesFetchResult(IssueData[] issues, object nextSkipToken) {
            Issues = issues;
            NextSkipToken = nextSkipToken;
        }

        public IssueData[] Issues { get; private set; }
        public object NextSkipToken { get; private set; }
    }
}
