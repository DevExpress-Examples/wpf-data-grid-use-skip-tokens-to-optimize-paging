Namespace PagedAsyncSourceSkipTokenSample
	Public Class IssuesFetchResult
		Public Sub New(ByVal issues() As IssueData, ByVal nextSkipToken As Object)
			Me.Issues = issues
			Me.NextSkipToken = nextSkipToken
		End Sub

		Private privateIssues As IssueData()
		Public Property Issues() As IssueData()
			Get
				Return privateIssues
			End Get
			Private Set(ByVal value As IssueData())
				privateIssues = value
			End Set
		End Property
		Private privateNextSkipToken As Object
		Public Property NextSkipToken() As Object
			Get
				Return privateNextSkipToken
			End Get
			Private Set(ByVal value As Object)
				privateNextSkipToken = value
			End Set
		End Property
	End Class
End Namespace
