Imports JHSoftware.SimpleDNS.Plugin

Friend Class DataSet
  Friend IP4 As IPAddressV4 = IPAddressV4.Parse("127.0.0.1")
  Friend IP6 As IPAddressV6 = IPAddressV6.Parse("::1")
  Friend TTL As Integer = 300
  Friend XDate As DateTime = Now.AddYears(10)

  Private PlusSet As New SubDataSet
  Private MinusSet As New SubDataSet

  Function Expired() As Boolean
    Return (XDate < Now)
  End Function

  Friend Function Contains(ByVal dom As DomainName) As Boolean
    If MinusSet.Contains(dom) Then Return False
    Return PlusSet.Contains(dom)
  End Function

  Friend Sub Load(ByVal fromStream As System.IO.Stream)
    Static WhiteSpace As Char() = New Char() {" "c, ChrW(9)}
    Dim rdr = New System.IO.StreamReader(fromStream, System.Text.Encoding.ASCII)

    Dim CurSubSet As SubDataSet
    Dim i As Integer
    Dim x, y As String
    Dim d As DomainName = Nothing
    While Not rdr.EndOfStream
      x = rdr.ReadLine()
      If x.Length = 0 Then Continue While
      i = x.IndexOf("#"c)
      If i >= 0 Then x = x.Substring(0, i).TrimEnd
      i = x.IndexOfAny(WhiteSpace)
      If i < 1 Then Continue While
      y = x.Substring(i + 1).Trim
      If y.Length = 0 Then Continue While
      x = x.Substring(0, i).ToUpper
      If x(0) = "!"c Then
        CurSubSet = MinusSet
        x = x.Substring(1)
      Else
        CurSubSet = PlusSet
      End If
      Select Case x
        Case "X"c
          If y.Length <> 8 Then Continue While
          Try
            XDate = New DateTime(Integer.Parse(y.Substring(0, 4)), Integer.Parse(y.Substring(4, 2)), Integer.Parse(y.Substring(6, 2)))
          Catch ex As Exception
            Continue While
          End Try
          If Expired() Then rdr.Close() : Exit Sub
        Case "I"c
          Dim tmpIP As IPAddress = Nothing
          If IPAddress.TryParse(y, tmpIP) Then
            If tmpIP.IPVersion = 4 Then IP4 = DirectCast(tmpIP, IPAddressV4) Else IP6 = DirectCast(tmpIP, IPAddressV6)
          End If
        Case "T"c
          Integer.TryParse(y, TTL)
        Case "M"c
          If y.StartsWith("*.") Then
            If Not DomainName.TryParse(y.Substring(2), d) Then Continue While
            Try
              CurSubSet.EndsWith.Add(d, Nothing)
            Catch
            End Try
          Else
            If Not DomainName.TryParse(y, d) Then Continue While
            Try
              CurSubSet.Exact.Add(d, Nothing)
            Catch
            End Try
          End If
        Case "E"c
          If Not DomainName.TryParse(y, d) Then Continue While
          Try
            CurSubSet.EndsWith.Add(d, Nothing)
          Catch
          End Try
          Try
            CurSubSet.Exact.Add(d, Nothing)
          Catch
          End Try
        Case "R"c
          Try
            CurSubSet.RegEx.Add(New System.Text.RegularExpressions.Regex(y, System.Text.RegularExpressions.RegexOptions.Compiled))
          Catch
          End Try
      End Select
    End While
    rdr.Close()
  End Sub

  Private Class SubDataSet
    Friend Exact As New Dictionary(Of DomainName, Object)
    Friend EndsWith As New Dictionary(Of DomainName, Object)
    Friend RegEx As New List(Of System.Text.RegularExpressions.Regex)

    Friend Function Contains(ByVal dom As DomainName) As Boolean
      If Exact.ContainsKey(dom) Then Return True

      If EndsWith.Count > 0 Then
        Dim tmpDom = dom
        Dim segCt = tmpDom.SegmentCount
        While segCt > 1
          tmpDom = tmpDom.Parent
          segCt -= 1
          If EndsWith.ContainsKey(tmpDom) Then Return True
        End While
      End If

      If RegEx.Count > 0 Then
        Dim domStr = dom.ToString
        For Each rx In RegEx
          If rx.IsMatch(domStr) Then Return True
        Next
      End If

      Return False
    End Function

  End Class

End Class