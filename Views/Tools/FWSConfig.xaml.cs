using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for FWSConfig.xaml
    /// </summary>
    
    public delegate void ehConfiguraLettore(object source, bool AUsaLettore, int AComPort);
    public delegate void ehConfiguraSemaforo(object source, bool AUsaSemaforo, 
        string AComPort, int ATipoSemaforo);
    public delegate void ehStatoSemaforo(object source, TStatoSemaforo AStato);
    public delegate void ehSalvaConfigurazioneLettore(object source, bool AUsaLettore, 
        int AComPort, string ASemComPort, bool AUsaSemaforo);

    public class TInfoSeriale
    {
        public string COM { get; set; }
        public string AssegnataA { get; set; }
        public string Tipo { get; set; }
    }

    public partial class FWSConfig : Window, INotifyPropertyChanged
    {
        public event ehConfiguraLettore ConfiguraLettore;
        public event ehConfiguraSemaforo ConfiguraSemaforo;
        public event ehStatoSemaforo StatoSemaforo;
        public event ehSalvaConfigurazioneLettore SalvaConfigurazioneLettore;

        public const string BC_ASSIGN = "Pistola Barcode";
        public const string SEM_ASSIGN = "Semaforo Seriale";
        public const string NO_ASSIGN = "-";

        private bool UsaLettore;
        private int ComPort;
        private bool UsaSemaforo;
        private int TipoSemaforo;
        private string SemComPort;
        private bool NoPorte;

        protected ObservableCollection<TInfoSeriale> _Seriali;
        public ObservableCollection<TInfoSeriale> Seriali
        {
            get => _Seriali;
            set
            {
                _Seriali = value;
                OnPropertyChanged("Seriali");
            }
        }
        public ICollectionView cvSeriali;
        private TInfoSeriale selectedItem = null;

        public FWSConfig()
        {
            InitializeComponent();

            NoPorte = false;
        }

        private void FWSConfig_OnLoaded(object sender, RoutedEventArgs e)
        {
            Seriali = new ObservableCollection<TInfoSeriale>();
            cvSeriali = CollectionViewSource.GetDefaultView(Seriali);
            cvSeriali.CurrentChanged += new EventHandler(cvSerialiCurrentChanged);

            CaricaSeriali();
            this.DataContext = this;
        }

        public void Configura()
        {
            // semaforo
            //grbSemaforo.Enabled = TotCfg.UsaSemaforo;
            if (VTConfig.UsaSemaforo)
            {
                // vedo il tipo di semaforo
                if (VTConfig.TipoSemaforo == VSDecl.SEMAFORO_IP)
                {
                    grbSemaforo.Header = "Configurazione Database Semaforo: IP " + VTConfig.IP_Com_Semaforo +
                                       "  Tipo: " + VTConfig.TipoSemaforo.ToString();
                    txtSemIP.Text = VTConfig.IP_Com_Semaforo;
                }
                if (VTConfig.TipoSemaforo == VSDecl.SEMAFORO_COM)
                    grbSemaforo.Header = "Configurazione Database Semaforo: Seriale " + VTConfig.IP_Com_Semaforo +
                                       "  Tipo: " + VTConfig.TipoSemaforo.ToString();
            }
            else
            {
                grbSemaforo.Header = "Nessun semaforo collegato - Semaforo ";
                if (VTConfig.TipoSemaforo == VSDecl.SEMAFORO_IP)
                    grbSemaforo.Header += "IP";
                else
                    grbSemaforo.Header += "COM";
            }   

            // lettore
            if (VTConfig.UsaLettore)
                grbLettore.Header = "Prova Lettore Barcode collegato su COM" + VTConfig.PortaLettore.ToString();

            else
                grbLettore.Header = "Nessun lettore Barcode collegato";

            // se è il semaforo ip comunque disabilito il pulsante
            //if (VTConfig.TipoSemaforo == VSDecl.SEMAFORO_IP)
            //    btnAssegnaSem.Enabled = false;
            //else
            //{
            //    txtSemIP.Enabled = false;
            //    label3.Enabled = false;
            //    btnSemAssegnaIP.Enabled = false;
            //    btnNoSemaforo.Enabled = false;
            //}
        }

        // listview --------------------------------------------------------------------------------
        
        private void cvSerialiCurrentChanged(object sender, EventArgs e)
        {
            TInfoSeriale infoSeriale = (TInfoSeriale)(cvSeriali.CurrentItem);
            if (infoSeriale != null)
            {
                selectedItem = infoSeriale;
            }
        }

        // SERIALI --------------------------------------------------------------------------------

        private void CaricaSeriali()
        {
            Seriali.Clear();
            selectedItem = null;
            foreach (COMPortInfo comPort in COMPortInfo.GetCOMPortsInfo())
            {
                TInfoSeriale ser = new TInfoSeriale()
                {
                    COM = comPort.Name,
                    AssegnataA = NO_ASSIGN,
                    Tipo = comPort.Description
                };

                // controllo se è assegnata al lettore
                if (VTConfig.UsaLettore && ser.COM == "COM" + VTConfig.PortaLettore.ToString())
                {
                    ser.AssegnataA = BC_ASSIGN;
                }

                Seriali.Add(ser);
            }

            // se non ci sono porte seriali attive inserisco una scritta
            if (Seriali.Count == 0)
            {
                Seriali.Add( new TInfoSeriale(){ COM = "Nessuna", Tipo = "", AssegnataA = "COM nel sistema"});
                NoPorte = true;
            }

            // setto i componenti in funzione delle porte
            //btnSalvaDB.Enabled = !NoPorte;
            btnAssegna.IsEnabled = Seriali.Count > 0;
            btnNoLettore.IsEnabled = Seriali.Count > 0;
            //btnAssegnaSem.IsEnabled = Seriali.Count > 0;

            UsaLettore = VTConfig.UsaLettore;
            ComPort = VTConfig.PortaLettore;
            UsaSemaforo = VTConfig.UsaSemaforo;
            TipoSemaforo = VTConfig.TipoSemaforo;
            SemComPort = VTConfig.IP_Com_Semaforo;
        }

        private void btnAggiorna_Click(object sender, RoutedEventArgs e)
        {
            CaricaSeriali();
            // riporto la situazione a prima
            if (ConfiguraLettore != null) { ConfiguraLettore(this, VTConfig.UsaLettore, VTConfig.PortaLettore); }
        }

        // pulsanti assegnamento --------------------------------------------------------------------------------

        private void btnAssegna_Click(object sender, RoutedEventArgs e)
        {
            // assegna a barcode la seriale corrente
            if (selectedItem == null) return;

            // verifico che non sia già assegnata
            if (selectedItem.AssegnataA != NO_ASSIGN)
            {
                MessageBox.Show("Porta già assegnata", "Exclamation",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // ora devo trovare la stringa COM
            string ss = selectedItem.COM;
            string removeString = "COM";
            int index = ss.LastIndexOf(removeString, StringComparison.Ordinal);
            if (index >= 0)
            {
                string st = ss.Remove(ss.IndexOf(removeString, StringComparison.Ordinal), removeString.Length);
                //string st = ss.Substring(index +3, ss.Length - index +3); // ss[index + 3].ToString();
                ComPort = Convert.ToInt16(st);
                UsaLettore = true;
                // ok ora apro
                if (ConfiguraLettore != null) { ConfiguraLettore(this, UsaLettore, ComPort); }
            }
            else
            {
                ComPort = VTConfig.PortaLettore;
                UsaLettore = false;
                // devo disabilitare l'oggetto
                if (ConfiguraLettore != null) { ConfiguraLettore(this, UsaLettore, ComPort); }
            }

            // però devo settare anche la lista
            foreach (TInfoSeriale infoSeriale in Seriali)
            {
                if (infoSeriale.AssegnataA == BC_ASSIGN)
                    infoSeriale.AssegnataA = NO_ASSIGN;
            }
            // ok ora lo scrivo sulla corretta porta com
            selectedItem.AssegnataA = BC_ASSIGN;

            if (UsaLettore) txtProva.Focus();
        }

        private void btnNoLettore_Click(object sender, RoutedEventArgs e)
        {
            // questa procedura libera la porta assegnata
            if (selectedItem == null) return;

            txtProva.Text = "";
            // se è già libera esco
            if (selectedItem.AssegnataA == NO_ASSIGN)
            {
                MessageBox.Show("Porta già libera", "Exclamation",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // controllo se è assegnata al barcode
            if (selectedItem.AssegnataA == BC_ASSIGN)
            {
                // libero la porta ed esco
                ComPort = VTConfig.PortaLettore;
                UsaLettore = false;
                selectedItem.AssegnataA = NO_ASSIGN;
                // devo disabilitare l'oggetto
                if (ConfiguraLettore != null) { ConfiguraLettore(this, UsaLettore, ComPort); }
                return;
            }
        }

        // pulsanti salvataggio e chiusura --------------------------------------------------------------------------------

        private void btnChiudi_Click(object sender, RoutedEventArgs e)
        {
            // prima testo se ci sono state variazioni non salvate
            if (VTConfig.UsaLettore != UsaLettore || VTConfig.PortaLettore != ComPort ||
                VTConfig.UsaSemaforo != UsaSemaforo || VTConfig.IP_Com_Semaforo != SemComPort)
            {
                if (MessageBox.Show("La configurazione del lettore è cambiata, vuoi chiudere la finestra senza salvarla?", "Question",
                        MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    this.Close();
            }
            else
                this.Close();
        }

        private void btnSalvaDB_Click(object sender, RoutedEventArgs e)
        {
            // testo se è cambiato qualcosa, sennò non salvo nemmeno e esco
            if (VTConfig.UsaLettore != UsaLettore || VTConfig.PortaLettore != ComPort ||
                VTConfig.UsaSemaforo != UsaSemaforo || VTConfig.IP_Com_Semaforo != SemComPort)
            {
                VTConfig.UsaLettore = UsaLettore;
                VTConfig.PortaLettore = ComPort;
                // posso solo modificare il semaforo com
                if (VTConfig.TipoSemaforo == VSDecl.SEMAFORO_COM)
                {
                    VTConfig.IP_Com_Semaforo = SemComPort;
                    VTConfig.UsaSemaforo = UsaSemaforo;
                }
                SalvaConfigurazioneLettore?.Invoke(this, UsaLettore, ComPort, SemComPort, UsaSemaforo);
                // aggiorno i campi
                Configura();
            }
            this.Close();
        }

        public void Semaforo(bool Attivato)
        {
            grbSemaforo.IsEnabled = Attivato;
        }

        // Semaforo ------------------------------------------------------------------

        private void btnLibero_Click(object sender, RoutedEventArgs e)
        {
            if (StatoSemaforo != null) { StatoSemaforo(this, TStatoSemaforo.stsLibero); }
        }

        private void btnSOccupato_Click(object sender, RoutedEventArgs e)
        {
            if (StatoSemaforo != null) { StatoSemaforo(this, TStatoSemaforo.stsOccupato); }
        }

        private void btnSFineOcc_Click(object sender, RoutedEventArgs e)
        {
            if (StatoSemaforo != null) { StatoSemaforo(this, TStatoSemaforo.stsFineoccupato); }
        }

        private void btnSErrore_Click(object sender, RoutedEventArgs e)
        {
            if (StatoSemaforo != null) { StatoSemaforo(this, TStatoSemaforo.stsErrore); }
        }

        // Lettore Barcode ------------------------------------------------------------------

        public void BadgeLetto(string AText)
        {
            //  lettura badge
            txtProva.Text = AText;
        }

        //private void frmConfig_Load(object sender, EventArgs e)
        //{
        //    // non serve
        //    lvSeriali.Focus();
        //}

        //private void frmConfig_Shown(object sender, EventArgs e)
        //{
        //    lvSeriali.Focus();

        //}

        // NOtify ------------------------------------------------------------------

        #region INotifyPropertyChanged
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string strPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(strPropertyName));
        }
        #endregion


    }
}
