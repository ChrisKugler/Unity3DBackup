using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Unity3DBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please ensure that Unity is closed and all files in your project are not in use.");
            string rootDir = null;
            if (args != null && args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
                rootDir = args[0];
            else
            {
                Console.WriteLine("Enter project root directory: ");
                rootDir = Console.ReadLine();
            }

            if (!string.IsNullOrWhiteSpace(rootDir) && Directory.Exists(rootDir))
            {
                // create temp directory for the project. 
                string tmpDir = Path.GetTempPath();
                DirectoryInfo rootDirInfo = new DirectoryInfo(rootDir);
                tmpDir = Path.Combine(tmpDir, string.Format("{0}_{1}", rootDirInfo.Name, DateTime.Now.ToString("MM_dd_yyyy_HH_mm"))); 
                if (Directory.Exists(tmpDir))
                    Directory.Delete(tmpDir, true);
                Directory.CreateDirectory(tmpDir);
                string zip = tmpDir + ".zip";
                if (File.Exists(zip))
                    File.Delete(zip);
                Console.WriteLine("Creating temporary location for project.");
                try
                {                                       
                    // copy project to temp dir
                    Console.WriteLine("Copying to temp location.");
                    DirectoryHelper.Copy(rootDir, tmpDir, true);

                    Console.WriteLine("Deleting generated directories...");
                    // remove generated directories.
                    string targetPath = Path.Combine(tmpDir, "Library"); 
                    if(Directory.Exists(targetPath))
                        Directory.Delete(targetPath, true);
                    targetPath = Path.Combine(tmpDir, "obj");
                    if(Directory.Exists(targetPath))
                        Directory.Delete(targetPath, true);

                    // zip                    
                    Console.WriteLine("Creating zip file...");
                    ZipFile.CreateFromDirectory(tmpDir, zip);
                    // copy zip file to the parent of the project root.
                    if (File.Exists(zip))
                    {
                        Console.WriteLine("Moving zip to project directory.");
                        FileInfo zipInfo = new FileInfo(zip); 
                        File.Move(zip, Path.Combine(rootDirInfo.Parent.FullName, zipInfo.Name));
                       // Process.Start(rootDirInfo.Parent.FullName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    Console.WriteLine("Cleaning up temp directory.");
                    if (Directory.Exists(tmpDir))
                        Directory.Delete(tmpDir, true);
                    if (File.Exists(zip))
                        File.Delete(zip);                   
                }
            }            
            else
                Console.WriteLine("Invalid or non-existent directory provided.");

            //return 0;
        }
    }
}
