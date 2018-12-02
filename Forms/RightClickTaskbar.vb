Imports System.ComponentModel

Public Class RightClickTaskbar

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
                Dim backupFile As String = My.Settings.FinalDestination & Watcher.RemoveWatchPath(c)
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
        Else

        End If
    End Sub

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