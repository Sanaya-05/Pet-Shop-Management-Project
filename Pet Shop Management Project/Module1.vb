Module SharedData
    Public Class PetInfo
        Public Property PetID As String
        Public Property Type As String
        Public Property Breed As String
        Public Property Age As String
        Public Property Gender As String
        Public Property Price As Decimal
    End Class
    Public SelectedPetList As New List(Of PetInfo)

End Module
