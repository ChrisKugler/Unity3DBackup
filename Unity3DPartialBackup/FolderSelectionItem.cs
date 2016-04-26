using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity3DPartialBackup
{
    public class FolderSelectionItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;        
        private bool selected;            
        private string path;

        public FolderSelectionItem(string name, string path)
        {
            this.name = name;
            this.path = path;            
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler h = this.PropertyChanged;
            if (h != null)
                h(this, new PropertyChangedEventArgs(name));
        }

        public string Name
        {
            get { return this.name; }
            set { if (this.name != value) { this.name = value; OnPropertyChanged("Name"); } }
        }        

        public bool Selected
        {
            get { return this.selected; }
            set { if (this.selected != value) { this.selected = value; OnPropertyChanged("Selected"); } }
        }        

        public string Path
        {
            get { return this.path; }
            set { if (this.path != value) { this.path = value; OnPropertyChanged("Path"); } }
        }

    }
}
