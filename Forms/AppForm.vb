Imports System.ComponentModel

Public Class AppForm
    Public visibilityCloak As Boolean = True
    Private WithEvents looper As ComponentModel.BackgroundWorker
    Public WithEvents myicon As New NotifyIcon
    Public ShutdownSequence As Boolean
    Private taskbarColor As Color = SystemColors.Control
    Private taskbarTextColor As Color = SystemColors.Control

    Public Sub New()

        ' -------------------------------------- '
        ' This call is required by the designer.
        InitializeComponent()
        ' -------------------------------------- '

        'Load the form
        'Create background worker to watch for file changes
        If looper Is Nothing Then
            looper = New ComponentModel.BackgroundWorker
            looper.WorkerSupportsCancellation = True
        End If
        'If the looper isn't busy then
        If Not looper.IsBusy Then
            'if there are no paths to watch
            If IsNothing(My.Settings.WatchPaths) Then
                'Show the form
                visibilityCloak = True
                Me.Show()
                My.Settings.WatchPaths = New Specialized.StringCollection
            Else
                'Don't show the form
                visibilityCloak = False
                'Add WatchPaths to Form
                For i = 0 To My.Settings.WatchPaths.Count - 1
                    ListBox1.Items.Add(My.Settings.WatchPaths(i))
                    Watcher.Main(My.Settings.WatchPaths(i))
                Next
                'Run the background worker
                looper.RunWorkerAsync()
            End If
        End If
        'Setup my taskbar icon
        myicon.Icon = Icon.ExtractAssociatedIcon("notification.ico")
        myicon.Text = "Cache Box" & vbNewLine & "Deliciously simple!"
        myicon.Visible = True

        'Initilize strings if necessary
        If IsNothing(My.Settings.FilesToCopy) Then My.Settings.FilesToCopy = New Specialized.StringCollection
        If IsNothing(My.Settings.FilesToDelete) Then My.Settings.FilesToDelete = New Specialized.StringCollection

        'Add Destination to form
        backupTextBox.Text = My.Settings.FinalDestination
        colorComboBox.Text = IIf(My.Settings.ColorScheme <> "", My.Settings.ColorScheme, "Grey")
    End Sub

    Protected Overrides Sub SetVisibleCore(value As Boolean)
        'If I'm wearing my invisibility cloak
        MyBase.SetVisibleCore(visibilityCloak)
    End Sub

    Private Sub looper_DoWork(sender As Object, e As DoWorkEventArgs) Handles looper.DoWork
        'If the looper has been canceled then
        If looper.CancellationPending Then
            e.Cancel = True
            Return
        Else
            'for each path to watch
            For Each c In Watcher.WatchPath
                'Register the event handlers
                Watcher.Main(c)
            Next
            Do
                'Pause for 2 seconds
                System.Threading.Thread.Sleep(2000)
                'If we're connected to the network and weren't previously
                If Watcher.NetworkConnected(Watcher.FinalDestinationFolder) Then
                    If Watcher.networkConnection = False Then
                        Watcher.networkConnection = True
                        'Call network reconnect (which will make this thread busy and not call another OnNetworkReconnected())
                        Watcher.OnNetworkReconnected()
                    End If
                Else
                    Watcher.networkConnection = False
                End If
            Loop Until looper.CancellationPending
            If looper.CancellationPending Then
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub Looper_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles looper.RunWorkerCompleted
        If e.Cancelled Then
            Debug.Print("It was cancelled!?!?! How!?")
        Else
            'Do stuff here
        End If
    End Sub

    Private Sub myicon_DoubleClick(sender As Object, e As EventArgs) Handles myicon.DoubleClick
        'Take off your invisibility cloak :(
        visibilityCloak = True
        SetVisibleCore(True)
        Me.Show()
    End Sub

    Private Sub myicon_Click(sender As Object, e As MouseEventArgs) Handles myicon.Click
        If e.Button = Windows.Forms.MouseButtons.Right Then
            Dim c As new RightClickTaskbar
            c.Show()
            c.ContextMenuStrip1.BackColor = taskbarColor
            c.ContextMenuStrip1.ForeColor = taskbarTextColor
            c.BackColor = taskbarColor
        End If
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        'Hide the Form
        visibilityCloak = False
        SetVisibleCore(False)
        Me.Hide()
        'As long as I'm not trying to shut the whole thing down
        If Not ShutdownSequence Then
            'Cancel the Closing of the form
            e.Cancel = True
            colorComboBox.Text = My.Settings.ColorScheme
        Else
            'Remove icon, we're outta here!
            myicon.Visible = False
        End If
    End Sub

    Private Sub deletePathBttn_Click(sender As Object, e As EventArgs) Handles deletePathBttn.Click
        'If there is an item selected
        If Not IsNothing(ListBox1.SelectedItem) Then
            'If it is a path that we are watching, then remove it
            If My.Settings.WatchPaths.Contains(ListBox1.SelectedItem.ToString) Then My.Settings.WatchPaths.Remove(ListBox1.SelectedItem.ToString)
            'Delete it
            ListBox1.Items.Remove(ListBox1.SelectedItem)
        End If
    End Sub

    Private Sub addPathBttn_Click(sender As Object, e As EventArgs) Handles addPathBttn.Click
        Dim c As New FolderBrowserDialog
        c.ShowNewFolderButton = True
        c.Description = "Select a folder to copy files from..."
        If c.ShowDialog = DialogResult.OK Then
            ListBox1.Items.Add(c.SelectedPath.ToString)
            Watcher.WatchPath.Add(c.SelectedPath.ToString)
            Watcher.Main(c.SelectedPath.ToString)
            My.Settings.WatchPaths.Add(c.SelectedPath)
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        'If the shortcut is exists
        If My.Computer.FileSystem.FileExists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) & "\CacheBox.lnk") Then
            'Delete it
            My.Computer.FileSystem.DeleteFile(Environment.GetFolderPath(Environment.SpecialFolder.Startup) & "\CacheBox.lnk")
        End If

        'If the check box is checked (Saying to open on startup)
        If CheckBox1.Checked Then
            'Create a shortcut
            Dim startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup)
            'Create a Wscript object (a powerful shell command thingy... see VBScript! :D)
            Dim Shell = CreateObject("Wscript.Shell")
            ' The full path for the shortcut to be in
            Dim shortCutLinkFilePath = startupFolderPath & "\CacheBox.lnk"
            'Create the shortcut, naming it like C:\<StartupFolder>\CacheBox.lnk
            Dim shortCutProgram = Shell.CreateShortcut(shortCutLinkFilePath)
            ' Give it a description
            shortCutProgram.Description = "A file backing up program"
            ' When the program is started, tell it which directory to call home
            shortCutProgram.WorkingDirectory = Application.StartupPath
            ' The actaull thing we're pointing to (The cachebox.exe)
            shortCutProgram.TargetPath = Application.ExecutablePath
            ' Save the shortcut
            shortCutProgram.Save()
        End If
    End Sub

    Private Sub backupTextBox_Validated(sender As Object, e As EventArgs) Handles backupTextBox.Validated
        ' If the directory exists
        If My.Computer.FileSystem.DirectoryExists(backupTextBox.Text) Then
            ' Set the FinalDestination
            My.Settings.FinalDestination = backupTextBox.Text
        Else
            ' If they tried to put something in
            If backupTextBox.Text <> "" Then
                ' Tell them it isn't valid
                MsgBox("This is an invalid backup path.", vbOK, "Invalid Path")
                ' Move back to the textbox
                backupTextBox.Select()
            End If
        End If
    End Sub

    Private Sub backupTextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles backupTextBox.KeyPress
        ' If you press enter
        If e.KeyChar = vbCr Then
            ' Validate the backup location and set
            backupTextBox_Validated(Me, EventArgs.Empty)
        End If
    End Sub

    Private Sub backupTextBox_DoubleClick(sender As Object, e As EventArgs) Handles backupTextBox.DoubleClick
        Dim c As New FolderBrowserDialog
        'Backup an empty folder? Sure, why not?
        c.ShowNewFolderButton = True
        c.Description = "Select a folder to copy files to..."
        'Show the folder browser (Which I think is ugly and annoying 
        '   cause I can't copy/paste a folder path, but whatever, I'm just the dev)
        If c.ShowDialog = DialogResult.OK Then
            'Set the textbox to the selected path if they clicked "OK" at the end
            backupTextBox.Text = c.SelectedPath
        End If
    End Sub

    Private Sub saveBttn_Click(sender As Object, e As EventArgs) Handles saveBttn.Click
        'Check that backup folder is selected and a folder to backup was selected
        If My.Settings.FinalDestination <> "" And My.Settings.WatchPaths.Count > 0 Then
            ' Save any settings that were changed
            My.Settings.Save()
            ' Hide the form
            visibilityCloak = False
            Me.Hide()
        Else
            'Alert the user folders are missing
            MsgBox("All folder paths need to be input before closing." & vbCrLf & "Close to revert your settings.", MsgBoxStyle.OkOnly, "Missing Folders")
        End If
    End Sub

    Private Sub colorComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles colorComboBox.SelectedIndexChanged
        Dim formColor As Color
        Dim textboxColor As Color
        Dim bttnColor As Color
        Dim bttnTxtColor As Color
        Dim textColor As Color
        'Based on the choice of the combo box
        Select Case colorComboBox.SelectedItem
            'Set a color variable to be used below
            Case "Black"
                formColor = Color.FromArgb(51, 50, 50)
                textboxColor = Color.FromArgb(228, 214, 167)
                bttnColor = Color.FromArgb(155, 41, 21)
                textColor = Color.FromArgb(255, 255, 255)
                bttnTxtColor = Color.Black
            Case "Purple"
                formColor = Color.FromArgb(175, 175, 220)
                textboxColor = Color.FromArgb(183, 211, 242)
                bttnColor = Color.FromArgb(138, 132, 226)
                textColor = Color.FromArgb(0, 0, 0)
                bttnTxtColor = Color.Black
            Case "Green"
                formColor = Color.FromArgb(127, 182, 141)
                textboxColor = Color.FromArgb(189, 255, 253)
                bttnColor = Color.FromArgb(159, 255, 245)
                textColor = Color.FromArgb(0, 0, 0)
                bttnTxtColor = Color.Black
            Case "Yellow"
                formColor = Color.FromArgb(233, 206, 44)
                textboxColor = Color.FromArgb(229, 249, 147)
                bttnColor = Color.FromArgb(191, 33, 30)
                textColor = Color.FromArgb(0, 0, 0)
                bttnTxtColor = Color.Black
            Case "Grey"
                formColor = SystemColors.Control
                textboxColor = SystemColors.Window
                bttnColor = SystemColors.ControlLight
                textColor = Color.FromArgb(0, 0, 0)
                bttnTxtColor = Color.Black
        End Select

        'Apply the colors to the form
        Me.BackColor = formColor
        taskbarColor = formColor

        backupTextBox.BackColor = textboxColor
        ListBox1.BackColor = textboxColor
        colorComboBox.BackColor = textboxColor

        addPathBttn.BackColor = bttnColor
        deletePathBttn.BackColor = bttnColor
        saveBttn.BackColor = bttnColor
        addPathBttn.ForeColor = bttnTxtColor
        deletePathBttn.ForeColor = bttnTxtColor
        saveBttn.ForeColor = bttnTxtColor

        optionsLabel.ForeColor = textColor
        backupLocationLabel.ForeColor = textColor
        backupFoldersLabel.ForeColor = textColor
        colorSchemeLabel.ForeColor = textColor
        CheckBox1.ForeColor = textColor
        taskbarTextColor = textColor

        'Update the color scheme preference
        My.Settings.ColorScheme = colorComboBox.SelectedText
    End Sub

    Private Sub AppForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        colorComboBox.Text = My.Settings.ColorScheme
    End Sub

    Public Sub shutdown()
        visibilityCloak = False
        Me.Close()
        Application.Exit()
    End Sub

    Private Sub CheckBox1_Click(sender As Object, e As EventArgs) Handles CheckBox1.Click
        My.Settings.LaunchOnStartup = CheckBox1.Checked
    End Sub

    Private Sub ListBox1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        ' TODO: Edit file paths on double click
    End Sub
End Class
