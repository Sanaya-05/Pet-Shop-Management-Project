Imports System.Data.SqlClient
Imports System.IO
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Public Class Form7
    Dim conn As New SqlConnection("Data Source=DESKTOP-E0FMQ4O\SQLEXPRESS;Initial Catalog=Pet Shop Management;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
    Private Sub Form7_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSupplierData()
    End Sub
    Public Sub LoadSupplierData()
        Try
            Dim query As String = "SELECT Supplier_Id, Name, Phone_Number, Pet_Id, Pet_Type FROM Supplier"
            Dim adapter As New SqlDataAdapter(query, conn)
            Dim table As New DataTable()
            adapter.Fill(table)
            DataGridView1.DataSource = table
        Catch ex As Exception
            MessageBox.Show("Error loading supplier data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        Form8.TextBox1.Text = ""
        Form8.TextBox2.Text = ""
        Form8.TextBox5.Text = ""
        Form8.TextBox3.Text = ""
        Form8.TextBox4.Text = ""
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            Dim supplierID As String = row.Cells("Supplier_Id").Value.ToString()
            Try
                Dim query As String = "SELECT * FROM Supplier WHERE Supplier_Id=@SupplierID"
                Dim cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@SupplierID", supplierID)
                conn.Open()
                Dim reader As SqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    Form8.TextBox4.Text = reader("Supplier_Id").ToString()
                    Form8.TextBox1.Text = reader("Name").ToString()
                    Form8.TextBox2.Text = reader("Phone_Number").ToString()
                    Form8.TextBox5.Text = reader("Pet_Id").ToString()
                    Form8.TextBox3.Text = reader("Pet_Type").ToString()
                End If
                reader.Close()
                conn.Close()
                Me.Hide()
                Form8.Show()
            Catch ex As Exception
                MessageBox.Show("Error fetching supplier details: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            Dim query As String = "SELECT Supplier_Id, Name, Phone_Number, Pet_Id, Pet_Type FROM Supplier WHERE Pet_Type LIKE @Search OR Name LIKE @Search"
            Dim adapter As New SqlDataAdapter(query, conn)
            adapter.SelectCommand.Parameters.AddWithValue("@Search", "%" & TextBox1.Text & "%")
            Dim table As New DataTable()
            adapter.Fill(table)
            DataGridView1.DataSource = table
        Catch ex As Exception
            MessageBox.Show("Error searching data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        TextBox1.Text = ""
        LoadSupplierData()
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form8.TextBox1.Text = ""
        Form8.TextBox2.Text = "+91"
        Form8.TextBox5.Text = ""
        Form8.TextBox3.Text = ""
        Form8.TextBox4.Text = ""
        Me.Hide()
        Form8.Show()
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Hide()
        Form3.Show()
    End Sub
End Class