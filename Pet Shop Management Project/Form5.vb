Imports System.Data.SqlClient
Imports System.Drawing.Printing
Imports System.Net.Security
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel
Public Class Form5
    Dim conn As New SqlConnection("Data Source=DESKTOP-E0FMQ4O\SQLEXPRESS;Initial Catalog=Pet Shop Management;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
    Dim printDoc As New PrintDocument()
    Dim receiptText As String = ""
    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = ""
        TextBox2.Text = "+91"
        TextBox3.Text = ""
        TextBox4.Text = ""
        TextBox5.Text = ""
        TextBox6.Text = ""
        ComboBox1.Text = ""
        Label10.Visible = False
        Label11.Visible = False
        TextBox3.Visible = False
        Button2.Enabled = False
        Button3.Visible = False
        Label14.Visible = False
        Label15.Visible = False
        TextBox4.Visible = False
        TextBox5.Visible = False
        Label16.Visible = False
        TextBox6.Visible = False
        With DataGridView1
            .Columns.Clear()
            .Rows.Clear()
            .Columns.Add("PetID", "Pet ID")
            .Columns.Add("Type", "Type")
            .Columns.Add("Breed", "Breed")
            .Columns.Add("Age", "Age")
            .Columns.Add("Gender", "Gender")
            .Columns.Add("Price", "Price")
        End With
        Dim totalPrice As Decimal = 0
        For Each pet As PetInfo In SelectedPetList
            DataGridView1.Rows.Add(pet.PetID, pet.Type, pet.Breed, pet.Age, pet.Gender, pet.Price)
            totalPrice += pet.Price
        Next
        Label8.Text = "Rs " & totalPrice.ToString("F2")
        Label12.Text = "Date&Time" & DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        AddHandler printDoc.PrintPage, AddressOf Me.PrintReceiptPage
    End Sub
    Private Function GenerateCustomerID() As String
        Dim cnewID As String = "C101"
        Try
            Dim query As String = "SELECT MAX(Customer_Id) FROM Customer"
            Dim cmd As New SqlCommand(query, conn)
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            Dim result As Object = cmd.ExecuteScalar()
            If result IsNot DBNull.Value AndAlso result IsNot Nothing Then
                Dim lastID As String = result.ToString()
                Dim num As Integer = Integer.Parse(lastID.Substring(1)) + 1
                cnewID = "C" & num.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show("Error generating Customer ID: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return cnewID
    End Function
    Private Function GenerateSalesID() As String
        Dim slnewID As String = "SL101"
        Try
            Dim query As String = "SELECT MAX(Sale_Id) FROM [Sales&Payment]"
            Dim cmd As New SqlCommand(query, conn)
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            Dim result As Object = cmd.ExecuteScalar()
            If result IsNot DBNull.Value AndAlso result IsNot Nothing Then
                Dim lastID As String = result.ToString()
                Dim num As Integer = Integer.Parse(lastID.Substring(2)) + 1
                slnewID = "SL" & num.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show("Error generating Sale ID: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return slnewID
    End Function
    Private Sub PrintReceiptPage(sender As Object, e As PrintPageEventArgs)
        Dim font As New Font("Arial", 24)
        e.Graphics.DrawString(receiptText, font, Brushes.Black, 200, 200)
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Hide()
        Form10.Show()
    End Sub
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedItem = "Cash" Then
            Label10.Visible = False
            Label11.Visible = False
            TextBox3.Visible = False
            Label14.Visible = False
            Label15.Visible = False
            Label16.Visible = False
            TextBox4.Visible = False
            TextBox5.Visible = False
            TextBox6.Visible = False
            Button2.Enabled = True
        ElseIf ComboBox1.SelectedItem = "UPI" Then
            Label10.Visible = True
            Label11.Visible = False
            TextBox3.Visible = True
            Label14.Visible = False
            Label15.Visible = False
            Label16.Visible = False
            TextBox4.Visible = False
            TextBox5.Visible = False
            TextBox6.Visible = False
            Button2.Enabled = True
        ElseIf ComboBox1.SelectedItem = "Card" Then
            Label10.Visible = False
            Label11.Visible = True
            TextBox3.Visible = True
            Label14.Visible = True
            Label15.Visible = True
            Label16.Visible = True
            TextBox4.Visible = True
            TextBox5.Visible = True
            TextBox6.Visible = True
            Button2.Enabled = True
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If TextBox1.Text = "" Or TextBox2.Text = "+91" Then
            MessageBox.Show("Please fill the Coustomer details!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If
        If TextBox2.Text.Length <> 13 OrElse Not IsNumeric(TextBox2.Text) Then
            MessageBox.Show("Invalid Phone number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        If ComboBox1.SelectedItem = "UPI" Then
            If TextBox3.Text = "" Then
                MessageBox.Show("Please enter your UPI id")
                Exit Sub
            End If
        ElseIf ComboBox1.SelectedItem = "Card" Then
            If TextBox3.Text = "" Then
                MessageBox.Show("Please enter your Card Number")
                Exit Sub
            End If
            If Not System.Text.RegularExpressions.Regex.IsMatch(TextBox3.Text, "^\d{16}$") Then
                MessageBox.Show("Invalid Card number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
        End If
        Try
            conn.Open()
            If conn.State <> ConnectionState.Open Then
                MessageBox.Show("Connection is not open.")
                Exit Sub
            End If
            Dim custID As String = GenerateCustomerID()
            Using cmdCustomer As New SqlCommand("INSERT INTO Customer (Customer_Id, Name, Phone_Number) VALUES (@cid, @name, @phone)", conn)
                cmdCustomer.Parameters.AddWithValue("@cid", custID)
                cmdCustomer.Parameters.AddWithValue("@name", TextBox1.Text)
                cmdCustomer.Parameters.AddWithValue("@phone", TextBox2.Text)
                cmdCustomer.ExecuteNonQuery()
            End Using
            For Each pet As PetInfo In SelectedPetList
                Dim saleID As String = GenerateSalesID()
                Using cmdSale As New SqlCommand("INSERT INTO [Sales&Payment] (Sale_Id, Pet_Id, Customer_Id, Customer_Name, Total_Amount, Payment_Method, Date) VALUES (@saleid, @pid, @cid, @cname, @total, @method, @date)", conn)
                    cmdSale.Parameters.AddWithValue("@saleid", saleID)
                    cmdSale.Parameters.AddWithValue("@pid", pet.PetID)
                    cmdSale.Parameters.AddWithValue("@cid", custID)
                    cmdSale.Parameters.AddWithValue("@cname", TextBox1.Text)
                    cmdSale.Parameters.AddWithValue("@total", Label8.Text.Replace("Rs ", ""))
                    cmdSale.Parameters.AddWithValue("@method", ComboBox1.Text & " - [" & TextBox3.Text & "]")
                    cmdSale.Parameters.AddWithValue("@date", DateTime.Now)
                    cmdSale.ExecuteNonQuery()
                End Using
            Next
            MessageBox.Show("Payment successful!")
            Button3.Visible = True
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("          PetForYou")
            sb.AppendLine("       -------------------")
            sb.AppendLine("     OFFICIAL RECEIPT")
            sb.AppendLine("       -------------------")
            sb.AppendLine($"Customer ID    : " & custID)
            sb.AppendLine($"Customer Name  : " & TextBox1.Text)
            sb.AppendLine($"Phone          : " & TextBox2.Text)
            sb.AppendLine($"Date           : {DateTime.Now:dd-MM-yyyy HH:mm:ss}")
            sb.AppendLine("----------------------------------")
            sb.AppendLine("Items Purchased:")
            For Each pet As PetInfo In SelectedPetList
                sb.AppendLine($"- {pet.Type} ({pet.Breed})  Rs {pet.Price:F2}")
            Next
            sb.AppendLine("----------------------------------")
            sb.AppendLine($"TOTAL:         Rs {Label8.Text.Replace("Rs ", "")}")
            sb.AppendLine($"Payment Method: " & ComboBox1.Text)
            sb.AppendLine()
            sb.AppendLine("Thank you for shopping with us!")
            sb.AppendLine("      Visit Again!!")
            receiptText = sb.ToString()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Not String.IsNullOrEmpty(receiptText) Then
            Dim preview As New PrintPreviewDialog()
            preview.Document = printDoc
            preview.ShowDialog()
        Else
            MessageBox.Show("No receipt to print. Please complete a payment first.", "Print Receipt", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        TextBox1.Clear()
        TextBox2.Text = "+91"
        TextBox3.Clear()
        ComboBox1.SelectedIndex = -1
        Label10.Visible = False
        Label11.Visible = False
        TextBox3.Visible = False
        Button2.Enabled = False
        Button3.Visible = False
        Form10.TextBox2.Text = ""
        Form10.Button4.Visible = False
        Form10.LoadPetData("")
        Form10.AttachClickEvents()
        SelectedPetList.Clear()
        Me.Hide()
        Form10.Show()
        If conn.State = ConnectionState.Open Then conn.Close()
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Me.Close()
        Form10.Show()
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