using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;

namespace VotoTouch.WPF.Models
{
    public class CVotazione_Custom_Multi_Manutencoop: CVotazione
    {

        // CLASSE CUSTOM PER MANUTENCOOP 2017
        
        public CVotazione_Custom_Multi_Manutencoop(Rect AFormRect) : base(AFormRect)
        {
            // costruttore
            CustomPaint = true;
        }

       public override void GetTouchVoteZone()
        {
            // DR12 OK
            TouchZoneVoto.Clear();

            CalcolaTouch_Manutencoop();
            
            // nella classe base c'è qualcosa
            base.GetTouchVoteZone();
        }

        // --------------------------------------------------------------
        //  CALCOLO DEL TOUCH MULTICANDIDATO PER MANUTENCOOP 2017
        // --------------------------------------------------------------

        #region calcolo candidato/multicandidato touch nuovo

        private const int basey = 119;
        private const int deltay = 74;      
        private static int y = basey;
        private static int b = y + deltay;
        
        private const int basex = 18;
        private const int deltax = 320;
        private static int x = basex;
        private static int r = x + deltax;

        public void CalcolaTouch_Manutencoop()
        {
            TTZone a;

            // prima colonna
            // 1 - LEVORATO CLAUDIO
            TouchZoneVoto.Add(calczone(0, "LEVORATO CLAUDIO"));
            y += deltay; b += deltay;

            // 2 - ENZO GRENZI
            TouchZoneVoto.Add(calczone(1, "ENZO GRENZI"));
            y += deltay; b += deltay;

            // 3 - CHIARA FILIPPI
            TouchZoneVoto.Add(calczone(2, "CHIARA FILIPPI"));
            y += deltay; b += deltay;

            // 4 - LUCA BUGLIONE
            TouchZoneVoto.Add(calczone(3, "LUCA BUGLIONE"));
            y += deltay; b += deltay;

            // 5 - STEFANIA LORI
            TouchZoneVoto.Add(calczone(4, "STEFANIA LORI"));
            y += deltay; b += deltay;

            // 6 - CARMELA ARMENTO
            TouchZoneVoto.Add(calczone(5, "CARMELA ARMENTO"));
            y += deltay; b += deltay;

            // 7 - ALESSANDRO BENSI
            TouchZoneVoto.Add(calczone(6, "ALESSANDRO BENSI"));
            y += deltay; b += deltay;

            // 8 - SERGIO CAPPE’
            TouchZoneVoto.Add(calczone(7, "SERGIO CAPPE’"));
            y += deltay; b += deltay;

            // 9 - FABRIZIO LAGHI
            TouchZoneVoto.Add(calczone(8, "FABRIZIO LAGHI"));
            y += deltay; b += deltay;

            // 10 - GINO SERGIO BENASSI
            TouchZoneVoto.Add(calczone(9, "GINO SERGIO BENASSI"));
            y += deltay; b += deltay;

            // seconda colonna
            x += deltax;
            r += deltax;
            y = basey;
            b = y + deltay;

            // 11 - RAFFAELE POTRINO
            TouchZoneVoto.Add(calczone(10, "RAFFAELE POTRINO"));
            y += deltay; b += deltay;

            // 12 - LUIGI FACCHINI
            TouchZoneVoto.Add(calczone(11, "LUIGI FACCHINI"));
            y += deltay; b += deltay;

            // 13 - EMMA RAPONE
            TouchZoneVoto.Add(calczone(12, "EMMA RAPONE"));
            y += deltay; b += deltay;

            // 14 - MARIANGELA FONTANA
            TouchZoneVoto.Add(calczone(13, "MARIANGELA FONTANA"));
            y += deltay; b += deltay;

            // 15 - GIULIANO DI BERNARDO
            TouchZoneVoto.Add(calczone(14, "GIULIANO DI BERNARDO"));
            y += deltay; b += deltay;

            // 16 - CLAUDIO BONAFE’
            TouchZoneVoto.Add(calczone(15, "CLAUDIO BONAFE’"));
            y += deltay; b += deltay;

            // 17 - SABRINA ANNOVI
            TouchZoneVoto.Add(calczone(16, "SABRINA ANNOVI"));
            y += deltay; b += deltay;

            // 18 - MARIA D’AMELIO
            TouchZoneVoto.Add(calczone(17, "MARIA D’AMELIO"));
            y += deltay; b += deltay;

            // 19 - ALESSANDRO DALLA TORRE
            TouchZoneVoto.Add(calczone(18, "ALESSANDRO DALLA TORRE"));
            y += deltay; b += deltay;

            // terza colonna
            x += deltax;
            r += deltax;
            y = basey;
            b = y + deltay;

            // 20 - LICIA AVRAAM
            TouchZoneVoto.Add(calczone(19, "LICIA AVRAAM"));
            y += deltay; b += deltay;

            // 21 - PAOLO ZANIBONI
            TouchZoneVoto.Add(calczone(20, "PAOLO ZANIBONI"));
            y += deltay; b += deltay;

            // 22 - FRANCO PALAGANO
            TouchZoneVoto.Add(calczone(21, "FRANCO PALAGANO"));
            y += deltay; b += deltay;

            // 23 - ENRICO INVERNO
            TouchZoneVoto.Add(calczone(22, "ENRICO INVERNO"));
            y += deltay; b += deltay;

            // 24 - CINZIA CATERI
            TouchZoneVoto.Add(calczone(23, "CINZIA CATERI"));
            y += deltay; b += deltay;

            // 25 - ALFREDO DELLISANTI
            TouchZoneVoto.Add(calczone(24, "ALFREDO DELLISANTI"));
            y += deltay; b += deltay;

            // 26 - LAURA DUO’
            TouchZoneVoto.Add(calczone(25, "LAURA DUO’"));
            y += deltay; b += deltay;

            // 27 - GABRIELE STANZANI
            TouchZoneVoto.Add(calczone(26, "GABRIELE STANZANI"));
            y += deltay; b += deltay;

            // 28 - CRISTINA CAVICCHIOLI
            TouchZoneVoto.Add(calczone(27, "CRISTINA CAVICCHIOLI"));
            y += deltay; b += deltay;

            // devo aggiungere il tasto avanti con evento           
            a = new TTZone();
            GetZone(ref a, 670, 820, 1000, 1000);
            a.expr = VSDecl.VOTO_MULTIAVANTI;
            a.Text = ""; a.ev = TTEvento.steMultiAvanti; a.pag = 0; a.cda = false; a.Multi = 0;
            TouchZoneVoto.Add(a);

            //// devo aggiungere il tasto uscita           
            //a = new TTZone();
            //GetZone(ref a, 24, 895, 240, 1000);
            //a.expr = VSDecl.VOTO_BTN_USCITA;
            //a.Text = ""; a.ev = TTEvento.steBottoneUscita; a.pag = 0; a.Multi = 0; 
            //Tz.Add(a);

        }

