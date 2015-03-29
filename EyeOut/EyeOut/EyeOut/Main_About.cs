using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows;


using System.Windows.Media.Imaging; // BitmapSource 
using System.Runtime.InteropServices;
using System.Windows.Threading; // dispatcherTimer

using System.Windows.Controls; // SelectionChangedEventArgs


using System.Collections.ObjectModel; // ObservableCollection
using System.Windows.Data; //CollectionViewSource


namespace EyeOut
{
    public class C_Lib
    {
        public string name;
        public string version;
        public string purpose;
        public C_Lib(string _name, string _version, string _purpose)
        {
            name = _name;
            version = _version;
            purpose = _purpose;
        }
    }
    /// <summary>
    /// About - gui
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ObservableCollection<C_Lib> aboutLibraries;
        public object dgAboutLibraries_lock;

        private void INIT_about()
        {
            INIT_dgAboutLib();
        }

        private void INIT_dgAboutLib()
        {
            aboutLibraries = new ObservableCollection<C_Lib>();

            string libs = "System|4.2|because\nSystem|4.2|because"; // read from file
            //libs = ResourceDictionary.
            //libss = Properties.Resources.ResourceManager.GetStream("aboutLibraries.txt").ToString();
            //libs= Properties.Resources.ResourceManager.GetString("aboutLibraries.txt");
            
            string[] lines = libs.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            char sep = '|';

            foreach (string line in lines)
            {

                string[] splited = line.Split(sep);
                aboutLibraries.Add(new C_Lib(splited[0], splited[1], splited[2]));
            }
            INIT_dgAboutLib_binding();
        }

        private void INIT_dgAboutLib_binding()
        {
            // binding
            CollectionViewSource ItemCollectionViewSource_aboutLibraries;
            ItemCollectionViewSource_aboutLibraries = (CollectionViewSource)(FindResource("ItemCollectionViewSource_aboutLibraries"));
            ItemCollectionViewSource_aboutLibraries.Source = aboutLibraries;

            // when binding is changing inner guts of dataGrid from different thread
            dgAboutLibraries_lock = new object(); // lock for datagrid
            BindingOperations.EnableCollectionSynchronization(aboutLibraries, dgAboutLibraries_lock); // for multi-thread updating
        }
    }
}
