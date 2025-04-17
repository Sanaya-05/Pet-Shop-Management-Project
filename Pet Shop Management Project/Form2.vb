Imports System.Data.SqlClient
Imports System.IO
Public Class Form2
    Dim con As New SqlConnection("Data Source=DESKTOP-E0FMQ4O\SQLEXPRESS;Initial Catalog=Pet Shop Management;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
    Private Function GenerateEmployeeID() As String
        Dim EnewID As String = "E101"
        Try
            Dim query As String = "SELECT MAX(Employee_Id) FROM Employee"
            Dim cmd As New SqlCommand(query, con)
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            Dim result As Object = cmd.ExecuteScalar()
            If result IsNot DBNull.Value AndAlso result IsNot Nothing Then
                Dim lastID As String = result.ToString()
                Dim num As Integer = Integer.Parse(lastID.Substring(1)) + 1
                EnewID = "E" & num.ToString()
            End If
            con.Close()
        Catch ex As Exception
            MessageBox.Show("Error generating Employee ID: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return EnewID
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim name As String = TextBox2.Text
        Dim ph_no As String = "+91" & TextBox3.Text
        Dim password As String = TextBox4.Text
        If TextBox3.Text.Length <> 13 OrElse Not IsNumeric(TextBox3.Text) Then
            MessageBox.Show("Invalid Phone number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        If name = "" Or password = "" Or ph_no = "" Then
            MessageBox.Show("Please fill all fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
        Try
            Using con As New SqlConnection("Data Source=DESKTOP-E0FMQ4O\SQLEXPRESS;Initial Catalog=Pet Shop Management;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
                con.Open()
                Dim query As String = ""
                query = "INSERT INTO Employee(Employee_Id, Name, Phone_Number, Password) VALUES (@id,@name,@phone,@pass)"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@id", GenerateEmployeeID())
                    cmd.Parameters.AddWithValue("@name", name)
                    cmd.Parameters.AddWithValue("@phone", ph_no)
                    cmd.Parameters.AddWithValue("@pass", password)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            MessageBox.Show("Registration Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Form1.ComboBox1.Text = ""
            Form1.TextBox1.Text = ""
            Form1.TextBox2.Text = ""
            Me.Hide()
            Form1.Show()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form1.ComboBox1.Text = ""
        Form1.TextBox1.Text = ""
        Form1.TextBox2.Text = ""
        Me.Hide()
        Form1.Show()
    End Sub
    Private Sub TextBox3_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox3.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub
    Private Sub textbox2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox2.KeyPress
        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub
End Class
