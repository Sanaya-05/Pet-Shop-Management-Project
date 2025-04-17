Imports System.Data.SqlClient
Imports System.IO
Public Class Form4
    Dim conn As New SqlConnection("Data Source=DESKTOP-E0FMQ4O\SQLEXPRESS;Initial Catalog=Pet Shop Management;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
    Dim adapter As SqlDataAdapter
    Dim dt As DataTable
    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadPetData("")
    End Sub
    Public Sub LoadPetData(searchQuery As String)
        Try
            conn.Open()
            Dim query As String = "SELECT Pet_Id, Type, Breed, Age, Gender, Price, Status, Picture FROM Pet WHERE 1=1"
            If searchQuery <> "" Then
                query &= " AND (Type LIKE @search OR Breed LIKE @search)"
            End If
            Dim cmd As New SqlCommand(query, conn)
            If searchQuery <> "" Then
                cmd.Parameters.AddWithValue("@search", "%" & searchQuery & "%")
            End If
            adapter = New SqlDataAdapter(cmd)
            dt = New DataTable()
            adapter.Fill(dt)
            PopulatePetCards(dt)
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub PopulatePetCards(dataTable As DataTable)
        FlowLayoutPanel1.Controls.Clear()
        For Each row As DataRow In dataTable.Rows
            Dim petCard As Panel = CreatePetCard(row)
            FlowLayoutPanel1.Controls.Add(petCard)
        Next
    End Sub
    Private Function CreatePetCard(row As DataRow) As Panel
        Dim petCard As New Panel With {.Size = New Size(180, 360), .BackColor = Color.White, .BorderStyle = BorderStyle.FixedSingle}
        Dim pic As New PictureBox With {.Name = "pic", .Size = New Size(165, 207), .SizeMode = PictureBoxSizeMode.StretchImage, .Location = New Point(10, 10), .BackColor = Color.LightGray, .Cursor = Cursors.Hand}
        If row.Table.Columns.Contains("Pet_Id") Then
            pic.Tag = row("Pet_Id").ToString()
        Else
            MessageBox.Show("Error: Column 'Pet_Id' does not exist in DataTable!", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End If
        AddHandler pic.Click, AddressOf Pic_Click
        Dim lblPetID As New Label With {.Name = "lblPetID", .Text = "ID: " & row("Pet_Id").ToString(), .AutoSize = True, .Location = New Point(10, 220), .ForeColor = Color.Red}
        Dim lblType As New Label With {.Name = "lblType", .Text = "Type: " & row("Type").ToString(), .AutoSize = True, .Location = New Point(10, 240)}
        Dim lblBreed As New Label With {.Name = "lblBreed", .Text = "Breed: " & row("Breed").ToString(), .AutoSize = True, .Location = New Point(10, 260)}
        Dim lblAge As New Label With {.Name = "lblAge", .Text = "Age: " & row("Age").ToString(), .AutoSize = True, .Location = New Point(10, 280)}
        Dim lblGender As New Label With {.Name = "lblGender", .Text = "Gender: " & row("Gender").ToString(), .AutoSize = True, .Location = New Point(10, 300)}
        Dim lblPrice As New Label With {.Name = "lblPrice", .Text = "Price: " & row("Price").ToString(), .AutoSize = True, .Location = New Point(10, 320)}
        Dim lblStatus As New Label With {.Name = "lblStatus", .Text = "Status: " & row("Status").ToString(), .AutoSize = True, .Location = New Point(10, 340)}
        If Not IsDBNull(row("Picture")) AndAlso TypeOf row("Picture") Is Byte() Then
            Dim imgBytes As Byte() = DirectCast(row("Picture"), Byte())
            Using ms As New MemoryStream(imgBytes)
                pic.Image = Image.FromStream(ms)
            End Using
        Else
            pic.Image = Nothing
        End If
        petCard.Controls.Add(pic)
        petCard.Controls.Add(lblPetID)
        petCard.Controls.Add(lblType)
        petCard.Controls.Add(lblBreed)
        petCard.Controls.Add(lblAge)
        petCard.Controls.Add(lblGender)
        petCard.Controls.Add(lblPrice)
        petCard.Controls.Add(lblStatus)
        Return petCard
    End Function
    Private Sub Pic_Click(sender As Object, e As EventArgs)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            MessageBox.Show("DataTable is empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        Dim pic As PictureBox = DirectCast(sender, PictureBox)
        Dim petID As String = pic.Tag.ToString()
        Dim selectedPet As DataRow = dt.Select("Pet_Id = '" & petID & "'").FirstOrDefault()
        If selectedPet IsNot Nothing Then
            Dim managePetsForm As New Form6()
            managePetsForm.LoadPetDetails(selectedPet) ' Call method in Form6 to load pet details
            managePetsForm.ShowDialog()
            LoadPetData("")
        Else
            MessageBox.Show("Pet not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        LoadPetData(TextBox2.Text)
    End Sub
    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Hide()
        Form3.Show()
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        TextBox2.Text = ""
        LoadPetData("")
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form6.ComboBox1.Text = ""
        Form6.ComboBox2.Text = ""
        Form6.TextBox1.Text = ""
        Form6.RadioButton1.Checked = False
        Form6.RadioButton2.Checked = False
        Form6.TextBox2.Text = ""
        Form6.ComboBox3.Text = ""
        Form6.PictureBox2.Image = Nothing
        Form6.Label10.Visible = True
        Form6.TextBox3.Text = ""
        Me.Hide()
        Form6.Show()
    End Sub
End Class