Public Class RefreshBackup


    'This returns a list of a full paths to files and searches recursively
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
                ' If the exection is an unauthorized access exception
                '   This is thrown in Win 10 when attempting to access an old
                '   shortcut style link to 'My Documents' which is kept around
                '   by Microsoft for old apps. It is not accessible and doesn't
                '   work. I should really find a better way to ignore it however
                If ex.GetType().Equals(New UnauthorizedAccessException()) Then
                    'Do nothing, ignore Unauthorized use exccptions... see above
                Else
                    Debug.Print("Error in backup: " & ex.Message)
                End If
            End Try
        End If
        Return testing
    End Function
End Class
