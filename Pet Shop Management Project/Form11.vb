Imports System.Data.SqlClient
Imports System.Windows.Controls
Public Class Form11
    Dim conn As New SqlConnection("Data Source=DESKTOP-E0FMQ4O\SQLEXPRESS;Initial Catalog=Pet Shop Management;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
    Private Sub Form11_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadAllSales()
    End Sub
    Public Sub LoadAllSales()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim query As String = "SELECT * FROM [Sales&Payment]"
            Dim adapter As New SqlDataAdapter(query, conn)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            DataGridView1.DataSource = dt
        Catch ex As Exception
            MessageBox.Show("Error loading sales: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            conn.Close()
        End Try
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim searchText As String = TextBox1.Text.Trim()
        If searchText = "" Then
            LoadAllSales()
            Return
        End If
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim query As String = "SELECT * FROM [Sales&Payment] WHERE Customer_Name LIKE @search OR Pet_Id LIKE @search"
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@search", "%" & searchText & "%")
            Dim adapter As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            DataGridView1.DataSource = dt
        Catch ex As Exception
            MessageBox.Show("Search error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            conn.Close()
        End Try
    End Sub
    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker1.ValueChanged
        Dim selectedDate As Date = DateTimePicker1.Value.Date
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim query As String = "SELECT * FROM [Sales&Payment] WHERE CAST(Date AS DATE) = @date"
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@date", selectedDate)
            Dim adapter As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            DataGridView1.DataSource = dt
        Catch ex As Exception
            MessageBox.Show("Date filter error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            conn.Close()
        End Try
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        DateTimePicker1.Value = Date.Today
        TextBox1.Text = ""
        LoadAllSales()
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Hide()
        Form3.Show()
    End Sub
End Class