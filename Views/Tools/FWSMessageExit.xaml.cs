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
using System.Windows.Shapes;

namespace VotoTouch.WPF.Views.Tools
{
    /// <summary>
    /// Interaction logic for FWSMessageExit.xaml
    /// </summary>
    public partial class FWSMessageExit : Window
    {
        public FWSMessageExit()
        {
            InitializeComponent();
        }

        private void ButtonAnnulla_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ButtonContinua_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}
