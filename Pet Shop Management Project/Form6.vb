Imports System.Data.SqlClient
Imports System.IO
Public Class Form6
    Dim conn As New SqlConnection("Data Source=DESKTOP-E0FMQ4O\SQLEXPRESS;Initial Catalog=Pet Shop Management;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
    Private Function GeneratePetID() As String
        Dim newID As String = "P101"
        Try
            Dim query As String = "SELECT MAX(Pet_Id) FROM Pet"
            Dim cmd As New SqlCommand(query, conn)
            conn.Open()
            Dim result As Object = cmd.ExecuteScalar()
            If result IsNot DBNull.Value AndAlso result IsNot Nothing Then
                Dim lastID As String = result.ToString()
                Dim num As Integer = Integer.Parse(lastID.Substring(1)) + 1
                newID = "P" & num.ToString()
            End If
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error generating Pet ID: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return newID
    End Function
    Public Sub LoadPetDetails(petRow As DataRow)
        If petRow Is Nothing Then Exit Sub
        TextBox3.Text = petRow("Pet_Id").ToString()
        ComboBox1.SelectedItem = petRow("Type").ToString()
        ComboBox2.SelectedItem = petRow("Breed").ToString()
        TextBox1.Text = petRow("Age").ToString()
        If petRow("Gender").ToString() = "M" Then
            RadioButton1.Checked = True
        Else
            RadioButton2.Checked = True
        End If
        TextBox2.Text = petRow("Price").ToString()
        ComboBox3.SelectedItem = petRow("Status").ToString()
        Dim imgData As Byte() = TryCast(petRow("Picture"), Byte())
        If imgData IsNot Nothing AndAlso imgData.Length > 0 Then
            Using ms As New MemoryStream(imgData)
                PictureBox2.Image = Image.FromStream(ms)
            End Using
        Else
            PictureBox2.Image = Nothing
        End If
        Label10.Visible = False
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            Dim petID As String = GeneratePetID()
            Dim query As String = "INSERT INTO Pet (Pet_Id, Type, Breed, Age, Gender, Price, Status, Picture) VALUES (@PetID, @PetType, @Breed, @Age, @Gender, @Price, @Status, @Photo)"
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@PetID", petID)
            cmd.Parameters.AddWithValue("@PetType", ComboBox1.Text)
            cmd.Parameters.AddWithValue("@Breed", ComboBox2.Text)
            cmd.Parameters.AddWithValue("@Age", TextBox1.Text)
            cmd.Parameters.AddWithValue("@Gender", If(RadioButton1.Checked, "M", "F"))
            cmd.Parameters.AddWithValue("@Price", TextBox2.Text)
            cmd.Parameters.AddWithValue("@Status", ComboBox3.Text)
            Dim ms As New MemoryStream()
            PictureBox2.Image.Save(ms, PictureBox2.Image.RawFormat)
            Dim imgByte As Byte() = ms.ToArray()
            cmd.Parameters.AddWithValue("@Photo", imgByte)
            conn.Open()
            cmd.ExecuteNonQuery()
            conn.Close()
            MessageBox.Show("Pet added successfully", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Form4.TextBox2.Text = ""
            Form4.LoadPetData("")
            Me.Hide()
            Form4.Show()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            Dim query As String = "UPDATE Pet SET Type=@PetType, Breed=@Breed, Age=@Age, Gender=@Gender, Price=@Price, Status=@Status, Picture=@Photo WHERE Pet_Id=@PetID"
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@PetType", ComboBox1.Text)
            cmd.Parameters.AddWithValue("@Breed", ComboBox2.Text)
            cmd.Parameters.AddWithValue("@Age", TextBox1.Text)
            cmd.Parameters.AddWithValue("@Gender", If(RadioButton1.Checked, "M", "F"))
            cmd.Parameters.AddWithValue("@Price", TextBox2.Text)
            cmd.Parameters.AddWithValue("@Status", ComboBox3.Text)
            cmd.Parameters.AddWithValue("@PetID", TextBox3.Text)
            Try
                If PictureBox2.Image Is Nothing Then
                    MessageBox.Show("No image selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If
                Dim tempBitmap As New Bitmap(PictureBox2.Image)
                Dim ms As New MemoryStream()
                tempBitmap.Save(ms, Imaging.ImageFormat.Png)
                Dim imgByte As Byte() = ms.ToArray()
                cmd.Parameters.AddWithValue("@Photo", imgByte)
            Catch ex As Exception
                MessageBox.Show("Image saving error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End Try
            conn.Open()
            cmd.ExecuteNonQuery()
            conn.Close()
            MessageBox.Show("Pet updated successfully!", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Form4.TextBox2.Text = ""
            Form4.LoadPetData("")
            Me.Hide()
            Form5.Show()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete it?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            If result = DialogResult.Yes Then
                Dim query As String = "DELETE FROM Pet WHERE Pet_Id=@PetID"
                Dim cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@PetID", TextBox3.Text)
                conn.Open()
                cmd.ExecuteNonQuery()
                conn.Close()
                MessageBox.Show("Pet deleted successfully!", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Form4.TextBox2.Text = ""
                Form4.LoadPetData("")
                Me.Hide()
                Form4.Show()
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Label10.Visible = False
        Dim ofd As New OpenFileDialog()
        ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
        If ofd.ShowDialog() = DialogResult.OK Then
            PictureBox2.Image = Image.FromFile(ofd.FileName)
        End If
    End Sub
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedItem = "Dog" Then
            ComboBox2.Items.Clear()
            ComboBox2.Items.Add("Labrador Retriever")
            ComboBox2.Items.Add("Golden Retriever")
            ComboBox2.Items.Add("German Shepherd")
            ComboBox2.Items.Add("Beagle")
            ComboBox2.Items.Add("Pug")
        End If
        If ComboBox1.SelectedItem = "Cat" Then
            ComboBox2.Items.Clear()
            ComboBox2.Items.Add("Ragdoll")
            ComboBox2.Items.Add("Maine Coon")
            ComboBox2.Items.Add("British Shorthair")
            ComboBox2.Items.Add("Scottish Fold")
            ComboBox2.Items.Add("Sphynx")
        End If
        If ComboBox1.SelectedItem = "Bird" Then
            ComboBox2.Items.Clear()
            ComboBox2.Items.Add("Parakeets")
            ComboBox2.Items.Add("Cockatiels")
            ComboBox2.Items.Add("Canaries")
            ComboBox2.Items.Add("Finches")
            ComboBox2.Items.Add("Lovebirds")
        End If
        If ComboBox1.SelectedItem = "Fish" Then
            ComboBox2.Items.Clear()
            ComboBox2.Items.Add("Betta fish")
            ComboBox2.Items.Add("Guppies")
            ComboBox2.Items.Add("Neon Tetras")
            ComboBox2.Items.Add("Mollies")
            ComboBox2.Items.Add("Platies")
        End If
        If ComboBox1.SelectedItem = "Rabbit" Then
            ComboBox2.Items.Clear()
            ComboBox2.Items.Add("Holland Lop")
            ComboBox2.Items.Add("Mini Rex")
            ComboBox2.Items.Add("Lionhead")
            ComboBox2.Items.Add("Netherland Dwarf")
            ComboBox2.Items.Add("Mini Lop")
        End If
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ComboBox1.Text = ""
        ComboBox2.Text = ""
        TextBox1.Text = ""
        RadioButton1.Checked = False
        RadioButton2.Checked = False
        TextBox2.Text = ""
        ComboBox3.Text = ""
        PictureBox2.Image = Nothing
        TextBox3.Text = ""
        Label10.Visible = True
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form4.TextBox2.Text = ""
        Form4.LoadPetData("")
        Me.Hide()
        Form4.Show()
    End Sub
End Class