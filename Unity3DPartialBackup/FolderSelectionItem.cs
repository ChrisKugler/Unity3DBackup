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
        private bool? selected;            
        private string path;
        private List<FolderSelectionItem> children;

        public FolderSelectionItem(string name, string path)
        {
            this.name = name;
            this.path = path;
            this.selected = false;
            this.children = new List<FolderSelectionItem>();    
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler h = this.PropertyChanged;
            if (h != null)
                h(this, new PropertyChangedEventArgs(name));
        }

        private void SelectAllChildren(bool val)
        {
            foreach(FolderSelectionItem item in this.children)
            {
                item.Selected = val;
                item.SelectAllChildren(val);
            }
        }

        public void AddChild(FolderSelectionItem item)
        {
            this.children.Add(item);
            item.PropertyChanged += Child_PropertyChanged;
        }

        private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Selected":                                       
                    if (this.Children.All(x => (x.Selected ?? false)))
                    {
                        this.selected = true;
                        OnPropertyChanged("Selected");
                    }
                    else if (this.Children.Any(x => (x.Selected ?? false) || !x.Selected.HasValue))
                    {
                        this.selected = null;
                        OnPropertyChanged("Selected");
                    }
                    else
                    {
                        this.selected = false;
                        OnPropertyChanged("Selected");
                    }                                        
                    break;
            }
        }

        public string Name
        {
            get { return this.name; }
            set { if (this.name != value) { this.name = value; OnPropertyChanged("Name"); } }
        }        

        public bool? Selected
        {
            get { return this.selected; }
            set
            {
                if (this.selected != value)
                {
                    this.selected = value;
                    OnPropertyChanged("Selected");
                    if(this.selected.HasValue)
                        this.SelectAllChildren(this.selected.Value);
                }
            }
        }        

        public string Path
        {
            get { return this.path; }
            set { if (this.path != value) { this.path = value; OnPropertyChanged("Path"); } }
        }
        
        public IEnumerable<FolderSelectionItem> Children
        {
            get { return this.children; }
            //set { if (this.children != value) { this.children = value; OnPropertyChanged("Children"); } }
        }      
    }
}
