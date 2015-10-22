using System;
using System.Collections.Generic;
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
using Com2Com.Common;
using Com2Com.View;

namespace Com2Com
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //NavigationHelper.NavigationFrame = mainFrame;
            NavigationHelper.RegisterPage<MasterDevicePage>();
            NavigationHelper.RegisterPage<SettingsPage>();

            NavigationHelper.NavigateTo<MasterDevicePage>();
        }
    }
}
