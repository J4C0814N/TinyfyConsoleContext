using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TinyfyConsoleContext
{
    class Program
    {
        static string AppName = "TinyfyConsoleContext";

        static void Main(string[] args)
        {
            TraceLogger.Info("Starting App.", AppName);
            // Probably not a good idea.. but makes installation easy for n00bs
            if (args.Length == 0)
            {
                if (!install())
                    PauseQuit();
                else
                    Environment.Exit(0);
            }

            // Make sure one argument was supplied
            // Redundant really... We already do something if its 0.
            if (args.Length < 1)
            {
                Console.WriteLine("Please enter an image location argument. or 'install' only.");
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

                TraceLogger.DisplayInfo("Tinifying file: " + input+" To: "+output, AppName);

                // create the WebClient and connect to the TinyPNG API
                WebClient client = new WebClient();
                string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + key));
                client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + auth);

                // Perform Image compression and poorly inform the user of the progress
                try
                {

                    TraceLogger.DisplayInfo("Uploading " + Path.GetFileName(input) + ". Please wait.", AppName);
                    client.UploadData(url, File.ReadAllBytes(input));
                    TraceLogger.DisplayInfo("File Uploaded.", AppName);
                    TraceLogger.DisplayInfo("Downloading compressed image. Please Wait.", AppName);
                    client.DownloadFile(client.ResponseHeaders["Location"], output);
                    TraceLogger.DisplayInfo("API Calls used: " + client.ResponseHeaders["Compression-Count"] + "/" + total, AppName);
                    TraceLogger.DisplayInfo("Image Downloaded to:" + output, AppName);

                }
                // Something went wrong!
                catch (WebException e)
                {
                    TraceLogger.DisplayError("Compression failed.", AppName);
                    TraceLogger.Error(e, AppName);
                    PauseQuit();
                }
                catch (Exception e)
                {
                    TraceLogger.DisplayError("Something went wrong.", AppName);
                    TraceLogger.Error(e, AppName);
                    PauseQuit();
                }

                // Done.
                TraceLogger.Info("Done, quitting.", AppName);
                Environment.Exit(0);
                //PauseQuit();

            }
        }

        /// <summary>
        /// Re-useable function to display a message and wait for user input before quitting
        /// </summary>
        private static void PauseQuit()
        {
            TraceLogger.Info("Pause quit.", AppName);
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

                TraceLogger.DisplayInfo("Installing.", AppName);

                try
                {
                    regmenu = Registry.ClassesRoot.CreateSubKey(MenuName);
                    if (regmenu != null)
                        regmenu.SetValue("", "Tinify");
                    regcmd = Registry.ClassesRoot.CreateSubKey(Command);
                    if (regcmd != null)
                        regcmd.SetValue("", me);

                    installed = true;
                    TraceLogger.DisplayInfo("Install succeeded.", AppName);
                }
                catch (UnauthorizedAccessException)
                {
                    TraceLogger.DisplayError("Install failed: Unauthorised", AppName);
                    // Trace log doesnt need this shit
                    Console.WriteLine("Not authorised to install.");
                    Console.WriteLine("Install requires Administrator permissions.");
                    Console.WriteLine("Run install as Administrator.");
                    installed = false;

                }
                catch (Exception e)
                {
                    TraceLogger.Error(e, AppName);
                    installed = false;
                    Console.WriteLine("Install failed. See log for details.");
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
