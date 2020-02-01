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

namespace VotoTouch.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for ULabelCandidati.xaml
    /// </summary>
    public partial class ULabelCandidati : UserControl
    {
        public ULabelCandidati()
        {
            InitializeComponent();
        }

        // DEPENDENCY PROPERY TESTO ------------------------------------------------------------------

        public static readonly DependencyProperty SetTextProperty = 
            DependencyProperty.Register("SetText", typeof(string), typeof(ULabelCandidati), new 
                PropertyMetadata("", new PropertyChangedCallback(OnSetTextChanged))); 
				
        public string SetText { 
            get => (string)GetValue(SetTextProperty);
            set => SetValue(SetTextProperty, value);
        } 
		
        private static void OnSetTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ULabelCandidati UserControl1Control) UserControl1Control.OnSetTextChanged(e);
            //UserControl1 UserControl1Control = d as UserControl1; 
            //UserControl1Control.OnSetTextChanged(e);
        } 
		
        private void OnSetTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                SetCandidatiText(e.NewValue.ToString());
        }  

        // CALCOLO DELLE LABEL ------------------------------------------------------------------

        private void SetCandidatiText(string AText)
        {
            String _TText = AText;
            char separator = ';';

            // ok ora devo verificare se ci sono dei separatori nel testo, possono essere ; o |
            // questi indicano se ci sono candidati e quindi colonne
            if (_TText.IndexOf(";", System.StringComparison.Ordinal) > 0)
                separator = ';';
            if (_TText.IndexOf("|", System.StringComparison.Ordinal) > 0)
                separator = '|';

            // ok ora trasformo la stringa in lista di strings
            List<string> ris = _TText.Split(separator).ToList();

            string txlab = "";
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
            // ok ora so quanti ce ne sono
            if (ris.Count <= 6)
            {
                // 1 colonna sola
                label1.Visibility = Visibility.Visible;
                label2.Visibility = Visibility.Collapsed;
                label3.Visibility = Visibility.Collapsed;
                column1.Width = new GridLength(100.0, GridUnitType.Star);
                column2.Width = new GridLength(0, GridUnitType.Pixel);
                column3.Width = new GridLength(0, GridUnitType.Pixel);
                // ok ora il testo
                txlab = ris.Aggregate("", (current, ss) => current + (ss + "\n"));
                label1.Text = txlab;
                return;
            }
            if (ris.Count <= 12)
            {
                // 2 colonne
                label1.Visibility = Visibility.Visible;
                label2.Visibility = Visibility.Visible;
                label3.Visibility = Visibility.Collapsed;
                column1.Width = new GridLength(50.0, GridUnitType.Star);
                column2.Width = new GridLength(50.0, GridUnitType.Star);
                column3.Width = new GridLength(0, GridUnitType.Pixel);
                // devo dividere in due il count
                float cc = ris.Count/2;
                if (cc == Math.Truncate(cc))
                {
                    // non ha decimali
                }
                else
                {
                    cc++;
                }
                int po = 1;
                txlab = "";
                foreach (string ss in ris)
                {
                    txlab += ss + "\n";
                    if (po == (int)cc)
                    {
                        label1.Text = txlab;
                        txlab = "";
                    }
                    po++;
                }
                label2.Text = txlab;
                return;
            }
            if (ris.Count <= 18)
            {
                label1.Visibility = Visibility.Visible;
                label2.Visibility = Visibility.Visible;
                label3.Visibility = Visibility.Visible;
                column1.Width = new GridLength(33.0, GridUnitType.Star);
                column2.Width = new GridLength(33.0, GridUnitType.Star);
                column3.Width = new GridLength(33.0, GridUnitType.Star);
                // devo dividere in tre il count
                // devo dividere in due il count
                float cc = ris.Count / 3;
                if (cc == Math.Truncate(cc))
                {
                    // non ha decimali
                }
                else
                {
                    cc++;
                }
                int po = 1;
                txlab = "";
                foreach (string ss in ris)
                {
                    txlab += ss + "\n";
                    if (po == (int)cc)
                    {
                        label1.Text = txlab;
                        txlab = "";
                    }
                    if (po == ((int)cc * 2))
                    {
                        label2.Text = txlab;
                        txlab = "";
                    }

                    po++;
                }
                label3.Text = txlab;

            }


        }

    }
}
