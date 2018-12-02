Public Class Watcher
    Public Shared networkConnection As Boolean
    Public Shared FinalDestinationFolder As String = My.Settings.FinalDestination
    Public Shared watcherList As New List(Of IO.FileSystemWatcher)

    Public Shared Sub Main(pathToWatch As String)
        ' Create a new FileSystemWatcher and set its properties.
        '   Watch for changes in LastAccess And LastWrite times, And
        '   the renaming of files Or directories. 
        Dim watcher As New IO.FileSystemWatcher With {
            .Path = pathToWatch,
            .NotifyFilter = (IO.NotifyFilters.LastAccess Or IO.NotifyFilters.LastWrite Or IO.NotifyFilters.FileName),
            .IncludeSubdirectories = True
        }

        ' Add event handlers.
        AddHandler watcher.Changed, AddressOf OnChanged
        AddHandler watcher.Created, AddressOf OnChanged
        AddHandler watcher.Deleted, AddressOf OnChanged
        AddHandler watcher.Renamed, AddressOf OnRenamed

        ' Begin watching.
        watcher.EnableRaisingEvents = True

        'Keep track of each FileSystemWatcher
        If Not My.Settings.WatchPaths.Contains(watcher.Path) Then My.Settings.WatchPaths.Add(watcher.Path)
        'Keep track of each watcher
        watcherList.Add(watcher)
        Debug.Print("Watcher list: " & watcherList.Count & " " & pathToWatch)
        My.Settings.Save()
    End Sub

    Private Shared Sub OnChanged(source As Object, e As IO.FileSystemEventArgs)
        Debug.Print("Changed: " & e.FullPath)

        'If the file doesn't have an associated watcher
        If IsNothing(RemoveWatchPath(e.FullPath)) Then
            Debug.Print("File was outside watch lists, deleting any related handlers")
            'For each watcher
            For Each c In watcherList
                ' If the path contains the path of the Watcher
                If e.FullPath.Contains(c.Path) Then
                    Debug.Print("Removing handlers for " & c.Path)
                    ' Stop listening for changes in the files
                    '   by removing the events created for it
                    RemoveHandler c.Changed, AddressOf OnChanged
                    RemoveHandler c.Created, AddressOf OnChanged
                    RemoveHandler c.Deleted, AddressOf OnChanged
                    RemoveHandler c.Renamed, AddressOf OnRenamed
                    ' Stop saving the (now dead) watcher
                    watcherList.Remove(c)
                    ' Stop saving the path
                    My.Settings.WatchPaths.Remove(c.Path)
                    My.Settings.Save()
                    ' The old watch was removed, done, so exit
                    Exit For
                End If
            Next
        Else
            'Select based on type of change to file
            Select Case e.ChangeType
                Case IO.WatcherChangeTypes.Created
                    'Copy file
                    CopyFileToNetwork(e.FullPath, FinalDestinationFolder & RemoveWatchPath(e.FullPath))
                Case IO.WatcherChangeTypes.Changed
                    'Copy file (Overwrites any changes)
                    CopyFileToNetwork(e.FullPath, FinalDestinationFolder & RemoveWatchPath(e.FullPath))
                Case IO.WatcherChangeTypes.Deleted
                    Debug.Print("Deleting file " & FinalDestinationFolder & RemoveWatchPath(e.FullPath))
                    'Delete file
                    DeleteFile(FinalDestinationFolder & RemoveWatchPath(e.FullPath))
            End Select
        End If
    End Sub

    Public Shared Function RemoveWatchPath(filePathName As String) As String
        RemoveWatchPath = vbNullString
        'Get the path minus the path we are watching
        '       (ie: from 'C:\test\file.txt' to '\test\file.txt' b/c watching 'C:')
        For Each watchingPath In My.Settings.WatchPaths
            ' If the path has a watch path in it
            If filePathName.Contains(watchingPath) Then
                'Remove the watch path
                RemoveWatchPath = filePathName.Replace(watchingPath, "")
            End If
        Next
        If IsNothing(RemoveWatchPath) Or RemoveWatchPath = "" Then
            Return RemoveWatchPath
        Else
            Return IIf(Left(RemoveWatchPath, 1) = "\", Right(RemoveWatchPath, Len(RemoveWatchPath) - 1), RemoveWatchPath)
        End If
    End Function

    Private Shared Sub OnRenamed(source As Object, e As IO.RenamedEventArgs)
        Debug.Print("Renamed File: " & e.OldFullPath & " to " & e.Name)
        'If the network is connected
        If NetworkConnected(FinalDestinationFolder) Then
            'If the file is already on the network
            If My.Computer.FileSystem.FileExists(FinalDestinationFolder & e.OldName) And Not My.Computer.FileSystem.FileExists(FinalDestinationFolder & e.Name) Then
                ' Rename file on network
                My.Computer.FileSystem.RenameFile(FinalDestinationFolder & e.OldName, FinalDestinationFolder & e.Name)
            Else
                ' Copy file to the network
                CopyFileToNetwork(e.FullPath, FinalDestinationFolder & e.Name)
            End If
        Else

        End If
    End Sub

    Public Shared Sub RenameFile(oldPathFileName As String, newPathFileName As String)
        'If we're connected to the network
        If NetworkConnected(FinalDestinationFolder) Then
            If My.Computer.FileSystem.FileExists(oldPathFileName) Then
                'Rename the file
                My.Computer.FileSystem.RenameFile(oldPathFileName, newPathFileName)
            ElseIf My.Computer.FileSystem.DirectoryExists(oldPathFileName) Then
                'Rename the directory
                My.Computer.FileSystem.RenameDirectory(oldPathFileName, newPathFileName)
            End If
        Else
            'If it's a file
            If My.Computer.FileSystem.FileExists(oldPathFileName) Then
                'For each file to copy
                For Each c In My.Settings.FilesToCopy
                    'If the file is in the oldpathfile name
                    If oldPathFileName.Contains(c) Then
                        'Remove the file from the copy list
                        My.Settings.FilesToCopy.Remove(c)
                        'Add the new file to the copy list
                        My.Settings.FilesToCopy.Add(newPathFileName)
                    End If
                Next
            ElseIf My.Computer.FileSystem.DirectoryExists(oldPathFileName) Then
                'TODO? Leave it cause who cares about directories? What about reorganization of directories?
            End If
        End If
    End Sub

    Public Shared Sub CopyFileToNetwork(filePathName As String, destinationFileName As String)
        Debug.Print("Copying file: " & filePathName & vbCr & "    to: " & destinationFileName)
        If NetworkConnected(FinalDestinationFolder) Then
            'if the path is a file
            If My.Computer.FileSystem.FileExists(filePathName) Then
                'Copy the file
                Try
                    My.Computer.FileSystem.CopyFile(filePathName, destinationFileName, FileIO.UIOption.OnlyErrorDialogs, FileIO.UICancelOption.DoNothing)
                Catch e As Exception
                    Debug.Print("Error: " & e.Message)
                End Try
            ElseIf My.Computer.FileSystem.DirectoryExists(filePathName) Then
                My.Computer.FileSystem.CreateDirectory(destinationFileName)
            End If
        Else
            'If it's a file
            If My.Computer.FileSystem.FileExists(filePathName) Then
                'Add the newly created file to the list of copy files
                My.Settings.FilesToCopy.Add(filePathName)
            End If
        End If
    End Sub

    Public Shared Sub DeleteFile(filePathName As String)
        'if the network is connected
        If NetworkConnected(FinalDestinationFolder) Then
            'If the file exists
            If My.Computer.FileSystem.FileExists(filePathName) Then
                'Delete the file
                My.Computer.FileSystem.DeleteFile(filePathName, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            Else
                Debug.Print("File doesn't exist: " & filePathName)
            End If
        Else
            'if i already asked for the file to be created then
            If My.Settings.FilesToCopy.Contains(filePathName) Then
                'Remove it from the list to copy later (we deleted it!)
                My.Settings.FilesToCopy.Remove(filePathName)
            Else
                My.Settings.FilesToDelete.Add(filePathName)
            End If
        End If
        My.Settings.Save()
    End Sub

    Public Shared Function NetworkConnected(path As String) As Boolean
        'If the directory root exists (ie: 'C:\' 'D:\'  )
        NetworkConnected = IO.Directory.Exists(IO.Directory.GetDirectoryRoot(path))
    End Function

    'This is called by the Taskbar's background worker when it finds that the network is connected and wasn't previously
    Public Shared Sub OnNetworkReconnected()
        'For each File to delete
        For c = 0 To My.Settings.FilesToDelete.Count - 1
            'Delete the file if it exists
            DeleteFile(My.Settings.FilesToDelete(c))
            'If the file doesn't exist
            If Not My.Computer.FileSystem.FileExists(My.Settings.FilesToDelete(c)) Then
                ' Take it off the list
                My.Settings.FilesToDelete.Remove(c)
            End If
        Next
        ' For each file to copy
        For c = 0 To My.Settings.FilesToCopy.Count - 1
            'Copy the file after taking off the watch path (C:\) and adding the final path (Z:\)
            CopyFileToNetwork(My.Settings.FilesToCopy(c), FinalDestinationFolder & RemoveWatchPath(My.Settings.FilesToCopy(c)))
            ' If the file was copied to the backup folder
            If My.Computer.FileSystem.FileExists(FinalDestinationFolder & RemoveWatchPath(My.Settings.FilesToCopy(c))) Then
                'Take it off the list
                My.Settings.FilesToCopy.Remove(c)
            End If
        Next
    End Sub
End Class