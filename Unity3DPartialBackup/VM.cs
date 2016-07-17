using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Unity3DPartialBackup
{
    public class VM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string rootDirectory;
        private string assetsDirectory;
        private string projectSettingsDirectory;
        private bool inProgress;

        public VM()
        {
            // arg 0 is the executable, arg 1 is the director selected from the shell. 
            string[] cmdArgs = Environment.GetCommandLineArgs();
            if (cmdArgs.Length > 1 && cmdArgs[1] != null)
                this.RootDirectory = cmdArgs[1];

            if (string.IsNullOrEmpty(this.RootDirectory))
                MessageBox.Show("Root Directoy was not valid... check the shell extension.");
                        
            this.Folders = new List<FolderSelectionItem>();
            this.InProgress = false;             
            this.Messages = new ObservableCollection<string>();
            this.GetFolders();
        }

        private void GetFolders()
        {
            this.assetsDirectory = Path.Combine(this.rootDirectory, "Assets");
            this.projectSettingsDirectory = Path.Combine(this.rootDirectory, "ProjectSettings");

            if (!Directory.Exists(this.projectSettingsDirectory))
            {
                MessageBox.Show("project settings directory could not be found. Is this a Unity3D Project?");
                return;
            }

            if (!Directory.Exists(this.assetsDirectory))
            {
                MessageBox.Show("assets directory could not be found. Is this a Unity3D Project?");
                return;
            }

            this.Folders.Add(new FolderSelectionItem("ProjectSettings", this.projectSettingsDirectory) { Selected = true });
            string[] assetDirectories = Directory.GetDirectories(this.assetsDirectory);
            if (assetDirectories != null)
            {
                foreach (string assetDir in assetDirectories)
                {
                    FolderSelectionItem rootItem = new FolderSelectionItem(Path.GetFileName(assetDir), assetDir);
                    this.BuildDirectoryTree(rootItem);
                    this.Folders.Add(rootItem);
                }
            }

            OnPropertyChanged("Folders");
        }
        
        private void BuildDirectoryTree(FolderSelectionItem parent)
        {
            string[] children = Directory.GetDirectories(parent.Path);
            if (children != null)
            {
                foreach (string directory in children)
                {
                    FolderSelectionItem item = new FolderSelectionItem(Path.GetFileName(directory), directory);                    
                    parent.AddChild(item);
                    this.BuildDirectoryTree(item);
                }
            }

        }

        private void AddMessage(string msg)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Messages.Add(msg);
            });
        }

        public async Task MakeBackup()
        {
            await Task.Factory.StartNew(() =>
            {
                this.InProgress = true;                 
                this.AddMessage("Creating temporary file location.");
                string tmpDir = Path.GetTempPath();
                DirectoryInfo rootDirInfo = new DirectoryInfo(this.rootDirectory);
                tmpDir = Path.Combine(tmpDir, string.Format("{0}Partial_{1}", rootDirInfo.Name, DateTime.Now.ToString("MM_dd_yyyy_HH_mm")));
                if (Directory.Exists(tmpDir))
                    Directory.Delete(tmpDir, true);

                Directory.CreateDirectory(tmpDir);
                string zipFile = tmpDir + ".zip";
                if (File.Exists(zipFile))
                    File.Delete(zipFile);

                try
                {
                    foreach (FolderSelectionItem folder in this.Folders)
                    {                        
                        this.Copy(folder, tmpDir);
                    }

                    // zip the backup.          
                    this.AddMessage("Creating zip file.");
                    ZipFile.CreateFromDirectory(tmpDir, zipFile);
                    if (File.Exists(zipFile))
                    {
                        this.AddMessage("Zip file created.");
                        FileInfo zipInfo = new FileInfo(zipFile);
                        File.Move(zipFile, Path.Combine(rootDirInfo.Parent.FullName, zipInfo.Name));
                        this.AddMessage(string.Format("Zip file {0} moved.", zipInfo.Name));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
                }
                finally
                {
                    if (Directory.Exists(tmpDir))
                        Directory.Delete(tmpDir, true);
                    if (File.Exists(zipFile))
                        File.Delete(zipFile);
                }
                this.InProgress = false;
            });               
        }

        private void Copy(FolderSelectionItem folder, string sourceDir)
        {            
            string targetDir = Path.Combine(sourceDir, folder.Name);            
            if (folder.Selected ?? false)
            {
                this.AddMessage(string.Format("Copying {0}.", folder.Path));
                DirectoryHelper.Copy(folder.Path, targetDir, false);
            }
                
            foreach (FolderSelectionItem child in folder.Children)
            {
                this.Copy(child, targetDir);
            }            
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler h = this.PropertyChanged;
            if (h != null)
                h(this, new PropertyChangedEventArgs(name));
        }

        public string RootDirectory
        {
            get { return this.rootDirectory; }
            set { if (this.rootDirectory != value) { this.rootDirectory = value; OnPropertyChanged("RootDirectory"); } }
        }

        public List<FolderSelectionItem> Folders { get; set; }
        public ObservableCollection<string> Messages { get; set; }        

        public bool InProgress
        {
            get { return this.inProgress; }
            set { if (this.inProgress != value) { this.inProgress = value; OnPropertyChanged("InProgress"); } }
        }
    }
}
