using DevExpress.Data.Filtering;
using DevExpress.Xpf.Data;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace PagedAsyncSourceSkipTokenSample {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            var source = new PagedAsyncSource() {
                ElementType = typeof(IssueData)
            };
            Unloaded += (o, e) => {
                source.Dispose();
            };
            source.FetchPage += (o, e) => {
                e.Result = FetchRowsAsync(e);
            };
            grid.ItemsSource = source;
        }

        static async Task<FetchRowsResult> FetchRowsAsync(FetchPageAsyncEventArgs e) {
            IssueSortOrder sortOrder = GetIssueSortOrder(e);

            var issuesFetchResult = await IssuesService.GetIssuesAsync(
                skipToken: e.SkipToken,
                pageSize: e.Take,
                sortOrder: sortOrder);

            return new FetchRowsResult(
                issuesFetchResult.Issues, 
                hasMoreRows: issuesFetchResult.Issues.Length == e.Take,
                nextSkipToken: issuesFetchResult.NextSkipToken);
        }

        static IssueSortOrder GetIssueSortOrder(FetchPageAsyncEventArgs e) {
            if(e.SortOrder.Length > 0) {
                var sort = e.SortOrder.Single();
                if(sort.PropertyName == "Created") {
                    return sort.Direction == ListSortDirection.Ascending
                        ? IssueSortOrder.CreatedAscending
                        : IssueSortOrder.CreatedDescending;
                }
            }
            return IssueSortOrder.CreatedDescending;
        }
    }
}
