using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using testroom;

namespace resorttestroom
{
    internal class AdminCommands
    {
        public static dynamic GetClients()
        {
            WebRequest request = WebRequest.Create(MainWindow.APIconnection + "/AdminAPI/GetClients.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "DatabaseName=" + "kerieri_Clients";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;

            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();

                //MainWindow.ShowError(html);
            }
            dynamic Clients = JsonConvert.DeserializeObject(html);

            return Clients;
        }

        public static dynamic GetClientsInfo(string id)
        {
            WebRequest request = WebRequest.Create(MainWindow.APIconnection + "/AdminAPI/GetClientInfo.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "ClientId=" + id + "&DatabaseName=" + "kerieri_Clients";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;

            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();

                //MainWindow.ShowError(html);
            }
            dynamic ClientsInfo = JsonConvert.DeserializeObject(html);

            return ClientsInfo;
        }

        public static dynamic CreateClient(string name, string email, string phonenumber, string stationarynumber, string country, string postcode, string address, string descritpion, string databasename)
        {
            WebRequest request = WebRequest.Create(MainWindow.APIconnection + "/AdminAPI/CreateClient.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "Name=" + name + "&Email=" + email + "&PhoneNumber=" + phonenumber + "&StationaryNumber=" + stationarynumber + "&Country=" + country
                + "&PostCode=" + postcode + "&Address=" + address + "&Description=" + descritpion + "&DatabaseName=" + databasename;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;

            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();

                //MainWindow.ShowError(html);
            }
            dynamic ClientsInfo = JsonConvert.DeserializeObject(html);

            if (ClientsInfo.response.success == 1)
            {
                return true;
            }
            else return false;
        }

        public static dynamic CreateCompany(string name, string email, string phonenumber, string stationarynumber, string country, string postcode, string address, string descritpion, string databasename)
        {
            WebRequest request = WebRequest.Create(MainWindow.APIconnection + "/AdminAPI/CreateCompany.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "Name=" + name + "&Email=" + email + "&PhoneNumber=" + phonenumber + "&StationaryNumber=" + stationarynumber + "&Country=" + country
                + "&PostCode=" + postcode + "&Address=" + address + "&Description=" + descritpion + "&DatabaseName=" + databasename;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;

            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();

                //MainWindow.ShowError(html);
            }
            dynamic ClientsInfo = JsonConvert.DeserializeObject(html);

            if (ClientsInfo.response.success == 1)
            {
                return true;
            }
            else return false;
        }
    }
}
