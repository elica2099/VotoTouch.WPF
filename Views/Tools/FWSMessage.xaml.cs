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
    /// Interaction logic for FWSMessage.xaml
    /// </summary>
    public partial class FWSMessage : Window
    {
        public FWSMessage(string AMessage)
        {
            InitializeComponent();

            lblMessage.Text = AMessage;
            btnChiudi.Focus();
        }

        private void ButtonChiudi_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
