using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media;
//using System.Management;
using System.Windows.Input;
using System.Windows.Threading;
using Application = System.Windows.Application;
using Cursor = System.Windows.Input.Cursor;
using Cursors = System.Windows.Input.Cursors;
using MessageBox = System.Windows.MessageBox;

namespace VotoTouch.WPF
{
    public static class Utils
    {

        // ----------------------------------------------------------------------------------------------
        //  ERROR
        // ----------------------------------------------------------------------------------------------

        // ---------------------------------------------------------------------------------
        //   errors / messages
        // ---------------------------------------------------------------------------------        

        private static ToolDebugError debugErr = null; // new ToolDebugError();

        //Closed += (sender, args) => this.m_myWindow = null;


        public static void errorCall(string function, string message)
        {
            // this routine il called when an error occour in a part of a code, normally in a try.catch context
            // first it write to the log
            Logging.WriteToLog(function + " > " + message);

            //#if DEBUG
            //            MessageBox.Show("Errore nella funzione " + function + "\n" + "Eccezione : \n" + message, "Error");
            //#endif

            // second it send a message via a timer (cause the error may be happened isnisde another message
            // and it cause an error
            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(100),
                Tag = (string)(function + " > " + message)
            };
            timer.Tick += delegate(object asender, EventArgs ae)
            {
                ((DispatcherTimer) timer).Stop();
                // send the message
                string ss = (string)timer.Tag;
                if (debugErr == null)
                {
                    debugErr = new ToolDebugError();
                    debugErr.Closed += (sender, args) => debugErr = null;
                    debugErr.Show();
                }
                debugErr?.addError(ss);
            };
            timer.Start();
        }

        public static Brush ColorToBrush(string color) // color = "#E7E44D"
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(color));
        }

        public static void RebootApplication()
        {
            //ToolReboot dd = new ToolReboot();
            //dd.Show();
            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(700)
            };
            timer.Tick += delegate (object asender, EventArgs ae)
            {
                ((DispatcherTimer)timer).Stop();

                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = "/C choice /C Y /N /D Y /T 1 & START \"\" \"" + Assembly.GetExecutingAssembly().Location + "\"";
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "cmd.exe";
                Process.Start(Info);
                Application.Current.Shutdown();
                //dd.Close();
                //dd = null;
            };
            timer.Start();
        }

        //  STRINGHE    ----------------------------------------------------------------------------------------------

        #region Stringhe

        public static string ExtractPureUsername(string username)
        {
            string[] parts = username.Split('@');
            if (parts.Length == 2)
            {
                return parts[0];
            }

            parts = username.Split('\\');
            if (parts.Length == 2)
            {
                return parts[1];
            }

            return username;
        }

        public static string Agg(string ss)
        {
            string quote = "\"";
            if (ss == null) ss = "";
            string sr = ss;
            sr = sr.Replace("'", "`");
            sr = sr.Replace(quote, "`");
            sr = sr.Replace("/", "-");
            //sr = sr.Replace(",", "");
            sr = sr.Replace("\r", "");
            sr = sr.Replace("\n", "");

            return sr;
        }

        public static string Agg(string ss, int n)
        {
            string quote = "\"";
            if (ss == null) ss = "";
            string sr = ss;
            sr = sr.Replace("'", "`");
            sr = sr.Replace(quote, "`");
            sr = sr.Replace("/", "-");
            //sr = sr.Replace(",", "");
            sr = sr.Replace("\r", "");
            sr = sr.Replace("\n", "");

            string st = StringTruncate(sr, n);
            return st;
        }

        public static string AggAp(string ss)
        {
            string quote = "\"";
            if (ss == null) ss = "";
            string sr = ss;
            sr = sr.Replace("'", "`");
            sr = sr.Replace(quote, "`");
            sr = sr.Replace("/", "-");
            //sr = sr.Replace(",", "");
            sr = sr.Replace("\r", "");
            sr = sr.Replace("\n", "");

            return "'" + sr + "'";
        }

        public static string AggWRet(string ss, int n)
        {
            string quote = "\"";
            if (ss == null) ss = "";
            string sr = ss;
            sr = sr.Replace("'", "`");
            sr = sr.Replace(quote, "`");
            sr = sr.Replace("/", "-");
            //sr = sr.Replace(",", "");
            //sr = sr.Replace("\r", "");
            //sr = sr.Replace("\n", "");

            string st = StringTruncate(sr, n);
            return st;
        }

        public static string AggWRet(string ss)
        {
            string quote = "\"";
            if (ss == null) ss = "";
            string sr = ss;
            sr = sr.Replace("'", "`");
            sr = sr.Replace(quote, "`");
            sr = sr.Replace("/", "-");
            //sr = sr.Replace(",", "");
            //sr = sr.Replace("\r", "");
            //sr = sr.Replace("\n", "");

            return sr;
        }

        public static string AggAp(string ss, int n)
        {
            string quote = "\"";
            if (ss == null) ss = "";
            string sr = ss;
            sr = sr.Replace("'", "`");
            sr = sr.Replace(quote, "`");
            sr = sr.Replace("/", "-");
            //sr = sr.Replace(",", "");
            //sr = sr.Replace("?", "");
            sr = sr.Replace("\r", "");
            sr = sr.Replace("\n", "");

            string st = StringTruncate(sr, n);

            return "'" + st + "'";
        }

        public static string StringTruncate(string source, int length)
        {
            if (source.Length > length)
            {
                source = source.Substring(0, length);
            }
            return source;
        }

        //public static string purifyBadgeString(string ABadge)
        //{
        //    if (ABadge.Length > CPRConfig.cfg.BadgeLen)
        //    {
        //        // get the correct badge
        //        string pBadge = ABadge.Substring((ABadge.Length - CPRConfig.cfg.BadgeLen), CPRConfig.cfg.BadgeLen);
        //        return pBadge.TrimStart('0');
        //    }
        //    else
        //        return ABadge.TrimStart('0');
        //}

        public static string UppercaseWords(string value)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }
        #endregion


    }

    public class WaitCursor : IDisposable
    {
        private Cursor _previousCursor;

        public WaitCursor()
        {
            _previousCursor = Mouse.OverrideCursor;

            Mouse.OverrideCursor = Cursors.Wait;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Mouse.OverrideCursor = _previousCursor;
        }

        #endregion
    }
}
