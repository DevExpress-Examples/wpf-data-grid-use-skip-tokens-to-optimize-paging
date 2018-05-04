Imports DevExpress.Data.Filtering
Imports DevExpress.Xpf.Data
Imports System
Imports System.ComponentModel
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Windows


Namespace PagedAsyncSourceSkipTokenSample
    Partial Public Class MainWindow
        Inherits Window

        Public Sub New()
            InitializeComponent()

            Dim source = New PagedAsyncSource() With {.ElementType = GetType(IssueData)}
            AddHandler Unloaded, Sub(o, e)
                source.Dispose()
            End Sub
            AddHandler source.FetchPage, Sub(o, e)
                e.Result = FetchRowsAsync(e)
            End Sub
            grid.ItemsSource = source
        End Sub

        Private Shared Async Function FetchRowsAsync(ByVal e As FetchPageAsyncEventArgs) As Task(Of FetchRowsResult)
            Dim sortOrder As IssueSortOrder = GetIssueSortOrder(e)

            Dim issuesFetchResult = Await IssuesService.GetIssuesAsync(skipToken:= e.SkipToken, pageSize:= e.Take, sortOrder:= sortOrder)

            Return New FetchRowsResult(issuesFetchResult.Issues, hasMoreRows:= issuesFetchResult.Issues.Length = e.Take, nextSkipToken:= issuesFetchResult.NextSkipToken)
        End Function

        Private Shared Function GetIssueSortOrder(ByVal e As FetchPageAsyncEventArgs) As IssueSortOrder
            If e.SortOrder.Length > 0 Then
                Dim sort = e.SortOrder.Single()
                If sort.PropertyName = "Created" Then
                    Return If(sort.Direction = ListSortDirection.Ascending, IssueSortOrder.CreatedAscending, IssueSortOrder.CreatedDescending)
                End If
            End If
            Return IssueSortOrder.CreatedDescending
        End Function
    End Class
End Namespace
