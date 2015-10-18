Public Class OptionsUI

  Public Overrides Sub LoadData(ByVal config As String)
    If config Is Nothing Then Exit Sub 'new instance
    Dim cfg = clConfig.FromString(config)
    txtFile.Text = cfg.FileName
    chkMonitor.Checked = cfg.Monitor
  End Sub

  Public Overrides Function SaveData() As String
    Dim cfg As New clConfig
    cfg.FileName = txtFile.Text.Trim
    cfg.Monitor = chkMonitor.Checked
    Return cfg.ToString
  End Function

  Public Overrides Function ValidateData() As Boolean
    If txtFile.Text.Trim.Length = 0 Then
      MessageBox.Show("Data file cannot be empty!", "Domain Blacklist", MessageBoxButtons.OK, MessageBoxIcon.Error)
      Return False
    End If
    If Not RemoteGUI Then
      If Not My.Computer.FileSystem.FileExists(txtFile.Text.Trim) Then
        MessageBox.Show("Data file does not exist!", "Domain Blacklist", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return False
      End If
    End If
    Return True
  End Function

  Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
    If RemoteGUI Then MessageBox.Show("This function is not available during remote management", _
                                  "Browse for file/folder", MessageBoxButtons.OK, _
                                  MessageBoxIcon.Warning) : Exit Sub

    Try
      OpenFileDialog1.FileName = txtFile.Text.Trim
    Catch
    End Try
    If OpenFileDialog1.ShowDialog() = DialogResult.Cancel Then Exit Sub
    txtFile.Text = OpenFileDialog1.FileName
  End Sub

  Private Sub linkSpecs_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles linkSpecs.LinkClicked
    Dim url = "http://www.simpledns.com/plugin-domainbl#filespecs"
    Try
      System.Diagnostics.Process.Start(url)
    Catch ex As Exception
      MessageBox.Show("Could not open the following Internet address in your default Internet browser:" & vbCrLf & _
                   url & vbCrLf & vbCrLf & _
                   "Error: " & ex.Message, "Domain Blacklist", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Try
  End Sub
End Class
