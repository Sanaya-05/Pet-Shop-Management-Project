Imports System.Data.SqlClient
Imports System.IO
Imports System.Security.Cryptography
Imports Pet_Shop_Management_Project.Form5
Public Class Form10
    Dim conn As New SqlConnection("Data Source=DESKTOP-E0FMQ4O\SQLEXPRESS;Initial Catalog=Pet Shop Management;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
    Dim adapter As SqlDataAdapter
    Dim dt As DataTable
    Private Sub Form10_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadPetData("")
        AttachClickEvents()
        Button4.Visible = False
        For Each container As Control In Me.Controls
            If TypeOf container Is FlowLayoutPanel Then
                For Each petPanel As Control In container.Controls
                    If TypeOf petPanel Is Panel Then
                        Debug.WriteLine("Found Panel: " & petPanel.Name)
                        AddHandler petPanel.Click, AddressOf PetCard_Click
                    End If
                Next
            End If
        Next
    End Sub
    Public Sub LoadPetData(searchQuery As String)
        Try
            conn.Open()
            Dim query As String = "SELECT * FROM Pet WHERE 1=1"
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
        Dim pic As New PictureBox With {.Name = "pic", .Size = New Size(165, 207), .SizeMode = PictureBoxSizeMode.StretchImage, .Location = New Point(10, 10), .BackColor = Color.LightGray}
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
    Private SelectedPets As New List(Of String)
    Private Sub PetCard_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim clickedControl As Control = CType(sender, Control)
        While Not (TypeOf clickedControl Is Panel) AndAlso clickedControl.Parent IsNot Nothing
            clickedControl = clickedControl.Parent
        End While
        Dim petCard As Panel = CType(clickedControl, Panel)
        Dim petID = ""
        Dim petType = ""
        Dim breed = ""
        Dim age = ""
        Dim gender = ""
        Dim price As Decimal = 0
        For Each ctrl As Control In petCard.Controls
            If TypeOf ctrl Is Label Then
                Dim lbl = CType(ctrl, Label)
                If lbl.Name = "lblPetID" Then petID = lbl.Text.Replace("ID: ", "")
                If lbl.Name = "lblType" Then petType = lbl.Text.Replace("Type: ", "")
                If lbl.Name = "lblBreed" Then breed = lbl.Text.Replace("Breed: ", "")
                If lbl.Name = "lblAge" Then age = lbl.Text.Replace("Age: ", "")
                If lbl.Name = "lblGender" Then gender = lbl.Text.Replace("Gender: ", "")
                If lbl.Name = "lblPrice" Then Decimal.TryParse(lbl.Text.Replace("Price: ", ""), price)
            End If
        Next
        Dim existingPet = SelectedPetList.FirstOrDefault(Function(p) p.PetID = petID)
        If petCard.BackColor = Color.LightBlue Then
            petCard.BackColor = Color.White
            If existingPet IsNot Nothing Then SelectedPetList.Remove(existingPet)
        Else
            petCard.BackColor = Color.LightBlue
            If existingPet Is Nothing Then
                SelectedPetList.Add(New PetInfo With {
                .PetID = petID, .Type = petType, .Breed = breed,
                                    .Age = age, .Gender = gender, .Price = price
                                })
            End If
            Dim anySelected As Boolean = False
            For Each container As Control In Me.Controls
                If TypeOf container Is FlowLayoutPanel Then
                    For Each petPanel As Control In container.Controls
                        If TypeOf petPanel Is Panel AndAlso petPanel.BackColor = Color.LightBlue Then
                            anySelected = True
                            Exit For
                        End If
                    Next
                End If
            Next
            Button4.Visible = anySelected
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        LoadPetData(TextBox2.Text)
        AttachClickEvents()
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form1.ComboBox1.Text = ""
        Form1.TextBox1.Text = ""
        Form1.TextBox2.Text = ""
        Me.Hide()
        Form1.Show()
    End Sub
    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = Form1.TextBox1.Text
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        TextBox2.Text = ""
        Button4.Visible = False
        LoadPetData("")
        AttachClickEvents()
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form5.TextBox1.Text = ""
        Form5.TextBox2.Text = "+91"
        Form5.TextBox3.Text = ""
        Form5.ComboBox1.Text = ""
        Form5.Label10.Visible = False
        Form5.Label11.Visible = False
        Form5.TextBox3.Visible = False
        Form5.Button2.Enabled = False
        Form5.Button3.Visible = False
        Me.Hide()
        Form5.Show()
    End Sub
    Public Sub AttachClickEvents()
        For Each petPanel As Control In FlowLayoutPanel1.Controls
            If TypeOf petPanel Is Panel Then
                RemoveHandler petPanel.Click, AddressOf PetCard_Click
                AddHandler petPanel.Click, AddressOf PetCard_Click
                For Each child As Control In petPanel.Controls
                    RemoveHandler child.Click, AddressOf PetCard_Click
                    AddHandler child.Click, AddressOf PetCard_Click
                Next
            End If
        Next
    End Sub
End Class
