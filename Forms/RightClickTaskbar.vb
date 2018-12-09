Imports System.ComponentModel

Public Class RightClickTaskbar
    Private deleteExtra As Boolean
    Private WithEvents backUpWorker As ComponentModel.BackgroundWorker

    Private Sub RightClickTaskbar_Load(sender As Object, e As EventArgs) Handles Me.Load
        ContextMenuStrip1.Show(Cursor.Position)
        Me.Height = ContextMenuStrip1.Height - 1
        Me.Width = ContextMenuStrip1.Width - 1
        Me.Left = ContextMenuStrip1.Left
        Me.Top = ContextMenuStrip1.Top
    End Sub

    Private Sub ContextMenuStrip1_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ContextMenuStrip1.ItemClicked
        Select Case e.ClickedItem.Text
            Case "Cache Box"
                AboutForm.Show()
            Case "Options"
                AppForm.visibilityCloak = True
                AppForm.Show()
            Case "Backup Now"
                Me.Hide()
                'Make sure there is a network connection to the backup folder before backing up
                If Watcher.networkConnection Then
                    ' If the background worker isn't initialized yet
                    If IsNothing(backUpWorker) Then
                        ' Initialize it
                        backUpWorker = New BackgroundWorker With {
                            .WorkerReportsProgress = True       ' It reports it's progress to a form
                        }
                    End If
                    ' If the backup isn't running yet
                    If Not backUpWorker.IsBusy Then
                        ' Start the backup
                        backUpWorker.RunWorkerAsync()
                    End If
                    BackgroundBackup.Show()
                    BackgroundBackup.titleLabel.Text = "Finding files to back up..."
                Else
                    MsgBox("Error: Backup Folder not found. Please check your connection and try again.", MsgBoxStyle.OkOnly, "Network Error")
                End If
            Case "Backup and Delete Extra"
                Me.Hide()
                If Watcher.networkConnection Then
                    If IsNothing(backUpWorker) Then
                        ' Initialize it
                        backUpWorker = New BackgroundWorker With {
                            .WorkerReportsProgress = True       ' It reports it's progress to a form
                        }
                    End If
                    ' If the backup isn't running yet
                    If Not backUpWorker.IsBusy() Then
                        deleteextra = True
                        backUpWorker.RunWorkerAsync()
                    End If
                    BackgroundBackup.Show()
                    BackgroundBackup.titleLabel.Text = "Finding files to back up..."
                Else
                    MsgBox("Error: Backup Folder not found. Please check your connection and try again.", MsgBoxStyle.OkOnly, "Network Error")
                End If
            Case "Exit"
                Me.Hide()
                AppForm.myicon.Visible = False
                AppForm.ShutdownSequence = True
                AppForm.shutdown()
        End Select
    End Sub

    Private Sub RightClickTaskbar_LostFocus(sender As Object, e As EventArgs) Handles Me.LostFocus
        Me.Hide()
    End Sub

    Private Sub backUpWorker_DoWork(sender As Object, e As DoWorkEventArgs) Handles backUpWorker.DoWork
        'If the network is connected
        If Watcher.NetworkConnected(My.Settings.FinalDestination) Then
            'get all the files from the backup folders
            Dim localCopy As New List(Of String)
            'Get all the files from the local folders
            For Each c In My.Settings.WatchPaths
                'Report which Path we're searching currently
                backUpWorker.ReportProgress(0, c)
                ' Add the files that are in that path by recursively searching
                localCopy.AddRange(RefreshBackup.testing(c))
            Next
            'For each local file
            For Each c In localCopy
                ' Get the matching name of the backup file
                Dim backupFile As String = My.Settings.FinalDestination & "\" & Watcher.RemoveWatchPath(c)
                ' Get the file write times
                Dim lastEditDate As Date = My.Computer.FileSystem.GetFileInfo(c).LastWriteTime
                Dim backupDate As Date = My.Computer.FileSystem.GetFileInfo(backupFile).LastWriteTime

                'If the backup copy contains the local copy & the last write times are within 10 seconds
                If My.Computer.FileSystem.FileExists(backupFile) And (lastEditDate - backupDate).Duration < New TimeSpan(0, 0, 10) Then
                    'This file is good (they both were last edited at the same time)
                    'Debug.Print("File times within 10 seconds!")
                Else
                    'Copy to new location
                    Watcher.CopyFileToNetwork(c, backupFile)
                End If
                ' Update the form's progress bar
                backUpWorker.ReportProgress((localCopy.IndexOf(c) / localCopy.Count) * 100, c)
            Next
            If deleteExtra Then
                Dim deleteFile As Boolean
                Dim backupcopy = RefreshBackup.testing(My.Settings.FinalDestination)
                'For each file
                For i = 0 To backupcopy.Count - 1
                    deleteFile = True

                    'For each watch path
                    For Each path In My.Settings.WatchPaths
                        'If it contains the file
                        If localCopy.Contains(path & Replace(backupcopy(i), My.Settings.FinalDestination, "")) Then
                            deleteFile = False
                            Exit For
                        End If
                    Next
                    If deleteFile Then
                        backUpWorker.ReportProgress(100 * i / (backupcopy.Count - 1), "Deleting : " & backupcopy(i))
                        Watcher.DeleteFile(backupcopy(i))
                    End If
                Next
                deleteEmptyDirectory(My.Settings.FinalDestination)
            End If
        Else

        End If
    End Sub

    Private Function deleteEmptyDirectory(c)
        Debug.Print(c)
        Dim fileCount = My.Computer.FileSystem.GetDirectoryInfo(c).GetFiles.Count
        Dim dirCount = My.Computer.FileSystem.GetDirectoryInfo(c).GetDirectories.Count
        ' If it has no files or directories
        If fileCount = 0 And dirCount = 0 Then
            My.Computer.FileSystem.GetDirectoryInfo(c).Delete()
            Return True
        ElseIf fileCount = 0 Then
            Dim allEmpty = True
            For i = 0 To My.Computer.FileSystem.GetDirectoryInfo(c).GetDirectories.Count - 1
                If My.Computer.FileSystem.GetDirectories(c).Count > i Then
                    If deleteEmptyDirectory(My.Computer.FileSystem.GetDirectories(c)(i)) Then
                        'i = i - 1
                    Else
                        allEmpty = False
                    End If
                Else
                    allEmpty = False
                End If
            Next
            If allEmpty Then
                My.Computer.FileSystem.GetDirectoryInfo(c).Delete(True)
                Return True
            Else
                Return False
            End If
        Else
            For Each directory In My.Computer.FileSystem.GetDirectories(c)
                deleteEmptyDirectory(directory)
            Next
            Return False
        End If
    End Function

    Private Sub backUpWorker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles backUpWorker.RunWorkerCompleted
        AppForm.myicon.ShowBalloonTip(2000, "Back Up Complete!", "Your files have been fully backed up.", ToolTipIcon.Info)
        BackgroundBackup.Hide()
    End Sub

    Private Sub backUpWorker_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles backUpWorker.ProgressChanged
        BackgroundBackup.titleLabel.Text = "Backing Up..."
        BackgroundBackup.ProgressBar1.Value = e.ProgressPercentage
        BackgroundBackup.filePathNameLabel.Text = e.UserState.ToString
    End Sub
End Class