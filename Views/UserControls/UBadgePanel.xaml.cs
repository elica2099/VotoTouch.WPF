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
    /// Interaction logic for UBadgePanel.xaml
    /// </summary>
    public partial class UBadgePanel : UserControl
    {
        public UBadgePanel()
        {
            InitializeComponent();

            edtBadge.PreviewKeyDown += EdtBadgeOnPreviewKeyDown;
        }

        private void EdtBadgeOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                App.ICMsn.NotifyColleaguesAsync(VSDecl.ICM_MAIN_BADGEREAD, edtBadge.Text, null, null);
                e.Handled = true;
            }
        }

        private void Button999999_OnClick(object sender, RoutedEventArgs e)
        {
            App.ICMsn.NotifyColleaguesAsync(VSDecl.ICM_MAIN_BADGEREAD, "999999", null, null);
        }

        private void Button88889999_OnClick(object sender, RoutedEventArgs e)
        {
            App.ICMsn.NotifyColleaguesAsync(VSDecl.ICM_MAIN_BADGEREAD, "88889999", null, null);
        }

        private void Button88889000_OnClick(object sender, RoutedEventArgs e)
        {
            App.ICMsn.NotifyColleaguesAsync(VSDecl.ICM_MAIN_BADGEREAD, "88889900", null, null);
        }


    }
}
