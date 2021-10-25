Option Explicit On
Imports System.IO
Imports System.Net.Sockets
Imports Tulpep.NotificationWindow
Imports MySql.Data.MySqlClient
Imports System.Deployment.Application
Public Class Form1

    Public Ambil_Data As String
    Public Form_Ambil_Data As String
    Public noTindakanPenunjang As String
    Public unit As String
    Public caraBayar As String

    Dim ind As String
    Dim idDetail As String

    Function totalTarif() As String
        Dim total As Long = 0
        Dim hasil As Long = 0
        For i As Integer = 0 To DataGridView2.Rows.Count - 1
            total = total + Val(DataGridView2.Rows(i).Cells(3).Value)
        Next
        hasil = total.ToString("#,##0")
        Return hasil
    End Function

    Sub cariDataPasien()
        conn.Close()
        Dim query As String
        query = "SELECT * 
                   FROM vw_pasienbdrs
                  WHERE noRekamedis Like '%" & txtNoRM.Text & "%'"
        cmd = New MySqlCommand(query, conn)
        da = New MySqlDataAdapter(cmd)

        Dim str As New DataTable
        str.Clear()
        da.Fill(str)

        txtNoReg.Text = ""
        txtNamaPasien.Text = ""
        txtUsia.Text = ""
        txtAlamat.Text = ""
        txtDokter.Text = ""
        txtKlinis.Text = ""
        txtTglReg.Text = ""
        txtNoPermintaan.Text = ""
        txtTglLahir.Text = ""
        noTindakanPenunjang = ""

        If str.Rows.Count() > 0 Then
            txtNoReg.Text = str.Rows(0)(1).ToString
            txtNamaPasien.Text = str.Rows(0)(2).ToString
            txtTglLahir.Text = str.Rows(0)(5).ToString
            txtAlamat.Text = str.Rows(0)(6).ToString
            txtNoPermintaan.Text = str.Rows(0)(7).ToString
            txtTglReg.Text = str.Rows(0)(8).ToString
            txtDokter.Text = str.Rows(0)(11).ToString
            txtKlinis.Text = str.Rows(0)(14).ToString
            noTindakanPenunjang = str.Rows(0)(15).ToString

            DataGridView1.DataSource = str

            txtKdInstalasi.Text = txtNoPermintaan.Text.Substring(0, 2)
            Select Case txtKdInstalasi.Text
                Case "RJ"
                    txtInstalasi.Text = "RAWAT JALAN"
                Case "RI"
                    txtInstalasi.Text = "RAWAT INAP"
            End Select

            Call tampilDataSudahDitindakAll()
        Else
            MessageBox.Show("Pasien Tidak Ada / Belum Terdaftar", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Timer5.Start()
        End If
        DataGridView1.ClearSelection()
        conn.Close()
    End Sub

    Sub tampilDataAll()
        Try
            Call koneksiServer()
            da = New MySqlDataAdapter("SELECT * FROM vw_pasienbdrs
                                        WHERE tglMasukBDRS BETWEEN '" & Format(DateTimePicker1.Value, "yyyy-MM-dd") & "' 
                                          AND '" & Format(DateAdd(DateInterval.Day, 1, DateTimePicker1.Value), "yyyy-MM-dd") & "'
                                          AND (totalTindakanBDRS IS NOT NULL AND totalTindakanBDRS != 0)
                                        ORDER BY tglMasukBDRS DESC", conn)
            'da = New MySqlDataAdapter("SELECT * FROM vw_pasienbdrs
            '                            WHERE tglMasukBDRS BETWEEN '2020-04-01' AND '2020-04-06'
            '                              AND (totalTindakanBDRS IS NOT NULL AND totalTindakanBDRS != 0)
            '                            ORDER BY tglMasukBDRS DESC", conn)
            ds = New DataSet
            da.Fill(ds, "vw_pasienbdrs")
            DataGridView1.DataSource = ds.Tables("vw_pasienbdrs")
            DataGridView1.ToString.ToUpper()

            Call aturDGV()
        Catch ex As Exception

        End Try
    End Sub

    Sub tampilDataSudahDitindakAll()
        Dim query As String = ""
        query = "CALL datadetailbdrs('" & noTindakanPenunjang & "')"

        Try
            Call koneksiServer()
            cmd = New MySqlCommand(query, conn)
            dr = cmd.ExecuteReader
            DataGridView2.Rows.Clear()

            Do While dr.Read
                DataGridView2.Rows.Add(dr.Item("kdTarif"), dr.Item("tindakan"), dr.Item("PPA"),
                                dr.Item("tarif"), dr.Item("statusTindakan"), dr.Item("tglMulaiLayaniPasien"),
                                dr.Item("tglSelesaiLayaniPasien"), dr.Item("idTindakanBDRS"),
                                dr.Item("noTindakanBDRS"))
            Loop
            dr.Close()
            conn.Close()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    Sub aturDGV()
        Try
            DataGridView1.Columns(0).Width = 100
            DataGridView1.Columns(1).Width = 160
            DataGridView1.Columns(2).Width = 220
            DataGridView1.Columns(3).Width = 150
            DataGridView1.Columns(4).Width = 150
            DataGridView1.Columns(5).Width = 150
            DataGridView1.Columns(6).Width = 250
            DataGridView1.Columns(7).Width = 150
            DataGridView1.Columns(8).Width = 170
            DataGridView1.Columns(9).Width = 150
            DataGridView1.Columns(10).Width = 100
            DataGridView1.Columns(11).Width = 300
            DataGridView1.Columns(12).Width = 100
            DataGridView1.Columns(13).Width = 250
            DataGridView1.Columns(14).Width = 250
            DataGridView1.Columns(15).Width = 200
            DataGridView1.Columns(16).Width = 100
            DataGridView1.Columns(17).Width = 150
            DataGridView1.Columns(18).Width = 120
            DataGridView1.Columns(0).HeaderText = "No.RM"
            DataGridView1.Columns(1).HeaderText = "No.Daftar"
            DataGridView1.Columns(2).HeaderText = "Nama Pasien"
            DataGridView1.Columns(3).HeaderText = "KD.Unit Asal"
            DataGridView1.Columns(4).HeaderText = "Asal Ruang/Poli"
            DataGridView1.Columns(5).HeaderText = "Tgl.Lahir"
            DataGridView1.Columns(6).HeaderText = "Alamat"
            DataGridView1.Columns(7).HeaderText = "No.Permintaan"
            DataGridView1.Columns(8).HeaderText = "Tgl.Masuk Lab"
            DataGridView1.Columns(9).HeaderText = "Status Tindakan"
            DataGridView1.Columns(10).HeaderText = "KD.Dokter"
            DataGridView1.Columns(11).HeaderText = "Dokter Pengirim"
            DataGridView1.Columns(12).HeaderText = "KD.Dokter"
            DataGridView1.Columns(13).HeaderText = "Dokter Laboratorium"
            DataGridView1.Columns(14).HeaderText = "Ket.Klinis"
            DataGridView1.Columns(15).HeaderText = "No.Tindakan"
            DataGridView1.Columns(16).HeaderText = "Total Biaya"
            DataGridView1.Columns(17).HeaderText = "Status Pembayaran"
            DataGridView1.Columns(18).HeaderText = "Penjamin"
        Catch ex As Exception

        End Try
    End Sub

    Sub updateRegistrasiBDRSRanap()
        Call koneksiServer()

        Dim dt As String
        dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

        Try
            Dim str As String
            str = "UPDATE t_registrasibdrsranap 
                      SET tglMulaiLayaniPasien = '" & dt & "', 
                          statusBDRS = 'DALAM TINDAKAN', 
                          kdDokterPemeriksa = '0000' 
                    WHERE noRegistrasiBDRS = '" & txtNoPermintaan.Text & "'"

            cmd = New MySqlCommand(str, conn)
            cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
            'MsgBox("Update data Registrasi Pemeriksaan Lab berhasil dilakukan", MessageBoxIcon.Information)
        Catch ex As Exception
            MsgBox("Update data Registrasi gagal dilakukan.", MessageBoxIcon.Error, "Error Registrasi Ranap")
        End Try

        conn.Close()
    End Sub

    Sub updateRegistrasiBDRSRajal()
        Call koneksiServer()

        Dim dt As String
        dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

        Try
            Dim str As String
            str = "UPDATE t_registrasibdrsrajal 
                      SET tglMulaiLayaniPasien = '" & dt & "',
                          statusBDRS = 'DALAM TINDAKAN', 
                          kdDokterPemeriksa = '0000' 
                    WHERE noRegistrasiBDRS = '" & txtNoPermintaan.Text & "'"

            cmd = New MySqlCommand(str, conn)
            cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
            'MsgBox("Update data Registrasi Pemeriksaan Lab berhasil dilakukan", MessageBoxIcon.Information)
        Catch ex As Exception
            MsgBox("Update data Registrasi gagal dilakukan.", MessageBoxIcon.Error, "Error Registrasi Rajal")
        End Try

        conn.Close()
    End Sub

    Sub updateTglTindakan()
        Dim dt As String
        dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

        Dim str As String = ""
        str = "UPDATE t_tindakanBDRS
                  SET tglTindakanBDRS= '" & dt & "'
                WHERE noTindakanBDRS = '" & noTindakanPenunjang & "'"

        Call koneksiServer()
        Try
            cmd = New MySqlCommand(str, conn)
            cmd.ExecuteNonQuery()
            'MsgBox("Update Tanggal Tindakan Lab berhasil dilakukan", MessageBoxIcon.Information)
        Catch ex As Exception
            MsgBox("Update Tanggal gagal dilakukan.", MessageBoxIcon.Error)
        End Try

        conn.Close()
    End Sub

    Sub updateStatusDetail()
        Dim str As String = ""

        str = "UPDATE t_detailtindakanbdrs
                  SET statusTindakan = 'DALAM TINDAKAN' 
                WHERE idTindakanbdrs = '" & idDetail & "'"

        Call koneksiServer()
        Try
            cmd = New MySqlCommand(str, conn)
            cmd.ExecuteNonQuery()
            'MsgBox("Update status tindakan berhasil dilakukan", MessageBoxIcon.Information)
        Catch ex As Exception
            MsgBox("Update status tindakan dilakukan.", MessageBoxIcon.Error)
        End Try

        conn.Close()
    End Sub

    Sub updateTglSelesaiTindakan()
        Dim dt As String
        dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

        Dim str As String = ""

        Select Case txtKdInstalasi.Text
            Case "RJ"
                str = "UPDATE t_registrasibdrsrajal
                          SET tglSelesaiLayaniPasien= '" & dt & "'
                        WHERE noRegistrasiBDRS = '" & txtNoPermintaan.Text & "'"
            Case "RI"
                str = "UPDATE t_registrasibdrsranap 
                          SET tglSelesaiLayaniPasien = '" & dt & "'
                        WHERE noRegistrasiBDRS = '" & txtNoPermintaan.Text & "'"
            Case "PASIEN LUAR"
        End Select

        Call koneksiServer()
        Try
            cmd = New MySqlCommand(str, conn)
            cmd.ExecuteNonQuery()
            'MsgBox("Update Tanggal Tindakan Lab berhasil dilakukan", MessageBoxIcon.Information)
        Catch ex As Exception
            MsgBox("Update 'Tanggal Selesai' gagal dilakukan.", MessageBoxIcon.Error)
        End Try

        conn.Close()
    End Sub

    Sub updateStatusRegPermintaan()
        Dim str As String = ""

        Select Case txtKdInstalasi.Text
            Case "RJ"
                str = "UPDATE t_registrasibdrsrajal
                          SET statusBDRS = 'SELESAI'
                        WHERE noRegistrasiBDRS = '" & txtNoPermintaan.Text & "'"
            Case "RI"
                str = "UPDATE t_registrasibdrsranap 
                          SET statusBDRS = 'SELESAI'
                        WHERE noRegistrasiBDRS = '" & txtNoPermintaan.Text & "'"
            Case "PASIEN LUAR"
        End Select

        Call koneksiServer()
        Try
            cmd = New MySqlCommand(str, conn)
            cmd.ExecuteNonQuery()
            'MsgBox("Update Tanggal Tindakan Lab berhasil dilakukan", MessageBoxIcon.Information)
        Catch ex As Exception
            MsgBox("Update 'Status Selesai' gagal dilakukan.", MessageBoxIcon.Error)
        End Try

        conn.Close()
    End Sub

    Private Sub ClickMulai()
        If txtKdInstalasi.Text = "RJ" Then
            Call updateRegistrasiBDRSRajal()
            Call updateTglTindakan()
            Call updateStatusDetail()
            'MsgBox("Tindakan Pemeriksaan dimulai", MsgBoxStyle.Information, "Informasi")
        Else
            Select Case txtKdInstalasi.Text
                Case "RI"
                    Call updateRegistrasiBDRSRanap()
                    Call updateTglTindakan()
                    Call updateStatusDetail()
                    'MsgBox("Tindakan Pemeriksaan dimulai", MsgBoxStyle.Information, "Informasi")
            End Select
        End If

        'DataGridView2.Columns.Clear()

        Call tampilDataAll()
        Call tampilDataSudahDitindakAll()
    End Sub

    Private Sub ClickSelesai(id As String)
        Dim str As String = ""

        str = "UPDATE t_detailtindakanbdrs 
                  SET statusTindakan = 'SELESAI'
                WHERE idTindakanBDRS = '" & id & "'"

        Call koneksiServer()
        Try
            cmd = New MySqlCommand(str, conn)
            cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
            'MsgBox("Pemeriksaan Lab selesai dilakukan", MessageBoxIcon.Information)
        Catch ex As Exception
            MsgBox("Error", MessageBoxIcon.Error)
        End Try

        Call updateTglSelesaiTindakan()

        'DataGridView2.Columns.Clear()

        Call tampilDataAll()
        Call tampilDataSudahDitindakAll()

        conn.Close()

        'Hasil.Ambil_Data = True
        'Hasil.Form_Ambil_Data = "Hasil"
        'Hasil.Show()
    End Sub

    Sub cekStatusSelesai()
        Dim rowCount As Integer = 0
        rowCount = DataGridView2.Rows.Count

        Dim itemCount As Integer
        For i As Integer = 0 To DataGridView2.Rows.Count - 1
            If DataGridView2.Rows(i).Cells(4).Value.ToString = "SELESAI" Then
                itemCount = itemCount + 1
            End If
        Next

        'MsgBox("Jumlah tindakan : " & rowCount)
        'MsgBox("Jumlah tindakan yg selesai : " & itemCount)

        If itemCount = rowCount Then
            Call updateStatusRegPermintaan()
            Call tampilDataAll()
            'MsgBox("Update Status")
            'Else
            'MsgBox("Masih ada tindakan yang belum terselesaikan")
        End If
    End Sub

    Sub setColor(button As Button)
        btnDash.BackColor = SystemColors.HotTrack
        btnTindakan.BackColor = SystemColors.HotTrack
        btnHasil.BackColor = SystemColors.HotTrack
        button.BackColor = Color.DodgerBlue
    End Sub

    Sub cariDataKelas()
        Call koneksiServer()
        Dim query As String = ""
        Dim cmd As MySqlCommand
        Dim dr As MySqlDataReader

        query = "SELECT kelas 
                   FROM vw_pasienrawatinap
                  WHERE noDaftar = '" & txtNoReg.Text & "'"

        cmd = New MySqlCommand(query, conn)
        dr = cmd.ExecuteReader
        txtKelas.Text = ""

        While dr.Read
            If txtKdInstalasi.Text = "RI" Then
                txtKelas.Text = dr.GetString(0).ToString
            Else
                txtKelas.Text = "-"
            End If
        End While
        dr.Close()
        conn.Close()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Normal
        Me.StartPosition = FormStartPosition.Manual
        With Screen.PrimaryScreen.WorkingArea
            Me.SetBounds(.Left, .Top, .Width, .Height)
        End With

        If ApplicationDeployment.IsNetworkDeployed Then
            Dim ver As ApplicationDeployment = ApplicationDeployment.CurrentDeployment
            Label8.Text = "Version " & ver.CurrentVersion.ToString()
        End If

        Dim pcname As String
        Dim ipadd As String = ""
        pcname = System.Net.Dns.GetHostName

        Dim objAddressList() As System.Net.IPAddress =
            System.Net.Dns.GetHostEntry("").AddressList
        For i = 0 To objAddressList.GetUpperBound(0)
            If objAddressList(i).AddressFamily = Net.Sockets.AddressFamily.InterNetwork Then
                ipadd = objAddressList(i).ToString
                txtIpAddress.Text = objAddressList(i).ToString
                Exit For
            End If
        Next

        txtHostname.Text = "PC Name : " & pcname & " | IP Address : " & ipadd & " | Username : " & LoginForm.user

        'Call isiComboUnit()
        Call tampilDataAll()
        Call aturDGV()
        DataGridView1.ClearSelection()

        btnTindakan.Enabled = False
        'btnHasil.Enabled = False

        btnDash.BackColor = Color.DodgerBlue

        'Call tampilDataSudahDitindakAll()
    End Sub

    Private Sub btnDash_Click(sender As Object, e As EventArgs) Handles btnDash.Click
        pnlStats.Height = btnDash.Height
        pnlStats.Top = btnDash.Top

        Dim btn As Button = CType(sender, Button)
        setColor(btn)
    End Sub

    Private Sub btnTindakan_Click(sender As Object, e As EventArgs) Handles btnTindakan.Click
        pnlStats.Height = btnTindakan.Height
        pnlStats.Top = btnTindakan.Top

        'Tindakan.Ambil_Data = True
        'Tindakan.Form_Ambil_Data = "Tindakan"
        'Tindakan.Show()
    End Sub

    Private Sub btnKeluar_Click(sender As Object, e As EventArgs) Handles btnKeluar.Click
        Dim konfirmasi As MsgBoxResult

        konfirmasi = MsgBox("Apakah anda yakin ingin keluar..?", vbQuestion + vbYesNo, "EXIT")
        If konfirmasi = vbYes Then
            Me.Close()
            LoginForm.Show()
        End If
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        txtJamDigi.Text = TimeOfDay
    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        txtTglDigi.Text = Format(Now, “dd/MM/yyyy”)
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        Dim noRm, noReg, namaPasien, alamat, usia, dokter1, dokter2, noPermin, tglReg, ketKlinis, noTindak As String

        If e.RowIndex = -1 Then
            Return
        End If

        noRm = DataGridView1.Rows(e.RowIndex).Cells(0).Value
        noReg = DataGridView1.Rows(e.RowIndex).Cells(1).Value
        namaPasien = DataGridView1.Rows(e.RowIndex).Cells(2).Value
        unit = DataGridView1.Rows(e.RowIndex).Cells(4).Value
        usia = DataGridView1.Rows(e.RowIndex).Cells(5).Value
        alamat = DataGridView1.Rows(e.RowIndex).Cells(6).Value
        noPermin = DataGridView1.Rows(e.RowIndex).Cells(7).Value
        tglReg = DataGridView1.Rows(e.RowIndex).Cells(8).Value
        dokter1 = DataGridView1.Rows(e.RowIndex).Cells(11).Value
        dokter2 = DataGridView1.Rows(e.RowIndex).Cells(13).Value.ToString
        ketKlinis = DataGridView1.Rows(e.RowIndex).Cells(14).Value.ToString
        noTindak = DataGridView1.Rows(e.RowIndex).Cells(15).Value.ToString
        caraBayar = DataGridView1.Rows(e.RowIndex).Cells(18).Value.ToString

        txtNoRM.Text = noRm
        txtNoReg.Text = noReg
        txtNamaPasien.Text = namaPasien
        txtAlamat.Text = alamat
        txtNoPermintaan.Text = noPermin
        txtTglReg.Text = tglReg
        txtTglLahir.Text = usia
        txtDokter.Text = dokter1
        txtKlinis.Text = ketKlinis
        noTindakanPenunjang = noTindak

        txtKdInstalasi.Text = txtNoPermintaan.Text.Substring(0, 2)
        Select Case txtKdInstalasi.Text
            Case "RJ"
                txtInstalasi.Text = "RAWAT JALAN"
                If unit = "IGD" Then
                    txtKelas.Text = "KELAS I"
                Else
                    txtKelas.Text = "KELAS III"
                End If
            Case "RI"
                txtInstalasi.Text = "RAWAT INAP"
                Call cariDataKelas()
        End Select

        'DataGridView2.Columns.Clear()

        Call tampilDataSudahDitindakAll()
        'Call tampilDataLab()
        'Call warnaStatus()
        Call totalTarif()
        'Timer4.Start()
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Dim noRm, noReg, namaPasien, alamat, usia, dokter1, dokter2, noPermin, tglReg, ketKlinis, noTindak As String

        If e.RowIndex = -1 Then
            Return
        End If

        noRm = DataGridView1.Rows(e.RowIndex).Cells(0).Value
        noReg = DataGridView1.Rows(e.RowIndex).Cells(1).Value
        namaPasien = DataGridView1.Rows(e.RowIndex).Cells(2).Value
        unit = DataGridView1.Rows(e.RowIndex).Cells(4).Value
        usia = DataGridView1.Rows(e.RowIndex).Cells(5).Value
        alamat = DataGridView1.Rows(e.RowIndex).Cells(6).Value
        noPermin = DataGridView1.Rows(e.RowIndex).Cells(7).Value
        tglReg = DataGridView1.Rows(e.RowIndex).Cells(8).Value
        dokter1 = DataGridView1.Rows(e.RowIndex).Cells(11).Value
        dokter2 = DataGridView1.Rows(e.RowIndex).Cells(13).Value.ToString
        ketKlinis = DataGridView1.Rows(e.RowIndex).Cells(14).Value.ToString
        noTindak = DataGridView1.Rows(e.RowIndex).Cells(15).Value.ToString
        caraBayar = DataGridView1.Rows(e.RowIndex).Cells(18).Value.ToString

        txtNoRM.Text = noRm
        txtNoReg.Text = noReg
        txtNamaPasien.Text = namaPasien
        txtAlamat.Text = alamat
        txtNoPermintaan.Text = noPermin
        txtTglReg.Text = tglReg
        txtTglLahir.Text = usia
        txtDokter.Text = dokter1
        txtKlinis.Text = ketKlinis
        noTindakanPenunjang = noTindak

        txtKdInstalasi.Text = txtNoPermintaan.Text.Substring(0, 2)
        Select Case txtKdInstalasi.Text
            Case "RJ"
                txtInstalasi.Text = "RAWAT JALAN"
                If unit = "IGD" Then
                    txtKelas.Text = "KELAS I"
                Else
                    txtKelas.Text = "KELAS III"
                End If
            Case "RI"
                txtInstalasi.Text = "RAWAT INAP"
                Call cariDataKelas()
        End Select

        'DataGridView2.Columns.Clear()

        Call tampilDataSudahDitindakAll()
        'Call tampilDataLab()
        'Call warnaStatus()
        Call totalTarif()
        'Timer4.Start()
    End Sub

    Private Sub DataGridView1_KeyUp(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyUp
        Dim noRm, noReg, namaPasien, alamat, usia, dokter1, dokter2, noPermin, tglReg, ketKlinis, noTindak As String

        If (e.KeyCode = Keys.Down Or e.KeyCode = Keys.Up) And DataGridView1.CurrentCell.RowIndex >= 0 Then
            e.Handled = True
            e.SuppressKeyPress = True

            Dim row As DataGridViewRow
            row = Me.DataGridView1.Rows(DataGridView1.CurrentCell.RowIndex)

            If DataGridView1.CurrentCell.RowIndex = -1 Then
                Return
            End If

            noRm = row.Cells(0).Value
            noReg = row.Cells(1).Value
            namaPasien = row.Cells(2).Value
            unit = row.Cells(4).Value
            usia = row.Cells(5).Value
            alamat = row.Cells(6).Value
            noPermin = row.Cells(7).Value
            tglReg = row.Cells(8).Value
            dokter1 = row.Cells(11).Value
            dokter2 = row.Cells(13).Value.ToString
            ketKlinis = row.Cells(14).Value.ToString
            noTindak = row.Cells(15).Value.ToString
            caraBayar = row.Cells(18).Value.ToString

            txtNoRM.Text = noRm
            txtNoReg.Text = noReg
            txtNamaPasien.Text = namaPasien
            txtAlamat.Text = alamat
            txtNoPermintaan.Text = noPermin
            txtTglReg.Text = tglReg
            txtTglLahir.Text = usia
            txtDokter.Text = dokter1
            txtKlinis.Text = ketKlinis
            noTindakanPenunjang = noTindak

            txtKdInstalasi.Text = txtNoPermintaan.Text.Substring(0, 2)
            Select Case txtKdInstalasi.Text
                Case "RJ"
                    txtInstalasi.Text = "RAWAT JALAN"
                    If unit = "IGD" Then
                        txtKelas.Text = "KELAS I"
                    Else
                        txtKelas.Text = "KELAS III"
                    End If
                Case "RI"
                    txtInstalasi.Text = "RAWAT INAP"
                    Call cariDataKelas()
            End Select

            'DataGridView2.Columns.Clear()

            Call tampilDataSudahDitindakAll()
            'Call tampilDataLab()
            'Call warnaStatus()
            Call totalTarif()
            'Timer4.Start()
        End If
    End Sub

    Private Sub txtNoRM_TextChanged(sender As Object, e As EventArgs) Handles txtNoRM.TextChanged
        If txtNoRM.Text = "" Then
            txtNoReg.Text = ""
            txtNamaPasien.Text = ""
            txtAlamat.Text = ""
            txtTglLahir.Text = ""
            txtNoPermintaan.Text = ""
            txtTglReg.Text = ""
            txtDokter.Text = ""
            txtUsia.Text = ""
        End If
        btnTindakan.Enabled = False
    End Sub

    Private Sub txtTglLahir_TextChanged(sender As Object, e As EventArgs) Handles txtTglLahir.TextChanged
        If Not String.IsNullOrEmpty(txtTglLahir.Text) Then
            Dim lahir As Date = txtTglLahir.Text
            txtUsia.Text = hitungUmur(lahir)
        Else
            Return
        End If
    End Sub

    Private Sub DataGridView2_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs)
        If DataGridView2.Rows(e.RowIndex).Cells(4).Value.ToString = "PERMINTAAN" Then
            'btnHasil.Enabled = False
        ElseIf DataGridView2.Rows(e.RowIndex).Cells(4).Value.ToString = "DALAM TINDAKAN" Or DataGridView2.Rows(e.RowIndex).Cells(4).Value.ToString = "SELESAI" Then
            btnHasil.Enabled = True
        Else
            btnHasil.Enabled = False
        End If
    End Sub

    Private Sub DataGridView2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellContentClick
        Dim konfirmasi As MsgBoxResult
        Dim tindakan As String
        idDetail = DataGridView2.Rows(e.RowIndex).Cells(7).Value.ToString
        tindakan = DataGridView2.Rows(e.RowIndex).Cells(1).Value.ToString

        If e.ColumnIndex = 9 Then
            Select Case DataGridView2.Rows(e.RowIndex).Cells(4).Value.ToString
                Case "PERMINTAAN"
                    konfirmasi = MsgBox("Apakah tindakan '" & tindakan & "' akan dimulai ?", vbQuestion + vbYesNo, "Laboratorium")
                    If konfirmasi = vbYes Then
                        Call ClickMulai()
                        'MsgBox(tindakan & " - Memulai tindakan", MsgBoxStyle.Information)
                    End If
                Case "DALAM TINDAKAN"
                    konfirmasi = MsgBox("Apakah tindakan '" & tindakan & "' sudah selesai ?", vbQuestion + vbYesNo, "Laboratorium")
                    If konfirmasi = vbYes Then
                        Call ClickSelesai(idDetail)
                        Call cekStatusSelesai()
                        MsgBox(tindakan & " - Selesai", MsgBoxStyle.Information)
                    End If
                Case "SELESAI"
                    konfirmasi = MsgBox("Apakah pembayaran '" & tindakan & "' sudah lunas ?", vbQuestion + vbYesNo, "Laboratorium")
                    If konfirmasi = vbYes Then
                        'Call ClickHasil()
                        MsgBox(tindakan & " - LUNAS", MsgBoxStyle.Information)
                    End If
            End Select
        End If

    End Sub

    Private Sub btnCari_MouseLeave(sender As Object, e As EventArgs) Handles btnCari.MouseLeave
        Me.btnCari.BackColor = Color.DodgerBlue
    End Sub

    Private Sub btnCari_MouseEnter(sender As Object, e As EventArgs) Handles btnCari.MouseEnter
        Me.btnCari.BackColor = Color.FromArgb(30, 100, 255)
    End Sub

    Private Sub btnCari_Click(sender As Object, e As EventArgs) Handles btnCari.Click
        If txtNoRM.Text = "" Then
            MessageBox.Show("Masukkan No.RM Pasien", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Call cariDataPasien()
            Timer5.Stop()
        End If
    End Sub

    Private Sub txtNoRM_KeyDown(sender As Object, e As KeyEventArgs) Handles txtNoRM.KeyDown
        If e.KeyCode = Keys.Enter And txtNoRM.Text = "" Then
            MessageBox.Show("Masukkan No.RM Pasien", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ElseIf e.KeyCode = Keys.Enter Then
            Call cariDataPasien()
            Timer5.Stop()
        End If
    End Sub

    Private Sub txtNoRM_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtNoRM.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub cmbDokter_KeyPress(sender As Object, e As KeyPressEventArgs)
        If System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "[^a-zA-Z()\b., ]") Then
            e.Handled = True
        End If
    End Sub

    Private Sub btnTindak_Click(sender As Object, e As EventArgs) Handles btnTindak.Click
        Select Case txtKdInstalasi.Text
            Case "RJ"
                CetakBukti.Ambil_Data = True
                CetakBukti.Form_Ambil_Data = "Cetak"
                CetakBukti.Show()
            Case "RI"
                CetakBukti.Ambil_Data = True
                CetakBukti.Form_Ambil_Data = "Cetak"
                CetakBukti.Show()
        End Select
    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        DataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.DeepSkyBlue
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DataGridView1.ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 12, FontStyle.Bold)
        DataGridView1.ColumnHeadersHeight = 40
        DataGridView1.DefaultCellStyle.Font = New Font("Tahoma", 10, FontStyle.Bold)
        DataGridView1.DefaultCellStyle.ForeColor = Color.Black
        DataGridView1.DefaultCellStyle.SelectionBackColor = Color.PaleTurquoise
        DataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black
        DataGridView1.RowHeadersVisible = False
        DataGridView1.AllowUserToResizeRows = False
        DataGridView1.EnableHeadersVisualStyles = False

        DataGridView1.Columns(0).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns(8).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns(9).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns(17).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns(18).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        DataGridView1.Columns(1).Visible = False
        DataGridView1.Columns(3).Visible = False
        DataGridView1.Columns(5).Visible = False
        DataGridView1.Columns(7).Visible = False
        DataGridView1.Columns(10).Visible = False
        DataGridView1.Columns(12).Visible = False
        DataGridView1.Columns(13).Visible = False
        DataGridView1.Columns(14).Visible = False
        DataGridView1.Columns(15).Visible = False
        DataGridView1.Columns(16).Visible = False

        For i = 0 To DataGridView1.RowCount - 1
            If i Mod 2 = 0 Then
                DataGridView1.Rows(i).DefaultCellStyle.BackColor = Color.White
            Else
                DataGridView1.Rows(i).DefaultCellStyle.BackColor = Color.AliceBlue
            End If
        Next

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            If DataGridView1.Rows(i).Cells(9).Value.ToString = "PERMINTAAN" Then
                DataGridView1.Rows(i).Cells(9).Style.BackColor = Color.Orange
                DataGridView1.Rows(i).Cells(9).Style.ForeColor = Color.White
            ElseIf DataGridView1.Rows(i).Cells(9).Value.ToString = "DALAM TINDAKAN" Then
                DataGridView1.Rows(i).Cells(9).Style.BackColor = Color.Green
                DataGridView1.Rows(i).Cells(9).Style.ForeColor = Color.White
            ElseIf DataGridView1.Rows(i).Cells(9).Value.ToString = "SELESAI" Then
                DataGridView1.Rows(i).Cells(9).Style.BackColor = Color.Red
                DataGridView1.Rows(i).Cells(9).Style.ForeColor = Color.White
                'DataGridView1.Rows(i).Visible = False
            End If
        Next

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            If DataGridView1.Rows(i).Cells(17).Value.ToString = "BELUM LUNAS" Then
                DataGridView1.Rows(i).Cells(17).Style.BackColor = Color.Orange
                DataGridView1.Rows(i).Cells(17).Style.ForeColor = Color.White
            ElseIf DataGridView1.Rows(i).Cells(17).Value.ToString = "LUNAS" Then
                DataGridView1.Rows(i).Cells(17).Style.BackColor = Color.Green
                DataGridView1.Rows(i).Cells(17).Style.ForeColor = Color.White
                'DataGridView1.Rows(i).Visible = False
            End If
        Next

    End Sub

    Private Sub DataGridView2_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView2.CellFormatting
        DataGridView2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.DeepSkyBlue
        DataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DataGridView2.ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 12, FontStyle.Bold)
        DataGridView2.ColumnHeadersHeight = 40
        DataGridView2.DefaultCellStyle.Font = New Font("Tahoma", 10, FontStyle.Bold)
        DataGridView2.DefaultCellStyle.ForeColor = Color.Black
        DataGridView2.DefaultCellStyle.SelectionBackColor = Color.PaleTurquoise
        DataGridView2.DefaultCellStyle.SelectionForeColor = Color.Black
        DataGridView2.RowHeadersVisible = False
        DataGridView2.AllowUserToResizeRows = False
        DataGridView2.EnableHeadersVisualStyles = False

        DataGridView2.Columns(3).DefaultCellStyle.Format = "###,###,###"
        DataGridView2.Columns(5).DefaultCellStyle.Format = "HH:mm"
        DataGridView2.Columns(6).DefaultCellStyle.Format = "HH:mm"

        DataGridView2.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView2.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView2.Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView2.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView2.Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView2.Columns(6).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        'DataGridView2.Columns(5).Visible = False
        'DataGridView2.Columns(6).Visible = False
        'DataGridView2.Columns(7).Visible = False

        For i As Integer = 0 To DataGridView2.RowCount - 1
            If DataGridView2.Rows(i).Cells(4).Value.ToString = "PERMINTAAN" Then
                DataGridView2.Rows(i).Cells(4).Style.BackColor = Color.Orange
                DataGridView2.Rows(i).Cells(4).Style.ForeColor = Color.White
            ElseIf DataGridView2.Rows(i).Cells(4).Value.ToString = "DALAM TINDAKAN" Then
                DataGridView2.Rows(i).Cells(4).Style.BackColor = Color.Green
                DataGridView2.Rows(i).Cells(4).Style.ForeColor = Color.White
            ElseIf DataGridView2.Rows(i).Cells(4).Value.ToString = "SELESAI" Then
                DataGridView2.Rows(i).Cells(4).Style.BackColor = Color.Red
                DataGridView2.Rows(i).Cells(4).Style.ForeColor = Color.White
                'DataGridView2.Rows(i).Visible = False
            End If
        Next

        For i = 0 To DataGridView2.RowCount - 1
            DataGridView2.Rows(i).Cells(9).Style.BackColor = Color.DodgerBlue
            DataGridView2.Rows(i).Cells(9).Style.ForeColor = Color.White
        Next

        For i = 0 To DataGridView2.RowCount - 1
            If i Mod 2 = 0 Then
                DataGridView2.Rows(i).DefaultCellStyle.BackColor = Color.White
            Else
                DataGridView2.Rows(i).DefaultCellStyle.BackColor = Color.AliceBlue
            End If
        Next

        For Each column As DataGridViewColumn In DataGridView2.Columns
            column.SortMode = DataGridViewColumnSortMode.NotSortable
        Next
    End Sub

    Private Sub btnTindak_MouseLeave(sender As Object, e As EventArgs) Handles btnTindak.MouseLeave
        Me.btnTindak.BackColor = Color.DodgerBlue
    End Sub

    Private Sub btnTindak_MouseEnter(sender As Object, e As EventArgs) Handles btnTindak.MouseEnter
        Me.btnTindak.BackColor = Color.FromArgb(30, 100, 255)
    End Sub

    Private Sub btnRefresh_MouseLeave(sender As Object, e As EventArgs) Handles btnRefresh.MouseLeave
        Me.btnRefresh.BackColor = Color.DodgerBlue
    End Sub

    Private Sub btnRefresh_MouseEnter(sender As Object, e As EventArgs) Handles btnRefresh.MouseEnter
        Me.btnRefresh.BackColor = Color.FromArgb(30, 100, 255)
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        Timer5.Start()
        Call tampilDataAll()
        DataGridView1.ClearSelection()
    End Sub

    Private Sub btnTambah_Click(sender As Object, e As EventArgs) Handles btnTambah.Click
        Dim jenisPasienFrm As New JenisPasien
        jenisPasienFrm.ShowDialog()
    End Sub

    Private Sub DataGridView1_CellMouseUp(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView1.CellMouseUp
        If e.Button = Windows.Forms.MouseButtons.Right Then
            DataGridView1.Rows(e.RowIndex).Selected = True
            ind = e.RowIndex
            DataGridView1.CurrentCell = DataGridView1.Rows(e.RowIndex).Cells(2)
            ContextMenuStrip1.Show(DataGridView1, e.Location)
            ContextMenuStrip1.Show(Cursor.Position)
        End If
    End Sub

    Private Sub DataGridView2_CellMouseUp(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView2.CellMouseUp
        If e.Button = Windows.Forms.MouseButtons.Right Then
            DataGridView2.Rows(e.RowIndex).Selected = True
            ind = e.RowIndex
            DataGridView2.CurrentCell = DataGridView2.Rows(e.RowIndex).Cells(1)
            ContextMenuStrip2.Show(DataGridView2, e.Location)
            ContextMenuStrip2.Show(Cursor.Position)
        End If
    End Sub

    Private Sub BatalMenuItem1_Click(sender As Object, e As EventArgs) Handles BatalMenuItem1.Click
        If Not DataGridView1.Rows(ind).IsNewRow Then
            Dim konfirmasi As MsgBoxResult
            konfirmasi = MsgBox("Apakah anda yakin ingin menghapus antrian tsb ?", vbQuestion + vbYesNo, "Konfirmasi")
            If konfirmasi = vbYes Then
                'MsgBox("Batal : " & DataGridView1.Rows(ind).Cells(7).Value.ToString)
                If DataGridView1.Rows(ind).Cells(9).Value.ToString = "DALAM TINDAKAN" Then
                    MsgBox("Tindakan tidak dapat dihapus, karena status 'DALAM TINDAKAN'")
                ElseIf DataGridView1.Rows(ind).Cells(9).Value.ToString = "SELESAI" Then
                    MsgBox("Tindakan tidak dapat dihapus, karena status 'SELESAI'")
                Else
                    Call deletePermintaan(DataGridView1.Rows(ind).Cells(7).Value.ToString)
                    Call deleteTindakan(DataGridView1.Rows(ind).Cells(15).Value.ToString)
                    Call deleteAllDetail(DataGridView1.Rows(ind).Cells(15).Value.ToString)
                    Call tampilDataAll()
                End If
            End If
        End If
    End Sub

    Sub deletePermintaan(noDel As String)
        Try
            Call koneksiServer()
            Dim query As String = ""
            Dim cmd As MySqlCommand

            Select Case txtKdInstalasi.Text
                Case "RJ"
                    query = "DELETE FROM t_registrasibdrsrajal WHERE noRegistrasiBDRS = '" & noDel & "'"
                Case "RI"
                    query = "DELETE FROM t_registrasibdrsranap WHERE noRegistrasiBDRS = '" & noDel & "'"
                Case "IGD"
                    query = "DELETE FROM t_registrasibdrsrajal WHERE noRegistrasiBDRS = '" & noDel & "'"
            End Select

            cmd = New MySqlCommand(query, conn)
            cmd.ExecuteNonQuery()
            conn.Close()
            MsgBox("Batal antrian berhasil", MsgBoxStyle.Information, "Information")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error Delete Antrian")
        End Try
    End Sub

    Sub deleteTindakan(noDel As String)
        Try
            Call koneksiServer()
            Dim query As String = ""
            Dim cmd As MySqlCommand

            Select Case txtKdInstalasi.Text
                Case "RJ"
                    query = "DELETE FROM t_tindakanbdrs WHERE noTindakanBDRS = '" & noDel & "'"
                Case "RI"
                    query = "DELETE FROM t_tindakanbdrs WHERE noTindakanBDRS = '" & noDel & "'"
                Case "IGD"
                    query = "DELETE FROM t_tindakanbdrs WHERE noTindakanBDRS = '" & noDel & "'"
            End Select

            cmd = New MySqlCommand(query, conn)
            cmd.ExecuteNonQuery()
            conn.Close()
            'MsgBox("Batal tindakan berhasil", MsgBoxStyle.Information, "Information")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error Delete Tindakan")
        End Try
    End Sub

    Sub deleteAllDetail(noDel As String)
        Try
            Call koneksiServer()
            Dim query As String = ""
            Dim cmd As MySqlCommand

            Select Case txtKdInstalasi.Text
                Case "RJ"
                    query = "DELETE FROM t_detailtindakanbdrs WHERE noTindakanBDRS = '" & noDel & "'"
                Case "RI"
                    query = "DELETE FROM t_detailtindakanbdrs WHERE noTindakanBDRS = '" & noDel & "'"
                Case "IGD"
                    query = "DELETE FROM t_detailtindakanbdrs WHERE noTindakanBDRS = '" & noDel & "'"
            End Select

            cmd = New MySqlCommand(query, conn)
            cmd.ExecuteNonQuery()
            conn.Close()
            'MsgBox("Batal detail berhasil", MsgBoxStyle.Information, "Information")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error Delete Detail")
        End Try
    End Sub

    Private Sub ContextMenuStrip2_Click(sender As Object, e As EventArgs) Handles ContextMenuStrip2.Click
        If Not DataGridView2.Rows(ind).IsNewRow Then
            Dim konfirmasi As MsgBoxResult
            Dim tarif, noTindakan As String
            tarif = DataGridView2.Rows(ind).Cells(3).Value.ToString
            noTindakan = DataGridView2.Rows(ind).Cells(8).Value.ToString
            konfirmasi = MsgBox("Apakah anda yakin ingin menghapus tindakan tsb ?", vbQuestion + vbYesNo, "Konfirmasi")
            If konfirmasi = vbYes Then
                'MsgBox("Batal : " & DataGridView2.Rows(ind).Cells(7).Value.ToString)
                If DataGridView2.Rows(ind).Cells(4).Value.ToString = "DALAM TINDAKAN" Then
                    MsgBox("Tindakan tidak dapat dihapus, karena status 'DALAM TINDAKAN'")
                ElseIf DataGridView2.Rows(ind).Cells(4).Value.ToString = "SELESAI" Then
                    MsgBox("Tindakan tidak dapat dihapus, karena status 'SELESAI'")
                Else
                    Call deleteDetail(DataGridView2.Rows(ind).Cells(7).Value.ToString)
                    Call updateAfterDelete(noTindakan, tarif)
                    Call tampilDataSudahDitindakAll()
                End If
            End If
        End If
    End Sub

    Sub deleteDetail(idDel As String)
        Try
            Call koneksiServer()
            Dim query As String = ""
            Dim cmd As MySqlCommand

            Select Case txtKdInstalasi.Text
                Case "RJ"
                    query = "DELETE FROM t_detailtindakanbdrs WHERE idTindakanBDRS = '" & idDel & "'"
                Case "RI"
                    query = "DELETE FROM t_detailtindakanbdrs WHERE idTindakanBDRS = '" & idDel & "'"
                Case "IGD"
                    query = "DELETE FROM t_detailtindakanbdrs WHERE idTindakanBDRS = '" & idDel & "'"
            End Select

            cmd = New MySqlCommand(query, conn)
            cmd.ExecuteNonQuery()
            conn.Close()
            MsgBox("Hapus tindakan berhasil", MsgBoxStyle.Information, "Information")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error Delete")
        End Try
    End Sub

    Sub updateAfterDelete(noTindakanDel As String, tarif As String)
        Dim total As Integer
        total = Val(totalTarif() - tarif)
        Try
            Call koneksiServer()
            Dim str As String = ""

            str = "UPDATE t_tindakanbdrs
                      SET totalTindakanBDRS = '" & total & "'
                    WHERE noTindakanBDRS = '" & noTindakanDel & "'"

            cmd = New MySqlCommand(str, conn)
            cmd.ExecuteNonQuery()
            'MsgBox("Update dokter berhasil dilakukan", MessageBoxIcon.Information)
        Catch ex As Exception
            MsgBox("Update data gagal dilakukan.", MessageBoxIcon.Error, "Error Update After Delete")
        End Try

        conn.Close()
    End Sub

    Private Sub TAMBAHMenuItem_Click(sender As Object, e As EventArgs) Handles TAMBAHMenuItem.Click
        Tindakan.Ambil_Data = True
        Tindakan.Form_Ambil_Data = "Edit Tindakan"
        Call Tindakan.tampilDataSudahDitindakAll(txtKdInstalasi.Text, noTindakanPenunjang)
        Tindakan.Show()
        Me.Hide()
    End Sub
End Class
