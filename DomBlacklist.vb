Imports JHSoftware.SimpleDNS.Plugin

Public Class DomBlacklist
  Implements IGetHostPlugIn
  Implements IListsDomainName

  Private MyCfg As clConfig
  Private MyData As DataSet
  Private LastReload As DateTime

  Private WithEvents fMon As System.IO.FileSystemWatcher

#Region "events"
  Public Event LogLine(ByVal text As String) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.LogLine
  Public Event AsyncError(ByVal ex As System.Exception) Implements JHSoftware.SimpleDNS.Plugin.IGetHostPlugIn.AsyncError
  Public Event SaveConfig(ByVal config As String) Implements JHSoftware.SimpleDNS.Plugin.IGetHostPlugIn.SaveConfig
#End Region

#Region "not implemented"
  Public Sub LookupTXT(ByVal req As IDNSRequest, ByRef resultText As String, ByRef resultTTL As Integer) Implements JHSoftware.SimpleDNS.Plugin.IGetHostPlugIn.LookupTXT
    Throw New NotSupportedException
  End Sub

  Public Sub LookupReverse(ByVal req As IDNSRequest, ByRef resultName As JHSoftware.SimpleDNS.Plugin.DomainName, ByRef resultTTL As Integer) Implements JHSoftware.SimpleDNS.Plugin.IGetHostPlugIn.LookupReverse
    Throw New NotSupportedException
  End Sub

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
      .InfoURL = "http://www.simpledns.com/plugin-domainbl"
      .ConfigFile = False
    End With
  End Function

  Public Function GetDNSAskAbout() As DNSAskAboutGH Implements JHSoftware.SimpleDNS.Plugin.IGetHostPlugIn.GetDNSAskAbout
    With GetDNSAskAbout
      .ForwardIPv4 = True
      .ForwardIPv6 = True
    End With
  End Function

  Public Sub StartService() Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.StartService
    MyData = LoadDataSet()

    If MyCfg.Monitor Then
      fMon = New System.IO.FileSystemWatcher
      fMon.Path = System.IO.Path.GetDirectoryName(MyCfg.FileName)
      fMon.Filter = System.IO.Path.GetFileName(MyCfg.FileName)
      fMon.IncludeSubdirectories = False
      fMon.NotifyFilter = IO.NotifyFilters.LastWrite
      fMon.EnableRaisingEvents = True
    End If
  End Sub

  Public Sub StopService() Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.StopService
    MyData = Nothing
    If fMon IsNot Nothing Then
      fMon.EnableRaisingEvents = False
      fMon.Dispose()
      fMon = Nothing
    End If
  End Sub

  Public Sub Lookup(ByVal req As IDNSRequest, ByRef resultIP As IPAddress, ByRef resultTTL As Integer) Implements JHSoftware.SimpleDNS.Plugin.IGetHostPlugIn.Lookup
    If MyData.XDate > Now AndAlso MyData.Contains(req.QName) Then
      If CType(req.QType, UShort) = 1 Then
        REM Request for type "A" (1) 
        resultIP = MyData.IP4
      Else
        REM Request for type "AAAA"
        resultIP = MyData.IP6
      End If
      resultTTL = MyData.TTL
    Else
      resultIP = Nothing
    End If
  End Sub

  Public Sub LoadConfig(ByVal config As String, ByVal instanceID As Guid, ByVal dataPath As String, ByRef maxThreads As Integer) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.LoadConfig
    MyCfg = clConfig.FromString(config)
  End Sub

  Public Function GetOptionsUI(ByVal instanceID As Guid, ByVal dataPath As String) As JHSoftware.SimpleDNS.Plugin.OptionsUI Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.GetOptionsUI
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

    If tmpDS.Expired Then RaiseEvent LogLine("Data not loaded - data file is past expiration date")

    Return tmpDS
  End Function

  Private Sub fMon_Changed(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs) Handles fMon.Changed
    Try

      If DateTime.UtcNow.Subtract(LastReload).TotalSeconds < 5 Then Exit Sub
      RaiseEvent LogLine("Data file update detected - reloading")
      Try
        MyData = LoadDataSet()
      Catch ex As Exception
        MyData = New DataSet
        RaiseEvent LogLine("Error reloading data file: " & ex.Message)
      End Try

    Catch ex As Exception
      RaiseEvent AsyncError(ex)
    End Try
  End Sub

  Public Function ListsDomainName(ByVal domain As JHSoftware.SimpleDNS.Plugin.DomainName) As Boolean Implements JHSoftware.SimpleDNS.Plugin.IListsDomainName.ListsDomainName
    Return (MyData.XDate > Now AndAlso MyData.Contains(domain))
  End Function
End Class


