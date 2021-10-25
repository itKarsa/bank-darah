Imports System.ComponentModel
Imports MySql.Data.MySqlClient
Public Class TambahPasien

    Public Ambil_Data As String
    Public Form_Ambil_Data As String

    Sub autoDokter()
        Call koneksiServer()

        Using cmd As New MySqlCommand("SELECT DISTINCT namapetugasMedis FROM t_tenagamedis2 WHERE kdKelompokTenagaMedis in('ktm1') ORDER BY namapetugasMedis ASC", conn)
            da = New MySqlDataAdapter(cmd)
            Dim dt As New DataTable
            da.Fill(dt)

            txtDokter.DataSource = dt
            txtDokter.DisplayMember = "namapetugasMedis"
            txtDokter.ValueMember = "namapetugasMedis"
            txtDokter.AutoCompleteMode = AutoCompleteMode.SuggestAppend
            txtDokter.AutoCompleteSource = AutoCompleteSource.ListItems
        End Using
        conn.Close()
    End Sub

    Sub addRegLabRanap()
        Dim dt As String
        dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

        Call koneksiServer()
        Try
            Dim str As String
            str = "INSERT INTO t_registrasibdrsranap(noRegistrasiBDRS,noDaftar,
                                                          kdUnitAsal,unitAsal,kdUnit,unit,
                                                          tglMasukBDRS,statusBDRS,
                                                          kdDokterPengirim,ketKlinis,
                                                          userModify,dateModify) 
                   VALUES ('" & txtNoPermintaan.Text & "','" & txtNoReg.Text & "','" & txtKdUnit.Text & "',
                           '" & txtUnitAsal.Text & "','3004','Bank Darah','" & dt & "','PERMINTAAN','" & txtKdDokter.Text & "',
                           '" & txtKetKlinis.Text & "','" & LoginForm.user & "','" & dt & "')"
            cmd = New MySqlCommand(str, conn)
            cmd.ExecuteNonQuery()
            MsgBox("Insert data Permintaan berhasil dilakukan", MsgBoxStyle.Information, "Information")
        Catch ex As Exception
            MsgBox("Insert data Permintaan gagal dilakukan.", MsgBoxStyle.Critical, "Error")
        End Try
        conn.Close()
    End Sub

    Sub addRegLabRajal()
        Dim dt As String
        dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

        Call koneksiServer()
        Try
            Dim str As String
            str = "INSERT INTO t_registrasibdrsrajal(noRegistrasiBDRS,noDaftar,
                                                          kdUnitAsal,unitAsal,kdUnit,unit,
                                                          tglMasukBDRS,statusBDRS,
                                                          kdDokterPengirim,ketKlinis,
                                                          userModify,dateModify) 
                   VALUES ('" & txtNoPermintaan.Text & "','" & txtNoReg.Text & "','" & txtKdUnit.Text & "',
                           '" & txtUnitAsal.Text & "','3004','Bank Darah','" & dt & "','PERMINTAAN','" & txtKdDokter.Text & "',
                           '" & txtKetKlinis.Text & "','" & LoginForm.user & "','" & dt & "')"
            cmd = New MySqlCommand(str, conn)
            cmd.ExecuteNonQuery()
            MsgBox("Insert data Permintaan berhasil dilakukan", MsgBoxStyle.Information, "Information")
        Catch ex As Exception
            MsgBox("Insert data Permintaan gagal dilakukan.", MsgBoxStyle.Critical, "Error")
        End Try
        conn.Close()
    End Sub

    Sub cariNoDaftar()

        conn.Close()
        Dim query As String = ""
        query = "SELECT * FROM t_registrasi WHERE noRekamedis LIKE '%" & txtNoRM.Text & "%' ORDER BY tglDaftar DESC LIMIT 1"
        cmd = New MySqlCommand(query, conn)
        da = New MySqlDataAdapter(cmd)

        Dim str As New DataTable
        str.Clear()
        da.Fill(str)

        txtNoReg.Text = ""
        txtNamaPasien.Text = ""
        txtUmurJk.Text = ""
        txtAlamat.Text = ""
        txtTglLahir.Text = ""
        txtDokter.Text = ""
        txtKdDokter.Text = ""
        txtNoPermintaan.Text = ""
        txtKetKlinis.Text = ""

        If str.Rows.Count() > 0 Then
            txtNoReg.Text = str.Rows(0)(0).ToString
            txtKdInst.Text = str.Rows(0)(9).ToString
        Else
            MessageBox.Show("Pasien Tidak Ada / Belum Terdaftar", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        conn.Close()
    End Sub

    Sub cariDataPasien()
        conn.Close()
        Try
            Dim query As String = ""
            query = "SELECT * 
                       FROM vw_datapasien
                      WHERE noRekamedis Like '%" & txtNoRM.Text & "%'"
            cmd = New MySqlCommand(query, conn)
            da = New MySqlDataAdapter(cmd)

            Dim str As New DataTable
            str.Clear()
            da.Fill(str)

            txtNamaPasien.Text = ""
            txtTglLahir.Text = ""
            txtUmurJk.Text = ""
            txtAlamat.Text = ""

            Dim jk As String

            If str.Rows.Count() > 0 Then
                txtNamaPasien.Text = str.Rows(0)(1).ToString
                txtTglLahir.Text = str.Rows(0)(4).ToString

                If str.Rows(0)(5).ToString.Contains("L") Then
                    jk = "LAKI-LAKI"
                ElseIf str.Rows(0)(5).ToString.Contains("P") Then
                    jk = "PEREMPUAN"
                End If

                txtUmurJk.Text = hitungUmur(Convert.ToDateTime(txtTglLahir.Text)) & " / " & jk
                txtAlamat.Text = str.Rows(0)(8).ToString

                Call cariInst()
                Call cariUnit()
                Call cariKelas()
                txtKetKlinis.Enabled = True
                txtKetKlinis.BackColor = Color.FromArgb(255, 112, 112)
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MessageBoxIcon.Exclamation, "SELECT DATA PASIEN")
        End Try
        conn.Close()
    End Sub

    Sub cariUnit()
        Call koneksiServer()
        Dim cmdUnit As MySqlCommand
        Dim daUnit As MySqlDataAdapter
        Dim queryUnit As String = ""

        If txtInst.Text.Contains("IGD") Then
            queryUnit = "SELECT kdUnit, unit, namapetugasMedis FROM vw_pasienrawatjalan WHERE noDaftar = '" & txtNoReg.Text & "'"
        ElseIf txtInst.Text.Contains("RAWAT INAP") Then
            queryUnit = "SELECT kdRawatInap, rawatInap, namapetugasMedis FROM vw_pasienrawatinap WHERE noDaftar = '" & txtNoReg.Text & "'"
        ElseIf txtInst.Text.Contains("RAWAT JALAN") Then
            queryUnit = "SELECT kdUnit, unit, namapetugasMedis FROM vw_pasienrawatjalan WHERE noDaftar = '" & txtNoReg.Text & "'"
        End If

        cmdUnit = New MySqlCommand(queryUnit, conn)
        daUnit = New MySqlDataAdapter(cmdUnit)

        Dim str As New DataTable
        str.Clear()
        daUnit.Fill(str)

        txtUnitAsal.Text = ""

        If str.Rows.Count() > 0 Then
            txtKdUnit.Text = str.Rows(0)(0).ToString
            txtUnitAsal.Text = str.Rows(0)(1).ToString
            txtDokter.Text = str.Rows(0)(2).ToString
        Else
            MessageBox.Show("Pasien Tidak Ada / Belum Terdaftar pada Poli atau Ruang", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        conn.Close()

        Call autoNoPermintaan()
    End Sub

    Sub cariKelas()
        Call koneksiServer()

        If txtInst.Text.Contains("IGD") Then
            txtKelas.Text = "KELAS I"
        ElseIf txtInst.Text.Contains("RAWAT INAP") Then
            Dim query As String = ""
            query = "SELECT kelas FROM vw_pasienrawatinap WHERE noDaftar = '" & txtNoReg.Text & "'"
            cmd = New MySqlCommand(query, conn)
            da = New MySqlDataAdapter(cmd)

            Dim str As New DataTable
            str.Clear()
            da.Fill(str)
            txtKelas.Text = ""

            If str.Rows.Count() > 0 Then
                txtKelas.Text = str.Rows(0)(0).ToString
            Else
                MessageBox.Show("Pasien Tidak Ada / Belum Terdaftar pada Poli atau Ruang", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        ElseIf txtInst.Text.Contains("RAWAT JALAN") Then
            txtKelas.Text = "KELAS III"
        End If

        conn.Close()
    End Sub

    Sub cariInst()
        Call koneksiServer()
        Try
            Dim cmdInst As MySqlCommand
            Dim daInst As MySqlDataAdapter
            Dim queryInst As String = ""

            queryInst = "SELECT instalasi FROM t_instalasiunit WHERE kdInstalasi = '" & txtKdInst.Text & "'"
            cmdInst = New MySqlCommand(queryInst, conn)
            daInst = New MySqlDataAdapter(cmdInst)

            Dim str As New DataTable
            str.Clear()
            daInst.Fill(str)

            txtInst.Text = ""
            If str.Rows.Count() > 0 Then
                txtInst.Text = str.Rows(0)(0).ToString
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MessageBoxIcon.Exclamation, "SELECT INSTALASI")
        End Try
        conn.Close()
    End Sub

    Sub autoNoPermintaan()
        Dim noPermintaanLab As String

        Try
            Call koneksiServer()
            Dim query As String = ""
            Dim kode As String = ""

            If txtInst.Text.Contains("IGD") Then
                query = "SELECT SUBSTR(noRegistrasiBDRS,17,4) FROM t_registrasibdrsrajal ORDER BY CAST(SUBSTR(noRegistrasiBDRS,17,4) AS UNSIGNED) DESC LIMIT 1"
                kode = "RJBD"
            ElseIf txtInst.Text.Contains("RAWAT INAP") Then
                query = "SELECT SUBSTR(noRegistrasiBDRS,17,4) FROM t_registrasibdrsranap ORDER BY CAST(SUBSTR(noRegistrasiBDRS,17,4) AS UNSIGNED) DESC LIMIT 1"
                kode = "RIBD"
            ElseIf txtInst.Text.Contains("RAWAT JALAN") Then
                query = "SELECT SUBSTR(noRegistrasiBDRS,17,4) FROM t_registrasibdrsrajal ORDER BY CAST(SUBSTR(noRegistrasiBDRS,17,4) AS UNSIGNED) DESC LIMIT 1"
                kode = "RJBD"
            End If

            cmd = New MySqlCommand(query, conn)
            dr = cmd.ExecuteReader
            dr.Read()
            If dr.HasRows Then
                dr.Read()
                noPermintaanLab = kode + Format(Now, "ddMMyyHHmmss") + "-" + (Val(Trim(dr.Item(0).ToString)) + 1).ToString
                txtNoPermintaan.Text = noPermintaanLab
            Else
                noPermintaanLab = kode + Format(Now, "ddMMyyHHmmss") + "-1"
                txtNoPermintaan.Text = noPermintaanLab
            End If
            dr.Close()
            conn.Close()
        Catch ex As Exception
            MsgBox(ex.Message, MessageBoxIcon.Exclamation, "NO.PERMINTAAN")
        End Try
    End Sub

    Sub autoComboPoliRuang()
        Call koneksiServer()
        Dim cmd As MySqlCommand
        Dim da As MySqlDataAdapter

        cmd = New MySqlCommand("SELECT a.*
                                FROM	(
				                            SELECT UPPER(unit) AS unit FROM t_unit WHERE kdInstalasi = 'ki1' ORDER BY unit ASC
			                            ) AS a
                                UNION ALL
                                SELECT b.*
                                FROM 	(
				                            SELECT UPPER(rawatInap) AS unit FROM t_rawatinap ORDER BY rawatInap ASC
			                            ) AS b", conn)
        da = New MySqlDataAdapter(cmd)
        Dim dt As New DataTable
        da.Fill(dt)

        txtUnitAsal.DataSource = dt
        txtUnitAsal.DisplayMember = "unit"
        txtUnitAsal.ValueMember = "unit"
        txtUnitAsal.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        txtUnitAsal.AutoCompleteSource = AutoCompleteSource.ListItems
        conn.Close()
    End Sub

    Private Sub TambahPasien_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim dt As String
        dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        datePermintaan.Text = dt

        Call autoComboPoliRuang()
        Call autoDokter()
    End Sub

    Private Sub btnCari_Click(sender As Object, e As EventArgs) Handles btnCari.Click
        If txtNoRM.Text = "" Then
            MessageBox.Show("Masukkan No.RM Pasien", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            If Ambil_Data = True Then
                Select Case Form_Ambil_Data
                    Case "PasienRS"
                        Call cariNoDaftar()
                        Call cariDataPasien()
                    Case "PasienLuar"
                        Return
                End Select
            End If

        End If
    End Sub

    Private Sub txtNoRM_KeyDown(sender As Object, e As KeyEventArgs) Handles txtNoRM.KeyDown
        If e.KeyCode = Keys.Enter And txtNoRM.Text = "" Then
            MessageBox.Show("Masukkan No.RM Pasien", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ElseIf e.KeyCode = Keys.Enter Then
            Call cariNoDaftar()
            Call cariDataPasien()
        End If
    End Sub

    Private Sub txtDokter_TextChanged(sender As Object, e As EventArgs) Handles txtDokter.TextChanged
        Call koneksiServer()
        Try
            Dim cmdDok As MySqlCommand
            Dim drDok As MySqlDataReader
            Dim queryDok As String = ""

            queryDok = "SELECT kdPetugasMedis FROM t_tenagamedis2 WHERE namapetugasMedis = '" & txtDokter.Text & "'"
            cmdDok = New MySqlCommand(queryDok, conn)
            drDok = cmdDok.ExecuteReader

            While drDok.Read
                txtKdDokter.Text = UCase(drDok.GetString("kdPetugasMedis"))
            End While
            drDok.Close()
        Catch ex As Exception
            MsgBox(ex.Message, MessageBoxIcon.Exclamation, "KODE DOKTER")
        End Try

        If txtDokter.Text <> "" Then
            txtDokter.BackColor = Color.White
        End If

        conn.Close()
    End Sub

    Private Sub btnTambah_Click(sender As Object, e As EventArgs) Handles btnTambah.Click
        If txtKetKlinis.Text = "" Then
            MsgBox("Mohon Ket.Klinis diisi terlebih dahulu !!", MsgBoxStyle.Exclamation)
            Me.ErrorProvider1.SetError(Me.txtKetKlinis, "ket.Klinis harus diisi")
        Else
            Me.ErrorProvider1.SetError(Me.txtKetKlinis, "")
            Select Case txtInst.Text
                Case "IGD"
                    Call addRegLabRajal()
                Case "RAWAT JALAN"
                    Call addRegLabRajal()
                Case "RAWAT INAP"
                    Call addRegLabRanap()
            End Select

            Tindakan.Ambil_Data = True
            Tindakan.Form_Ambil_Data = "Tindakan RS"
            Tindakan.Show()

            Me.Close()
            Form1.Hide()
        End If
    End Sub

    Private Sub txtKetKlinis_TextChanged(sender As Object, e As EventArgs) Handles txtKetKlinis.TextChanged
        If txtKetKlinis.Text <> "" Then
            txtKetKlinis.BackColor = Color.White
        End If
    End Sub

    Private Sub txtKetKlinis_LostFocus(sender As Object, e As EventArgs) Handles txtKetKlinis.LostFocus
        If txtKetKlinis.Text = "" Then
            txtKetKlinis.BackColor = Color.FromArgb(255, 112, 112)
        End If
    End Sub

    Private Sub txtKetKlinis_KeyDown(sender As Object, e As KeyEventArgs) Handles txtKetKlinis.KeyDown
        If e.KeyCode = Keys.Enter Then
            SendKeys.Send("{TAB}")
            If txtKetKlinis.Text = "" Then
                txtKetKlinis.BackColor = Color.FromArgb(255, 112, 112)
            End If
        End If
    End Sub

    Private Sub txtDokter_LostFocus(sender As Object, e As EventArgs) Handles txtDokter.LostFocus
        If txtDokter.Text = "-" Then
            txtDokter.BackColor = Color.FromArgb(255, 112, 112)
        End If
    End Sub

    Private Sub txtDokter_KeyDown(sender As Object, e As KeyEventArgs) Handles txtDokter.KeyDown
        If e.KeyCode = Keys.Enter Then
            SendKeys.Send("{TAB}")
            If txtDokter.Text = "-" Then
                txtDokter.BackColor = Color.FromArgb(255, 112, 112)
            End If
        End If
    End Sub

    Private Sub Tambah_Pasien_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        txtNoRM.Text = ""
        txtNoReg.Text = ""
        txtNamaPasien.Text = ""
        txtUmurJk.Text = ""
        txtAlamat.Text = ""
        txtTglLahir.Text = ""
        txtInst.Text = ""
        txtKdInst.Text = ""
        txtUnitAsal.Text = ""
        txtKdUnit.Text = ""
        txtDokter.Text = ""
        txtKdDokter.Text = ""
        txtNoPermintaan.Text = ""
        txtKetKlinis.Text = ""
    End Sub

    Private Sub txtUnitAsal_SelectedIndexChanged(sender As Object, e As EventArgs) Handles txtUnitAsal.SelectedIndexChanged
        Call koneksiServer()
        Try
            Dim cmd As MySqlCommand
            Dim dr As MySqlDataReader
            Dim query As String = ""
            query = "SELECT a.*
                        FROM	(
				                    SELECT kdUnit FROM t_unit WHERE unit = '" & txtUnitAsal.Text & "'
			                    ) AS a
                    UNION ALL
                        SELECT b.*
                        FROM    (
				                    SELECT kdRawatInap FROM t_rawatinap WHERE rawatInap = '" & txtUnitAsal.Text & "'
			                    ) AS b"
            cmd = New MySqlCommand(query, conn)
            dr = cmd.ExecuteReader

            While dr.Read
                txtKdUnit.Text = UCase(dr.GetString("kdUnit"))
            End While
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

        If txtDokter.Text <> "" Then
            txtDokter.BackColor = Color.White
        End If

        conn.Close()
    End Sub
End Class