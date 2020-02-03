using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using VotoTouch.WPF.Models;

namespace VotoTouch.WPF
{
    public class CVotazione_Candidato: CVotazione
    {

        // CLASSE DELLA votazione di candidato
        
        public CVotazione_Candidato(Rect AFormRect) : base(AFormRect)
        {
            // costruttore
        }

        public override void GetTouchVoteZone()
        {
            TouchZoneVoto.Clear();

            if (NListe <= 6)
                GetTouchVoteZoneSmall();
            else
                GetTouchVoteZoneOriginal();

            // Le schede Speciali
            MettiSchedeSpecialiDiVoto();

            // nella classe base c'è qualcosa
            base.GetTouchVoteZone();
        }

        // VERSIONE PICCOLA CON 6 O MENO LISTE ------------------------------------------------------------

        #region small vote

        private void GetTouchVoteZoneSmall()
        { 
            // funziona almeno se in tutto ci sono <= 6 candidati e ci siano <= 4
            // presentati da cda, altrimenti è di pagina
            if (NListe <= 6)
            {
                switch (NListe)
                {                    
                    case 1:
                        #region 1 candidato
                        // 1 candidato presentato da cda / normale, è lo stesso, 99% che sarà sempre questo
                        TLista li = Liste[0];
                        TTZone a = new TTZone();
                        GetZone(ref a, 160, 310, 850, 500);
                        a.expr = 0; a.ev = TTEvento.steVotoValido; a.Text = li.DescrLista;
                        a.Multi = 0; a.pag = 0; a.cda = li.PresentatodaCDA; // (AVotazione.NPresentatoCDA == 1); 
                        TouchZoneVoto.Add(a);
                        #endregion
                        break;

                    case 2:
                        #region 2 candidati
                        // 2 candidati presentato da cda o altro non importa li metto sempre in verticale
                        if (NListe == 2) // && AVotazione.NPresentatoCDA == 3)
                        {
                            int str = 290; // partenza
                            int ha = 120; // altezza dei bottoni
                            int sp = 80; // spazio tra i bottoni
                            // ciclo
                            for (int z = 0; z < NListe; z++)
                            {
                                li = Liste[z];
                                a = new TTZone();
                                GetZone(ref a, 180, str, 830, str + ha);
                                a.expr = z; a.ev = TTEvento.steVotoValido; a.Text = li.DescrLista;
                                a.Multi = 0; a.pag = 0; a.cda = li.PresentatodaCDA;
                                TouchZoneVoto.Add(a);
                                str = str + sp + ha;
                            }
                        }
                        #endregion
                        break;

                    case 3:
                        #region 3 candidati
                        // 3 candidati presentato da cda o altro non importa li metto sempre in verticale
                        if (NListe == 3) // && AVotazione.NPresentatoCDA == 3)
                        {
                            int str = 270; // partenza
                            int ha = 100; // altezza dei bottoni
                            int sp = 50;  // spazio tra i bottoni
                            // ciclo
                            for (int z = 0; z < NListe; z++)
                            {
                                li = Liste[z];
                                a = new TTZone();
                                GetZone(ref a, 180, str, 830, str + ha);
                                a.expr = z; a.ev = TTEvento.steVotoValido; a.Text = li.DescrLista;
                                a.Multi = 0; a.pag = 0; a.cda = li.PresentatodaCDA;
                                TouchZoneVoto.Add(a);
                                str = str + sp + ha;
                            }
                        }
                        #endregion
                        break;

                    case 4:
                        #region 4 candidati
                        // sono sempre in due linee, da capire come sono messi
                        // possono essere 1 - 3,  2 - 2,  3 - 1  
                        if (NListe == 4)
                        {
                            int ha = 13; // altezza dei bottoni
                            // schema 2 + 2
                            int[] bx = new int[] { 900, 550, 90, 550 };
                            int[] by = new int[] { 280, 280, 480, 480 };
                            int[] bw = new int[] { 370, 370, 370, 370 };

                            // possono esserci delle differenze se sono 1 - 3 o 3 - 1
                            if (NPresentatoCDA == 1)   // 1 - 3
                            {
                                bx = new int[] { 350, 30, 350, 670 };
                                by = new int[] { 280, 480, 480, 480 };
                                bw = new int[] { 300, 300, 300, 300 };
                            }
                            if (NPresentatoCDA == 3)   // 3 - 1
                            {
                                bx = new int[] { 30, 350, 670, 350 };
                                by = new int[] { 280, 280, 280, 480 };
                                bw = new int[] { 300, 300, 300, 300 };
                            }
                
                            // ciclo, tanto sono sempre ordinati prima cda e poi norm
                            for (int z = 0; z < NListe; z++)
                            {
                                li = Liste[z];
                                a = new TTZone();                        
                                // ok ora mi calcolo
                                GetZone(ref a, bx[z], by[z], bx[z] + bw[z], by[z] + ha);
                                a.expr = z; a.ev = TTEvento.steVotoValido; a.Text = li.DescrLista;
                                a.Multi = 0; a.pag = 0; a.cda = li.PresentatodaCDA;
                                TouchZoneVoto.Add(a);
                            }
                        }
                        #endregion
                        break;

                    case 5:
                        #region 5 candidati
                        // ***** 5 Candidati *****
                        // sono sempre in tre linee, da capire come sono messi
                        // 2 2 1
                        // 1 2 2
                        // 2 1 2
                        if (NListe == 5)
                        {
                            int ha = 100; // altezza dei bottoni
                            int bw = 370;
                            // schema 2 + 2
                            int[] bx = new int[] { 90, 550, 90, 550, 90 };
                            int[] by = new int[] { 270, 270, 420, 420, 570 };

                            // possono esserci delle differenze se sono 1 - 3 o 3 - 1
                            if (NPresentatoCDA == 1)   // 1 - 2 - 2
                            {
                                bx = new int[] { 310, 90, 550, 90, 550 };
                                by = new int[] { 270, 420, 420, 570, 570 };
                            }
                            if (NPresentatoCDA == 3)   // 2 - 1 - 2
                            {
                                bx = new int[] { 90, 550, 90, 90, 550 };
                                by = new int[] { 270, 270, 420, 570, 570 };
                            }

                            // ciclo, tanto sono sempre ordinati prima cda e poi norm
                            for (int z = 0; z < NListe; z++)
                            {
                                li = Liste[z];
                                a = new TTZone();
                                // ok ora mi calcolo
                                GetZone(ref a, bx[z], by[z], bx[z] + bw, by[z] + ha);
                                a.expr = z; a.ev = TTEvento.steVotoValido; a.Text = li.DescrLista;
                                a.Multi = 0; a.pag = 0; a.cda = li.PresentatodaCDA;
                                TouchZoneVoto.Add(a);
                            }
                        }
                        #endregion
                        break;
                
                    case 6:
                        #region 6 candidati
                        // ***** 6 Candidati *****
                        // sono sempre in tre linee, da capire come sono messi
                        // 2, 4 - 2 2 2
                        // 1 - 1 3 2
                        // 3 - 3 3 
                        // 5 - 3 2 1
                        if (NListe == 6)
                        {
                            int ha = 100; // altezza dei bottoni
                            int bw = 370;
                            // schema 2 + 2
                            int[] bx = new int[] { 90, 550, 90, 550, 90, 550 };
                            int[] by = new int[] { 270, 270, 420, 420, 570, 570 };

                            // possono esserci delle differenze
                            if (NPresentatoCDA == 1)   // 1 3 2
                            {
                                bx = new int[] { 350, 30, 350, 670, 30, 350 };
                                by = new int[] { 260, 430, 430, 430, 580, 580 };
                                bw = 30;
                            }
                            if (NPresentatoCDA == 3)   // 3 - 3 
                            {
                                bx = new int[] { 30, 350, 670, 30, 350, 670 };
                                by = new int[] { 280, 280, 280, 500, 500, 500 };
                                bw = 30;
                                ha = 13;
                            }
                            if (NPresentatoCDA == 5)   // 3 - 2 - 1
                            {
                                bx = new int[] { 30, 350, 670, 30, 350, 350 };
                                by = new int[] { 260, 260, 260, 410, 410, 580 };
                                bw = 30;
                            }

                            // ciclo, tanto sono sempre ordinati prima cda e poi norm
                            for (int z = 0; z < NListe; z++)
                            {
                                li = Liste[z];
                                a = new TTZone();
                                // ok ora mi calcolo
                                GetZone(ref a, bx[z], by[z], bx[z] + bw, by[z] + ha);
                                a.expr = z; a.ev = TTEvento.steVotoValido; a.Text = li.DescrLista;
                                a.Multi = 0; a.pag = 0; a.cda = li.PresentatodaCDA;
                                TouchZoneVoto.Add(a);
                            }
                        }
                        #endregion
                        break;
                }
            }

        }
        
