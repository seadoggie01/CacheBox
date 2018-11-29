Public Class RefreshBackup

    Shared Function testing(directory As String) As List(Of String)
        testing = New List(Of String)
        'If the directory exists
        If My.Computer.FileSystem.DirectoryExists(directory) Then
            Try
                'Add this directories files to the list
                testing.AddRange(My.Computer.FileSystem.GetFiles(directory))
                'For each sub-directory
                For Each c In My.Computer.FileSystem.GetDirectories(directory)
                    'Add the sub-directorys' files to this list
                    testing.AddRange(testing(c))
                Next
            Catch ex As Exception
                If ex.GetType().Equals(New UnauthorizedAccessException()) Then
                    'Do nothing, ignore Unauthorized use
                Else
                    Debug.Print("Error in backup: " & ex.Message)
                End If
            End Try
        End If
        Return testing
    End Function
End Class
