<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AppForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.deletePathBttn = New System.Windows.Forms.Button()
        Me.addPathBttn = New System.Windows.Forms.Button()
        Me.backupFoldersLabel = New System.Windows.Forms.Label()
        Me.backupTextBox = New System.Windows.Forms.TextBox()
        Me.backupLocationLabel = New System.Windows.Forms.Label()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.optionsLabel = New System.Windows.Forms.Label()
        Me.saveBttn = New System.Windows.Forms.Button()
        Me.colorSchemeLabel = New System.Windows.Forms.Label()
        Me.colorComboBox = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'ListBox1
        '
        Me.ListBox1.BackColor = System.Drawing.SystemColors.Window
        Me.ListBox1.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.ItemHeight = 16
        Me.ListBox1.Location = New System.Drawing.Point(12, 107)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(646, 148)
        Me.ListBox1.TabIndex = 0
        '
        'deletePathBttn
        '
        Me.deletePathBttn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.deletePathBttn.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.deletePathBttn.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.deletePathBttn.Location = New System.Drawing.Point(12, 260)
        Me.deletePathBttn.Name = "deletePathBttn"
        Me.deletePathBttn.Size = New System.Drawing.Size(106, 26)
        Me.deletePathBttn.TabIndex = 2
        Me.deletePathBttn.Text = "Remove Folder"
        Me.deletePathBttn.UseVisualStyleBackColor = False
        '
        'addPathBttn
        '
        Me.addPathBttn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.addPathBttn.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.addPathBttn.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.addPathBttn.Location = New System.Drawing.Point(562, 260)
        Me.addPathBttn.Name = "addPathBttn"
        Me.addPathBttn.Size = New System.Drawing.Size(96, 26)
        Me.addPathBttn.TabIndex = 3
        Me.addPathBttn.Text = "Add Folder"
        Me.addPathBttn.UseVisualStyleBackColor = False
        '
        'backupFoldersLabel
        '
        Me.backupFoldersLabel.AutoSize = True
        Me.backupFoldersLabel.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.backupFoldersLabel.Location = New System.Drawing.Point(9, 88)
        Me.backupFoldersLabel.Name = "backupFoldersLabel"
        Me.backupFoldersLabel.Size = New System.Drawing.Size(122, 16)
        Me.backupFoldersLabel.TabIndex = 4
        Me.backupFoldersLabel.Text = "Folders To Back Up"
        '
        'backupTextBox
        '
        Me.backupTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.backupTextBox.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.backupTextBox.Location = New System.Drawing.Point(12, 63)
        Me.backupTextBox.Name = "backupTextBox"
        Me.backupTextBox.Size = New System.Drawing.Size(646, 22)
        Me.backupTextBox.TabIndex = 5
        '
        'backupLocationLabel
        '
        Me.backupLocationLabel.AutoSize = True
        Me.backupLocationLabel.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.backupLocationLabel.Location = New System.Drawing.Point(12, 42)
        Me.backupLocationLabel.Name = "backupLocationLabel"
        Me.backupLocationLabel.Size = New System.Drawing.Size(105, 16)
        Me.backupLocationLabel.TabIndex = 6
        Me.backupLocationLabel.Text = "Backup Location"
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.CheckBox1.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CheckBox1.Location = New System.Drawing.Point(12, 326)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(229, 20)
        Me.CheckBox1.TabIndex = 7
        Me.CheckBox1.Text = "Launch on Startup (Recommended)"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'optionsLabel
        '
        Me.optionsLabel.Dock = System.Windows.Forms.DockStyle.Top
        Me.optionsLabel.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.optionsLabel.Location = New System.Drawing.Point(0, 0)
        Me.optionsLabel.Name = "optionsLabel"
        Me.optionsLabel.Size = New System.Drawing.Size(670, 42)
        Me.optionsLabel.TabIndex = 8
        Me.optionsLabel.Text = "Options"
        Me.optionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'saveBttn
        '
        Me.saveBttn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.saveBttn.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.saveBttn.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.saveBttn.Location = New System.Drawing.Point(289, 342)
        Me.saveBttn.Name = "saveBttn"
        Me.saveBttn.Size = New System.Drawing.Size(96, 26)
        Me.saveBttn.TabIndex = 9
        Me.saveBttn.Text = "Save"
        Me.saveBttn.UseVisualStyleBackColor = False
        '
        'colorSchemeLabel
        '
        Me.colorSchemeLabel.AutoSize = True
        Me.colorSchemeLabel.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.colorSchemeLabel.Location = New System.Drawing.Point(12, 299)
        Me.colorSchemeLabel.Name = "colorSchemeLabel"
        Me.colorSchemeLabel.Size = New System.Drawing.Size(90, 16)
        Me.colorSchemeLabel.TabIndex = 10
        Me.colorSchemeLabel.Text = "Color Scheme"
        '
        'colorComboBox
        '
        Me.colorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.colorComboBox.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.colorComboBox.FormattingEnabled = True
        Me.colorComboBox.Items.AddRange(New Object() {"Grey", "Green", "Yellow", "Purple", "Black"})
        Me.colorComboBox.Location = New System.Drawing.Point(108, 299)
        Me.colorComboBox.Name = "colorComboBox"
        Me.colorComboBox.Size = New System.Drawing.Size(111, 24)
        Me.colorComboBox.TabIndex = 11
        '
        'AppForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ClientSize = New System.Drawing.Size(670, 380)
        Me.Controls.Add(Me.colorComboBox)
        Me.Controls.Add(Me.colorSchemeLabel)
        Me.Controls.Add(Me.saveBttn)
        Me.Controls.Add(Me.optionsLabel)
        Me.Controls.Add(Me.CheckBox1)
        Me.Controls.Add(Me.backupLocationLabel)
        Me.Controls.Add(Me.backupTextBox)
        Me.Controls.Add(Me.backupFoldersLabel)
        Me.Controls.Add(Me.addPathBttn)
        Me.Controls.Add(Me.deletePathBttn)
        Me.Controls.Add(Me.ListBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "AppForm"
        Me.ShowIcon = False
        Me.Text = "Cache Box"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ListBox1 As ListBox
    Friend WithEvents deletePathBttn As Button
    Friend WithEvents addPathBttn As Button
    Friend WithEvents backupFoldersLabel As Label
    Friend WithEvents backupTextBox As TextBox
    Friend WithEvents backupLocationLabel As Label
    Friend WithEvents CheckBox1 As CheckBox
    Friend WithEvents optionsLabel As Label
    Friend WithEvents saveBttn As Button
    Friend WithEvents colorSchemeLabel As Label
    Friend WithEvents colorComboBox As ComboBox
End Class