        #endregion

        // VERSIONE CON PIU LISTE ------------------------------------------------------------

        #region VERSIONE CON PIU LISTE

        private void GetTouchVoteZoneOriginal()
        {
            // metto i rettangoli del candidato a singola pagina
            NewCalcolaTouchCandidatoPagina();
            // ora devo mettere i tabs
            CalcolaTouchTabsPagina();
            // nella classe base c'è qualcosa
            base.GetTouchVoteZone();
        }


        public void NewCalcolaTouchCandidatoPagina()
        {
            // DR12 OK
            TTZone a;
            TLista li;
            int z, PosPresCda, PosCandAlt;
            TTEvento evento;

            // in funzione della votazione seleziono l'evento corrispondente al tocco del voto
            // se è Multicandidato, l'evento sarà solo locale e setterà un flag nella collection, 
            // altrimenti richiamerà il voto valido all'esterno
            if (TipoVoto == VSDecl.VOTO_MULTICANDIDATO)
                evento = TTEvento.steMultiValido;
            else
                evento = TTEvento.steVotoValido;

            PosPresCda = 1;
            PosCandAlt = 1;

            // ok, ciclo lungo i candidati per metterli nell'area giusta
            //foreach(TNewLista li in AFVotaz.Liste)
            for (z = 0; z < NListe; z++)
            {
                li = (TLista)Liste[z];

                // Devo testare se il candidato è presentato dal cda
                if (li.PresentatodaCDA)
                {
                    // ok, è presentato, va nell'area in alto
                    a = new TTZone();
                    GetNew_CandidatoCdaZone(ref a, PosPresCda, NPresentatoCDA, AreaVoto);
                    a.expr = z; a.ev = evento;
                    a.Text = li.DescrLista;
                    a.Multi = 0; a.cda = true;
                    a.pag = 0;
                    TouchZoneVoto.Add(a);
                    PosPresCda++;
                }
                else
                {
                    // una voltra controllavo la pagina corrente /if (li.Pag == CurrPag) ora non si usa più
                    // perché aggiungo tutti i candidati e 
                    // non è presentato, va nell'area in basso
                    a = new TTZone();
                    GetNew_CandidatoAltPaginaZone(ref a, PosCandAlt, AreaVoto);
                    a.expr = z; a.ev = evento;
                    a.Text = li.DescrLista;
                    a.Multi = 0; a.cda = false;
                    a.pag = li.Pag;
                    TouchZoneVoto.Add(a);
                    PosCandAlt++;
                    // aggiunta successiva
                    if (PosCandAlt > AreaVoto.CandidatiPerPagina)
                        PosCandAlt = 1;
                }
            }

            // Le schede Speciali
            //MettiSchedeSpeciali(AFVotaz);

            // Attenzione, nel caso la votazione sia di tipo Multicandidato, devo Aggiungere un tasto
            // "Avanti" o "Conferma" per continuare ed è possibile che ci sia un tasto SelezionaTuttiCDA
            if (TipoVoto == VSDecl.VOTO_MULTICANDIDATO)
            {
                // devo aggiungere il tasto con evento           
                a = new TTZone();
                GetZone(ref a, 640, 810, 970, 910); a.expr = VSDecl.VOTO_MULTIAVANTI;
                a.Text = ""; a.ev = TTEvento.steMultiAvanti; a.pag = 0; a.cda = false; a.Multi = 0;
                TouchZoneVoto.Add(a);

                // se nella votazione è presente il seleziona TuttoCDA
                if (SelezionaTuttiCDA && NPresentatoCDA > 0)
                {
                    // devo mettere il tasto
                    a = new TTZone();
                    //int y;
                    // devo fare attenzione a quante righe ha il cda e spostare il tasto
                    if (NPresentatoCDA <= 3)
                        GetZone(ref a, (AreaVoto.RCda() - 240), (AreaVoto.BCda() + 20),
                            AreaVoto.RCda(), (AreaVoto.BCda() + 8));
                    else
                        GetZone(ref a, (AreaVoto.RCda() - 240), (AreaVoto.BCda() - 40),
                            AreaVoto.RCda(), (AreaVoto.BCda() + 3));
                    a.expr = 999; a.cda = false;
                    a.Text = ""; a.ev = TTEvento.steMultiSelezTuttiCDA; a.pag = 0;
                    TouchZoneVoto.Add(a);
                }
            }
        }

