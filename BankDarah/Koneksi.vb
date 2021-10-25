Imports MySql.Data.MySqlClient
Module Koneksi

    Public conn As MySqlConnection
    Public cmd As MySqlCommand
    Public da As MySqlDataAdapter
    Public dr As MySqlDataReader
    Public ds As DataSet
    Public str As String

    Public Sub koneksiServer()
        Try
            Dim str As String = "Server=192.168.200.2;user id=lis;password=lis1234;database=simrs;default command timeout=120"
            conn = New MySqlConnection(str)
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            MsgBox("Terputus dari server, Silahkan Login kembali/Hubungi Tim IT", MsgBoxStyle.Exclamation, "Laboratorium : Information")
            LoginForm.Close()
        End Try
    End Sub

    Public Sub koneksiLis()
        Try
            Dim str As String = "Server=192.168.200.2;user id=lis;password=lis1234;database=simrslab"
            conn = New MySqlConnection(str)
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Function hitungUmur(ByVal tanggal As Date) As String
        Dim y, m, d As Integer
        y = Now.Year - tanggal.Year
        m = Now.Month - tanggal.Month
        d = Now.Day - tanggal.Day

        If Math.Sign(d) = -1 Then
            d = 30 - Math.Abs(d)
            m -= 1
        End If
        If Math.Sign(m) = -1 Then
            m = 12 - Math.Abs(m)
            y -= 1
        End If

        Return y & "Th, " & m & "Bln, " & d & "hr"
    End Function

End Module
