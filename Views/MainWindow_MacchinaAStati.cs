using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VotoTouch.WPF
{
    // MACCHINA A STATI

    public partial class MainWindow : Window
    {

        //		ROUTINE DI GESTIONE DEGLI STATI ----------------------------------------------------------------

        #region Macchina A Stati

        private void CambiaStato()
        {
            // disaccoppia la funzione attraverso un timer che chiama un evento
            timCambiaStato.Start();
        }

        private void timCambiaStato_Tick(object sender, EventArgs e)
        {
            timCambiaStato.Stop();
            CambiaStatoDaTimer();
        }

        private void CambiaStatoDaTimer()
        {
            //TAzionista c;
            // gestione degli stati della votazione
            switch (Stato)
            {
                case TAppStato.ssvBadge:
                    timAutoRitorno.Stop();
                    oVotoTouch.CalcolaTouchSpecial(null);
                    //oVotoTouch.CalcolaTouchSpecial(Stato, false);
                    SettaComponenti(false);
                    UscitaInVotazione = false;
                    // labels
                    TxtDirittiDiVoto = "";
                    TxtNomeAzStart = "";
                    // ok ora testo eventuali eventi di apertura o chisura votazione dall'esterno
                    // questo mi viene dalla variabile AperturaVotoEsterno, che viene settata nell'evento
                    // di controllo del timer che testa la variabile votoaperto relativo alla postazione
                    // sul db. 
                    // All'avvio dell'applicazione AperturaVotoEsterno viene settato come la lettura
                    // sul db, quindi quando qui cambia vuol dire (in funzione del valore)
                    // che un evento di apertura/chiusura votazione è avvenuto
                    if (AperturaVotoEsterno != VTConfig.VotoAperto)
                    {
                        if (AperturaVotoEsterno)
                        {
                            // TODO: Possibili bachi: Ricaricamento Liste ad apertura votazione, per ora disabilitata
                            Logging.WriteToLog("Evento Apertura votazione");
                            Rect FFormRect = new Rect(0, 0, this.Width, this.Height);
                            Votazioni.CaricaListeVotazioni(VTConfig.Data_Path, FFormRect, false);
                        }
                        else
                            Logging.WriteToLog("Evento Chiusura votazione");
                        // ok, ora setto la variabile locale di configurazione
                        VTConfig.VotoAperto = AperturaVotoEsterno;
                        // se la votazione è aperta il timer di controllo voto batte di meno
                        timVotoAperto.Interval = TimeSpan.FromMilliseconds(VTConfig.VotoAperto ? VSDecl.TIM_CKVOTO_MAX : VSDecl.TIM_CKVOTO_MIN);
                    }

                    // a seconda dello stato, mostro il semaforo e metto l'immagine corretta
                    if (VTConfig.VotoAperto)
                    {
                        oSemaforo.SemaforoLibero();
                        oVotoImg.LoadImages(VSDecl.IMG_Badge);
                    }
                    else
                    {
                        oSemaforo.SemaforoChiusoVoto();
                        oVotoImg.LoadImages(VSDecl.IMG_Votochiuso);
                    }
                    break;

                case TAppStato.ssvVotoStart:
                    oVotoTouch.CalcolaTouchSpecial(Azionisti.HaDirittiDiVotoMultipli()
                                                       ? Votazioni.ClasseTipoVotoStartDiff
                                                       : Votazioni.ClasseTipoVotoStartNorm);
                    oSemaforo.SemaforoOccupato();
                    // quà metto il voto differenziato
                    MettiComponentiStartVoto();
                    break;

                case TAppStato.ssvVoto:
                    // in generale non so quale è il voto corrente, perchè non c'è più una sequenza
                    // ma dipende dai diritti di voto dell'azionista, espressi o no, potrei avere espresso
                    // tutti i voti sulla prima e nessuno sulla seconda votazione.
                    // lo vedo caricando di volta in volta l'azionista che non ha diritti di voto espressi (havotato = false)
                    if (!RitornaDaAnnulla)
                    {
                        foreach (TTZone item in Votazioni.VotoCorrente.TouchZoneVoto.TouchZone)
                        {
                            item.Multi = 0;
                        }
                    }
                    else
                        RitornaDaAnnulla = false;
                    // ok, ora estraggo l'azionista o il gruppo di azionisti (se non è differenziato) che devono votare
                    // in Azionisti.AzionistiInVotoCorrente ho l'elenco dei diritti
                    // setto il voto corrente sul primo item dell'oggetto
                    if (Azionisti.EstraiAzionisti_VotoCorrente(IsVotazioneDifferenziata) &&
                        Votazioni.SetVotoCorrente(Azionisti.DammiIDVotazione_VotoCorrente()))
                    {
                        // calibro il touch sul voto
                        oVotoTouch.CalcolaTouchVote(Votazioni.VotoCorrente);
                        // ora devo capire che votazione è e mettere i componenti, attenzione che posso tornare da un'annulla
                        SettaComponenti(false);
                        // cancello i voti temporanei correnti 
                        CancellaTempVotiCorrenti();
                        // ora metto in quadro l'immagine, che deve essere presa da un file composto da
                        oVotoImg.LoadImages(VSDecl.IMG_voto + Votazioni.VotoCorrente.IDVoto.ToString());
                        // mostro comunque i diritti di voto in lbDirittiDiVoto e il nome di quello corrente
                        TxtNomeDisgiunto = App.Instance.getLang("SAPP_VOTE_D_RASO") + "\n" +
                                               Azionisti.DammiNomeAzionistaInVoto_VotoCorrente(IsVotazioneDifferenziata);
                        TxtNomeDisgiuntoVis = true;
                        //lbNomeDisgiunto.Visible = (IsVotazioneDifferenziata || Azionisti.DammiCountDirittiDiVoto_VotoCorrente() ==1);
                        int dir_riman = IsVotazioneDifferenziata
                                            ? Azionisti.DammiTotaleDirittiRimanenti_VotoCorrente()
                                            : Azionisti.DammiCountAzioniVoto_VotoCorrente(); // DammiCountDirittiDiVoto_VotoCorrente();
                        int deleghe_riman = IsVotazioneDifferenziata
                                            ? Azionisti.DammiTotaleDirittiRimanenti_VotoCorrente()
                                            : Azionisti.DammiCountDirittiDiVoto_VotoCorrente();
                        if (!IsVotazioneDifferenziata && deleghe_riman > 1)
                            TxtNomeDisgiunto += " e altre " + (deleghe_riman - 1).ToString() + " deleghe";
                        TxtDirittiDiVoto = dir_riman.ToString() + App.Instance.getLang("SAPP_VOTE_D_DIRITTI");
                        if (IsVotazioneDifferenziata) TxtDirittiDiVoto = "Voto Differenziato \n " + TxtDirittiDiVoto + " rimanenti";
                        TxtDirittiDiVotoVis = true;
        
                        // se la votazione corrente è di candidato su più pagine disegno i rettangoli
                        if (Stato == TAppStato.ssvVoto && 
                            (Votazioni.VotoCorrente.TipoVoto == VSDecl.VOTO_CANDIDATO ||
                             Votazioni.VotoCorrente.TipoVoto == VSDecl.VOTO_CANDIDATO_SING))
                        {
                            // ora lbCandidati_PresCDA
                            if (Votazioni.VotoCorrente.NPresentatoCDA > 0)
                            {
                                TxtCandidati_PresCDA = "Proposto (Cooptato) C.d.A.";
                                TxtCandidati_PresCDAVis = true;
                            }
                            // Proposte alternative
                            if ((Votazioni.VotoCorrente.NListe - Votazioni.VotoCorrente.NPresentatoCDA) > 0)
                            {
                                TxtCandidati_Altern = "Candidati Alternativi";
                                TxtCandidati_PresCDAVis = true;
                            }
                        }

                        // se è multicandidato metto le labelapposite
                        if (Votazioni.VotoCorrente.TipoVoto == VSDecl.VOTO_MULTICANDIDATO)
                        {
                            // label nselezioni
                            int nsel = Votazioni.VotoCorrente.NMultiSelezioni;
                            //if (nsel >= vt.MinScelte && nsel <= vt.MaxScelte)
                            //    myBrush1 = new System.Drawing.SolidBrush(Color.Green);  //E3E3E3
                            //else
                            //    myBrush1 = new System.Drawing.SolidBrush(Color.Red);  //E3E3E3
                            TxtNSelezioni = nsel + " scelte espresse";
                            TxtNSelezioniVis = true;

                            // ora lbCandidati_PresCDA
                            if (Votazioni.VotoCorrente.NPresentatoCDA > 0)
                            {
                                TxtCandidati_PresCDA = "Proposto (Cooptato) C.d.A.";
                                TxtCandidati_PresCDAVis = true;
                            }
                            // Proposte alternative
                            if ((Votazioni.VotoCorrente.NListe - Votazioni.VotoCorrente.NPresentatoCDA) > 0)
                            {
                                TxtCandidati_Altern = "Proposte Alternative";
                                TxtCandidati_PresCDAVis = true;
                            }
                        }

                    }
                    else
                    {
                        // si sono verificati dei problemi, lo segnalo
                        Logging.WriteToLog("Errore fn Azionisti.EstraiAzionisti_VotoCorrente(IsVotazioneDifferenziata), zero ");
                        MessageBox.Show("Si è verificato un errore (Azionisti.EstraiAzionisti_VotoCorrente(IsVotazioneDifferenziata))" + "\n\n" +
                            "Chiamare operatore esterno.\n\n ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Stato = TAppStato.ssvBadge;
                        CambiaStato();
                    }
                    break;

                case TAppStato.ssvVotoConferma:
                    oVotoTouch.CalcolaTouchSpecial(Votazioni.ClasseTipoVotoConferma);
                    //oVotoTouch.CalcolaTouchSpecial(Stato, false);
                    SettaComponenti(false);
                    // ora metto in quadro l'immagine, che deve essere presa da un file composto da
                    oVotoImg.LoadImages(VSDecl.IMG_voto + Votazioni.VotoCorrente.IDVoto.ToString() + VSDecl.IMG_voto_c);
                    // conferma
                    MettiComponentiConferma();
                    TxtNomeDisgiuntoVis = true; // (IsVotazioneDifferenziata || Azionisti.DammiCountDirittiDiVoto_VotoCorrente() == 1);
                    break;

                case TAppStato.ssvVotoContinua:
                    oVotoTouch.CalcolaTouchSpecial(null);
                    //oVotoTouch.CalcolaTouchSpecial(Stato, false);
                    break;

                case TAppStato.ssvVotoFinito:
                    oVotoTouch.CalcolaTouchSpecial(null);
                    //oVotoTouch.CalcolaTouchSpecial(Stato, false);
                    TxtDirittiDiVotoVis = false;
                    SettaComponenti(false);
                    // labels
                    TxtDirittiDiVoto = "";
                    // messaggio di arrivederci
                    if (VTConfig.UsaLettore)
                    {
                        NewReader.Flush();
                    }
                    // se è uscito in votazione con il 999999
                    if (UscitaInVotazione)
                    {
                        UscitaInVotazione = false;
                        TornaInizio();
                    }
                    else
                    {
                        oVotoImg.LoadImages(VSDecl.IMG_fine);
                        // ora devo vediricare se è attivo AttivaAutoRitornoVoto
                        if (VTConfig.AttivaAutoRitornoVoto)
                        {
                            timAutoRitorno.Start();
                        }
                    }

                    break;

                case TAppStato.ssvSalvaVoto:
                    // resetto l'eventuale richiesta di votaz differeniata
                    LocalAbilitaVotazDifferenziataSuRichiesta = false;

                    if (Azionisti.DammiQuantiDirittiSonoStatiVotati() > VSDecl.MINVOTI_PROGRESSIVO)
                    {
                        // metti lo spinning wheel
                    }
                    // salvo i dati sul database
                    oDBDati.SalvaTutto(Badge_Letto, ref Azionisti);

                    // TODO: GEAS VERSIONE (salvataggio voti)
                    // Salva i voti in GEAS
                    if (VTConfig.ModoAssemblea == VSDecl.MODO_AGM_SPA && VTConfig.SalvaVotoInGeas)                    
                        oDBDati.SalvaTuttoInGeas(Badge_Letto, ref Azionisti);

                    // togli lo spinning wheel
                    //pbSalvaDati.Visible = false;
                    
                    oSemaforo.SemaforoFineOccupato();
                    Stato = TAppStato.ssvVotoFinito;
                    CambiaStato();
                    break;
            }
        }

        private void timVotoAperto_Tick(object sender, EventArgs e)
        {
            // devo verificare sul database se il voto per questa postazione è aperto
            int getvtaperto = oDBDati.CheckStatoVoto(VTConfig.NomeTotem);
            // se sono in una condizione di errore (es db non risponde) lascio il valore precedente
            if (getvtaperto != -1)
            {
                bool vtaperto = getvtaperto == 1;
                // se sono diversi e sono all'inizio allora cambio lo stato
                if (VTConfig.VotoAperto != vtaperto)
                {
                    // segnalo che c'è stato un evento e setto una variabile che sarà controllata
                    // appena lo stato sarà su Badge, quindi sarà finita l'eventuale votazione 
                    // in corso
                    AperturaVotoEsterno = vtaperto;
                    // ma se per caso sono in badge devo forzare
                    if (Stato == TAppStato.ssvBadge)
                        CambiaStato();
                }
            }
            // chiaramente se non sono diversi non faccio nulla
        }

        private void timAutoRitorno_Tick(object sender, EventArgs e)
        {
            timAutoRitorno.Stop();

            // esco
            TornaInizio();
        }

        private void timPopup_Tick(object sender, EventArgs e)
        {
            //timPopup.Stop();
            //pnPopupRed.Visible = false;
        }

        #endregion

    }
}
