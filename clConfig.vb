Friend Class clConfig
  Friend FileName As String
  Friend Monitor As Boolean

  Public Overrides Function ToString() As String
    Return PipeEncode(FileName) & "|" & _
           If(Monitor, "Y", "N")
  End Function

  Public Shared Function FromString(ByVal s As String) As clConfig
    Dim rv As New clConfig
    Dim sa = PipeDecode(s)
    If sa.Length > 0 Then rv.FileName = sa(0)
    If sa.Length > 1 Then rv.Monitor = (sa(1) = "Y")
    Return rv
  End Function

End Class
