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
        public PDF(int id, string filename)
        {
            InitializeComponent();

            try
            {
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
                    FileStream fileStream = new FileStream("D:\\" + filename + ".pdf", FileMode.Create);
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
    }
}
