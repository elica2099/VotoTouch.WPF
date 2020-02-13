using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VotoTouch.WPF.Models;
using VotoTouch.WPF.Views.Tools;

namespace VotoTouch.WPF
{
    // EVENTI_TOUCHSCREEN

    public partial class MainWindow : Window
    {
        // DR16 - Classe intera

        //	GESTIONE DELL'AREA SENSIBILE ----------------------------------------------------------

        private void MainWindow_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // non so se serve
        }

        private void MainWindow_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // non so se serve
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            // verifica e crea l'adorner dell'area premuta
        }

        private void MainWindow_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            // chiamo il metodo in CTouchscreen che mi ritornerà eventi diversi a seconda del caso
            Point currentPoint = e.GetPosition(this);
            oVotoTouch.TastoPremuto(currentPoint);
        }

        //	EVENTI DI PRESSIONE SCHERMO DA CTOUCHSCREEN ----------------------------------------------------------------

        #region Eventi pressione schermo da CTouchscreen

        // ATTENZIONE, mettere una modifica di un qualsiasi controllo in queste routine, 
        // può causare refresh e paint indesiderati (caso a.Text) fare attenzione

        public void onPremutoTab(object source, int VParam)
        {
            // arriva l'evento cambio pagina (ora lo faccio all'interno di Ctouchscreen)
            //oVotoTouch.CurrPag = VParam;
            // ricalcolo, non serve più con la nuova gestione pagine
            //oVotoTouch.CalcolaTouch(this, Stato, ref FParVoto[CurrVoteIDX], DatiUsr.utente_voti > 1);
            // devo ridisegnare tutto, qui lo voglio
            this.InvalidateVisual();
        }

        public void onPremutoVotaNormale(object source, int VParam)
        {
            // ok, questo evento arriva all'inizio votazione quando è stato premuto l'avvio del voto normale
            // o nel caso di scelta differenziato/normale, evidenzia il voto in un unica soluzione
            IsVotazioneDifferenziata = false;
            Logging.WriteToLog("Voto normale");
            Stato = TAppStato.ssvVoto;
            CambiaStato();
        }

        public void onPremutoVotaDifferenziato(object source, int VParam)
        {
            // prima di tutto devo verificare se è abilitato il flag 
            // AbilitaDifferenziatoSuRichiesta e se LocalAbilitaVotazDifferenziataSuRichiesta è false, in tal caso esco
            if (VTConfig.AbilitaDifferenziatoSuRichiesta && !LocalAbilitaVotazDifferenziataSuRichiesta)
                return;
            // ok, questo evento arriva all'inizio votazione 
            // nel caso di scelta differenziato/normale, evidenzia il voto in soluzioni separate
            IsVotazioneDifferenziata = true;
            Logging.WriteToLog("Voto differenziato");
            Stato = TAppStato.ssvVoto;
            CambiaStato();
        }

        public void onPremutoVotoValido(object source, int VParam, bool ZParam)
        {
            // TODO: Usare IdScheda invece di indice in VParam

            // ok, questo evento arriva quando, nella selezione del voto, è stata
            // premuta una zona valida devo veder in funzione della lista selezionata
            TLista a;
            CVotoEspresso VExp;
            // verifico se è null
            if (Votazioni.VotoCorrente.Liste == null) return;

            // questo controllo dell'indice è inutile, però è meglio farlo,
            // in caso di problemi, indici scassati, mette una scheda bianca
            int ct = Votazioni.VotoCorrente.Liste.Count;
            if (VParam >= 0 && VParam < ct)
            {
                a = Votazioni.VotoCorrente.Liste[VParam];
                VotoEspresso = a.IDScheda;
                VotoEspressoStr = a.ListaElenco;
                VotoEspressoStrUp = a.DescrLista;
                VotoEspressoStrNote = a.Presentatore;
                // da aggiungere successivamente:
                VExp = new CVotoEspresso
                    {
                        NumVotaz = a.NumVotaz,
                        VotoExp_IDScheda = a.IDScheda,
                        TipoCarica = a.TipoCarica,
                    };
                FVotiExpr.Add(VExp);
            }
            else
            {
                // se succede qualcosa di strano mette sk bianca
                Logging.WriteToLog("<error> onPremutoVotoValido Indice voto non valido");
                VotoEspresso = VSDecl.VOTO_SCHEDABIANCA;
                VotoEspressoStr = "";
                VotoEspressoStrUp = App.Instance.getLang("SAPP_SKBIANCA");      // "Scheda Bianca";
                VotoEspressoStrNote = "";
                VExp = new CVotoEspresso
                {
                    NumVotaz = Votazioni.VotoCorrente.NumVotaz,
                    VotoExp_IDScheda = VSDecl.VOTO_SCHEDABIANCA,
                    TipoCarica = 0,
                };
                FVotiExpr.Add(VExp);
            }
            // a questo punto vado in conferma con la stessa CurrVote
            Stato = TAppStato.ssvVotoConferma;
            CambiaStato();
        }

        public void onPremutoGruppoAvanti(object source, int VParam)
        {
            // prende il voto gruppo
            if (!Votazioni.VotoCorrente.HaUserControl) return;
            // devo prendere i voti dall'usercontrol
            List<CVotoEspresso> votiE = Votazioni.VotoCorrente.UserControlVoto.GetVotes();
            // li copio
            VotoEspresso = VSDecl.VOTO_GRUPPOAVANTI;
            VotoEspressoStr = "";
            VotoEspressoStrUp = "";     
            foreach (CVotoEspresso espresso in votiE)
            {
                FVotiExpr.Add(espresso);
            }
            // la descrizione
            List<string> votiS = Votazioni.VotoCorrente.UserControlVoto.GetVotesDescr();
            foreach (string s in votiS)
            {
                VotoEspressoStr += s + ";";
            }
            // a questo punto vado in conferma con la stessa CurrVote
            Stato = TAppStato.ssvVotoConferma;
            CambiaStato();
        }

        // ORA E' MULTICANDIDATO, MA DIVENTERA' STANDARD
        public void onPremutoMultiAvanti(object source, int VParam, ref List<int> voti)
        {
            // in realtà corrisponde all'AVANTI
            if (voti == null) return;  // in teoria non serve
            int ct = Votazioni.VotoCorrente.Liste.Count;
            VotoEspressoStr = "";
            VotoEspressoStrUp = "";      // "Scheda Bianca";
            // ok, ora riempio la collection di voti
            for (int i = 0; i < voti.Count; i++)
            {
                if (voti[i] >= 0 && voti[i] < ct)
                {
                    TLista a = Votazioni.VotoCorrente.Liste[voti[i]];
                    CVotoEspresso vt = new CVotoEspresso
                    {
                        NumVotaz = a.NumVotaz,
                        TipoCarica = a.TipoCarica,
                        VotoExp_IDScheda = a.IDScheda,
                    };
                    FVotiExpr.Add(vt);
                    // ora aggiungo il candidato
                    VotoEspressoStr += a.ListaElenco + ";";
                    VotoEspressoStrUp += a.DescrLista + ";";
                    VotoEspressoStrNote = "";
                }
            }
            // a questo punto vado in conferma con la stessa CurrVote
            Stato = TAppStato.ssvVotoConferma;
            CambiaStato();
        }

        public void onPremutoVotoMulti(object source, int VParam)
        {
            // mi serve per il repaint per settare o meno i tasti verdi
            this.InvalidateVisual();
        }

        public void onPremutoSchedaBianca(object source, int VParam)
        {
            // scheda bianca
            VotoEspresso = VSDecl.VOTO_SCHEDABIANCA;
            VotoEspressoStr = "";
            VotoEspressoStrUp = App.Instance.getLang("SAPP_SKBIANCA");      // "Scheda Bianca";
            VotoEspressoStrNote = "";
            // nuova versione array
            CVotoEspresso VExp = new CVotoEspresso
                {
                    NumVotaz = Votazioni.VotoCorrente.NumVotaz,
                    VotoExp_IDScheda = VSDecl.VOTO_SCHEDABIANCA,
                    TipoCarica = 0,
                };
            FVotiExpr.Add(VExp);
            // a questo punto vado in conferma con la stessa CurrVote
            Stato = TAppStato.ssvVotoConferma;
            CambiaStato();
        }

        public void onPremutoContrarioTutti(object source, int VParam)
        {
            // ContrarioATutti
            VotoEspresso = VSDecl.VOTO_CONTRARIO_TUTTI;
            VotoEspressoStr = "";
            VotoEspressoStrUp = VTConfig.ContrarioATutti; // rm.GetString("SAPP_SKCONTRARIOTUTTI");
            VotoEspressoStrNote = "";
            // nuova versione array
            CVotoEspresso VExp = new CVotoEspresso
            {
                NumVotaz = Votazioni.VotoCorrente.NumVotaz,
                VotoExp_IDScheda = VSDecl.VOTO_CONTRARIO_TUTTI,
                TipoCarica = 0,
            };
            FVotiExpr.Add(VExp);
            // a questo punto vado in conferma con la stessa CurrVote
            Stato = TAppStato.ssvVotoConferma;
            CambiaStato();
        }

        public void onPremutoAstenutoTutti(object source, int VParam)
        {
            // Astenuto Tutti
            VotoEspresso = VSDecl.VOTO_ASTENUTO_TUTTI;
            VotoEspressoStr = "";
            VotoEspressoStrUp = VTConfig.AstenutoATutti; // rm.GetString("SAPP_SKASTENUTOTUTTI");
            VotoEspressoStrNote = "";
            // nuova versione array
            CVotoEspresso VExp = new CVotoEspresso
            {
                NumVotaz = Votazioni.VotoCorrente.NumVotaz,
                VotoExp_IDScheda = VSDecl.VOTO_ASTENUTO_TUTTI,
                TipoCarica = 0,
            };
            FVotiExpr.Add(VExp);
            // a questo punto vado in conferma con la stessa CurrVote
            Stato = TAppStato.ssvVotoConferma;
            CambiaStato();
        }

        public void onPremutoNonVoto(object source, int VParam)
        {
            // non votante
            VotoEspresso = VSDecl.VOTO_NONVOTO;
            VotoEspressoStr = "";
            VotoEspressoStrUp = App.Instance.getLang("SAPP_NOVOTO");      // "Non Voglio Votare";
            VotoEspressoStrNote = "";
            // nuova versione array
            CVotoEspresso VExp = new CVotoEspresso
                {
                    NumVotaz = Votazioni.VotoCorrente.NumVotaz,
                    VotoExp_IDScheda = VSDecl.VOTO_NONVOTO,
                    TipoCarica = 0,
                };
            FVotiExpr.Add(VExp);
            // a questo punto vado in conferma con la stessa CurrVote
            Stato = TAppStato.ssvVotoConferma;
            CambiaStato();
        }

        public void onPremutoInvalido(object source, int VParam)
        {
            // ok, questo evento arriva quando, nella selezione del voto, è stata
            // premuna una zona invalida, quindi nulla, potrebbe essere un beep
            SystemSounds.Beep.Play();
        }

        public void onPremutoBottoneUscita(object source, int VParam)
        {
            // Bottone Uscita
            FWSMessageExit FMsgExit = new FWSMessageExit();
            if (FMsgExit.ShowDialog() == true)
            {
                CodiceUscitaInVotazione();
            }
            FMsgExit = null;
        }

        public void onPremutoConferma(object source, int VParam)
        {
            // ok, questo evento arriva quando, nella conferma del voto, è stata scelta l'opzione
            //  conferma, cioè il salvataggio del voto

            // CHiamo la funzione di Conferma Voti di Azionisti con l'array di voti espressi
            Azionisti.ConfermaVoti_VotoCorrente(ref FVotiExpr);

            // cambio stato
            Stato = Azionisti.TuttiIDirittiSonoStatiEspressi() ? TAppStato.ssvSalvaVoto : TAppStato.ssvVoto;
            // cambio
            CambiaStato();
        }

        public void onPremutoAnnulla(object source, int VParam)
        {
            RitornaDaAnnulla = true;
            // ok, questo evento arriva quando, nella conferma del voto, è stata scelta l'opzione
            //  annulla, cioè il ritorno all'espressione del voto
            CancellaTempVotiCorrenti();
            // Annulla del voto (torno dove ero prima)
            Stato = TAppStato.ssvVoto;
            CambiaStato();
        }

        public void CancellaTempVotiCorrenti()
        {
            // cancella i voti correnti
            FVotiExpr.Clear();
            VotoEspresso = -1;
            VotoEspressoStr = "";
            VotoEspressoStrUp = "";
        }

        public void CancellaTempMultiVotiCorrenti()
        {
            foreach (TTZone item in Votazioni.VotoCorrente.TouchZoneVoto)
            {
                item.Multi = 0;
            }
        }

        #endregion

    }
}
