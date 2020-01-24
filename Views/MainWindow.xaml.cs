﻿using System;
using System.Collections;
using System.Collections.Generic;
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
    public partial class MainWindow : Window, IInterClassMessenger
    {
        public delegate void EventDataReceived(object source, string messaggio);
        public event EventDataReceived evtDataReceived;

        // timer di disaccoppiamento
        private DispatcherTimer timLetturaBadge;
        private DispatcherTimer timCambiaStato;
        private DispatcherTimer timConfigura;
        private DispatcherTimer timAutoRitorno;
        private DispatcherTimer timPopup;
        private DispatcherTimer timVotoAperto;

        // oggetti demo
        private Button btnBadgeUnVoto;
        private Button btnBadgePiuVoti;
        private Button btnFineVotoDemo;

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
            #endregion

            // data_path
            CheckDataFolder();
            // variabili speciali demo/debug....
            VTConfig.IsDebugMode = File.Exists(VTConfig.Data_Path + "VTS_DEBUG.txt");
            VTConfig.IsPaintTouch = File.Exists(VTConfig.Data_Path + "VTS_PAINT_TOUCH.txt");
            VTConfig.IsDemoMode = File.Exists(VTConfig.Data_Path + "VTS_DEMO.txt");
            VTConfig.IsAdmin = File.Exists(VTConfig.Data_Path + "VTS_ADMIN.txt");
            VTConfig.IsStandalone = File.Exists(VTConfig.Data_Path + "VTS_STANDALONE.txt");

            // resize
            this.SizeChanged += OnWindowSizeChanged;

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
            // inizializzazione classe del tema
            oVotoTheme = new CVotoTheme();
            oVotoTheme.CaricaTemaDaXML(VTConfig.Img_Path);

            // creazione timer di disaccoppiamento funzioni
            // TODO: METTERE I MESSAGGI
            // timer di lettura badge
            timVotoAperto = new DispatcherTimer {IsEnabled = false, Interval = TimeSpan.FromMilliseconds(VSDecl.TIM_CKVOTO_MIN)};
            timVotoAperto.Tick += timVotoAperto_Tick;
            // timer di lettura badge
            timLetturaBadge = new DispatcherTimer {IsEnabled = false, Interval = TimeSpan.FromMilliseconds(30)};
            timLetturaBadge.Tick += timLetturaBadge_Tick;
            // timer di cambio stato
            timCambiaStato = new DispatcherTimer {IsEnabled = false, Interval = TimeSpan.FromMilliseconds(30)};
            timCambiaStato.Tick += timCambiaStato_Tick;
            // timer di configurazione
            timConfigura = new DispatcherTimer {IsEnabled = false, Interval = TimeSpan.FromMilliseconds(30)};
            timConfigura.Tick += timConfigura_Tick;
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
			InizializzaControlli();
            // Se è in demo mode metto i controlli
            if (VTConfig.IsDemoMode)
                InizializzaControlliDemo();

			// ora inizializzo la macchina a stati
			Stato = TAppStato.ssvBadge;

            // se sono in debug evidenzio le zone sensibili
            oVotoTouch.PaintTouchOnScreen = VTConfig.IsPaintTouch;

            // se la votazione è aperta il timer di controllo voto batte di meno
            timVotoAperto.Interval = TimeSpan.FromMilliseconds(VTConfig.VotoAperto ? VSDecl.TIM_CKVOTO_MAX : VSDecl.TIM_CKVOTO_MIN);

            // Attivo la macchina a stati (in FMain_MacchinaAStati.cs)
            CambiaStato();

            timVotoAperto.Start();

            // da qui in avanti era in shown
            // attivo il barcode reader
            NewReader = new CNETActiveReader();
            NewReader.ADataRead += ObjDataReceived;
            evtDataReceived += new EventDataReceived(onDataReceived);
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
                    imgERBarcode.Visible = true;
                }
                else
                    imgERSemaf.Visible = false;
            }
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
            lblMsgPopup.Text = messaggio;
            pnPopupRed.Visible = true;
            timPopup.Start();
        }

        // IInterClassMessenger ---------------------------------------------------------------------------------

        public void InterClassCommand(string ACommand, object AParam, object WParam, object YParam, object ZParam)
        {
            //here it comes the commands from IInterClassMessenger interface and from other classes
            switch (ACommand)
            {
                case VSDecl.ICM_MAIN_BADGEREAD:
                    string badge = (string) AParam;
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
                    oVotoTheme.PaintlabelProposteCdaAlt(sender, e, Votazioni.VotoCorrente, true);
                    // paint dei Bottoni
                    oVotoTouch.PaintButtonCandidatoPagina(sender, e, false, oVotoTheme.BaseFontCandidato, 
                        oVotoTheme.BaseFontCandidatoBold, oVotoTheme.BaseColorCandidato);
                }
                // se la votazione corrente è di MULTIcandidato su più pagine disegno i rettangoli
                if (Stato == TAppStato.ssvVoto && Votazioni.VotoCorrente.TipoVoto == VSDecl.VOTO_MULTICANDIDATO)
                {
                    // paint delle label Aggiuntive
                    oVotoTheme.PaintlabelProposteCdaAlt(sender, e, Votazioni.VotoCorrente, false);
                    oVotoTheme.PaintlabelNSelezioni(sender, e, Votazioni.VotoCorrente, false);
                    // paint dei bottoni
                    oVotoTouch.PaintButtonCandidatoPagina(sender, e, true, oVotoTheme.BaseFontCandidato,
                        oVotoTheme.BaseFontCandidatoBold, oVotoTheme.BaseColorCandidato);
                }

                // ******* OBSOLETO ********/
                // votazione VOTO_CANDIDATO_SING, candidato a singola pagina, disegno i rettangoli
                //if (Stato == TAppStato.ssvVoto && (FParVoto[CurrVoteIDX].TipoVoto == VSDecl.VOTO_CANDIDATO_SING))
                //    oVotoTouch.PaintButtonCandidatoSingola(sender, e);
                
                // se sono nello stato di votostart e il n. di voti è > 1
                if (Stato == TAppStato.ssvVotoStart) // && Azionisti.HaDirittiDiVotoMultipli())
                {
                    // faccio il paint del numero di diritti di voto nel bottone in basso a sx , 
                    // in questo caso uso un paint e non una label per un problema grafico di visibilità
                    int VVoti = VTConfig.ModoAssemblea == VSDecl.MODO_AGM_POP
                                   ? Azionisti.DammiMaxNumeroDirittiDiVotoTotali()
                                   : Azionisti.DammiMaxNumeroVotiTotali();
                    string ss = string.Format("{0:N0}", VVoti.ToString());
                    if (Azionisti.HaDirittiDiVotoMultipli())
                    {
                        ss += "(d)";
                        oVotoTheme.PaintDirittiDiVoto(sender, e, ss);
                    }
                    //oVotoTheme.PaintDirittiDiVoto(sender, e, VVoti);
                }
            }

            // se è demo devo stampare una label
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
        }
         
        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //double newWindowHeight = e.NewSize.Height;
            //double newWindowWidth = e.NewSize.Width;
            //double prevWindowHeight = e.PreviousSize.Height;
            //double prevWindowWidth = e.PreviousSize.Width;
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            // immagine del salvataggio
            pbSalvaDati.Left = (this.Width / 2) - (pbSalvaDati.Width / 2);
            pbSalvaDati.Top = (this.Height / 2) - (pbSalvaDati.Height  / 2);

            Rectangle FFormRect = new Rectangle(0, 0, this.Width, this.Height);

            // devo dire alla nuova touch le dimensioni della finestra
            //if (oVotoTouch != null)
            //{
            //    oVotoTouch.CalcolaVotoTouch(FFormRect);
            //}
            // lo stesso faccio per la classe del thema che si occupa di disegnare 
            // le label di informazione
            if (oVotoTheme != null)
            {
                oVotoTheme.FFormRect = FFormRect;
                CaricaTemaInControlli();
            }
            // ok ora le votazioni
            if (Votazioni != null)
            {
                Votazioni.ResizeZoneVotazioni(FFormRect);
                //Votazioni.CalcolaTouchZoneVotazioni(FFormRect);
            }
            
            // ok, ora se è in demo mode faccio il resize dei controlli
            if (VTConfig.IsDemoMode)
            {
                // bottone un voto
                if (btnBadgeUnVoto != null)
                {
                    btnBadgeUnVoto.Left = this.Width / 7;
                    btnBadgeUnVoto.Top = (this.Height / 10) * 6;
                    btnBadgeUnVoto.Width = (this.Width / 7) * 2;
                    btnBadgeUnVoto.Height = (this.Height / 10) *2;
                }
                // bottone più voto
                if (btnBadgePiuVoti != null)
                {
                    btnBadgePiuVoti.Left = (this.Width / 7) * 4;
                    btnBadgePiuVoti.Top = (this.Height / 10) * 6;
                    btnBadgePiuVoti.Width = (this.Width / 7) * 2;
                    btnBadgePiuVoti.Height = (this.Height / 10) * 2;
                }
                // bottone finevotodemo
                if (btnFineVotoDemo != null)
                {
                    btnFineVotoDemo.Left = (this.Width / 7) * 2;
                    btnFineVotoDemo.Top = (this.Height / 10) * 6;
                    btnFineVotoDemo.Width = (this.Width / 7) * 3;
                    btnFineVotoDemo.Height = (this.Height / 10) * 2;
                }
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

        #region Finestra Configurazione

        private void timConfigura_Tick(object sender, EventArgs e)
        {
            timConfigura.Stop();
            if (Stato == TAppStato.ssvBadge) MostraFinestraConfig();
        }

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
            imgERSemaf.Visible = !bok;
        }
        
        public Screen GetSecondaryScreen()
        {
            if (Screen.AllScreens.Length == 1)
            {
                return null;
            }

            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Primary == false)
                {
                    return screen;
                }
            }

            return null;
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
                    MostraPannelloStato();
                    break;

                // Stato Azionista
                case Key.A when Keyboard.Modifiers == ModifierKeys.Alt:
                    MostaPannelloStatoAzionista();
                    break;

                // Ctrl + 1 Massimizza la finestra
                case Key.F1 when Keyboard.Modifiers == ModifierKeys.Control:
                    e.Handled = true;
                    this.WindowState = WindowState.Maximized;
                    break;

                // Ctrl + F2 Va sul secondo schermo
                case Key.F2 when Keyboard.Modifiers == ModifierKeys.Control:
                    e.Handled = true;
                    if (Screen.AllScreens.Length > 1)
                    {
                        // Important !
                        this.StartPosition = FormStartPosition.Manual;
                        this.WindowState = FormWindowState.Normal;
                        // Get the second monitor screen
                        Screen screen = GetSecondaryScreen();
                        // set the location to the top left of the second screen
                        this.Location = screen.WorkingArea.Location;
                        // set it fullscreen
                        this.Size = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height);
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

        private void MostraPannelloStato()
        {
            TNewLista a;
            int z;
            
            //lbVersion.Visible = true;
            Panel4.Left = this.Width - Panel4.Width - 5;
            Panel4.Top = 5;
            label1.Text = "Informazioni sulla Versione;";

            lbVersion.Items.Clear();
            lbVersion.Items.Add(VSDecl.VTS_VERSION);
#if _DBClose
            lbVersion.Items.Add("DBClose version");
#endif
            lbVersion.Items.Add("Configurazione");
            lbVersion.Items.Add("Usalettore: " + VTConfig.UsaLettore.ToString() + " Porta: " + VTConfig.PortaLettore.ToString());
            lbVersion.Items.Add("UsaSemaforo: " + VTConfig.UsaSemaforo.ToString() + " IP: " + VTConfig.IP_Com_Semaforo.ToString());
            lbVersion.Items.Add("IDSeggio: " + VTConfig.IDSeggio.ToString() + " NomeComputer: " + VTConfig.NomeTotem);
            lbVersion.Items.Add("ControllaPresenze: " + VTConfig.ControllaPresenze.ToString());
            lbVersion.Items.Add("     0: Non controllare, 1: Blocca");
            lbVersion.Items.Add("     2: Forza Ingresso (ora) 3: Forza ingresso Geas");
            lbVersion.Items.Add("MaxDeleghe: " + VTConfig.MaxDeleghe);
            lbVersion.Items.Add("AbilitaDifferenziataSuRichiesta: " + VTConfig.AbilitaDifferenziatoSuRichiesta.ToString());
            lbVersion.Items.Add("CodiceUscita: " + VTConfig.CodiceUscita);
            lbVersion.Items.Add("SalvaLinkVoto: " + VTConfig.SalvaLinkVoto.ToString());
            lbVersion.Items.Add("SalvaVotoNonConfermato: " + VTConfig.SalvaVotoNonConfermato.ToString());
            lbVersion.Items.Add("SalvaVoto In geas: " + VTConfig.SalvaVotoInGeas.ToString());
            lbVersion.Items.Add("IDSchedaUscitaForzata: " + VTConfig.IDSchedaUscitaForzata.ToString());
            lbVersion.Items.Add("AbilitaDirittiNonVoglioVotare: " + VTConfig.AbilitaDirittiNonVoglioVotare.ToString());
            lbVersion.Items.Add("TimerAutoritorno: " + VTConfig.AttivaAutoRitornoVoto.ToString());
            lbVersion.Items.Add("Tempo TimerAutoritorno (ms): " + VTConfig.TimeAutoRitornoVoto.ToString());
            lbVersion.Items.Add("");
            // le votazioni
            foreach (TNewVotazione fVoto in Votazioni.Votazioni)
            {
                lbVersion.Items.Add("Voto: " + fVoto.IDVoto.ToString() + ", Tipo: " +
                    fVoto.TipoVoto.ToString() + ", " + fVoto.Descrizione);
                lbVersion.Items.Add("   NListe: " + fVoto.NListe + ", MaxScelte: " +
                    fVoto.MaxScelte);
                lbVersion.Items.Add("   SKBianca: " + fVoto.SkBianca.ToString() +
                    ", SKNonVoto: " + fVoto.SkNonVoto);
                // Le liste
                for (z = 0; z < fVoto.NListe; z++)
                {
                    a = (TNewLista)fVoto.Liste[z];
                    lbVersion.Items.Add("    Lista:" + a.IDLista.ToString() + ", IdSk:" +
                        a.IDScheda.ToString() + ", " + a.DescrLista + ", p" +
                        a.Pag.ToString() + " " + a.PagInd + "  cda: " + a.PresentatodaCDA.ToString());
                }
            }
            Panel4.Visible = true;

        }

        private void MostaPannelloStatoAzionista()
        {
            Panel4.Left = this.Width - Panel4.Width - 5;
            Panel4.Top = 5;
            label1.Text = "Informazioni sull'Azionista";

            lbVersion.Items.Clear();
            lbVersion.Items.Add(VSDecl.VTS_VERSION);
#if _DBClose
            lbVersion.Items.Add("DBClose version");
#endif
            foreach (TAzionista c in Azionisti.Azionisti)
            {
                lbVersion.Items.Add("Badge: " + c.IDBadge.ToString() + " " + c.RaSo.Trim());
                lbVersion.Items.Add("   IDazion:" + c.IDAzion.ToString() + " *** IDVotaz: " + c.IDVotaz.ToString());
                lbVersion.Items.Add("   ProgDeleg:" + c.ProgDeleg.ToString() + " Coaz:" + c.CoAz +
                            " AzOrd: " + c.NVoti.ToString());

            }
            Panel4.Visible = true;
        }

        public void onTouchWatchDog(object source, int VParam)
        {
            Logging.WriteToLog("     >> Touch Watchdog intervenuto");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

             // ricarico le liste
            if (Stato == TAppStato.ssvVotoStart)
            {
                if (MessageBox.Show(App.Instance.getLang("SAPP_CLOSE"), "Question",
                     MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    Application.Current.Shutdown();
            }
            else
                MessageBox.Show(App.Instance.getLang("SAPP_CLOSE_ERR"), "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnRicaricaListe_Click(object sender, EventArgs e)
        {
            // ricarico le liste
            if (Stato == TAppStato.ssvVotoStart)
            {
                if (MessageBox.Show("Questa operazione ricaricherà le liste/votazioni rileggendole " +
                    "dal database?\n Vuoi veramente continuare?", "Question",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    Rect FFormRect = new Rect(0, 0, this.Width, this.Height);
                    bool pippo = Votazioni.CaricaListeVotazioni(Data_Path, FFormRect, false);
                    if (pippo)
                        MessageBox.Show("Liste/votazioni caricate correttamente.", "information",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Problemi nel caricamento Liste/votazioni.", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);

                }

            }
            else
                MessageBox.Show("Impossibile effettuare questa operazione durante la votazione.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnCloseInfo_Click(object sender, EventArgs e)
        {
            Panel4.Visible = false;
        }

        private void btnCancVoti_Click(object sender, EventArgs e)
        {
#if DEBUG
            if (MessageBox.Show("Questa operazione cancellerà TUTTI i voti " +
                "dal database?\n Vuoi veramente continuare?", "Question",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                oDBDati.CancellaTuttiVoti();
            }
#else
            MessageBox.Show("Funzione non disponibile", "Exclamation",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#endif
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

        #endregion

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    //StartTest();
        //    ////TListaAzionisti azio = new TListaAzionisti(oDBDati);
        //    ////azio.CaricaDirittidiVotoDaDatabase(10005, ref fVoto, NVoti);
        //    ////List<TAzionista> aziofilt = azio.DammiDirittiDiVotoPerIDVotazione(1, true);
        //    //TListaVotazioni vot = new TListaVotazioni(oDBDati);
        //    //vot.CaricaListeVotazioni();
        //}

        private void MainWindow_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!VTConfig.IsDebugMode) return;
            TextBlock lblMouse = (TextBlock) this.mainGrid.FindName("lblMouse");
            if (lblMouse == null) return;
            Point dd = e.GetPosition(this);
            lblMouse.Text = "ScreenActual: " + this.ActualWidth + " / " + (int)this.ActualHeight +
                            "Mouse: " + (int)dd.X + " / " + (int)dd.Y;
        }

//        private void frmMain_MouseMove(object sender, MouseEventArgs e)
//        {
//#if DEBUG
//            float vx = (VSDecl.VOTESCREEN_DIVIDE_WIDTH / this.Width) * Cursor.Position.X;
//            float vy = (VSDecl.VOTESCREEN_DIVIDE_HEIGHT / this.Height) * Cursor.Position.Y;
//            float lx = (100 / this.Width) * Cursor.Position.X;
//            float ly = (100 / this.Height) * Cursor.Position.Y;

//            labelMousee.Text = "Local: " + e.X + ", " + e.Y + ". \n"
//               + "Vote: " + (int)vx + ", " + (int)vy + ". \n"
//                +"Label: " + (int)lx + ", " + (int)ly + ". \n"
//               + "Globalis " + Cursor.Position.X + ", " + Cursor.Position.Y + ".";
//#endif
//        }

    }
}