        private void GetNew_CandidatoCdaZone(ref TTZone a, int APosPresCda, int ATotPresCda, TAreaVotazione Area)
        {
            // ho una zona di voto che è in Area, ho il totale di quanti sono, ela posizione 
            float x, y, r, b, ax, ar;
            int nct, nrt, npc, npr;

            // PER ETRURIA
            // se ho un candidato solo, come nel 99% dei casi di voto singolo, metto 
            //if (ATotPresCda == 1)
            //{
            //    ax = Area.XCda +16;
            //    y = Area.YCda;
            //    ar = Area.RCda() -16;
            //    b = Area.BCda() +1;

            //    GetZone(ref a, (int)ax, (int)y, (int)ar, (int)b);
            //    return;
            //}


            // definisco il n. di colonne totali / per il numero totale di cda
            int[] ncolt = new int[] { 0, 1, 2, 3, 2, 3, 3 };
            nct = ncolt[ATotPresCda];
            // definisco il n. di righe totali
            if (ATotPresCda <= 3) nrt = 1; else nrt = 2;
            // stabilisco la posizione reale del rett, in che colonna è, in funzione di APosPresCda
            // c'è un correttivo, perchè nel caso di 4 cvandidati non vado su tre file, ma su due
            int[] nposc = new int[] { 0, 1, 2, 3, 1, 2, 3 };
            npc = nposc[APosPresCda];
            if (ATotPresCda == 4 && APosPresCda == 3) npc = 1;
            if (ATotPresCda == 4 && APosPresCda == 4) npc = 2;
            // stabilisco la posizione nella riga
            int[] nposr = new int[] { 0, 1, 1, 1, 2, 2, 2 };
            npr = nposr[APosPresCda];
            if (ATotPresCda == 4 && APosPresCda == 3) npr = 2;
            if (ATotPresCda == 4 && APosPresCda == 4) npr = 2;

            // calcolo il rettangolo globale
            x = Area.XCda + ((Area.WCda / nct) * (npc - 1));
            y = Area.YCda + ((Area.HCda / nrt) * (npr - 1));
            r = x + (Area.WCda / nct);
            b = y + (Area.HCda / nrt);
            // devo ora calcolarmi il vero rettangolo interno
            if (npr == 1)
                b = y + VSDecl.HRETT_CANDIDATO;
            else
                y = b - VSDecl.HRETT_CANDIDATO;
            // devo centrare i rettangoli
            float[] dimr = new float[] { 0, 38, 34, 29 };
            ax = x + ((r - x - dimr[nct]) / 2);
            ar = r - ((r - x - dimr[nct]) / 2);

            GetZone(ref a, (int)ax, (int)y, (int)ar, (int)b);
        }

