<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OptionsUI
    Inherits JHSoftware.SimpleDNS.Plugin.OptionsUI

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
    Me.components = New System.ComponentModel.Container
    Me.Label1 = New System.Windows.Forms.Label
    Me.txtFile = New System.Windows.Forms.TextBox
    Me.btnBrowse = New System.Windows.Forms.Button
    Me.chkMonitor = New System.Windows.Forms.CheckBox
    Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
    Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
    Me.linkSpecs = New System.Windows.Forms.LinkLabel
    Me.SuspendLayout()
    '
    'Label1
    '
    Me.Label1.AutoSize = True
    Me.Label1.Location = New System.Drawing.Point(-3, 0)
    Me.Label1.Name = "Label1"
    Me.Label1.Size = New System.Drawing.Size(128, 13)
    Me.Label1.TabIndex = 0
    Me.Label1.Text = "Domain Blacklist data file:"
    '
    'txtFile
    '
    Me.txtFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.txtFile.Location = New System.Drawing.Point(0, 16)
    Me.txtFile.Name = "txtFile"
    Me.txtFile.Size = New System.Drawing.Size(306, 20)
    Me.txtFile.TabIndex = 1
    '
    'btnBrowse
    '
    Me.btnBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnBrowse.Location = New System.Drawing.Point(312, 14)
    Me.btnBrowse.Name = "btnBrowse"
    Me.btnBrowse.Size = New System.Drawing.Size(26, 23)
    Me.btnBrowse.TabIndex = 2
    Me.btnBrowse.Text = "..."
    Me.ToolTip1.SetToolTip(Me.btnBrowse, "Browse...")
    Me.btnBrowse.UseVisualStyleBackColor = True
    '
    'chkMonitor
    '
    Me.chkMonitor.AutoSize = True
    Me.chkMonitor.Checked = True
    Me.chkMonitor.CheckState = System.Windows.Forms.CheckState.Checked
    Me.chkMonitor.Location = New System.Drawing.Point(0, 54)
    Me.chkMonitor.Margin = New System.Windows.Forms.Padding(3, 15, 3, 3)
    Me.chkMonitor.Name = "chkMonitor"
    Me.chkMonitor.Size = New System.Drawing.Size(252, 17)
    Me.chkMonitor.TabIndex = 3
    Me.chkMonitor.Text = "Automatically re-load data file when it is updated"
    Me.chkMonitor.UseVisualStyleBackColor = True
    '
    'OpenFileDialog1
    '
    Me.OpenFileDialog1.DefaultExt = "txt"
    Me.OpenFileDialog1.FileName = "OpenFileDialog1"
    Me.OpenFileDialog1.Filter = "All files|*.*"
    Me.OpenFileDialog1.Title = "Select Domain Blacklist data file"
    '
    'linkSpecs
    '
    Me.linkSpecs.AutoSize = True
    Me.linkSpecs.Location = New System.Drawing.Point(-3, 111)
    Me.linkSpecs.Name = "linkSpecs"
    Me.linkSpecs.Size = New System.Drawing.Size(145, 13)
    Me.linkSpecs.TabIndex = 4
    Me.linkSpecs.TabStop = True
    Me.linkSpecs.Text = "Data file format specifications"
    '
    'OptionsUI
    '
    Me.Controls.Add(Me.linkSpecs)
    Me.Controls.Add(Me.chkMonitor)
    Me.Controls.Add(Me.btnBrowse)
    Me.Controls.Add(Me.txtFile)
    Me.Controls.Add(Me.Label1)
    Me.Name = "OptionsUI"
    Me.Size = New System.Drawing.Size(338, 132)
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents Label1 As System.Windows.Forms.Label
  Friend WithEvents txtFile As System.Windows.Forms.TextBox
  Friend WithEvents btnBrowse As System.Windows.Forms.Button
  Friend WithEvents chkMonitor As System.Windows.Forms.CheckBox
  Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
  Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
  Friend WithEvents linkSpecs As System.Windows.Forms.LinkLabel

End Class
