using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Patcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /* ---- Step 1 Read config file ---- */
            var configParser = new ConfigParser(System.AppDomain.CurrentDomain.BaseDirectory + @"\config.yaml");
            var config = configParser.readConfig();

            /*---- Step 2 Get patch data ---- */
            var patchData = config.patchList.First(patch => string.Equals(config.patchType, patch.patchType));

            var patchParser = new PatchParser(patchData.patchListURL);
            if (patchParser.loadPatchData())
            {

            }
        }
    }
}
