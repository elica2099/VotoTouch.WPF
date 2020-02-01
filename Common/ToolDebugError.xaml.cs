using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;

namespace VotoTouch.WPF
{
    /// <summary>
    /// Interaction logic for NewAzionista.xaml
    /// </summary>
    public partial class ToolDebugError : Window
    {

        public ToolDebugError()
        {
            InitializeComponent();

        }

        public void addError(string errorstring)
        {
            //lbxErrors.Items.Insert(0, DateTime.Now + "\t" + errorstring);
            lbxErrors.Items.Add(DateTime.Now + "\t" + errorstring);
            lbxErrors.Items.MoveCurrentToLast();
            lbxErrors.ScrollIntoView(lbxErrors.Items.CurrentItem);
        }

    }
}
