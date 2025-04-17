Imports System.Data.SqlClient
Public Class Form1
    Dim con As New SqlConnection("Data Source=DESKTOP-E0FMQ4O\SQLEXPRESS;Initial Catalog=Pet Shop Management;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim name As String = TextBox1.Text
        Dim password As String = TextBox2.Text
        If name = "" Or password = "" Then
            MessageBox.Show("Please enter both name and password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ComboBox1.Text = ""
            TextBox1.Text = ""
            TextBox2.Text = ""
            Exit Sub
        End If
        Try
            con.Open()
            If ComboBox1.SelectedItem = "" Then
                MessageBox.Show("Please select a role!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ComboBox1.Text = ""
                TextBox1.Text = ""
                TextBox2.Text = ""
            End If
            If ComboBox1.SelectedItem = "Admin" Then
                Dim cmdAdmin As New SqlCommand("SELECT * FROM Admin WHERE Name=@name AND Password=@password", con)
                cmdAdmin.Parameters.AddWithValue("@name", name)
                cmdAdmin.Parameters.AddWithValue("@password", password)
                Dim readerAdmin As SqlDataReader = cmdAdmin.ExecuteReader()
                If readerAdmin.HasRows Then
                    readerAdmin.Read()
                    Me.Hide()
                    Form3.Show()
                    con.Close()
                    Exit Sub
                End If
                readerAdmin.Close()
            ElseIf ComboBox1.SelectedItem = "Employee" Then
                Dim cmdEmp As New SqlCommand("SELECT * FROM Employee WHERE Name=@name AND Password=@password", con)
                cmdEmp.Parameters.AddWithValue("@name", name)
                cmdEmp.Parameters.AddWithValue("@password", password)
                Dim readerEmp As SqlDataReader = cmdEmp.ExecuteReader()
                If readerEmp.HasRows Then
                    readerEmp.Read()
                    Me.Hide()
                    Form10.Show()
                    con.Close()
                    Exit Sub
                End If
                readerEmp.Close()
            End If
            MessageBox.Show("Invalid Credentials!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ComboBox1.Text = ""
            TextBox1.Text = ""
            TextBox2.Text = ""
            con.Close()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ComboBox1.Text = ""
            TextBox1.Text = ""
            TextBox2.Text = ""
        End Try
    End Sub
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Form2.TextBox2.Text = ""
        Form2.TextBox3.Text = "+91"
        Form2.TextBox4.Text = ""
        Me.Hide()
        Form2.Show()
    End Sub
    Private Sub textbox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress
        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub
End Class
