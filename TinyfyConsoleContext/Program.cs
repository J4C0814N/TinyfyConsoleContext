﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TinyfyConsoleContext
{
    class Program
    {
        // See https://stackoverflow.com/questions/6179562/adding-my-program-to-right-click-menu
        // Basic idea is to create a simple console app that takes an image argument and uses the 
        // TinyPNG API https://tinypng.com/developers to compress and replace (or append name) the passed image
        // With the addition of a windows context menu so a user can simply right click > compress a file
 
        static void Main(string[] args)
        {
            string key = ConfigurationManager.AppSettings["APIKey"];
            string input = "large-input.png";
            string output = "tiny-output.png";

            string url = ConfigurationManager.AppSettings["APIURL"]; ;

            WebClient client = new WebClient();
            string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + key));
            client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + auth);

            try
            {
                client.UploadData(url, File.ReadAllBytes(input));
                /* Compression was successful, retrieve output from Location header. */
                client.DownloadFile(client.ResponseHeaders["Location"], output);
            }
            catch (WebException)
            {
                /* Something went wrong! You can parse the JSON body for details. */
                Console.WriteLine("Compression failed.");
            }
        }
    }
}
