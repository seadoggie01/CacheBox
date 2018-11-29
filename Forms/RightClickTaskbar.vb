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
                If IsNothing(backUpWorker) Then
                    backUpWorker = New BackgroundWorker
                    backUpWorker.WorkerSupportsCancellation = True
                    backUpWorker.WorkerReportsProgress = True
                End If
                If backUpWorker.IsBusy Then backUpWorker.CancelAsync()
                If Not backUpWorker.IsBusy Then
                    backUpWorker.RunWorkerAsync()
                Else
                    BackgroundBackup.Show()
                End If
                BackgroundBackup.Show()
                BackgroundBackup.titleLabel.Text = "Finding files to back up..."
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
        'get all the files from the backup folders
        'Dim backupCopy As List(Of String) = RefreshBackup.testing(Watcher.FinalDestinationFolder)
        Dim localCopy As New List(Of String)
        'Get all the files from the local folders
        For Each c In My.Settings.WatchPaths
            backUpWorker.ReportProgress(0, c)
            localCopy = RefreshBackup.testing(c)
        Next
        'For each local file
        For Each c In localCopy
            'If My.Computer.FileSystem.GetFileInfo(c).Directory.LastWriteTime Then

            'End If
            backUpWorker.ReportProgress((localCopy.IndexOf(c) / localCopy.Count) * 100, c)
            'Get the matching name of the backup file
            Dim backupFile As String = My.Settings.FinalDestination & Watcher.RemoveWatchPath(c)
            My.Computer.FileSystem.GetFileInfo(c).Refresh()
            My.Computer.FileSystem.GetFileInfo(backupFile).Refresh()
            'If the backup copy contains the local copy & the last write times match
            If My.Computer.FileSystem.FileExists(backupFile) Then 'And My.Computer.FileSystem.GetFileInfo(c).LastWriteTime <= My.Computer.FileSystem.GetFileInfo(backupFile).LastWriteTime Then
                'This file is good (they both were last edited at the same time)
                My.Computer.FileSystem.GetFileInfo(backupFile).LastWriteTime = My.Computer.FileSystem.GetFileInfo(c).LastWriteTime
            Else
                'Copy to new location
                Debug.Print(My.Computer.FileSystem.GetFileInfo(c).LastWriteTime & " and " & My.Computer.FileSystem.GetFileInfo(backupFile).LastWriteTime)
                Watcher.CopyFileToNetwork(c, backupFile)
                'Update last write time (Last Access Time doesn't work)
                If My.Computer.FileSystem.FileExists(backupFile) Then My.Computer.FileSystem.GetFileInfo(backupFile).LastWriteTime = My.Computer.FileSystem.GetFileInfo(c).LastWriteTime
            End If
        Next
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