Imports System.Data.SqlClient
Imports System.IO
Public Class Form8
    Dim conn As New SqlConnection("Data Source=DESKTOP-E0FMQ4O\SQLEXPRESS;Initial Catalog=Pet Shop Management;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
    Private Sub Form8_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cleartext()
        GenerateSupplierID()
    End Sub
    Private Function GenerateSupplierID() As String
        Dim newID As String = "S101"
        Try
            Dim query As String = "SELECT MAX(Supplier_Id) FROM Supplier"
            Dim cmd As New SqlCommand(query, conn)
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            Dim result As Object = cmd.ExecuteScalar()
            If result IsNot DBNull.Value AndAlso result IsNot Nothing Then
                Dim lastID As String = result.ToString()
                Dim num As Integer = Integer.Parse(lastID.Substring(1)) + 1
                newID = "S" & num.ToString()
            End If
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error generating Supplier ID: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return newID
    End Function
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            Dim supplierID As String = GenerateSupplierID()
            Dim query As String = "INSERT INTO Supplier (Supplier_Id, Name, Phone_Number, Pet_Id, Pet_Type) VALUES (@SupplierId, @Name, @Phone, @PetID, @PetType)"
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@SupplierId", supplierID)
            cmd.Parameters.AddWithValue("@Name", TextBox1.Text)
            cmd.Parameters.AddWithValue("@Phone", TextBox2.Text)
            cmd.Parameters.AddWithValue("@PetID", TextBox5.Text)
            cmd.Parameters.AddWithValue("@PetType", TextBox3.Text)
            If TextBox2.Text.Length <> 13 OrElse Not IsNumeric(TextBox2.Text) Then
                MessageBox.Show("Invalid Phone number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            cmd.ExecuteNonQuery()
            conn.Close()
            MessageBox.Show("Supplier added successfully!", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Form7.TextBox1.Text = ""
            Form7.LoadSupplierData()
            Form7.Show()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            Dim query As String = "UPDATE Supplier SET Name=@Name, Phone_Number=@Phone, Pet_Id=@PetId, Pet_Type=@PetType WHERE Supplier_Id=@SupplierID"
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Name", TextBox1.Text)
            cmd.Parameters.AddWithValue("@Phone", TextBox2.Text)
            cmd.Parameters.AddWithValue("@PetID", TextBox5.Text)
            cmd.Parameters.AddWithValue("@PetType", TextBox3.Text)
            cmd.Parameters.AddWithValue("@SupplierID", TextBox4.Text)
            If TextBox2.Text.Length <> 13 OrElse Not IsNumeric(TextBox2.Text) Then
                MessageBox.Show("Invalid Phone number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            cmd.ExecuteNonQuery()
            conn.Close()
            MessageBox.Show("Supplier updated successfully!", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Form7.TextBox1.Text = ""
            Form7.LoadSupplierData()
            Form7.Show()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete it?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            If result = DialogResult.Yes Then
                Dim query As String = "DELETE FROM Supplier WHERE Supplier_Id=@SupplierID"
                Dim cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@SupplierID", TextBox4.Text)
                If conn.State = ConnectionState.Closed Then
                    conn.Open()
                End If
                cmd.ExecuteNonQuery()
                conn.Close()
                MessageBox.Show("Supplier deleted successfully!", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Form7.TextBox1.Text = ""
                Form7.LoadSupplierData()
                Form7.Show()
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        TextBox1.Text = ""
        TextBox2.Text = "+91"
        TextBox5.Text = ""
        TextBox3.Text = ""
        TextBox4.Text = ""
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form7.TextBox1.Text = ""
        Form7.LoadSupplierData()
        Me.Hide()
        Form7.Show()
    End Sub
    Public Sub cleartext()
        TextBox1.Text = ""
        TextBox2.Text = "+91"
        TextBox5.Text = ""
        TextBox3.Text = ""
        TextBox4.Text = ""
    End Sub
    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TextBox5.TextChanged
        Try
            Dim query As String = "SELECT Type FROM Pet WHERE Pet_Id=@PetID"
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@PetID", TextBox5.Text)
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            Dim result As Object = cmd.ExecuteScalar()
            conn.Close()
            If result IsNot Nothing Then
                TextBox3.Text = result.ToString()
            Else
                TextBox3.Text = ""
            End If
        Catch ex As Exception
            MessageBox.Show("Error fetching Pet Type: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub TextBox2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox2.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub
    Private Sub textbox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress
        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub
End Class
