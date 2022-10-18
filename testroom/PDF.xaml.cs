using System;
using System.IO;
using System.IO.Packaging;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace testroom
{
    /// <summary>
    /// Interaction logic for PDF.xaml
    /// </summary>
    public partial class PDF : Window
    {
        public PDF(int id, dynamic pdfinfo)
        {
            InitializeComponent();

            try
            {
                //DocumentNumber.Content = pdfinfo.DocumentNumber;
                //CreatedDate.Content = pdfinfo.CreatedDate;
                //FromDate.Content = pdfinfo.FromDate;
                //ToDate.Content = pdfinfo.ToDate;
                //CustomerName.Content = pdfinfo.CustomerName;
                //CustomerAddress.Content = pdfinfo.CustomerAddress;
                //CustomerContact.Content = pdfinfo.CustomerContact;

                //GenerateItems(pdfinfo);

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
                    FileStream fileStream = new FileStream("C:\\Users\\kosak\\OneDrive\\Documents\\" + pdfinfo.DocumentName + ".pdf", FileMode.Create);
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
                    FileStream fileStream = new FileStream("Documents\\" + pdfinfo.DocumentName + ".pdf", FileMode.Create);
                    outStream.CopyTo(fileStream);

                    // Clean up
                    outStream.Flush();
                    outStream.Close();
                    fileStream.Flush();
                    fileStream.Close();
                }

                else if (id == 3)
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
                    FileStream fileStream = new FileStream("D:\\preview.pdf", FileMode.Create);
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
                PublicCommands.ShowError(ex.Message);
            }

            this.Close();
        }

        private void GenerateItems(dynamic pdfinfo)
        {
            decimal totalprice = 0.00m;

            RowDefinition newrow = new RowDefinition();
            newrow.Height = new GridLength(30);
            ItemsGrid.RowDefinitions.Add(newrow);

            int row = 1;

            foreach(var info in pdfinfo.Items)
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
                button.Content = (decimal.Parse(info.Price.Replace('.', ',')) * decimal.Parse(info.Quantity.ToString())) + "€";
                button.Style = (Style)this.Resources["Item"];
                Grid.SetColumn(button, 2);
                Grid.SetRow(button, row);
                ItemsGrid.Children.Add(button);

                totalprice = totalprice + ((decimal.Parse(info.Price.ToString().Replace('.', ',')) * decimal.Parse(info.Quantity.ToString())));

                row++;

                newrow = new RowDefinition();
                newrow.Height = new GridLength(30);
                ItemsGrid.RowDefinitions.Add(newrow);
            }

            TotalPrice.Content = totalprice.ToString().Replace(',', '.') + "€";
        }
    }
}
