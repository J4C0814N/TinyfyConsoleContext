using Microsoft.Win32;
using System;
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
 
        static void Main(string[] args)
        {
            // Probably not a good idea.. but makes installation easy for n00bs
            if (args.Length == 0)
            {
                if (!install())
                    PauseQuit();
                else
                    Environment.Exit(0);
            }

            // Make sure one argument was supplied
            if (args.Length < 1)
            {
                System.Console.WriteLine("Please enter an image location argument. or 'install' only.");
                PauseQuit();
            }else{

                // Install the context menu and quit.
                if (args[0].ToLower() == "install")
                {
                    if(!install())
                        PauseQuit();
                    else
                        Environment.Exit(0);
                }
                // Set up vars
                string key = ConfigurationManager.AppSettings["APIKey"];
                string url = ConfigurationManager.AppSettings["APIURL"];
                string total = ConfigurationManager.AppSettings["APITotal"];
                string input = args[0];
                string ext = Path.GetExtension(input);
                // Might be a better way to combine the new filename, but this works at least.
                string output = Path.GetDirectoryName(input)+@"\"+Path.GetFileNameWithoutExtension(input)+"-Tinyfied"+ext;

                Console.WriteLine("Tinifying file: " + input+" To: "+output);

                // create the WebClient and connect to the TinyPNG API
                WebClient client = new WebClient();
                string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + key));
                client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + auth);

                // Perform Image compression and poorly inform the user of the progress
                try
                {

                    Console.WriteLine("Uploading "+Path.GetFileName(input)+". Please wait.");
                    client.UploadData(url, File.ReadAllBytes(input));
                    Console.WriteLine("File Uploaded.");
                    Console.WriteLine("Downloading compressed image. Please Wait.");
                    client.DownloadFile(client.ResponseHeaders["Location"], output);
                    Console.WriteLine("API Calls used: "+client.ResponseHeaders["Compression-Count"]+"/"+total);
                    Console.WriteLine("Image Downloaded to:"+ output);

                }
                // Something went wrong!
                catch (WebException)
                {
                    Console.WriteLine("Compression failed.");
                    PauseQuit();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong:");
                    Console.WriteLine(e.Message);
                    PauseQuit();
                }

                // Done.
                Environment.Exit(0);
                //PauseQuit();

            }
        }

        /// <summary>
        /// Re-useable function to display a message and wait for user input before quitting
        /// </summary>
        private static void PauseQuit()
        {
            Console.WriteLine("Press any key to quit.");
            Console.ReadLine();
            Environment.Exit(0);
        }

        /// <summary>
        /// Add the necessary registry keys to create a context menu.
        /// * Must to be run as admin / elevated cmd
        /// </summary>
        /// <returns>Ture|False whether the reg key was added</returns>
        private static bool install()
        {
            List<string> FileTypes = new List<string> { "jpegfile", "pngfile" };

            string me = System.Reflection.Assembly.GetEntryAssembly().Location + " %1";
            string MenuName = "\\shell\\NewMenuOption";
            string Command = "\\shell\\NewMenuOption\\command";
            bool installed = false;

            RegistryKey regmenu = null;
            RegistryKey regcmd = null;

            foreach (var i in FileTypes)
            {
                MenuName = i + MenuName;
                Command = i + Command;

                Console.WriteLine("installing "+i+"...");

                try
                {
                    regmenu = Registry.ClassesRoot.CreateSubKey(MenuName);
                    if (regmenu != null)
                        regmenu.SetValue("", "Tinify");
                    regcmd = Registry.ClassesRoot.CreateSubKey(Command);
                    if (regcmd != null)
                        regcmd.SetValue("", me);

                    installed = true;
                    Console.WriteLine("Install Successful!");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Not authorised to install.");
                    Console.WriteLine("Install requires Administrator permissions.");
                    Console.WriteLine("Run install as Administrator.");
                    installed = false;

                }
                catch (Exception e)
                {
                    Console.WriteLine("Install Failed:");
                    installed = false;
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    if (regmenu != null)
                        regmenu.Close();
                    if (regcmd != null)
                        regcmd.Close();

                }

            }


            return installed;

        }
    }
}