        private void GetNew_CandidatoAltPaginaZone(ref TTZone a, int APosCandAlt, TAreaVotazione Area)
        {
            // ho il n. di candidati per pagina in Area.CandidatiPerPagina

            float x, y, r, b, ax, ar;
            int nct, nrt, npc, npr;

            // definisco il n. di colonne totali in funzione del numero di candidati
            if (Area.CandidatiPerPagina == 1) nct = 1; else nct = 2;
            // definisco il n. di righe totali
            int[] nrowt = new int[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5 };
            nrt = nrowt[Area.CandidatiPerPagina];
            // posizione del rettangolo sulla riga
            int[] nposc = new int[] { 0, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            npc = nposc[APosCandAlt];

            // posizione sulla riga
            npr = nrowt[APosCandAlt];

            // calcolo il rettangolo globale
            x = Area.XAlt + ((Area.WAlt / nct) * (npc - 1));
            y = Area.YAlt + ((Area.HAlt / nrt) * (npr - 1));
            r = x + (Area.WAlt / nct);
            b = y + (Area.HAlt / nrt);
            // devo ora calcolarmi il vero rettangolo interno
            b = y + VSDecl.HRETT_CANDIDATO;
            // devo centrare i rettangoli
            float[] dimr = new float[] { 0, 380, 340, 280 };
            ax = x + ((r - x - dimr[nct]) / 2);
            ar = r - ((r - x - dimr[nct]) / 2);
            GetZone(ref a, (int)ax, (int)y, (int)ar, (int)b);
        }

        // --------------------------------------------------------------
        //  CALCOLO DEL TOUCH TABS
        // --------------------------------------------------------------

        public void CalcolaTouchTabsPagina()
        {
            // DR12 OK
            // se non ho bisogno di tab è inutile, tanto vale uscire
            if (!AreaVoto.NeedTabs) return;

            // mette i tabs che ci sono in funzione delle pagine contenute in Pagina
            int i, ncol, acol, arow;
            TIndiceListe il;
            int x, y, ax, ay, w, h;
            TTZone a;

            // x:995 y:320 w:295 h:600
            x = 77;
            y = AreaVoto.YAlt; // 31;
            h = 5; // altezza dei tabs fissa
            // bisogna stabilire quante colonne ci sono
            ncol = ((Pagine.Count - 1) / 8) + 1;
            w = 24 / ncol;

            acol = 0;
            arow = 0;
            // parto da 1 perche la pagina 0 è quella dei candidati cda
            for (i = 1; i < Pagine.Count; i++)
            {
                il = (TIndiceListe)Pagine[i];
                // ok, ora inserisco in funzione della posizione
                a = new TTZone();
                ax = x + (w * acol);
                ay = y + (6 * arow);

                GetZone(ref a, ax, ay, ax + w - 1, ay + h);

                a.expr = il.pag; a.ev = TTEvento.steTabs;
                a.Text = il.indice.ToLower();
                a.Multi = 0; a.pag = 0;
                TouchZoneVoto.Add(a);

                arow++;
                if (arow >= 8)
                {
                    arow = 0;
                    acol++;
                }
            }
        }

        #endregion

    }
}
