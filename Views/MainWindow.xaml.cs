﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using VotoTouch.WPF.Views.Tools;
using VotoTouch.WPF.Views.UserControls;
using WpfScreenHelper;

namespace VotoTouch.WPF
{
    // TODO: verificare multicandidato e pagine
    // TODO: In caso di Votazione con AbilitaDiritti... mettere sulla videata di inizio lo stato dei diritti espressi
    // TODO: ModoAssemblea, salvare azioni o voti, mostrare azioni o voti
    // TODO: Mettere finestra riepilogo azionista
    // TODO: Unificare pop e agm in TListaAzionisti

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IInterClassMessenger, INotifyPropertyChanged
    {
        public delegate void EventDataReceived(object source, string messaggio);
        public event EventDataReceived evtDataReceived;

        // timer di disaccoppiamento
        //private DispatcherTimer timLetturaBadge;
        private DispatcherTimer timCambiaStato;
        //private DispatcherTimer timConfigura;
        private DispatcherTimer timAutoRitorno;
        private DispatcherTimer timPopup;
        private DispatcherTimer timVotoAperto;

        // oggetti demo (li creo dinamicamente)
        //private Button btnBadgeUnVoto;
        //private Button btnBadgePiuVoti;
        //private Button btnFineVotoDemo;

        private static Mutex appMutex;

        // finestre e usercontrol
        public FWSMessage frmVSMessage;
        public FWSConfig fConfig;
        //public SplashScreen splash;
        //public FWSStart FStart;
	    //public LabelCandidati lbConferma; ------------------------------ ??????

		// oggetti globali
		public  CVotoTouchScreen oVotoTouch;    // classe del touch
        public  CVotoTheme oVotoTheme;          // classe del tema grafico
        public  CVotoBaseDati oDBDati;          // classe del database
        public  CBaseSemaphore oSemaforo;       // classe del semaforo
        public  CNETActiveReader NewReader;
        public  CVotoImages oVotoImg;
        // strutture
        public ConfigDbData DBConfig;           // database
        public TAppStato Stato;                 // macchina a stato
        public string   LogVotiNomeFile;        // nome file del log
        public bool CtrlPrimoAvvio;             // serve per chiudere la finestra in modo corretto
        
        // Votazioni
	    public TListaVotazioni Votazioni;
        // Dati dell'azionista e delle deleghe che si porta dietro
        public TListaAzionisti Azionisti;
        // variabili relative alla votazione
        public bool IsVotazioneDifferenziata = false;
        public bool LocalAbilitaVotazDifferenziataSuRichiesta = false;
        // cpontrollo degli eventi di voto
	    private bool AperturaVotoEsterno;
        // flag uscita in votazione
        public bool UscitaInVotazione;
        // public bool 
	    public bool RitornaDaAnnulla = false;
        // Variabile temporanea voti espressi Nuova Versione (Array)
        public ArrayList FVotiExpr;

        // Variabile temporanea Voti Espressi
        public int VotoEspresso;
        public string VotoEspressoStr;
        public string VotoEspressoStrUp;
	    public string VotoEspressoStrNote;
		public int Badge_Letto;
        public string Badge_Seriale;

        public MainWindow()
        {
            InitializeComponent();
            
            // registrazione dei metodi interclasse (IInterClassMessenger)
            #region InterClassMessages
            App.ICMsn.RegisterMessage(this, VSDecl.ICM_MAIN_BADGEREAD);
            App.ICMsn.RegisterMessage(this, VSDecl.ICM_MAIN_CLOSESTATUSPANEL);
            #endregion

            // resize
            this.SizeChanged += OnWindowSizeChanged;

            // data_path
            CheckDataFolder();
            // variabili speciali demo/debug....
            VTConfig.IsDebugMode = File.Exists(VTConfig.Data_Path + "VTS_DEBUG.txt");
            VTConfig.IsPaintTouch = File.Exists(VTConfig.Data_Path + "VTS_PAINT_TOUCH.txt");
            VTConfig.IsDemoMode = File.Exists(VTConfig.Data_Path + "VTS_DEMO.txt");
            VTConfig.IsAdmin = File.Exists(VTConfig.Data_Path + "VTS_ADMIN.txt");
            VTConfig.IsStandalone = File.Exists(VTConfig.Data_Path + "VTS_STANDALONE.txt");


            // finestra di start
            FWSStart FStart = new FWSStart(this);
            if (FStart.ShowDialog() == false)
            {
                Application.Current.Shutdown();
                return;
            }
            FStart = null;

            // inizializzazione Classe del TouchScreen
            oVotoTouch = new CVotoTouchScreen(); //ref TotCfg);
            oVotoTouch.ShowPopup += new ehShowPopup(oVotoTouch_ShowPopup);
            oVotoTouch.PremutoVotaNormale += new ehPremutoVotaNormale(onPremutoVotaNormale);
            oVotoTouch.PremutoVotaDifferenziato += new ehPremutoVotaDifferenziato(onPremutoVotaDifferenziato);
            oVotoTouch.PremutoConferma += new ehPremutoConferma(onPremutoConferma);
            oVotoTouch.PremutoAnnulla += new ehPremutoAnnulla(onPremutoAnnulla);
            oVotoTouch.PremutoVotoValido += new ehPremutoVotoValido(onPremutoVotoValido);
            oVotoTouch.PremutoSchedaBianca += new ehPremutoSchedaBianca(onPremutoSchedaBianca);
            oVotoTouch.PremutoNonVoto += new ehPremutoNonVoto(onPremutoNonVoto);
            oVotoTouch.PremutoInvalido += new ehPremutoInvalido(onPremutoInvalido);
            oVotoTouch.PremutoTab += new ehPremutoTab(onPremutoTab);
            oVotoTouch.TouchWatchDog += new ehTouchWatchDog(onTouchWatchDog);
            oVotoTouch.PremutoMultiAvanti += new ehPremutoMultiAvanti(onPremutoVotoValidoMulti);
            oVotoTouch.PremutoMulti += new ehPremutoMulti(onPremutoVotoMulti);
            oVotoTouch.PremutoBottoneUscita += new ehPremutoBottoneUscita(onPremutoBottoneUscita);
            oVotoTouch.PremutoContrarioTutti += new ehPremutoContrarioTutti(onPremutoContrarioTutti);
            oVotoTouch.PremutoAstenutoTutti += new ehPremutoAstenutoTutti(onPremutoAstenutoTutti);
            // se sono in debug evidenzio le zone sensibili
            oVotoTouch.PaintTouchOnScreen = VTConfig.IsPaintTouch;
            // inizializzazione classe del tema
            oVotoTheme = new CVotoTheme();
            oVotoTheme.CaricaTemaDaXML(VTConfig.Img_Path);

            // creazione timer di disaccoppiamento funzioni
            // TODO: METTERE I MESSAGGI
            // timer di lettura badge
            timVotoAperto = new DispatcherTimer {IsEnabled = false, Interval = TimeSpan.FromMilliseconds(VSDecl.TIM_CKVOTO_MIN)};
            timVotoAperto.Tick += timVotoAperto_Tick;
            //// timer di lettura badge
            //timLetturaBadge = new DispatcherTimer {IsEnabled = false, Interval = TimeSpan.FromMilliseconds(30)};
            //timLetturaBadge.Tick += timLetturaBadge_Tick;
            // timer di cambio stato
            timCambiaStato = new DispatcherTimer {IsEnabled = false, Interval = TimeSpan.FromMilliseconds(30)};
            timCambiaStato.Tick += timCambiaStato_Tick;
            // timer di configurazione
            //timConfigura = new DispatcherTimer {IsEnabled = false, Interval = TimeSpan.FromMilliseconds(30)};
            //timConfigura.Tick += timConfigura_Tick;
            // timer di autoritorno
            timAutoRitorno = new DispatcherTimer {IsEnabled = false, Interval = TimeSpan.FromMilliseconds(VTConfig.TimeAutoRitornoVoto) };
            timAutoRitorno.Tick += timAutoRitorno_Tick;
            // popup multicandidati
            timPopup = new DispatcherTimer { IsEnabled = false, Interval = TimeSpan.FromMilliseconds(6000) };
            timPopup.Tick += timPopup_Tick;

            // ritrovo il nome della macchina che mi servirà per interrogare il db
            int i;
            VTConfig.NomeTotem = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            for (i = 0; i < VTConfig.NomeTotem.Length; i++)
                if (VTConfig.NomeTotem[i] == '\\') break;
            VTConfig.NomeTotem = VTConfig.NomeTotem.Remove(0, i + 1);

            // inizializza alcune variabili
            IsVotazioneDifferenziata = false;               // non è differenziata
            Badge_Letto = 0;
            Badge_Seriale = "";
            UscitaInVotazione = false;
            CtrlPrimoAvvio = false;

            // ok ora creo i controlli
            CreaControlli();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Massimizzo la finestra
#if DEBUG
            this.WindowState = WindowState.Normal;
		    this.Left = 0;
		    this.Height = 0;
            this.Width = 1280;
            this.Height = 1024;
#else      
            WindowState = FormWindowState.Maximized;
#endif
            // gestione immagini
            oVotoImg = new CVotoImages {MainForm = this};
            CtrlPrimoAvvio = oVotoImg.CheckImageFolder();
		    //pnPopupRed.Left = 5;
            //pnPopupRed.Top = 5;

            //btnCancVoti.Visible = VTConfig.IsAdmin;

            // identificazione della versione demo, nella cartella data o nella sua cartella
            if (VTConfig.IsDemoMode)
            {
                // Ok è la versione demo
                Logging.generateInternalLogFileName(VTConfig.Data_Path, "VotoTouch_" + VTConfig.NomeTotem);
                Logging.WriteToLog("---- DEMO MODE ----");
                // ok, ora creo la classe che logga i voti
                LogVotiNomeFile = LogVote.GenerateDefaultLogFileName(VTConfig.Data_Path, "VotoT_" + VTConfig.NomeTotem);
            }
            else
            {
                // ok, qua devo vedere i due casi:
                // il primo è VTS_STANDALONE.txt presente il che vuol dire che ho la configurazione
                // in locale, caricando comunque un file GEAS.sql da data
                if (VTConfig.IsStandalone)
                {
                    Logging.generateInternalLogFileName(VTConfig.Data_Path, "VotoTouch_" + VTConfig.NomeTotem);
                    Logging.WriteToLog("---- STANDALONE MODE ----");
                }
                else
                {
                    // verifica della mappatura
                    if (!Directory.Exists(@"M:\"))
                    {
                        MessageBox.Show(App.Instance.getLang("SAPP_START_ERRMAP"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        CtrlPrimoAvvio = false;
                        return;
                    }

                    // Inizializzo il log
                    if (!Directory.Exists(@"M:\LST\VotoTouch\"))
                        Directory.CreateDirectory(@"M:\LST\VotoTouch\");
                    Logging.generateInternalLogFileName(@"M:\LST\VotoTouch\", "VotoTouch_" + VTConfig.NomeTotem);
                }
            }
            // loggo l'inizio dell'applicazione
            Logging.WriteToLog("<start> Inizio Applicazione");

            // classe lbConferma
		    //lbConferma = new LabelCandidati {Visible = false, Parent = this};

		    // Inizializzo la classe del database
            if (VTConfig.IsDemoMode)
                oDBDati = new CVotoFileDati(DBConfig, VTConfig.IsStandalone, VTConfig.Data_Path);
            else
                oDBDati = new CVotoDBDati(DBConfig, VTConfig.IsStandalone, VTConfig.Data_Path);
            // carico la configurazione
            if (!oDBDati.CaricaConfig())
            {
                Logging.WriteToLog("<dberror> Problemi nel caricamento della configurazione DB, mappatura");
                MessageBox.Show(App.Instance.getLang("SAPP_START_ERRCFG"), "", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                CtrlPrimoAvvio = false;
                return;
            }
            // vado avanti con il database mi connetto
            if (oDBDati.DBConnect() != null)
            {
                int DBOk = 0;  // variabile di controllo sul caricamento
                // leggo la configurazione del badge/impianto
                DBOk += oDBDati.CaricaConfigDB(ref VTConfig.BadgeLen, ref VTConfig.CodImpianto);
                // leggo la configurazione generale
                DBOk += oDBDati.DammiConfigDatabase(); //ref TotCfg);
                // leggo la configurazione del singolo totem
                DBOk += oDBDati.DammiConfigTotem(); //, ref TotCfg);
                if (VTConfig.VotoAperto) Logging.WriteToLog("Votazione già aperta");
                // carica le votazioni, le carica comunque all'inizio
                Rect FFormRect = new Rect(0, 0, Width, Height);
                Votazioni = new TListaVotazioni(oDBDati);
                Votazioni.CaricaListeVotazioni(VTConfig.Data_Path, FFormRect, true);
                // ok, finisce
                if (DBOk == 0)
                {
                    // nel log va tutto bene
                    Logging.WriteToLog("<startup> Caricamento dati database OK");
                }
                else
                {
                    Logging.WriteToLog("<dberror> Problemi nel caricamento configurazione db");
                    MessageBox.Show(App.Instance.getLang("SAPP_START_ERRDB"), "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                Logging.WriteToLog("<dberror> Problemi nella connessione al Database");
                MessageBox.Show(App.Instance.getLang("SAPP_START_ERRCONN"), "", MessageBoxButton.OK, MessageBoxImage.Error);
                CtrlPrimoAvvio = false;
                return;
            }
            
            // semaforo
            oSemaforo = new CIPThreadSemaphore {ConnAddress = VTConfig.IP_Com_Semaforo};
            oSemaforo.ChangeSemaphore += onChangeSemaphore;
            // se è attivato lo setto
            if (VTConfig.UsaSemaforo)  oSemaforo.AttivaSemaforo(true);

            // array dei voti temporanei
            FVotiExpr = new ArrayList();
            // azionisti
            Azionisti = new TListaAzionisti(oDBDati);
            // Apertura voto lo setto uguale così in stato badge non carica 2 volte le Liste
            AperturaVotoEsterno = VTConfig.VotoAperto;  

            //pnSemaf.BackColor = Color.Transparent;

            //splash.SetSplash(90, rm.GetString("SAPP_START_INITVAR"));   //"Inizializzo variabili...");
            // scrive la configurazione nel log
            Logging.WriteToLog(VSDecl.VTS_VERSION);
            Logging.WriteToLog("** Configurazione:");
            Logging.WriteToLog("   Usalettore: " + VTConfig.UsaLettore.ToString());
            Logging.WriteToLog("   Porta: " + VTConfig.PortaLettore.ToString());
            Logging.WriteToLog("   UsaSemaforo: " + VTConfig.UsaSemaforo.ToString());
            Logging.WriteToLog("   IPSemaforo: " + VTConfig.IP_Com_Semaforo);
            Logging.WriteToLog("   IDSeggio: " + VTConfig.IDSeggio.ToString());
            Logging.WriteToLog("   NomeComputer: " + VTConfig.NomeTotem);
            Logging.WriteToLog("   ControllaPresenze: " + VTConfig.ControllaPresenze.ToString());
            Logging.WriteToLog("** CodiceUscita: " + VTConfig.CodiceUscita);
            Logging.WriteToLog("");
            
            // inizializzo i componenti
			//InizializzaControlli();
            // Se è in demo mode metto i controlli
            if (VTConfig.IsDemoMode)
                InizializzaControlliDemo();

			// ora inizializzo la macchina a stati
			Stato = TAppStato.ssvBadge;

            // se la votazione è aperta il timer di controllo voto batte di meno
            timVotoAperto.Interval = TimeSpan.FromMilliseconds(VTConfig.VotoAperto ? VSDecl.TIM_CKVOTO_MAX : VSDecl.TIM_CKVOTO_MIN);

            // Attivo la macchina a stati (in FMain_MacchinaAStati.cs)
            CambiaStato();

            timVotoAperto.Start();

            // da qui in avanti era in shown
            // attivo il barcode reader
            NewReader = new CNETActiveReader();
            NewReader.ADataRead += new DataRead(SerialDataReceived);
            //evtDataReceived += new EventDataReceived(onDataReceived);
            // ora cerco se c'è qualche porta che va bene
            string ComPort = "";
            string ComDescr = "";
            int ComPortInt = 0;
            bool foundsomething = NewReader.AutodiscoverBarcode(ref ComPort, ref ComDescr, ref ComPortInt);
            // Attivo        
            if (VTConfig.UsaLettore)
            {
                if (foundsomething && VTConfig.PortaLettore != ComPortInt)
                {
                    // todo: ha perso la configurazione della com, non sa cosa fare
                }
                NewReader.PortName = "COM" + VTConfig.PortaLettore.ToString();
            }
            else
            {
                // se ho trovato qualcosa e non c'era ancora la configurazione allora lo salvo comunque
                if (foundsomething)
                {
                    VTConfig.UsaLettore = true;
                    VTConfig.PortaLettore = ComPortInt;
                    NewReader.PortName = "COM" + VTConfig.PortaLettore.ToString();
                    // salvo nel db
                    oDBDati.SalvaConfigurazionePistolaBarcode();
                }
            }
            if (VTConfig.UsaLettore)
            {
                if (!NewReader.Open())
                {
                    // ci sono stati errori con la com all'apertura
                    VTConfig.UsaLettore = false;
                    MessageBox.Show(
                        App.Instance.getLang("SAPP_START_ERRCOM1") + VTConfig.PortaLettore + 
                        App.Instance.getLang("SAPP_START_ERRCOM2"),"Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    ShowNoBarcode = true;
                }
                else
                    ShowNoBarcode = false;
            }

            // set datacontext
            this.DataContext = this;
        }

        private void frmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timVotoAperto.Stop();
            // alcune cose sul database
            oDBDati.DBDisconnect();
            NewReader.Close();
        }

        void oVotoTouch_ShowPopup(object source, string messaggio)
        {
            //lblMsgPopup.Text = messaggio;
            //pnPopupRed.Visible = true;
            //timPopup.Start();
        }

        // IInterClassMessenger ---------------------------------------------------------------------------------

        public void InterClassCommand(string ACommand, object AParam, object WParam, object YParam, object ZParam)
        {
            //here it comes the commands from IInterClassMessenger interface and from other classes
            switch (ACommand)
            {
                case VSDecl.ICM_MAIN_BADGEREAD:
                    Badge_Seriale = (string) AParam;
                    Serial_NewRead(Badge_Seriale);
                    break;
                case VSDecl.ICM_MAIN_CLOSESTATUSPANEL:
                    UStatusPanel stp = (UStatusPanel) this.mainGrid.FindName("statusPanel");
                    if (stp != null)
                    {
                        stp.Visibility = Visibility.Hidden;
                        stp = null;
                    }
                    break;
                case VSDecl.ICM_MAIN_SHOWCONFIG:
                    if (Stato == TAppStato.ssvBadge) MostraFinestraConfig();
                    break;
            }
        }

        //  PAINT AND RESIZE ------------------------

        private void frmMain_Paint(object sender, PaintEventArgs e)
        {
            // ok, questa funzione serve all'oggetto CTouchscreen per evidenziare le zone sensibili
            if (oVotoTouch != null)
            {
                oVotoTouch.PaintTouch(sender, e);

                // se la votazione corrente è di candidato su più pagine disegno i rettangoli
                if (Stato == TAppStato.ssvVoto && 
                    (Votazioni.VotoCorrente.TipoVoto == VSDecl.VOTO_CANDIDATO ||
                        Votazioni.VotoCorrente.TipoVoto == VSDecl.VOTO_CANDIDATO_SING))
                {
                    // paint delle label Aggiuntive
                    //oVotoTheme.PaintlabelProposteCdaAlt(sender, e, ref Votazioni.VotoCorrente, true);
                   // oVotoTheme.PaintlabelProposteCdaAlt(sender, e, Votazioni.VotoCorrente, true);
                    // paint dei Bottoni
                    oVotoTouch.PaintButtonCandidatoPagina(sender, e, false, oVotoTheme.BaseFontCandidato, 
                        oVotoTheme.BaseFontCandidatoBold, oVotoTheme.BaseColorCandidato);
                }
                // se la votazione corrente è di MULTIcandidato su più pagine disegno i rettangoli
                if (Stato == TAppStato.ssvVoto && Votazioni.VotoCorrente.TipoVoto == VSDecl.VOTO_MULTICANDIDATO)
                {
                    // paint delle label Aggiuntive
                    //oVotoTheme.PaintlabelProposteCdaAlt(sender, e, Votazioni.VotoCorrente, false);
                    //oVotoTheme.PaintlabelNSelezioni(sender, e, Votazioni.VotoCorrente, false);
                    // paint dei bottoni
                    oVotoTouch.PaintButtonCandidatoPagina(sender, e, true, oVotoTheme.BaseFontCandidato,
                        oVotoTheme.BaseFontCandidatoBold, oVotoTheme.BaseColorCandidato);
                }

                // ******* OBSOLETO ********/
                // votazione VOTO_CANDIDATO_SING, candidato a singola pagina, disegno i rettangoli
                //if (Stato == TAppStato.ssvVoto && (FParVoto[CurrVoteIDX].TipoVoto == VSDecl.VOTO_CANDIDATO_SING))
                //    oVotoTouch.PaintButtonCandidatoSingola(sender, e);
                
                // se sono nello stato di votostart e il n. di voti è > 1
                //if (Stato == TAppStato.ssvVotoStart) // && Azionisti.HaDirittiDiVotoMultipli())
                //{
                //    // faccio il paint del numero di diritti di voto nel bottone in basso a sx , 
                //    // in questo caso uso un paint e non una label per un problema grafico di visibilità
                //    int VVoti = VTConfig.ModoAssemblea == VSDecl.MODO_AGM_POP
                //                   ? Azionisti.DammiMaxNumeroDirittiDiVotoTotali()
                //                   : Azionisti.DammiMaxNumeroVotiTotali();
                //    string ss = string.Format("{0:N0}", VVoti.ToString());
                //    if (Azionisti.HaDirittiDiVotoMultipli())
                //    {
                //        ss += "(d)";
                //        oVotoTheme.PaintDirittiDiVoto(sender, e, ss);
                //    }
                //    //oVotoTheme.PaintDirittiDiVoto(sender, e, VVoti);
                //}
            }

            // se è demo devo stampare una label
            /*
            if (VTConfig.IsDemoMode)
            {
                try
                {
                    System.Drawing.Drawing2D.GraphicsState gs = e.Graphics.Save();
                    Font fn = new Font("Tahoma", 90, FontStyle.Bold);
                    string str = rm.GetString("SAPP_DEMO");
                    StringFormat sf = (StringFormat)StringFormat.GenericTypographic.Clone();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisWord;
                    Color semiTransparentColor = Color.FromArgb(50, Color.DarkBlue);
                    SolidBrush whiteBrush = new SolidBrush(semiTransparentColor);
                    e.Graphics.RotateTransform(-35);
                    e.Graphics.TranslateTransform(-400, 350);

                    e.Graphics.DrawString(str, fn, whiteBrush, new
                        RectangleF(5, 5, this.ClientRectangle.Width - 15, this.ClientRectangle.Height - 10), sf);
                    e.Graphics.Restore(gs);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    // non faccio nulla, non serve, al massimo non apparirà la scritta
                }
            }
            */
        }
         
        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //double newWindowHeight = e.NewSize.Height;
            //double newWindowWidth = e.NewSize.Width;
            //double prevWindowHeight = e.PreviousSize.Height;
            //double prevWindowWidth = e.PreviousSize.Width;
            MainWindowSizeChanged();
        }

        private void MainWindowSizeChanged()
        {
            // immagine del salvataggio non serve qui
            //pbSalvaDati.Left = (this.Width / 2) - (pbSalvaDati.Width / 2);
            //pbSalvaDati.Top = (this.Height / 2) - (pbSalvaDati.Height  / 2);

            Rect FFormRect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);

            // lo stesso faccio per la classe del thema che si occupa di disegnare 
            if (oVotoTheme != null)
            {
                oVotoTheme.FFormRect = FFormRect;
                CaricaTemaInControlli();
            }
            // ok ora le votazioni
            Votazioni?.ResizeZoneVotazioni(FFormRect);
            //Votazioni.CalcolaTouchZoneVotazioni(FFormRect);

            // ok, ora se è in demo mode faccio il resize dei controlli
            if (VTConfig.IsDemoMode)
            {
                ResizeControlliDemo();
            }
        }

		//  SALVATAGGIO DEI DATI DI VOTO COME SCHEDA BIANCA o nulla DA INTERRUZIONE ------------------------

		private int MettiSchedeDaInterruzione()
		{
            // prima di tutto vedo se è attivato SalvaVotoNonConfermato
            // se sono nello stato di conferma, confermo il voto espresso e poi metto le altre schede
            if (Stato == TAppStato.ssvVotoConferma && VTConfig.SalvaVotoNonConfermato) 
                Azionisti.ConfermaVoti_VotoCorrente(ref FVotiExpr);

            // Dopodichè segnalo ad azionisti di riempire le votazioni con schede bianche, ma solo  
            // in funzione di AbilitaDirittiNonVoglioVotare:
            //      false - mi comporto normalmente, salvo i non votati con IDSchedaUscitaForzata
            //      true  - non faccio nulla, verranno come non votati e saranno disponibili alla nuova votazione

            if (!VTConfig.AbilitaDirittiNonVoglioVotare)
            {
                TVotoEspresso vz = new TVotoEspresso
                    {
                        NumVotaz = Votazioni.VotoCorrente.IDVoto,
                        VotoExp_IDScheda = VTConfig.IDSchedaUscitaForzata,
                        TipoCarica = 0,
                        //Str_ListaElenco = "",
                        //StrUp_DescrLista = ""
                    };

                Azionisti.ConfermaVotiDaInterruzione(vz);
            }
		    return 0;
		}

        // DataPath  ----------------------------------------------------------------

        private bool CheckDataFolder()
        {
            // ok, per prima cosa verifico se c'è la cartella c:\data, se si ok
            // sennò devo considerare la cartella dell'applicazione, se non c'è esco
            string SourceExePath = System.IO.Path.GetDirectoryName( new Uri( 
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath );
            VTConfig.Exe_Path = SourceExePath + @"\";

            // se ci solo nel immagini in c:\data\VtsNETImg
            if (System.IO.Directory.Exists("c:" + VSDecl.DATA_PATH_ABS) &&      // "\\Data\\";
                System.IO.Directory.Exists("c:" + VSDecl.IMG_PATH_ABS))         // "\\Data\\VtsNETImg\\";
            {
                // allora i path sono quelli assoluti  c:\data\VtsNETImg
                VTConfig.Data_Path = "c:" + VSDecl.DATA_PATH_ABS;
                VTConfig.Img_Path = "c:" + VSDecl.IMG_PATH_ABS;
            }
            else
            {
                // controllo se esistono le cartelle locali nella cartella applicazione cioè la 
                // cartella \\VtsNETImgLocali\\ nel caso il VotoSegreto es. fosse sotto c:\Programmi
                if (System.IO.Directory.Exists(SourceExePath + VSDecl.IMG_PATH_LOC))  // "\\VtsNETImgLocali\\";
                {
                    // metto i corretti path
                    VTConfig.Data_Path = SourceExePath + @"\";
                    VTConfig.Img_Path = SourceExePath + VSDecl.IMG_PATH_LOC;
                }
                else
                {
                    // l'ultimo controllo che faccio è sulla cartella c:\Data\VtsNETImgLocali\
                    if (System.IO.Directory.Exists("c:" + VSDecl.IMG_PATH_LOC_ABS))
                    {
                        // metto i corretti path
                        VTConfig.Data_Path = @"c:\data\";
                        VTConfig.Img_Path = SourceExePath + VSDecl.IMG_PATH_LOC_ABS;
                    }
                    else
                    {
                        // Non ho trovato nessuna cartella, quindi mi creo il ramo c:\\Data\\VtsNETImg\\
                        Directory.CreateDirectory("c:" + VSDecl.DATA_PATH_ABS);
                        Directory.CreateDirectory("c:" + VSDecl.IMG_PATH_ABS);
                        VTConfig.Data_Path = "c:" + VSDecl.DATA_PATH_ABS;
                        VTConfig.Img_Path = "c:" + VSDecl.IMG_PATH_ABS;
                    }
                }
            }
            return true;
        }

        // CONFIGURAZIONE ok ----------------------------------------------------------------

        #region Finestra Configurazione/semaforo

        private void MostraFinestraConfig()
        {
            fConfig = new FWSConfig();
            fConfig.ConfiguraLettore += new ehConfiguraLettore(OnConfiguraLettore);
            fConfig.SalvaConfigurazioneLettore += new ehSalvaConfigurazioneLettore(OnSalvaConfigurazioneLettore);
            fConfig.ConfiguraSemaforo += new ehConfiguraSemaforo(OnConfiguraSemaforo);
            fConfig.StatoSemaforo += new ehStatoSemaforo(OnStatoSemaforo);

            fConfig.Configura();
            fConfig.ShowDialog();
            fConfig = null;
 
            // aggiorna il componente (lo faccio comunque)
            CfgLettore(VTConfig.UsaLettore, VTConfig.PortaLettore);
            OnConfiguraSemaforo(this, VTConfig.UsaSemaforo,
                VTConfig.IP_Com_Semaforo, VTConfig.TipoSemaforo);

            // metto il semaforo libero
            oSemaforo.SemaforoLibero();
        }

        public void OnConfiguraLettore(object sender, bool AUsaLettore, int AComPort)
        {
            // aggiorna il componente
            CfgLettore(AUsaLettore, AComPort);
        }

        public void OnSalvaConfigurazioneLettore(object sender, bool AUsaLettore, int AComPort,
                string ASemComPort, bool AUsaSemaforo)
        {
            // aggiorna le variabili
            VTConfig.UsaLettore = AUsaLettore;
            VTConfig.PortaLettore = AComPort;
            if (VTConfig.TipoSemaforo == VSDecl.SEMAFORO_COM)
            {
                VTConfig.UsaSemaforo = AUsaSemaforo;
                VTConfig.IP_Com_Semaforo = ASemComPort;
            }
            // salva la configurazione sul database
            if (oDBDati.SalvaConfigurazione() == 1)
                MessageBox.Show("Configurazione salvata sul database", "Information",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            //// aggiorna il componente (non serve)
            //CfgLettore(AUsaLettore, AComPort);
        }

        public void CfgLettore(bool EUsaLettore, int EComPort)
        {
            // aggiorna il componente
            if (EUsaLettore)
            {
                NewReader.Close();
                NewReader.PortName = "COM" + EComPort.ToString();
                NewReader.Open();
            }
            else
            {
                NewReader.Close();
                NewReader.PortName = "COM" + EComPort.ToString();
            }

        }

        public void OnConfiguraSemaforo(object sender, bool AUsaSemaforo, 
            string AComPort, int ATipoSemaforo)
        {
            // cambia port semaforo
            oSemaforo.AttivaSemaforo(false);
            if (AUsaSemaforo)
            {
                oSemaforo.ConnAddress = AComPort;
                oSemaforo.AttivaSemaforo(true);
                oSemaforo.SemaforoLibero();
            }
        }

        public void OnStatoSemaforo(object sender, TStatoSemaforo AStato)
        {
            // ribatto il comando
            switch (AStato)
            {
                case TStatoSemaforo.stsOccupato:
                    oSemaforo.SemaforoOccupato();
                    break;
                case TStatoSemaforo.stsLibero:
                    oSemaforo.SemaforoLibero();
                    break;
                case TStatoSemaforo.stsErrore:
                    oSemaforo.SemaforoErrore();
                    break;
                case TStatoSemaforo.stsFineoccupato:
                    oSemaforo.SemaforoFineOccupato();
                    break;
            }
        }

        #endregion

        //  varie ----------------------------------------------------------------

        #region Varie

        private void SemaforoOKImg(bool bok)
        {
            ShowNoSemaph = !bok;
        }
        
        private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // Ctrl + Q : USCITA
                case Key.Q when Keyboard.Modifiers == ModifierKeys.Control:
                case Key.Q when Keyboard.Modifiers == ModifierKeys.Alt:
                {
                    e.Handled = true;
                    if (MessageBox.Show(App.Instance.getLang("SAPP_CLOSE"), "Question",
                            MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                        Application.Current.Shutdown();
                    break;
                }

                // Ctrl + S : Configurazione
                case Key.S when Keyboard.Modifiers == ModifierKeys.Control:
                {
                    e.Handled = true;
                    if (Stato == TAppStato.ssvBadge) MostraFinestraConfig();
                    break;
                }

                // Stato
                case Key.S when Keyboard.Modifiers == ModifierKeys.Alt:
                case Key.W when Keyboard.Modifiers == ModifierKeys.Control:
                    e.Handled = true;
                    MostraPannelloStato(true, false);
                    break;

                // Stato Azionista
                case Key.A when Keyboard.Modifiers == ModifierKeys.Alt:
                    MostraPannelloStato(false, true);
                    break;

                // Ctrl + 1 Massimizza la finestra
                case Key.F1 when Keyboard.Modifiers == ModifierKeys.Control:
                    e.Handled = true;
                    this.WindowState = WindowState.Maximized;
                    break;

                // Ctrl + F2 Va sul secondo schermo
                case Key.F2 when Keyboard.Modifiers == ModifierKeys.Control:
                    e.Handled = true;
                    if (Screen.AllScreens.Count() > 1)
                    {
                        this.WindowState = WindowState.Normal;
                        List<Screen> Screens = new List<Screen>(Screen.AllScreens);
                        Screen screen = Screens[1];
                        Left = screen.Bounds.Left;
                        Top = screen.Bounds.Top;
                        Width = screen.Bounds.Width;
                        Height = screen.Bounds.Height;
                    }
                    break;

                // Ctrl + F8 mette la risoluzione a 1280*1024
                case Key.F8 when Keyboard.Modifiers == ModifierKeys.Control:
                    e.Handled = true;
                    this.WindowState = WindowState.Normal;
                    this.Width = 1280;
                    this.Height = 1024;
                    break;

                // Ctrl + F9 mette la risoluzione a 1024*768
                case Key.F9 when Keyboard.Modifiers == ModifierKeys.Control:
                    e.Handled = true;
                    this.WindowState = WindowState.Normal;
                    this.Width = 1024;
                    this.Height = 768;
                    break;

            }
            //// Unità di test programma
            //if (e.Key == Key.T && Keyboard.Modifiers == ModifierKeys.Alt)
            //{
            //    e.Handled = true;
            //    FTest formTest = new FTest(oDBDati, this);
            //    formTest.ShowDialog();
            //    formTest = null;
            //}
        }

        private void MostraPannelloStato(bool ShowVotazioni, bool ShowAzionista)
        {
            // prima testo se c'è
            UStatusPanel stp = (UStatusPanel) this.mainGrid.FindName("statusPanel");
            if (stp != null)
            {
                stp.Visibility = Visibility.Hidden;
                stp = null;
            }
            // ora lo creo
            UStatusPanel statusPanel = new UStatusPanel(ShowVotazioni ? Votazioni : null, 
                ShowAzionista ? Azionisti : null, Stato, oDBDati)
            {
                Name = "statusPanel",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0,20,20,0),
                Visibility = Visibility.Visible
            };
            mainGrid.Children.Add(statusPanel);
            mainGrid.RegisterName(statusPanel.Name, statusPanel);
        }

        public void onTouchWatchDog(object source, int VParam)
        {
            Logging.WriteToLog("     >> Touch Watchdog intervenuto");
        }

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


        private void MainWindow_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!VTConfig.IsDebugMode) return;
            TextBlock lblMouse = (TextBlock) this.mainGrid.FindName("lblMouse");
            if (lblMouse == null) return;
            Point dd = e.GetPosition(this);
            lblMouse.Text = "ScreenActual: " + this.ActualWidth + " / " + (int)this.ActualHeight +
                            "Mouse: " + (int)dd.X + " / " + (int)dd.Y;
        }

        #endregion

        //  property of the UI  ---------------------------------------------------------------------------------

        #region property of the UI

        private bool _ShowNoBarcode;  
        public bool ShowNoBarcode
        {
            get => _ShowNoBarcode;
            set
            {
                _ShowNoBarcode = value;
                OnPropertyChanged("ShowNoBarcode");
            }
        }
      
        private bool _ShowNoSemaph;  
        public bool ShowNoSemaph
        {
            get => _ShowNoSemaph;
            set
            {
                _ShowNoSemaph = value;
                OnPropertyChanged("ShowNoSemaph");
            }
        }

        // labels
        private string _TxtNomeDisgiunto;  
        public string TxtNomeDisgiunto
        {
            get => _TxtNomeDisgiunto;
            set
            {
                _TxtNomeDisgiunto = value;
                OnPropertyChanged("TxtNomeDisgiunto");
            }
        }

        private bool _TxtNomeDisgiuntoVis;  
        public bool TxtNomeDisgiuntoVis
        {
            get => _TxtNomeDisgiuntoVis;
            set
            {
                _TxtNomeDisgiuntoVis = value;
                OnPropertyChanged("TxtNomeDisgiuntoVis");
            }
        }

        private string _TxtDisgiuntoRimangono;  
        public string TxtDisgiuntoRimangono
        {
            get => _TxtDisgiuntoRimangono;
            set
            {
                _TxtDisgiuntoRimangono = value;
                OnPropertyChanged("TxtDisgiuntoRimangono");
            }
        }

        private bool _TxtDisgiuntoRimangonoVis;  
        public bool TxtDisgiuntoRimangonoVis
        {
            get => _TxtDisgiuntoRimangonoVis;
            set
            {
                _TxtDisgiuntoRimangonoVis = value;
                OnPropertyChanged("TxtDisgiuntoRimangonoVis");
            }
        }

        private string _TxtDirittiStart;  
        public string TxtDirittiStart
        {
            get => _TxtDirittiStart;
            set
            {
                _TxtDirittiStart = value;
                OnPropertyChanged("TxtDirittiStart");
            }
        }

        private bool _TxtDirittiStartVis;  
        public bool TxtDirittiStartVis
        {
            get => _TxtDirittiStartVis;
            set
            {
                _TxtDirittiStartVis = value;
                OnPropertyChanged("TxtDirittiStartVis");
            }
        }

        private string _TxtDirittiStartMin;  
        public string TxtDirittiStartMin
        {
            get => _TxtDirittiStartMin;
            set
            {
                _TxtDirittiStartMin = value;
                OnPropertyChanged("TxtDirittiStartMin");
            }
        }

        private bool _TxtDirittiStartMinVis;  
        public bool TxtDirittiStartMinVis
        {
            get => _TxtDirittiStartMinVis;
            set
            {
                _TxtDirittiStartMinVis = value;
                OnPropertyChanged("TxtDirittiStartMinVis");
            }
        }

        private string _TxtDirittiDiVoto;  
        public string TxtDirittiDiVoto
        {
            get => _TxtDirittiDiVoto;
            set
            {
                _TxtDirittiDiVoto = value;
                OnPropertyChanged("TxtDirittiDiVoto");
            }
        }

        private bool _TxtDirittiDiVotoVis;  
        public bool TxtDirittiDiVotoVis
        {
            get => _TxtDirittiDiVotoVis;
            set
            {
                _TxtDirittiDiVotoVis = value;
                OnPropertyChanged("TxtDirittiDiVotoVis");
            }
        }

        private string _TxtConferma;  
        public string TxtConferma
        {
            get => _TxtConferma;
            set
            {
                _TxtConferma = value;
                OnPropertyChanged("TxtConferma");
            }
        }

        private bool _TxtConfermaVis;  
        public bool TxtConfermaVis
        {
            get => _TxtConfermaVis;
            set
            {
                _TxtConfermaVis = value;
                OnPropertyChanged("TxtConfermaVis");
            }
        }

        private string _TxtConfermaUp;  
        public string TxtConfermaUp
        {
            get => _TxtConfermaUp;
            set
            {
                _TxtConfermaUp = value;
                OnPropertyChanged("TxtConfermaUp");
            }
        }

        private bool _TxtConfermaUpVis;  
        public bool TxtConfermaUpVis
        {
            get => _TxtConfermaUpVis;
            set
            {
                _TxtConfermaUpVis = value;
                OnPropertyChanged("TxtConfermaUpVis");
            }
        }

        private string _TxtConfermaNVoti;  
        public string TxtConfermaNVoti
        {
            get => _TxtConfermaNVoti;
            set
            {
                _TxtConfermaNVoti = value;
                OnPropertyChanged("TxtConfermaNVoti");
            }
        }

        private bool _TxtConfermaNVotiVis;  
        public bool TxtConfermaNVotiVis
        {
            get => _TxtConfermaNVotiVis;
            set
            {
                _TxtConfermaNVotiVis = value;
                OnPropertyChanged("TxtConfermaNVotiVis");
            }
        }

        private string _TxtNomeAzStart;  
        public string TxtNomeAzStart
        {
            get => _TxtNomeAzStart;
            set
            {
                _TxtNomeAzStart = value;
                OnPropertyChanged("TxtNomeAzStart");
            }
        }

        private bool _TxtNomeAzStartVis;  
        public bool TxtNomeAzStartVis
        {
            get => _TxtNomeAzStartVis;
            set
            {
                _TxtNomeAzStartVis = value;
                OnPropertyChanged("TxtNomeAzStartVis");
            }
        }

        private string _TxtNSelezioni;  
        public string TxtNSelezioni
        {
            get => _TxtNSelezioni;
            set
            {
                _TxtNSelezioni = value;
                OnPropertyChanged("TxtNSelezioni");
            }
        }

        private bool _TxtNSelezioniVis;  
        public bool TxtNSelezioniVis
        {
            get => _TxtNSelezioniVis;
            set
            {
                _TxtNSelezioniVis = value;
                OnPropertyChanged("TxtNSelezioniVis");
            }
        }

        private string _TxtCandidati_PresCDA;  
        public string TxtCandidati_PresCDA
        {
            get => _TxtCandidati_PresCDA;
            set
            {
                _TxtCandidati_PresCDA = value;
                OnPropertyChanged("TxtCandidati_PresCDA");
            }
        }

        private bool _TxtCandidati_PresCDAVis;  
        public bool TxtCandidati_PresCDAVis
        {
            get => _TxtCandidati_PresCDAVis;
            set
            {
                _TxtCandidati_PresCDAVis = value;
                OnPropertyChanged("TxtCandidati_PresCDAVis");
            }
        }

        private string _TxtCandidati_Altern;  
        public string TxtCandidati_Altern
        {
            get => _TxtCandidati_Altern;
            set
            {
                _TxtCandidati_Altern = value;
                OnPropertyChanged("TxtCandidati_Altern");
            }
        }

        private bool _TxtCandidati_AlternVis;  
        public bool TxtCandidati_AlternVis
        {
            get => _TxtCandidati_AlternVis;
            set
            {
                _TxtCandidati_AlternVis = value;
                OnPropertyChanged("TxtCandidati_AlternVis");
            }
        }

        #endregion

        //  inotify  ---------------------------------------------------------------------------------

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