        private TTZone calczone(int expr, string cand)
        {
            TTZone a = new TTZone();
            GetZone(ref a, x, y, r, b);
            a.expr = expr; a.pag = 0; a.cda = false; a.Multi = 0;
            a.PaintMode = VSDecl.PM_ONLYCHECK;
            a.Text = cand; a.ev = TTEvento.steMultiValido;
            a.CKRect = new Rect(a.x + 23, a.y + 5, 56, 60);

            return a;
        }

        /*       
        LEVORATO CLAUDIO        ENZO GRENZI             CHIARA FILIPPI        LUCA BUGLIONE        STEFANIA LORI        
        CARMELA ARMENTO        ALESSANDRO BENSI        SERGIO CAPPE’        FABRIZIO LAGHI        GINO SERGIO BENASSI
        
        RAFFAELE POTRINO        LUIGI FACCHINI        EMMA RAPONE        MARIANGELA FONTANA        GIULIANO DI BERNARDO
        CLAUDIO BONAFE’        SABRINA ANNOVI        MARIA D’AMELIO       ALESSANDRO DALLA TORRE        
        
        LICIA AVRAAM            PAOLO ZANIBONI        FRANCO PALAGANO        ENRICO INVERNO        CINZIA CATERI        
        ALFREDO DELLISANTI        LAURA DUO’        GABRIELE STANZANI        CRISTINA CAVICCHIOLI
         
        
        */

        #endregion

    }
}
