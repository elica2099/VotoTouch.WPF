using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;


namespace VotoTouch.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for UStatusPanel.xaml
    /// </summary>
    public partial class UStatusPanel : UserControl
    {
        private readonly TAppStato Stato;
        private readonly CVotoBaseDati oDBDati;
        private readonly TListaVotazioni Votazioni;
        
        public UStatusPanel(TListaVotazioni AVotaz, TListaAzionisti AAzion, TAppStato AStato, CVotoBaseDati AoDBDati)
        {
            InitializeComponent();

            Stato = AStato;
            oDBDati = AoDBDati;
            Votazioni = AVotaz;
            //carica la lista  ICM_MAIN_CLOSESTATUSPANEL
            CaricaListaStato(AVotaz, AAzion);
        }

        public void CaricaListaStato(TListaVotazioni AVotaz, TListaAzionisti AAzion)
        {
            // inizio con il caricare lo stato
            ObservableCollection<string> stato = new ObservableCollection<string>
            {
                VSDecl.VTS_VERSION,
#if _DBClose
                "DBClose version",
#endif
                "Configurazione",
                "Usalettore: " + VTConfig.UsaLettore.ToString() + " Porta: " + VTConfig.PortaLettore.ToString(),
                "UsaSemaforo: " + VTConfig.UsaSemaforo.ToString() + " IP: " + VTConfig.IP_Com_Semaforo.ToString(),
                "IDSeggio: " + VTConfig.IDSeggio.ToString() + " NomeComputer: " + VTConfig.NomeTotem,
                "ControllaPresenze: " + VTConfig.ControllaPresenze.ToString(),
                "     0: Non controllare, 1: Blocca",
                "     2: Forza Ingresso (ora) 3: Forza ingresso Geas",
                "MaxDeleghe: " + VTConfig.MaxDeleghe,
                "AbilitaDifferenziataSuRichiesta: " + VTConfig.AbilitaDifferenziatoSuRichiesta.ToString(),
                "CodiceUscita: " + VTConfig.CodiceUscita,
                "SalvaLinkVoto: " + VTConfig.SalvaLinkVoto.ToString(),
                "SalvaVotoNonConfermato: " + VTConfig.SalvaVotoNonConfermato.ToString(),
                "SalvaVoto In geas: " + VTConfig.SalvaVotoInGeas.ToString(),
                "IDSchedaUscitaForzata: " + VTConfig.IDSchedaUscitaForzata.ToString(),
                "AbilitaDirittiNonVoglioVotare: " + VTConfig.AbilitaDirittiNonVoglioVotare.ToString(),
                "TimerAutoritorno: " + VTConfig.AttivaAutoRitornoVoto.ToString(),
                "Tempo TimerAutoritorno (ms): " + VTConfig.TimeAutoRitornoVoto.ToString(),
                ""
            };
            if (AVotaz != null)
            {
                // ok ora le votazioni
                stato.Add("=== Votazioni ===");
                foreach (TNewVotazione fVoto in AVotaz.Votazioni)
                {
                    stato.Add("Voto: " + fVoto.IDVoto.ToString() + ", Tipo: " +
                              fVoto.TipoVoto.ToString() + ", " + fVoto.Descrizione);
                    stato.Add("   NListe: " + fVoto.NListe + ", MaxScelte: " +
                              fVoto.MaxScelte);
                    stato.Add("   SKBianca: " + fVoto.SkBianca.ToString() +
                              ", SKNonVoto: " + fVoto.SkNonVoto);
                    // Le liste
                    foreach (TNewLista a in fVoto.Liste)
                    {
                        stato.Add("    Lista:" + a.IDLista.ToString() + ", IdSk:" +
                                  a.IDScheda.ToString() + ", " + a.DescrLista + ", p" +
                                  a.Pag.ToString() + " " + a.PagInd + "  cda: " + a.PresentatodaCDA.ToString());
                    }
                }
                stato.Add("");
            }

            // ora gli azionisti
            if (AAzion != null)
            {
                stato.Add("=== Azionisti ===");
                foreach (TAzionista c in AAzion.Azionisti)
                {
                    stato.Add("Badge: " + c.IDBadge.ToString() + " " + c.RaSo.Trim());
                    stato.Add("   IDazion:" + c.IDAzion.ToString() + " *** IDVotaz: " + c.IDVotaz.ToString());
                    stato.Add("   ProgDeleg:" + c.ProgDeleg.ToString() + " Coaz:" + c.CoAz +
                              " AzOrd: " + c.NVoti.ToString());
                }
            }
            // bindo
            listBox.ItemsSource = stato;
        }

        private void CancellaVoti_OnClick(object sender, RoutedEventArgs e)
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

        private void RicaricaListe_OnClick(object sender, RoutedEventArgs e)
        {
            // ricarico le liste
            if (Stato == TAppStato.ssvVotoStart)
            {
                if (MessageBox.Show("Questa operazione ricaricherà le liste/votazioni rileggendole " +
                                    "dal database?\n Vuoi veramente continuare?", "Question",
                        MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    Rect FFormRect = new Rect(0, 0, this.Width, this.Height);
                    bool pippo = Votazioni.CaricaListeVotazioni(VTConfig.Data_Path, FFormRect, false);
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

        private void Test_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ChiudiApp_OnClick(object sender, RoutedEventArgs e)
        {
            // chiudo l'applicazione
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

        private void ChiudiPannello_OnClick(object sender, RoutedEventArgs e)
        {
            App.ICMsn.NotifyColleaguesSync(VSDecl.ICM_MAIN_CLOSESTATUSPANEL, null);
        }


        //private void button2_Click(object sender, EventArgs e)
        //{
        //    //StartTest();
        //    ////TListaAzionisti azio = new TListaAzionisti(oDBDati);
        //    ////azio.CaricaDirittidiVotoDaDatabase(10005, ref fVoto, NVoti);
        //    ////List<TAzionista> aziofilt = azio.DammiDirittiDiVotoPerIDVotazione(1, true);
        //    //TListaVotazioni vot = new TListaVotazioni(oDBDati);
        //    //vot.CaricaListeVotazioni();
        //}


    }
}
