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
    /// Interaction logic for UTextBlock.xaml
    /// </summary>
    public partial class UTextBlock : UserControl
    {
        public UTextBlock()
        {
            InitializeComponent();
        }

        // DEPENDENCY PROPERTY TESTO ------------------------------------------------------------------

        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register("Text", typeof(string), typeof(UTextBlock), new 
                PropertyMetadata("", new PropertyChangedCallback(OnTextChanged))); 
				
        public string Text { 
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        } 
		
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UTextBlock UserControl1Control) UserControl1Control.OnTextChanged(e);
        } 
		
        private void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                TextBlock.Text = e.NewValue.ToString();
        }  

        // DEPENDENCY PROPERTY TEXTALIGNMENT ------------------------------------------------------------------

        private TextAlignment _TextAlignment;
        public TextAlignment TextAlignment
        {
            get => _TextAlignment;
            set
            {
                _TextAlignment = value;
                TextBlock.TextAlignment = _TextAlignment;
            }
        }


        /*
        public static readonly DependencyProperty TextAlignmentProperty = 
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(UTextBlock), new 
                PropertyMetadata("", new PropertyChangedCallback(OnTextAlignmentChanged))); 
				
        public string SetText { 
            get => (string)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        } 
		
        private static void OnTextAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UTextBlock UserControl1Control) UserControl1Control.OnTextAlignmentChanged(e);
        } 
		
        private void OnTextAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                TextBlock.TextAlignment = (TextAlignment)e.NewValue;
        }  
        */
    }
}
