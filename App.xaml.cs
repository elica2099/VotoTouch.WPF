using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Windows.Markup;
using System.Windows.Threading;

namespace VotoTouch.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private CompositionContainer _container;
        public static App Instance;
        // nuovo sistema messaggi interni
        private static readonly InterClassMessenger IClassMessenger = new InterClassMessenger();
        internal static InterClassMessenger ICMsn => IClassMessenger;

        public App()
        {
            ShutdownMode = ShutdownMode.OnLastWindowClose;

            // Initialize static variables
            Instance = this;
            //GSV.currentLanguage = 0;
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            Process thisProcess = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(thisProcess.ProcessName).Length > 1)
            {
                MessageBox.Show("Un'altra istanza dell'applicazione è in esecuzione/Another instance of the application is running", "Attenzione!", MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
                return;
            }

            if (!Compose()) return;
            // bisogna fare questa chiamata per il problema di Crystal Repost 
            if (RuntimePolicyHelper.LegacyV2RuntimeEnabledSuccessfully)
            {
                // nullo
            }

            // setto la mainwindow
            Window view = new MainWindow();
            Current.MainWindow = view;
            // show the main form
            view.Show();
        }

        private bool Compose()
        {
            // An aggregate catalog can contain one or more types of catalog
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            _container = new CompositionContainer(catalog);

            try
            {
                // Perform the composition
                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                MessageBox.Show(compositionException.ToString());
                return false;
            }
            return true;
        }

        //   International ----------------------------------------------------------------------------------------

        public void setLocalizedStrings(int lang)
        {
            //GSV.currentLanguage = lang;
            switch (lang)
            {
                case VSDecl.LANGUAGE_IT:
                    setLocalizedStrings("it-IT");
                    break;
                case VSDecl.LANGUAGE_EN:
                    setLocalizedStrings("en-EN");
                    break;
                case VSDecl.LANGUAGE_DE:
                    setLocalizedStrings("de-DE");
                    break;
                default:
                    setLocalizedStrings("it-IT");
                    break;
            }
        }

        public void setLocalizedStrings(string lang)
        {
            ResourceDictionary dictionary = new ResourceDictionary();

            Uri languageUri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Languages-" + lang + ".xaml");

            if (File.Exists(languageUri.LocalPath))
            {
                try
                {
                    dictionary.Source = languageUri;
                    //this.Resources.MergedDictionaries.Clear();
                    this.Resources.MergedDictionaries.Add(dictionary);
                }
                catch (Exception objExc)
                {
                    //App.Instance.errorCall("<dberror> Error in function setLocalizedStrings: " + objExc.Message);
                } //No action, the default LocTable.xaml is used
            }
        }

        public string getLang(string key)
        {
            ResourceDictionary rm = Instance.Resources.MergedDictionaries[Resources.MergedDictionaries.Count - 1];
            return (string)rm[key];
        }

        public int getCurrentLanguage()
        {
            return 0; //GSV.currentLanguage;
        }

    }

}
