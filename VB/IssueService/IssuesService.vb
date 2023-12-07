Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks

Namespace PagedAsyncSourceSkipTokenSample

    Public Module IssuesService

#Region "helpers"
        Private SyncObject As Object = New Object()

        Private AllIssues As System.Lazy(Of PagedAsyncSourceSkipTokenSample.IssueData()) = New System.Lazy(Of PagedAsyncSourceSkipTokenSample.IssueData())(Function()
            Dim [date] = System.DateTime.Today
            Dim rnd = New System.Random(0)
            Return System.Linq.Enumerable.Range(0, 100000).[Select](Function(i)
                [date] = [date].AddSeconds(-rnd.[Next](20 * 60))
                Return New PagedAsyncSourceSkipTokenSample.IssueData(subject:=PagedAsyncSourceSkipTokenSample.OutlookDataGenerator.GetSubject(), user:=PagedAsyncSourceSkipTokenSample.OutlookDataGenerator.GetFrom(), created:=[date], votes:=rnd.[Next](100), priority:=PagedAsyncSourceSkipTokenSample.OutlookDataGenerator.GetPriority())
            End Function).ToArray()
        End Function)

#End Region
        Private Class SkipToken

            Private _CreatedValue As DateTime

            Public Sub New(ByVal createdValue As System.DateTime)
                Me.CreatedValue = createdValue
            End Sub

            Public Property CreatedValue As DateTime
                Get
                    Return _CreatedValue
                End Get

                Private Set(ByVal value As DateTime)
                    _CreatedValue = value
                End Set
            End Property
        End Class

        Public Async Function GetIssuesAsync(ByVal skipToken As Object, ByVal pageSize As Integer, ByVal sortOrder As PagedAsyncSourceSkipTokenSample.IssueSortOrder) As Task(Of PagedAsyncSourceSkipTokenSample.IssuesFetchResult)
            Await System.Threading.Tasks.Task.Delay(CInt((300))).ConfigureAwait(False)
            Dim issues As System.Collections.Generic.IEnumerable(Of PagedAsyncSourceSkipTokenSample.IssueData) = PagedAsyncSourceSkipTokenSample.IssuesService.AllIssues.Value
            Dim createdValue = If(skipToken IsNot Nothing, CType(skipToken, PagedAsyncSourceSkipTokenSample.IssuesService.SkipToken).CreatedValue, DirectCast(Nothing, System.DateTime?))
            If sortOrder = PagedAsyncSourceSkipTokenSample.IssueSortOrder.CreatedAscending Then
                issues = issues.OrderBy(Function(x) x.Created).Where(Function(x) If(createdValue IsNot Nothing, x.Created > createdValue, True))
            Else
                issues = issues.OrderByDescending(Function(x) x.Created).Where(Function(x) If(createdValue IsNot Nothing, x.Created < createdValue, True))
            End If

            Dim rows = issues.Take(pageSize).ToArray()
            Dim nextSkipToken = If(rows.Any(), New PagedAsyncSourceSkipTokenSample.IssuesService.SkipToken(System.Linq.Enumerable.Last(Of PagedAsyncSourceSkipTokenSample.IssueData)(rows).Created), Nothing)
            Return New PagedAsyncSourceSkipTokenSample.IssuesFetchResult(rows, nextSkipToken)
        End Function
    End Module
End Namespace
