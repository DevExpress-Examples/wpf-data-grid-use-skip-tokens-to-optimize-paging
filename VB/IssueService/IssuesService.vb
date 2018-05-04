Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks

Namespace PagedAsyncSourceSkipTokenSample
    Public NotInheritable Class IssuesService

        Private Sub New()
        End Sub

        #Region "helpers"
        Private Shared SyncObject As New Object()
        Private Shared AllIssues As Lazy(Of IssueData()) = New Lazy(Of IssueData())(Function()
            Dim [date] = Date.Today
            Dim rnd = New Random(0)
            Return Enumerable.Range(0, 100000).Select(Function(i)
                [date] = [date].AddSeconds(-rnd.Next(20 * 60))
                Return New IssueData(subject:= OutlookDataGenerator.GetSubject(), user:= OutlookDataGenerator.GetFrom(), created:= [date], votes:= rnd.Next(100), priority:= OutlookDataGenerator.GetPriority())
            End Function).ToArray()
        End Function)
        #End Region

        Private Class SkipToken
            Public Sub New(ByVal createdValue As Date)
                Me.CreatedValue = createdValue
            End Sub
            Private privateCreatedValue As Date
            Public Property CreatedValue() As Date
                Get
                    Return privateCreatedValue
                End Get
                Private Set(ByVal value As Date)
                    privateCreatedValue = value
                End Set
            End Property
        End Class

        Public Async Shared Function GetIssuesAsync(ByVal skipToken As Object, ByVal pageSize As Integer, ByVal sortOrder As IssueSortOrder) As Task(Of IssuesFetchResult)
            Await Task.Delay(300)
            Dim issues As IEnumerable(Of IssueData) = AllIssues.Value

            Dim createdValue = If(skipToken IsNot Nothing, DirectCast(skipToken, SkipToken).CreatedValue, Nothing)
            If sortOrder = IssueSortOrder.CreatedAscending Then
                issues = issues.OrderBy(Function(x) x.Created).Where(Function(x)If(createdValue IsNot Nothing, x.Created > createdValue, True))
            Else
                issues = issues.OrderByDescending(Function(x) x.Created).Where(Function(x)If(createdValue IsNot Nothing, x.Created < createdValue, True))
            End If

            Dim rows = issues.Take(pageSize).ToArray()
            Dim nextSkipToken = If(rows.Any(), New SkipToken(rows.Last().Created), Nothing)
            Return New IssuesFetchResult(rows, nextSkipToken)
        End Function
    End Class
End Namespace
