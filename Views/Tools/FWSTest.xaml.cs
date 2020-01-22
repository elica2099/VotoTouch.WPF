using System;
using System.Windows;
using System.Windows.Threading;

namespace VotoTouch.WPF.Views.Tools
{
    /// <summary>
    /// Interaction logic for FWSTest.xaml
    /// </summary>
    public partial class FWSTest : Window
    {
        public FWSTest(string ABadge)
        {
            InitializeComponent();

            lbTest.Text = ABadge;
            int timsec = 5;

            // chiudi finestra
            DispatcherTimer timCloseForm = new DispatcherTimer { IsEnabled = false, Interval = TimeSpan.FromMilliseconds(5000) };
            timCloseForm.Tick += delegate (object asender, EventArgs ae)
            {
                this.Close();
            };
            timCloseForm.Start();

            // chiudi finestra label
            DispatcherTimer timCloseFormLabel = new DispatcherTimer { IsEnabled = false, Interval = TimeSpan.FromMilliseconds(1000) };
            timCloseFormLabel.Tick += delegate (object asender, EventArgs ae)
            {
                timsec--;
                label1.Text = "La finestra si chiuderà tra " + timsec.ToString() + " secondi";
            };
            timCloseFormLabel.Start();
        }


    }
}
