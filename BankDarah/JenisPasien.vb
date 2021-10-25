Public Class JenisPasien

    Public Ambil_Data As String
    Public Form_Ambil_Data As String

    Private Sub JenisPasien_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Dispose()
        TambahPasien.Ambil_Data = True
        TambahPasien.Form_Ambil_Data = "PasienRS"
        TambahPasien.ShowDialog()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        MsgBox("Maaf masih dalam pengembangan ..", MsgBoxStyle.Information)
        'Me.Dispose()
        'TambahPasien.Ambil_Data = True
        'TambahPasien.Form_Ambil_Data = "PasienLuar"
        'TambahPasien.ShowDialog()
    End Sub
End Class