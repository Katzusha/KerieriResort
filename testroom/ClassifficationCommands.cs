﻿using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace testroom
{
    class ClassifficationCommands
    {
        //Retrive all classiffications from clients database
        public static dynamic GetAll()
        {
            WebRequest request = WebRequest.Create(MainWindow.APIconnection + "/ClassifficationsAPI/GetAll.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "DatabaseName=" + MainWindow.DatabaseName;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            //Set the ContentLength property of the WebRequest.
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
            dynamic AllClassiffications = JsonConvert.DeserializeObject(html);

            return AllClassiffications;
        }

        //Retrive searched classiffications from clients database
        public static dynamic GetSearched(string searched)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create(MainWindow.APIconnection + "/ClassifficationsAPI/GetSearched.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "searched=" + searched + "&DatabaseName=" + MainWindow.DatabaseName;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            //request.ContentType = "application/x-www-form-urlencoded";
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
            dynamic SearchedReservations = JsonConvert.DeserializeObject(html);

            return SearchedReservations;
        }

        //Post classiffication information to clients database
        public static bool CreateClassiffication(string Name, string SerialNumber, string Price, string Size, string MaxReservants)
        {
            try
            {

                WebRequest request = WebRequest.Create("http://www.kerieri.eu/API/ClassifficationsAPI/PostClassiffication.php");
                // Set the Method property of the request to POST.
                request.Method = "POST";
                // Create POST data and convert it to a byte array.

                string eur = Price.Substring(0, (Price.Length - 2));
                string cent = Price.Substring(Price.Length - 2);

                string postData = "DatabaseName=" + MainWindow.DatabaseName + "&Name=" + Name + "&SerialNumber=SN-" + SerialNumber + "&PriceEuros=" + eur + "&PriceCents=" + cent + "&Size=" + Size + "&MaxReservants=" + MaxReservants;
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
                dynamic PostResponse = JsonConvert.DeserializeObject(html);

                if (PostResponse.response.success == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (WebException ex)
            {
                // Handle the exception
                if (ex.Response is HttpWebResponse response)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        // Handle the 404 error (Not Found) specifically
                        Console.WriteLine("API endpoint not found.");
                    }
                    else
                    {
                        // Handle other HTTP errors
                        Console.WriteLine($"HTTP error: {response.StatusCode}");
                    }
                }
                else
                {
                    // Handle other types of WebExceptions
                    Console.WriteLine($"WebException: {ex.Message}");
                }
            }

            return false;
        }

        public static double GetClassifficationPrice(int ID)
        {
            WebRequest request = WebRequest.Create(MainWindow.APIconnection + "/ClassifficationsAPI/GetClassifficationPrice.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "DatabaseName=" + MainWindow.DatabaseName + "&ClassifficationId=" + ID.ToString();
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
            dynamic PostResponse = JsonConvert.DeserializeObject(html);
            double price = 0;

            foreach (var information in PostResponse)
            {
                price = double.Parse(information.Price.ToString());
            }


            return price;
        }

        public static bool DeleteClassiffication(string id, string user)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create(MainWindow.APIconnection + "/ClassifficationsAPI/Delete.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "ClassifficationId=" + id + "&User=" + user + "&DatabaseName=" + MainWindow.DatabaseName;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            { }
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
            }
            dynamic DeleteInformation = JsonConvert.DeserializeObject(html);

            if (DeleteInformation.response.success == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
