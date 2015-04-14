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
        public C_Lib(string _name, string _version, string _purpose, string _license, string _about, string _site)
        {
            Name = _name;
            Version = _version;
            Purpose = _purpose;
            License = _license;
            About = _about;
            Site = _site;
        }
        
        public C_Lib(string[] splited)
        {
            for(int q=0; q<splited.Length; q++)
            {
                SET_string(q, splited[q]);
            }
        }

        private void SET_string(int index, string str)
        {
            switch(index)
            {
                case(0): Name = str; break;
                case(1): Version = str; break;
                case(2): Purpose = str; break;
                case(3): License = str; break;
                case(4): About = str; break;
                case(5): Site = str; break;
                default: break;
            }
        }

        public string Name { get; set; }
        public string Version { get; set; }
        public string Purpose { get; set; }
        public string License { get; set; }
        public string About { get; set; }
        public string Site { get; set; }

    }
    /// <summary>
    /// About - gui
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<C_Lib> aboutLibraries;
        public object dgAboutLibraries_lock;

        private void INIT_about()
        {
            INIT_dgAboutLib();
            /*
            C_Camera.camList = new ObservableCollection<C_VideoDevice>();
            
            for (int i = 0; i < C_Camera.numCamSources; i++)
            {
                //fill web cam array
                C_Camera.camList.Add(
                    new C_VideoDevice(i, _SystemCamereas[i].Name, _SystemCamereas[i].ClassID)
                    );

            }

            // binding
            CollectionViewSource ItemCollectionViewSource_cam;
            ItemCollectionViewSource_cam = (CollectionViewSource)(FindResource("ItemCollectionViewSource_cam"));
            ItemCollectionViewSource_cam.Source = C_Camera.camList;

            // when binding is changing inner guts of dataGrid from different thread
            dgCam_lock = new object(); // lock for datagrid
            BindingOperations.EnableCollectionSynchronization(C_Camera.camList, dgCam_lock); // for multi-thread updating
            */


        }

        private void INIT_dgAboutLib()
        {
            aboutLibraries = new ObservableCollection<C_Lib>();

            string libs = Properties.Resources.ResourceManager.GetString("aboutLibraries");

            string[] lines = libs.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            char sep = '|';

            foreach (string line in lines)
            {

                string[] splited = line.Split(sep);
                aboutLibraries.Add(new C_Lib(splited));
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
