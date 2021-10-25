Imports MySql.Data.MySqlClient
Public Class Tindakan

    Public Ambil_Data As String
    Public Form_Ambil_Data As String

    Sub setColor(button As Button)
        btnDash.BackColor = SystemColors.HotTrack
        btnTindakan.BackColor = SystemColors.HotTrack
        button.BackColor = Color.DodgerBlue
    End Sub

    Sub tampilData()
        Call koneksiServer()
        da = New MySqlDataAdapter("SELECT kdTarif, tindakan, tarif 
                                     FROM vw_tindakanlab 
                                    WHERE kdKelompokTindakan = 59 AND 
                                          kelas = '" & txtKelas.Text & "'", conn)
        ds = New DataSet
        da.Fill(ds, "vw_tindakanlab")
        BunifuDgv1.DataSource = ds.Tables("vw_tindakanlab")
        conn.Close()
    End Sub

    Sub aturDGV()
        Try
            BunifuDgv1.Columns(0).Width = 70
            BunifuDgv1.Columns(1).Width = 300
            BunifuDgv1.Columns(2).Width = 130
            BunifuDgv1.Columns(0).HeaderText = "KODE"
            BunifuDgv1.Columns(1).HeaderText = "TINDAKAN"
            BunifuDgv1.Columns(2).HeaderText = "TARIF"
            BunifuDgv1.Columns(2).DefaultCellStyle.Format = "###,###,###"
            BunifuDgv1.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            BunifuDgv1.DefaultCellStyle.ForeColor = Color.Black
            BunifuDgv1.DefaultCellStyle.SelectionForeColor = Color.Black

            dgv2.DefaultCellStyle.ForeColor = Color.Black
            dgv2.DefaultCellStyle.SelectionForeColor = Color.Black

            For i = 0 To BunifuDgv1.RowCount - 1
                If i Mod 2 = 0 Then
                    BunifuDgv1.Rows(i).DefaultCellStyle.BackColor = Color.White
                Else
                    BunifuDgv1.Rows(i).DefaultCellStyle.BackColor = Color.AliceBlue
                End If
            Next

            For i = 0 To dgv2.RowCount - 1
                If i Mod 2 = 0 Then
                    dgv2.Rows(i).DefaultCellStyle.BackColor = Color.White
                Else
                    dgv2.Rows(i).DefaultCellStyle.BackColor = Color.AliceBlue
                End If
            Next

        Catch ex As Exception

        End Try
    End Sub

    Sub autoNoTindakan()
        Dim noTindakan As String

        Try
            Call koneksiServer()
            Dim query As String
            query = "SELECT SUBSTR(noTindakanBDRS,17,4) FROM t_tindakanbdrs ORDER BY CAST(SUBSTR(noTindakanBDRS,17,4) AS UNSIGNED) DESC LIMIT 1"
            cmd = New MySqlCommand(query, conn)
            dr = cmd.ExecuteReader
            dr.Read()
            If dr.HasRows Then
                dr.Read()
                noTindakan = "TBD" + Format(Now, "ddMMyyHHmmss") + "-" + (Val(Trim(dr.Item(0).ToString)) + 1).ToString
                txtNoTindakan.Text = noTindakan
            Else
                noTindakan = "TBD" + Format(Now, "ddMMyyHHmmss") + "-1"
                txtNoTindakan.Text = noTindakan
            End If
            conn.Close()
        Catch ex As Exception

        End Try

    End Sub

    Sub transferSelected()
        Call koneksiServer()
        Dim dt As New DataTable
        Dim dr As New System.Windows.Forms.DataGridViewRow

        Dim R As DataRow = dt.NewRow

        For Each dr In Me.BunifuDgv1.SelectedRows
            dgv2.Rows.Add(1)
            dgv2.Rows(dgv2.RowCount - 1).Cells(0).Value = txtNoPermintaan.Text
            dgv2.Rows(dgv2.RowCount - 1).Cells(1).Value = txtNoTindakan.Text
            dgv2.Rows(dgv2.RowCount - 1).Cells(2).Value = dr.Cells(0).Value.ToString
            dgv2.Rows(dgv2.RowCount - 1).Cells(3).Value = dr.Cells(1).Value.ToString
            dgv2.Rows(dgv2.RowCount - 1).Cells(4).Value = dr.Cells(2).Value.ToString
            dgv2.Rows(dgv2.RowCount - 1).Cells(5).Value = 1
            dgv2.Rows(dgv2.RowCount - 1).Cells(6).Value = dr.Cells(2).Value.ToString
            dgv2.Rows(dgv2.RowCount - 1).Cells(7).Value = datePermintaan.Text
            dgv2.Rows(dgv2.RowCount - 1).Cells(8).Value = txtKdDokter.Text
            dgv2.Rows(dgv2.RowCount - 1).Cells(9).Value = txtDokter.Text
            dgv2.Rows(dgv2.RowCount - 1).Cells(10).Value = txtNoReg.Text
            dgv2.Rows(dgv2.RowCount - 1).Cells(11).Value = "PERMINTAAN"
            dgv2.Update()
        Next

        For i As Integer = 0 To dgv2.RowCount - 1
            If dgv2.Rows(i).Cells(11).Value.ToString = "PERMINTAAN" Then
                dgv2.Rows(i).Cells(11).Style.BackColor = Color.Orange
                dgv2.Rows(i).Cells(11).Style.ForeColor = Color.White
            End If
        Next

        conn.Close()
    End Sub

    Sub totalTarif()
        Dim totTarif As Long = 0

        For i As Integer = 0 To dgv2.Rows.Count - 1
            totTarif = totTarif + Val(dgv2.Rows(i).Cells(6).Value)
        Next
        txtTotalTarif.Text = totTarif
    End Sub

    Sub totalTarifDgv3()
        Dim totTarif As Long = 0

        For i As Integer = 0 To dgv3.Rows.Count - 1
            totTarif = totTarif + Val(dgv3.Rows(i).Cells(3).Value)
        Next
        txtTotalTarifDgv3.Text = totTarif
    End Sub

    Sub addTindakan()
        Call koneksiServer()

        Try
            Dim str As String = ""

            str = "INSERT INTO t_tindakanbdrs(noTindakanBDRS,noRegistrasiBDRS,
                                              totalTindakanBDRS,statusPembayaran) 
                   VALUES ('" & txtNoTindakan.Text & "','" & txtNoPermintaan.Text & "',
                           '" & txtTotalTarif.Text & "','BELUM LUNAS')"

            cmd = New MySqlCommand(str, conn)
            cmd.ExecuteNonQuery()
            MessageBox.Show("Tindakan Penunjang berhasil disimpan", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(ex.Message & " - Insert Tindakan Penunjang gagal dilakukan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        conn.Close()
    End Sub

    Sub addDetail()
        Call koneksiServer()

        Try
            Dim str1 As String
            Dim val0, val1, val2, val3, val4, val5 As String
            str1 = "INSERT INTO t_detailtindakanbdrs
                              (noTindakanBDRS,kdTarif,tindakan,tarif,jumlahTindakan,totalTarif,statusTindakan) 
                       VALUES (@noTindakanBDRS,@kdTarif,@tindakan,@tarif,@qty,@totalTarif,'PERMINTAAN')"
            cmd = New MySqlCommand(str1, conn)

            For i As Integer = 0 To dgv2.Rows.Count - 1

                val0 = dgv2.Rows(i).Cells(1).Value
                val1 = dgv2.Rows(i).Cells(2).Value
                val2 = dgv2.Rows(i).Cells(3).Value
                val3 = dgv2.Rows(i).Cells(4).Value
                val4 = dgv2.Rows(i).Cells(5).Value
                val5 = dgv2.Rows(i).Cells(6).Value

                cmd.Parameters.AddWithValue("@noTindakanBDRS", val0)
                cmd.Parameters.AddWithValue("@kdTarif", val1)
                cmd.Parameters.AddWithValue("@tindakan", val2)
                cmd.Parameters.AddWithValue("@tarif", val3)
                cmd.Parameters.AddWithValue("@qty", val4)
                cmd.Parameters.AddWithValue("@totalTarif", val5)

                cmd.ExecuteNonQuery()
                cmd.Parameters.Clear()
            Next
            'MessageBox.Show("Detail Tindakan Penunjang berhasil disimpan", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error")
            cmd.Dispose()
        End Try
        conn.Close()

    End Sub

    Sub tampilDataSudahDitindakAll(kdIns, noTindakan)
        Dim query As String = ""
        query = "CALL datadetailbdrs('" & noTindakan & "')"

        Try
            Call koneksiServer()
            cmd = New MySqlCommand(query, conn)
            dr = cmd.ExecuteReader
            dgv3.Rows.Clear()

            Do While dr.Read
                dgv3.Rows.Add(dr.Item("kdTarif"), dr.Item("tindakan"), dr.Item("PPA"),
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

    Sub updateTarifPermintaan(noTindakan As String)
        Dim total As Integer
        total = Integer.Parse(txtTotalTarifDgv3.Text) + Integer.Parse(txtTotalTarif.Text)

        Call koneksiServer()
        Dim str As String = ""

        str = "UPDATE t_tindakanbdrs
                  SET totalTindakanBDRS = '" & total & "'
                WHERE noTindakanBDRS = '" & noTindakan & "'"
        Try
            cmd = New MySqlCommand(str, conn)
            cmd.ExecuteNonQuery()
            'MsgBox("Update dokter berhasil dilakukan", MessageBoxIcon.Information)
        Catch ex As Exception
            MsgBox("Update data gagal dilakukan.", MessageBoxIcon.Error, "Error Update After Delete")
        End Try

        conn.Close()
    End Sub

    Sub caridata()
        Call koneksiServer()

        Dim query As String
        query = "SELECT kdTarif, tindakan, tarif 
                   FROM vw_tindakanlab 
                  WHERE kdKelompokTindakan = 59 AND 
                        kelas = '" & txtKelas.Text & "' AND 
                        tindakan LIKE '%" & txtCari.Text & "%'"
        da = New MySqlDataAdapter(query, conn)

        Dim str As New DataTable
        str.Clear()
        da.Fill(str)
        BunifuDgv1.DataSource = str

        conn.Close()
    End Sub

    Private Sub Tindakan_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Normal
        Me.StartPosition = FormStartPosition.Manual
        With Screen.PrimaryScreen.WorkingArea
            Me.SetBounds(.Left, .Top, .Width, .Height)
        End With

        pnlStats.Height = btnTindakan.Height
        pnlStats.Top = btnTindakan.Top

        btnTindakan.BackColor = Color.DodgerBlue

        If Ambil_Data = True Then
            Select Case Form_Ambil_Data
                Case "Tindakan RS"
                    txtNoRM.Text = TambahPasien.txtNoRM.Text
                    txtNoReg.Text = TambahPasien.txtNoReg.Text
                    txtNamaPasien.Text = TambahPasien.txtNamaPasien.Text
                    txtAlamat.Text = TambahPasien.txtAlamat.Text
                    txtUsia.Text = TambahPasien.txtUmurJk.Text
                    txtKdDokter.Text = TambahPasien.txtKdDokter.Text
                    txtDokter.Text = TambahPasien.txtDokter.Text
                    txtRs.Text = "RSU KARSA HUSADA BATU"
                    txtInst.Text = TambahPasien.txtInst.Text
                    txtUnit.Text = TambahPasien.txtUnitAsal.Text
                    dateReg.Text = TambahPasien.datePermintaan.Text
                    txtNoPermintaan.Text = TambahPasien.txtNoPermintaan.Text
                    txtKetKlinis.Text = TambahPasien.txtKetKlinis.Text
                    txtKelas.Text = TambahPasien.txtKelas.Text
                    SplitContainer1.Panel2Collapsed = True
                    Call autoNoTindakan()
                Case "Tindakan Luar"
                    'txtNoRM.Text = TambahPasienLuar.txtNoRM.Text
                    'txtNoReg.Text = TambahPasienLuar.txtNoReg.Text
                    'txtNamaPasien.Text = TambahPasienLuar.txtNamaPasien.Text
                    'txtAlamat.Text = TambahPasienLuar.txtAlamat.Text
                    'If TambahPasienLuar.txtJk.Text = "L" Then
                    '    jk = "LAKI-LAKI"
                    'ElseIf TambahPasienLuar.txtJk.Text = "L" Then
                    '    jk = "PEREMPUAN"
                    'End If
                    'txtUsia.Text = TambahPasienLuar.txtUmur.Text & " / " & jk
                    'txtDokter.Text = TambahPasienLuar.txtDokter.Text
                    'txtRs.Text = TambahPasienLuar.txtRs.Text
                    'txtInst.Text = TambahPasienLuar.txtInst.Text
                    'txtUnit.Text = TambahPasienLuar.txtUnitAsal.Text
                    'txtKelas.Text = "KELAS I"
                    'dateReg.Text = TambahPasienLuar.datePermintaan.Text
                    'txtNoPermintaan.Text = TambahPasienLuar.txtNoPermintaan.Text
                    'txtKetKlinis.Text = TambahPasienLuar.txtKetKlinis.Text
                    'SplitContainer1.Panel2Collapsed = True
                    'Call autoNoTindakan()
                Case "Edit Tindakan"
                    txtNoRM.Text = Form1.txtNoRM.Text
                    txtNoReg.Text = Form1.txtNoReg.Text
                    txtNamaPasien.Text = Form1.txtNamaPasien.Text
                    txtAlamat.Text = Form1.txtAlamat.Text
                    txtUsia.Text = Form1.txtUsia.Text
                    'txtKdDokter.Text = Form1.txtKdDokter.Text
                    txtDokter.Text = Form1.txtDokter.Text
                    txtRs.Text = "RSU KARSA HUSADA BATU"
                    txtInst.Text = Form1.txtInstalasi.Text
                    txtUnit.Text = Form1.unit
                    'dateReg.Text = Form1.datePermintaan.Text
                    txtNoPermintaan.Text = Form1.txtNoPermintaan.Text
                    txtNoTindakan.Text = Form1.noTindakanPenunjang
                    txtKetKlinis.Text = Form1.txtKlinis.Text
                    txtKelas.Text = Form1.txtKelas.Text
                    SplitContainer1.Panel2Collapsed = False
                    btnSimpan.Enabled = True
                    btnSimpan.Text = "Update Tindakan"
                    btnSimpan.BackColor = Color.DodgerBlue
                    Call totalTarifDgv3()
            End Select
        End If

        Call tampilData()
        Call aturDGV()
        Call totalTarif()
    End Sub

    Private Sub btnDash_Click(sender As Object, e As EventArgs) Handles btnDash.Click
        Dim konfirmasi As MsgBoxResult

        konfirmasi = MsgBox("Apakah tindakan sudah disimpan ?", vbQuestion + vbYesNo, "EXIT")
        If konfirmasi = vbYes Then
            Form1.pnlStats.Height = Form1.btnDash.Height
            Form1.pnlStats.Top = Form1.btnDash.Top
            Form1.btnDash.BackColor = Color.DodgerBlue
            Form1.Show()
            Me.Close()
        End If
    End Sub

    Private Sub btnTindakan_Click(sender As Object, e As EventArgs) Handles btnTindakan.Click
        pnlStats.Height = btnTindakan.Height
        pnlStats.Top = btnTindakan.Top

        Dim btn As Button = CType(sender, Button)
        setColor(btn)

    End Sub

    Private Sub btnKeluar_Click(sender As Object, e As EventArgs) Handles btnKeluar.Click
        Dim konfirmasi As MsgBoxResult

        konfirmasi = MsgBox("Apakah tindakan sudah disimpan ?", vbQuestion + vbYesNo, "EXIT")
        If konfirmasi = vbYes Then
            Form1.pnlStats.Height = Form1.btnDash.Height
            Form1.pnlStats.Top = Form1.btnDash.Top
            Form1.btnDash.BackColor = Color.DodgerBlue
            Form1.Show()
            Me.Close()
        End If
    End Sub

    Private Sub btnPilihOk_Click(sender As Object, e As EventArgs) Handles btnPilihOk.Click
        Call transferSelected()

        If dgv2.Rows.Count > 0 Then
            Me.btnSimpan.Enabled = True
        Else
            Me.btnSimpan.Enabled = False
        End If

        Call totalTarif()
    End Sub

    Private Sub btnPilihCancel_Click(sender As Object, e As EventArgs) Handles btnPilihCancel.Click
        Dim drDgv As New DataGridViewRow
        For Each drDgv In Me.dgv2.SelectedRows
            dgv2.Rows.Remove(drDgv)
        Next

        If dgv2.Rows.Count > 0 Then
            Me.btnSimpan.Enabled = True
        Else
            Me.btnSimpan.Enabled = False
        End If

        Call totalTarif()
    End Sub

    Sub deleteTindakan(id As String)
        Call koneksiServer()
        Dim query As String
        query = "DELETE FROM t_detailtindakanbdrs WHERE idTindakanBDRS = '" & id & "'"
        cmd = New MySqlCommand(query, conn)
        cmd.ExecuteNonQuery()

        Call totalTarif()
        conn.Close()
    End Sub

    Private Sub txtCari_KeyDown(sender As Object, e As KeyEventArgs) Handles txtCari.KeyDown
        If e.KeyCode = Keys.Enter Then
            Call caridata()
        End If
    End Sub

    Private Sub btnSimpan_Click(sender As Object, e As EventArgs) Handles btnSimpan.Click
        If btnSimpan.Text = "Update Tindakan" Then 'UPDATE
            Call addDetail()
            Call updateTarifPermintaan(txtNoTindakan.Text)
        Else                                       'INSERT
            Call addTindakan()
            Call addDetail()
        End If
        dgv2.Rows.Clear()
        Me.Close()
        Form1.pnlStats.Height = Form1.btnDash.Height
        Form1.pnlStats.Top = Form1.btnDash.Top
        Form1.btnDash.BackColor = Color.DodgerBlue
        Form1.Show()
        Call Form1.tampilDataAll()

    End Sub

    Private Sub BunifuDgv1_KeyDown(sender As Object, e As KeyEventArgs) Handles BunifuDgv1.KeyDown
        If e.KeyCode = Keys.Enter And BunifuDgv1.CurrentCell.RowIndex >= 0 Then
            e.Handled = True
            e.SuppressKeyPress = True

            Dim row As DataGridViewRow
            row = Me.BunifuDgv1.Rows(BunifuDgv1.CurrentCell.RowIndex)

            If BunifuDgv1.CurrentCell.RowIndex = -1 Then
                Return
            End If

            Call transferSelected()
            Call totalTarif()
            btnSimpan.Enabled = True
        End If
    End Sub

    Private Sub dgv2_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles dgv2.RowPostPaint
        Dim dg As DataGridView = DirectCast(sender, DataGridView)
        ' Current row record
        Dim rowNumber As String = (e.RowIndex + 1).ToString()

        ' Position text
        Dim size As SizeF = e.Graphics.MeasureString(rowNumber, Me.Font)
        If dg.RowHeadersWidth < CInt(size.Width + 20) Then
            dg.RowHeadersWidth = CInt(size.Width + 20)
        End If

        ' Use default system text brush
        Dim b As Brush = SystemBrushes.ControlText

        ' Draw row number
        e.Graphics.DrawString(rowNumber, dg.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2))
    End Sub

    Private Sub dgv3_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles dgv3.RowPostPaint
        Dim dg As DataGridView = DirectCast(sender, DataGridView)
        ' Current row record
        Dim rowNumber As String = (e.RowIndex + 1).ToString()

        ' Position text
        Dim size As SizeF = e.Graphics.MeasureString(rowNumber, Me.Font)
        If dg.RowHeadersWidth < CInt(size.Width + 20) Then
            dg.RowHeadersWidth = CInt(size.Width + 20)
        End If

        ' Use default system text brush
        Dim b As Brush = SystemBrushes.ControlText

        ' Draw row number
        e.Graphics.DrawString(rowNumber, dg.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2))
    End Sub

    Private Sub BunifuDgv1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles BunifuDgv1.CellFormatting
        For i = 0 To BunifuDgv1.RowCount - 1
            If i Mod 2 = 0 Then
                BunifuDgv1.Rows(i).DefaultCellStyle.BackColor = Color.White
            Else
                BunifuDgv1.Rows(i).DefaultCellStyle.BackColor = Color.AliceBlue
            End If
        Next
    End Sub

    Private Sub dgv2_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgv2.CellFormatting
        dgv2.Columns(4).DefaultCellStyle.Format = "###,###,###"
        dgv2.Columns(6).DefaultCellStyle.Format = "###,###,###"
        dgv2.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dgv2.Columns(6).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dgv2.Columns(11).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        For k As Integer = 0 To dgv2.RowCount - 1
            If k Mod 2 = 0 Then
                dgv2.Rows(k).DefaultCellStyle.BackColor = Color.White
            Else
                dgv2.Rows(k).DefaultCellStyle.BackColor = Color.AliceBlue
            End If
        Next
    End Sub

    Private Sub dgv2_RowsAdded(sender As Object, e As DataGridViewRowsAddedEventArgs) Handles dgv2.RowsAdded
        Call totalTarif()
    End Sub

    Private Sub dgv2_RowsRemoved(sender As Object, e As DataGridViewRowsRemovedEventArgs) Handles dgv2.RowsRemoved
        If dgv2.Rows.Count = 0 Then
            Me.btnSimpan.Enabled = False
        Else
            Me.btnSimpan.Enabled = True
        End If

        Call totalTarif()
    End Sub

    Private Sub txtKelas_SelectedIndexChanged(sender As Object, e As EventArgs) Handles txtKelas.SelectedIndexChanged
        Call tampilData()
    End Sub

    Private Sub dgv3_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgv3.CellFormatting
        For k As Integer = 0 To dgv3.RowCount - 1
            If k Mod 2 = 0 Then
                dgv3.Rows(k).DefaultCellStyle.BackColor = Color.White
            Else
                dgv3.Rows(k).DefaultCellStyle.BackColor = Color.AliceBlue
            End If
        Next
    End Sub

    Private Sub dgv2_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgv2.CellValueChanged
        For i As Integer = 0 To dgv2.RowCount - 1
            If dgv2.Rows(i).Cells(5).Value IsNot Nothing Then
                Dim subTotal As Integer
                subTotal = Integer.Parse(dgv2.Rows(i).Cells(5).Value) * Integer.Parse(dgv2.Rows(i).Cells(4).Value)
                dgv2.Rows(i).Cells(6).Value = subTotal
                Call totalTarif()
            End If
        Next
    End Sub


End Class