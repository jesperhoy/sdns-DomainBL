Imports System.Threading.Tasks
Imports JHSoftware.SimpleDNS.Plugin

Public Class DomBlacklist
  Implements ILookupHost
  Implements IOptionsUI
  Implements IListsDomainName

  Private MyCfg As clConfig
  Private MyData As DataSet
  Private LastReload As DateTime

  Private WithEvents fMon As System.IO.FileSystemWatcher

  Public Property Host As IHost Implements IPlugInBase.Host

#Region "not implemented"
  Public Function SaveState() As String Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.SaveState
    Return ""
  End Function

  Public Sub LoadState(ByVal state As String) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.LoadState
  End Sub

  Public Function InstanceConflict(ByVal config1 As String, ByVal config2 As String, ByRef errorMsg As String) As Boolean Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.InstanceConflict
    Return False
  End Function
#End Region

  Public Function GetPlugInTypeInfo() As JHSoftware.SimpleDNS.Plugin.IPlugInBase.PlugInTypeInfo Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.GetPlugInTypeInfo
    With GetPlugInTypeInfo
      .Name = "Domain Blacklist"
      .Description = "Serves list of blocked domain names from a file"
      .InfoURL = "https://simpledns.plus/kb/171/domain-blacklist-plug-in"
    End With
  End Function

  Private Function StartService() As Task Implements IPlugInBase.StartService
    MyData = LoadDataSet()
    If MyCfg.Monitor Then
      fMon = New System.IO.FileSystemWatcher
      fMon.Path = System.IO.Path.GetDirectoryName(MyCfg.FileName)
      fMon.Filter = System.IO.Path.GetFileName(MyCfg.FileName)
      fMon.IncludeSubdirectories = False
      fMon.NotifyFilter = IO.NotifyFilters.LastWrite
      fMon.EnableRaisingEvents = True
    End If
    Return Task.CompletedTask
  End Function

  Public Sub StopService() Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.StopService
    MyData = Nothing
    If fMon IsNot Nothing Then
      fMon.EnableRaisingEvents = False
      fMon.Dispose()
      fMon = Nothing
    End If
  End Sub

  Public Function LookupHost(req As IDNSRequest) As Task(Of LookupResult(Of SdnsIP)) Implements ILookupHost.LookupHost
    Return Task.FromResult(LookupHost2(req))
  End Function
  Public Function LookupHost2(req As IDNSRequest) As LookupResult(Of SdnsIP)
    If MyData.XDate > Now AndAlso MyData.Contains(req.QName) Then
      If req.QType = DNSRecType.A Then
        Return New LookupResult(Of SdnsIP) With {.Value = MyData.IP4, .TTL = MyData.TTL}
      Else
        Return New LookupResult(Of SdnsIP) With {.Value = MyData.IP6, .TTL = MyData.TTL}
      End If
    Else
      Return Nothing
    End If
  End Function

  Public Sub LoadConfig(ByVal config As String, ByVal instanceID As Guid, ByVal dataPath As String) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.LoadConfig
    MyCfg = clConfig.FromString(config)
  End Sub

  Private Function GetOptionsUI(instanceID As Guid, dataPath As String) As Plugin.OptionsUI Implements IOptionsUI.GetOptionsUI
    Return New OptionsUI
  End Function

  Friend Function LoadDataSet() As DataSet
    LastReload = DateTime.UtcNow

    Dim file As System.IO.FileStream
    Dim failCt As Integer
    Do
      Try
        file = New System.IO.FileStream(MyCfg.FileName, IO.FileMode.Open)
        Exit Do
      Catch ex As System.IO.FileNotFoundException
        Throw ex
      Catch ex As System.IO.IOException
        failCt += 1
        REM continue trying for 5 seconds (20 x 1/4 second)
        If failCt >= 20 Then Throw ex
        Threading.Thread.Sleep(250)
      End Try
    Loop

    Dim tmpDS As New DataSet
    tmpDS.Load(file)

    If tmpDS.Expired Then Host.LogLine("Data not loaded - data file is past expiration date")

    Return tmpDS
  End Function

  Private Sub fMon_Changed(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs) Handles fMon.Changed
    Try

      If DateTime.UtcNow.Subtract(LastReload).TotalSeconds < 5 Then Exit Sub
      Host.LogLine("Data file update detected - reloading")
      Try
        MyData = LoadDataSet()
      Catch ex As Exception
        MyData = New DataSet
        Host.LogLine("Error reloading data file: " & ex.Message)
      End Try

    Catch ex As Exception
      Host.AsyncError(ex)
    End Try
  End Sub

  Private Function ListsDomainName(domain As DomName) As Task(Of Boolean) Implements IListsDomainName.ListsDomainName
    Return Task.FromResult((MyData.XDate > Now AndAlso MyData.Contains(domain)))
  End Function

End Class


