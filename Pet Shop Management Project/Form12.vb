Imports System.Data.SqlClient
Imports System.Drawing.Printing
Public Class Form12
    Dim conn As New SqlConnection("Data Source=DESKTOP-E0FMQ4O\SQLEXPRESS;Initial Catalog=Pet Shop Management;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
    Dim printDocument As New PrintDocument()
    Dim printTable As DataTable
    Dim printFont As New Font("Arial", 10)
    Dim currentRow As Integer = 0
    Private Sub Form12_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSalesReport()
        LoadPaymentMethods()
    End Sub
    Public Sub LoadSalesReport()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim query As String = "SELECT Sale_Id, Pet_Id, Customer_Name, Total_Amount, Date FROM [Sales&Payment]"
            Dim adapter As New SqlDataAdapter(query, conn)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            DataGridView1.DataSource = dt
            CalculateTotal(dt)
        Catch ex As Exception
            MessageBox.Show("Error loading report: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            conn.Close()
        End Try
    End Sub
    Private Sub LoadPaymentMethods()
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim fromDate As Date = DateTimePicker1.Value.Date
        Dim toDate As Date = DateTimePicker2.Value.Date
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim query As String = "SELECT Sale_Id, Pet_Id, Customer_Name, Total_Amount, Date FROM [Sales&Payment] WHERE CAST(Date AS DATE) BETWEEN @from AND @to"
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@from", fromDate)
            cmd.Parameters.AddWithValue("@to", toDate)
            Dim adapter As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            DataGridView1.DataSource = dt
            CalculateTotal(dt)
        Catch ex As Exception
            MessageBox.Show("Filter error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            conn.Close()
        End Try
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        DateTimePicker1.Value = Date.Today
        DateTimePicker2.Value = Date.Today
        LoadSalesReport()
    End Sub
    Private Sub CalculateTotal(dt As DataTable)
        Dim total As Decimal = 0
        For Each row As DataRow In dt.Rows
            total += Convert.ToDecimal(row("Total_Amount"))
        Next
        Label6.Text = "Total Sales: Rs " & total.ToString("F2")
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        printTable = CType(DataGridView1.DataSource, DataTable)
        If printTable Is Nothing OrElse printTable.Rows.Count = 0 Then
            MessageBox.Show("No data to print.")
            Return
        End If
        currentRow = 0
        RemoveHandler printDocument.PrintPage, AddressOf PrintPageHandler
        Dim paperSize As New PaperSize("Custom", 1300, 900)
        With printDocument.DefaultPageSettings
            .Landscape = True
            .PaperSize = paperSize
            .Margins = New Margins(30, 30, 30, 30)
        End With
        AddHandler printDocument.PrintPage, AddressOf PrintPageHandler
        PrintPreviewDialog1.Document = printDocument
        PrintPreviewDialog1.ShowDialog()
    End Sub
    Private Sub PrintPageHandler(sender As Object, e As PrintPageEventArgs)
        Dim x As Integer = e.MarginBounds.Left
        Dim y As Integer = e.MarginBounds.Top
        Dim lineHeight As Integer = printFont.Height + 10
        Dim columnCount As Integer = printTable.Columns.Count
        Dim columnWidth As Integer = e.MarginBounds.Width \ columnCount
        e.Graphics.DrawString("PetForYou - Sales Receipt", New Font("Arial", 20, FontStyle.Bold), Brushes.Black, x, y)
        y += lineHeight * 2
        e.Graphics.DrawString("Date: " & DateTime.Now.ToString("dd MMMM yyyy"), printFont, Brushes.Black, x, y)
        y += lineHeight * 2
        Dim columnX As Integer = x
        For Each column As DataColumn In printTable.Columns
            e.Graphics.DrawString(column.ColumnName, New Font("Arial", 10, FontStyle.Bold), Brushes.Black, columnX, y)
            columnX += columnWidth
        Next
        y += lineHeight
        While currentRow < printTable.Rows.Count
            columnX = x
            For Each item As Object In printTable.Rows(currentRow).ItemArray
                Dim text As String = item.ToString()
                Dim maxChars As Integer = CInt(columnWidth / 7)
                If text.Length > maxChars Then
                    text = text.Substring(0, maxChars - 3) & "..."
                End If
                e.Graphics.DrawString(text, printFont, Brushes.Black, columnX, y)
                columnX += columnWidth
            Next
            y += lineHeight
            currentRow += 1
            If y + lineHeight > e.MarginBounds.Bottom Then
                e.HasMorePages = True
                Return
            End If
        End While
        y += lineHeight
        Dim totalAmount As Decimal = 0
        For Each row As DataRow In printTable.Rows
            totalAmount += Convert.ToDecimal(row("Total_Amount"))
        Next
        e.Graphics.DrawString("Total Amount: Rs " & totalAmount.ToString("F2"), New Font("Arial", 12, FontStyle.Bold), Brushes.Black, x, y)
        e.HasMorePages = False
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Hide()
        Form3.Show()
    End Sub
End Class