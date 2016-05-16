using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Unity3DPartialBackup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new VM();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            VM vm = this.DataContext as VM;
            if (vm != null)
            {
                await vm.MakeBackup();

                //MessageBoxResult res = MessageBox.Show("Backup Complete: Open Directory?", "Job's Done!", MessageBoxButton.YesNo);
                //if (res == MessageBoxResult.Yes)
                Process.Start(new DirectoryInfo(vm.RootDirectory).Parent.FullName);

                Application.Current.Shutdown();
            }
        }
    }
}
