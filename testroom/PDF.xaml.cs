using System;
using System.IO;
using System.IO.Packaging;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace testroom
{
    /// <summary>
    /// Interaction logic for PDF.xaml
    /// </summary>
    public partial class PDF : Window
    {
        public PDF(int id, string filename, string documentnumber, string createddate, string fromdate, string todate, string customername, string customeraddress, string customercontact, dynamic config)
        {
            InitializeComponent();

            try
            {
                DocumentNumber.Content = documentnumber;
                CreatedDate.Content = createddate;
                FromDate.Content = fromdate;
                ToDate.Content = todate;
                CustomerName.Content = customername;
                CustomerAddress.Content = customeraddress;
                CustomerContact.Content = customercontact;

                GenerateItems(config);

                if (id == 1)
                { 
                    MemoryStream lMemoryStream = new MemoryStream();
                    Package package = Package.Open(lMemoryStream, FileMode.Create);
                    XpsDocument doc = new XpsDocument(package);
                    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);

                    // This is your window
                    writer.Write(print);

                    doc.Close();
                    package.Close();

                    // Convert 
                    MemoryStream outStream = new MemoryStream();
                    PdfSharp.Xps.XpsConverter.Convert(lMemoryStream, outStream, false);

                    // Write pdf file
                    FileStream fileStream = new FileStream("D:\\" + filename + ".pdf", FileMode.Create);
                    outStream.CopyTo(fileStream);

                    // Clean up
                    outStream.Flush();
                    outStream.Close();
                    fileStream.Flush();
                    fileStream.Close();
                }
                else if (id == 2)
                {
                    PrintDialog printDialog = new PrintDialog();
                    if (printDialog.ShowDialog() == true)
                    {
                        printDialog.PrintVisual(print, "invoice");
                    }

                    MemoryStream lMemoryStream = new MemoryStream();
                    Package package = Package.Open(lMemoryStream, FileMode.Create);
                    XpsDocument doc = new XpsDocument(package);
                    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);

                    // This is your window
                    writer.Write(print);

                    doc.Close();
                    package.Close();

                    // Convert 
                    MemoryStream outStream = new MemoryStream();
                    PdfSharp.Xps.XpsConverter.Convert(lMemoryStream, outStream, false);
                    
                    // Write pdf file
                    FileStream fileStream = new FileStream("\\Documents\\" + filename + ".pdf", FileMode.Create);
                    outStream.CopyTo(fileStream);

                    // Clean up
                    outStream.Flush();
                    outStream.Close();
                    fileStream.Flush();
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
                MainWindow.ShowError(ex.Message);
            }

            this.Close();
        }

        private void GenerateItems(dynamic config)
        {
            RowDefinition newrow = new RowDefinition();
            newrow.Height = new GridLength(30);
            ItemsGrid.RowDefinitions.Add(newrow);

            int row = 1;

            foreach(var info in config.Items)
            {
                Label button = new Label();
                button.Content = info.Quantity;
                button.Style = (Style)this.Resources["Item"];
                Grid.SetColumn(button, 0);
                Grid.SetRow(button, row);
                ItemsGrid.Children.Add(button);

                button = new Label();
                button.Content = info.Item;
                button.Style = (Style)this.Resources["Item"];
                Grid.SetColumn(button, 1);
                Grid.SetRow(button, row);
                ItemsGrid.Children.Add(button);

                button = new Label();
                button.Content = info.Price;
                button.Style = (Style)this.Resources["Item"];
                Grid.SetColumn(button, 2);
                Grid.SetRow(button, row);
                ItemsGrid.Children.Add(button);

                row++;

                newrow = new RowDefinition();
                newrow.Height = new GridLength(30);
                ItemsGrid.RowDefinitions.Add(newrow);
            }
        }
    }
}
