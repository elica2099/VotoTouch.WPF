using System;
using System.Windows;
using System.IO;

namespace VotoTouch.WPF.Views.Tools
{
    /// <summary>
    /// Interaction logic for FWSStart.xaml
    /// </summary>
    public partial class FWSStart : Window
    {
        
        public FWSStart()
        {
            InitializeComponent();
            
            listBox1.Items.Clear();
            listBox1.Items.Add("Informazioni sulla versione: ");
            listBox1.Items.Add("");
            listBox1.Items.Add(VSDecl.VTS_VERSION);
#if _DBClose
            listBox1.Items.Add("DBClose version");
#endif

            if (VTConfig.IsDemoMode)
            {
                listBox1.Items.Add("");
                listBox1.Items.Add("Versione DEMO");
                return;
            }

            if (VTConfig.IsStandalone)
            {
                listBox1.Items.Add("");
                listBox1.Items.Add("Versione STANDALONE");
                listBox1.Items.Add("Usa GEAS.sql in locale");
                CaricaConfig(true);
                return;
            }

            // alcuni controlli : disco m:
            if (!System.IO.Directory.Exists(@"M:\"))
            {
                listBox1.Items.Add("");
                listBox1.Items.Add("ATTENZIONE: DISCO M non presente!");
                listBox1.Items.Add("Mappatura errata dischi.");
            }
            else
                CaricaConfig(false);

            // versione demo
            if (VTConfig.IsDemoMode)
            {
                listBox1.Items.Add("");
                listBox1.Items.Add("Versione DEMO");
            }
        }

        // carica la configurazione 
        public Boolean CaricaConfig(bool ADataLocal)
        {
            //DR11 OK
            string GeasFileName;

            // verifica se è locale oppure no
            if (ADataLocal)
            {
                if (File.Exists("c:\\data\\geas.sql"))
                    GeasFileName = "c:\\data\\geas.sql";
                else
                    return false;
            }
            else
            {
                if (File.Exists("M:\\geas.sql"))
                    GeasFileName = "M:\\geas.sql";
                else
                    return false;
            }

            // leggo cosa c'è dentro
            try
            {
                StreamReader file1 = File.OpenText(GeasFileName);
                string ss = file1.ReadLine();
                // testo se il file è giusto
                if (ss != "") 
                //if (ss == "GEAS 2000 -- Stringa Connesione a SQL")
                {
                    // tutto ok leggo
                    ss = file1.ReadLine();
                    ss = file1.ReadLine(); //DB_Dsn
                    ss = file1.ReadLine(); // DB_Name
                    listBox1.Items.Add( "Db:  " + ss);
                    ss = file1.ReadLine(); //DB_Uid
                    ss = file1.ReadLine(); // DB_Pwd
                    ss = file1.ReadLine(); // DB_Server
                    listBox1.Items.Add("Srv:  " + ss);
                    file1.Close();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }

        }

        private void ButtonAvvia_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void ButtonChiudi_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
