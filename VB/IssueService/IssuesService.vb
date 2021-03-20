Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks

Namespace PagedAsyncSourceSkipTokenSample
	Public Module IssuesService
		#Region "helpers"
		Private SyncObject As New Object()
		Private AllIssues As New Lazy(Of IssueData())(Function()
			Dim [date] = DateTime.Today
			Dim rnd = New Random(0)
			Return Enumerable.Range(0, 100000).Select(Function(i)
				[date] = [date].AddSeconds(-rnd.Next(20 * 60))
				Return New IssueData(subject:= OutlookDataGenerator.GetSubject(), user:= OutlookDataGenerator.GetFrom(), created:= [date], votes:= rnd.Next(100), priority:= OutlookDataGenerator.GetPriority())
			End Function).ToArray()
		End Function)
		#End Region

		Private Class SkipToken
			Public Sub New(ByVal createdValue As DateTime)
				Me.CreatedValue = createdValue
			End Sub
			Private privateCreatedValue As DateTime
			Public Property CreatedValue() As DateTime
				Get
					Return privateCreatedValue
				End Get
				Private Set(ByVal value As DateTime)
					privateCreatedValue = value
				End Set
			End Property
		End Class

		Public Async Function GetIssuesAsync(ByVal skipToken As Object, ByVal pageSize As Integer, ByVal sortOrder As IssueSortOrder) As Task(Of IssuesFetchResult)
			Await Task.Delay(300).ConfigureAwait(False)
			Dim issues As IEnumerable(Of IssueData) = AllIssues.Value

			Dim createdValue = If(skipToken IsNot Nothing, DirectCast(skipToken, SkipToken).CreatedValue, CType(Nothing, DateTime?))
			If sortOrder = IssueSortOrder.CreatedAscending Then
'INSTANT VB WARNING: VB does not allow comparing non-nullable value types with 'null' - they are never equal to 'null':
'ORIGINAL LINE: issues = issues.OrderBy(x => x.Created).Where(x => createdValue != null ? x.Created > createdValue : true);
				issues = issues.OrderBy(Function(x) x.Created).Where(Function(x)If(True, x.Created > createdValue, True))
			Else
'INSTANT VB WARNING: VB does not allow comparing non-nullable value types with 'null' - they are never equal to 'null':
'ORIGINAL LINE: issues = issues.OrderByDescending(x => x.Created).Where(x => createdValue != null ? x.Created < createdValue : true);
				issues = issues.OrderByDescending(Function(x) x.Created).Where(Function(x)If(True, x.Created < createdValue, True))
			End If

			Dim rows = issues.Take(pageSize).ToArray()
			Dim nextSkipToken = If(rows.Any(), New SkipToken(rows.Last().Created), Nothing)
			Return New IssuesFetchResult(rows, nextSkipToken)
		End Function
	End Module
End Namespace
