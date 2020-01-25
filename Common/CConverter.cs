using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace VotoTouch.WPF.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolToOppositeBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(Visibility), typeof(Visibility))]
    public class VisibilityToOppositeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a Visibility");
            //this converter skip Visibility.Collapsed
            return ((Visibility)value) == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt = (DateTime)value;
            return (dt == DateTime.MinValue) ? string.Empty : dt.ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() != typeof(DateTime))
                    throw new InvalidOperationException("The object type be a Datetime");
                DateTime dateOut;
                return (!DateTime.TryParse(System.Convert.ToDateTime(value).ToString(culture), out dateOut))
                           ? dateOut
                           : DateTime.MinValue; //DependencyProperty.UnsetValue
            }
            return DateTime.MinValue;
        }
    }

    [ValueConversion(typeof(Enum), typeof(bool))]
    public class EnumConverter : IValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (targetType.IsAssignableFrom(typeof(Boolean)) && targetType.IsAssignableFrom(typeof(Enum)))
        //        throw new ArgumentException("EnumConverter can only convert to boolean or string.");
        //    if (targetType == typeof(String))
        //        return value.ToString();

        //    return String.Compare(value.ToString(), (String)parameter, StringComparison.InvariantCultureIgnoreCase) == 0;
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (targetType.IsAssignableFrom(typeof(Boolean)) && targetType.IsAssignableFrom(typeof(Enum)))
        //        throw new ArgumentException("EnumConverter can only convert back value from a Enum or a boolean.");
        //    if (!targetType.IsEnum)
        //        throw new ArgumentException("EnumConverter can only convert value to an Enum Type.");

        //    if (value.GetType() == typeof(Enum))
        //    {
        //        return Enum.Parse(targetType, (String)value, true);
        //    }

        //    //We have a boolean, as for binding to a checkbox. we use parameter
        //    if ((Boolean)value)
        //        return Enum.Parse(targetType, (String)parameter, true);

        //    return null;
        //}
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            return checkValue.Equals(targetValue,
                     StringComparison.InvariantCultureIgnoreCase);
            
            //if (parameter.Equals(value))
            //    return true;
            //else
            //    return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            bool useValue = (bool)value;
            string targetValue = parameter.ToString();
            if (useValue)
                return Enum.Parse(targetType, targetValue);

            return null;
            //return parameter;
        }
    }

    [ValueConversion(typeof(IList), typeof(Visibility))]
    public class VisibleWhenNonEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && ((IList)value).Count != 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RadioBoolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int integer = (int)value;
            if (integer == int.Parse(parameter.ToString()))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }

    [ValueConversion(typeof(System.Enum), typeof(System.Array))]
    public class EnumToValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            var ret = (from item in Enum.GetNames(value.GetType())
                      where item != "None"
                      select item).ToArray();
            return ret;
            //or
            //return Enum.GetNames(value.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(System.Enum), typeof(System.Boolean))]
    public class EnumBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            object parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, parameterString);
        }
        #endregion
    }

    public class BooleanToHiddenVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility rv = Visibility.Visible;
            try
            {
                var x = bool.Parse(value.ToString());
                if (x)
                {
                    rv = Visibility.Visible;
                }
                else
                {
                    rv = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
            }
            return rv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

    }

    public class BooleanToHiddenMaintainVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility rv = Visibility.Visible;
            try
            {
                var x = bool.Parse(value.ToString());
                if (x)
                {
                    rv = Visibility.Visible;
                }
                else
                {
                    rv = Visibility.Hidden;
                }
            }
            catch (Exception)
            {
            }
            return rv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

    }

    public class NotBooleanToHiddenVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility rv = Visibility.Visible;
            try
            {
                var x = bool.Parse(value.ToString());
                if (x)
                {
                    rv = Visibility.Collapsed;
                }
                else
                {
                    rv = Visibility.Visible;
                }
            }
            catch (Exception)
            {
            }
            return rv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool)) throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public class BoolInverterConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                return !((bool)value);
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
            //throw new NotImplementedException();
        }
        #endregion
    }

    public sealed class StringFormatConverter : IValueConverter
    {
        private static readonly StringFormatConverter instance = new StringFormatConverter();
        public static StringFormatConverter Instance
        {
            get
            {
                return instance;
            }
        }

        private StringFormatConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format(culture, (string)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class StyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FrameworkElement targetElement = values[0] as FrameworkElement;
            string styleName = values[1] as string;

            if (styleName == null)
                return null;

            Style newStyle = (Style)targetElement.TryFindResource(styleName);

            if (newStyle == null)
                newStyle = (Style)targetElement.TryFindResource("MyDefaultStyleName");

            return newStyle;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ColorToBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return Brushes.Black; // Default color

            Color color = (Color)value;

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public static class AttachedCommands
    {

        #region DataGridDoubleClickCommand

        public static readonly DependencyProperty DataGridDoubleClickProperty =
            DependencyProperty.RegisterAttached("DataGridDoubleClickCommand", typeof (ICommand),
                                                typeof (AttachedCommands),
                                                new PropertyMetadata(
                                                    new PropertyChangedCallback(AttachOrRemoveDataGridDoubleClickEvent)));

        public static ICommand GetDataGridDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(DataGridDoubleClickProperty);
        }

        public static void SetDataGridDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DataGridDoubleClickProperty, value);
        }

        public static void AttachOrRemoveDataGridDoubleClickEvent(DependencyObject obj,
                                                                  DependencyPropertyChangedEventArgs args)
        {
            ListBox dataGrid = obj as ListBox;
            if (dataGrid != null)
            {
                ICommand cmd = (ICommand) args.NewValue;

                if (args.OldValue == null && args.NewValue != null)
                    dataGrid.MouseDoubleClick += ExecuteDataGridDoubleClick;
                else if (args.OldValue != null && args.NewValue == null)
                    dataGrid.MouseDoubleClick -= ExecuteDataGridDoubleClick;
            }
        }

        private static void ExecuteDataGridDoubleClick(object sender, MouseButtonEventArgs args)
        {
            DependencyObject obj = sender as DependencyObject;
            ICommand cmd = (ICommand) obj.GetValue(DataGridDoubleClickProperty);
            if (cmd != null)
            {
                if (cmd.CanExecute(obj))
                    cmd.Execute(obj);
            }
        }

        #endregion

        #region ListViewDoubleClickCommand

        public static readonly DependencyProperty ListViewDoubleClickProperty =
            DependencyProperty.RegisterAttached("ListViewDoubleClickCommand", typeof(ICommand),
                                    typeof(AttachedCommands),
                                    new PropertyMetadata(
                                        new PropertyChangedCallback(AttachOrRemoveListViewDoubleClickEvent)));

        public static ICommand GetListViewDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ListViewDoubleClickProperty);
        }

        public static void SetListViewDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ListViewDoubleClickProperty, value);
        }

        public static void AttachOrRemoveListViewDoubleClickEvent(DependencyObject obj,
                                                                  DependencyPropertyChangedEventArgs args)
        {
            ListView dataGrid = obj as ListView;
            if (dataGrid != null)
            {
                ICommand cmd = (ICommand)args.NewValue;

                if (args.OldValue == null && args.NewValue != null)
                    dataGrid.MouseDoubleClick += ExecuteListViewDoubleClick;
                else if (args.OldValue != null && args.NewValue == null)
                    dataGrid.MouseDoubleClick -= ExecuteListViewDoubleClick;
            }
        }

        private static void ExecuteListViewDoubleClick(object sender, MouseButtonEventArgs args)
        {
            DependencyObject obj = sender as DependencyObject;
            ICommand cmd = (ICommand)obj.GetValue(ListViewDoubleClickProperty);
            if (cmd != null)
            {
                if (cmd.CanExecute(obj))
                    cmd.Execute(obj);
            }
        }

        #endregion

        #region ListViewClickCommand

        public static readonly DependencyProperty ListViewClickProperty =
            DependencyProperty.RegisterAttached("ListViewClickCommand", typeof(ICommand),
                                    typeof(AttachedCommands),
                                    new PropertyMetadata(
                                        new PropertyChangedCallback(AttachOrRemoveListViewClickEvent)));

        public static ICommand GetListViewClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ListViewClickProperty);
        }

        public static void SetListViewClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ListViewClickProperty, value);
        }

        public static void AttachOrRemoveListViewClickEvent(DependencyObject obj,
                                                                  DependencyPropertyChangedEventArgs args)
        {
            ListView dataGrid = obj as ListView;
            if (dataGrid != null)
            {
                ICommand cmd = (ICommand)args.NewValue;

                if (args.OldValue == null && args.NewValue != null)
                    dataGrid.PreviewMouseDown += ExecuteListViewClick;
                else if (args.OldValue != null && args.NewValue == null)
                    dataGrid.PreviewMouseDown -= ExecuteListViewClick;
            }
        }

        private static void ExecuteListViewClick(object sender, MouseButtonEventArgs args)
        {
            DependencyObject obj = sender as DependencyObject;
            ICommand cmd = (ICommand)obj.GetValue(ListViewClickProperty);
            if (cmd != null)
            {
                if (cmd.CanExecute(obj))
                    cmd.Execute(obj);
            }
        }

        #endregion

    }

    [ValueConversion(typeof(string), typeof(object))]
    public class StringToXamlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;
            if (input == null) return null;
            var textBlock = new TextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;
            string escapedXml = SecurityElement.Escape(input);

            while (escapedXml != null && escapedXml.IndexOf("|~S~|") != -1)
            {
                //up to |~S~| is normal
                textBlock.Inlines.Add(new Run(escapedXml.Substring(0, escapedXml.IndexOf("|~S~|"))));
                //between |~S~| and |~E~| is highlighted
                textBlock.Inlines.Add(new Run(escapedXml.Substring(escapedXml.IndexOf("|~S~|") + 5,
                    escapedXml.IndexOf("|~E~|") - (escapedXml.IndexOf("|~S~|") + 5))) 
                { FontWeight = FontWeights.Bold, Background = Brushes.Yellow });
                //the rest of the string (after the |~E~|)
                escapedXml = escapedXml.Substring(escapedXml.IndexOf("|~E~|") + 5);
            }

            if (!string.IsNullOrEmpty(escapedXml))
            {
                textBlock.Inlines.Add(new Run(escapedXml));
            }
            return textBlock;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This converter cannot be used in two-way binding.");
        }
    }

    public class StringToXamlDetailPreviewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;
            if (input == null) return null;
            var textBlock = new TextBlock();
            textBlock.FontSize = 11;
            textBlock.Foreground = Brushes.Gray;
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.ToolTip = (string)value;
            string escapedXml = SecurityElement.Escape(input);

            while (escapedXml != null && escapedXml.IndexOf("|~S~|") != -1)
            {
                //up to |~S~| is normal
                textBlock.Inlines.Add(new Run(escapedXml.Substring(0, escapedXml.IndexOf("|~S~|"))));
                //between |~S~| and |~E~| is highlighted
                textBlock.Inlines.Add(new Run(escapedXml.Substring(escapedXml.IndexOf("|~S~|") + 5,
                    escapedXml.IndexOf("|~E~|") - (escapedXml.IndexOf("|~S~|") + 5))) { FontWeight = FontWeights.Bold, Background = Brushes.Yellow });
                //the rest of the string (after the |~E~|)
                escapedXml = escapedXml.Substring(escapedXml.IndexOf("|~E~|") + 5);
            }

            if (!string.IsNullOrEmpty(escapedXml))
            {
                textBlock.Inlines.Add(new Run(escapedXml));
            }
            return textBlock;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This converter cannot be used in two-way binding.");
        }
    }

    public class NegateConverter : IValueConverter
    {

        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( value is bool ) {
                return !(bool)value;
            }
            return value;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( value is bool ) {
                return !(bool)value;
            }
            return value;
        }

    }
}
