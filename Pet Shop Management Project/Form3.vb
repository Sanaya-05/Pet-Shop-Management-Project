Public Class Form3
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form1.ComboBox1.Text = ""
        Form1.TextBox1.Text = ""
        Form1.TextBox2.Text = ""
        Me.Hide()
        Form1.Show()
    End Sub
    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = Form1.TextBox1.Text
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form4.TextBox2.Text = ""
        Form4.LoadPetData("")
        Form4.Show()
        Me.Hide()
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form7.TextBox1.Text = ""
        Form7.LoadSupplierData()
        Me.Hide()
        Form7.Show()
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form11.DateTimePicker1.Value = Date.Today
        Form11.LoadAllSales()
        Form11.TextBox1.Text = ""
        Me.Hide()
        Form11.Show()
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form12.DateTimePicker1.Value = Date.Today
        Form12.DateTimePicker2.Value = Date.Today
        Form12.LoadSalesReport()
        Me.Hide()
        Form12.Show()
    End Sub
End Class