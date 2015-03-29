using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel; // ObservableCollection
using System.Windows; // Window
using System.Windows.Data; //CollectionViewSource
using System.Windows.Controls; // checkbox

using System.IO.Ports;

using System.Windows.Input; // GUI eventArgs

namespace EyeOut
{
    /// <summary>
    /// Oculus - gui
    /// </summary>
    /// 

    class C_Oculus
    {
    }

    public partial class MainWindow : Window
    {

        private void btnStartOculus_Click(object sender, RoutedEventArgs e)
        {

            using (var program = new RiftGame())
            {
                program.Run();
            }
        }
    }
}
