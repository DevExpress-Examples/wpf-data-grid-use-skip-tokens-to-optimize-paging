Namespace PagedAsyncSourceSkipTokenSample

    Public Class IssuesFetchResult

        Private _Issues As PagedAsyncSourceSkipTokenSample.IssueData(), _NextSkipToken As Object

        Public Sub New(ByVal issues As IssueData(), ByVal nextSkipToken As Object)
            Me.Issues = issues
            Me.NextSkipToken = nextSkipToken
        End Sub

        Public Property Issues As IssueData()
            Get
                Return _Issues
            End Get

            Private Set(ByVal value As IssueData())
                _Issues = value
            End Set
        End Property

        Public Property NextSkipToken As Object
            Get
                Return _NextSkipToken
            End Get

            Private Set(ByVal value As Object)
                _NextSkipToken = value
            End Set
        End Property
    End Class
End Namespace
