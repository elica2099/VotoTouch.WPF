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
    public class CVotazione_Lista: CVotazione
    {

        // CLASSE DELLA votazione di lista
        
        public CVotazione_Lista(Rect AFormRect) : base(AFormRect)
        {
            // costruttore
        }

        public override void GetTouchVoteZone()
        {
            TTZone a;

            TouchZoneVoto.Clear();

            switch (NListe)
            {
                // ok, in funzione dell liste e della votazione faccio
                case 1:
                    a = new TTZone();
                    GetZone(ref a, 160, 240, 850, 680); a.expr = 0; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    break;
                // 2 Liste
                case 2:
                    // 1° Lista
                    a = new TTZone();
                    GetZone(ref a, 80, 240, 430, 680); a.expr = 0; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // 2° Lista
                    a = new TTZone();
                    GetZone(ref a, 560, 240, 920, 680); a.expr = 1; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    break;
                // 3 Liste
                case 3:
                    // 1° Lista
                    a = new TTZone();
                    GetZone(ref a, 30, 240, 310, 680); a.expr = 0; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // 2° Lista
                    a = new TTZone();
                    GetZone(ref a, 360, 240, 630, 680); a.expr = 1; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // 3° Lista
                    a = new TTZone();
                    GetZone(ref a, 680, 240, 960, 680); a.expr = 2; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    break;
                // 4 Liste
                case 4:
                    // 1° Lista
                    a = new TTZone();
                    GetZone(ref a, 50, 220, 450, 440); a.expr = 0; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // 2° Lista
                    a = new TTZone();
                    GetZone(ref a, 540, 220, 940, 440); a.expr = 1; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // 3° Lista
                    a = new TTZone();
                    GetZone(ref a, 50, 490, 450, 710); a.expr = 2; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // 4° Lista
                    a = new TTZone();
                    GetZone(ref a, 540, 490, 940, 710); a.expr = 3; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    break;
                // 5 Liste
                case 5:
                    // 1° Lista
                    a = new TTZone();
                    GetZone(ref a, 20, 230, 300, 430); a.expr = 0; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // 2° Lista
                    a = new TTZone();
                    GetZone(ref a, 340, 230, 650, 430); a.expr = 1; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // 3° Lista
                    a = new TTZone();
                    GetZone(ref a, 690, 230, 980, 430); a.expr = 2; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // seconda riga
                    // 4° Lista
                    a = new TTZone();
                    GetZone(ref a, 170, 480, 450, 680); a.expr = 3; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // 5° Lista
                    a = new TTZone();
                    GetZone(ref a, 540, 480, 820, 680); a.expr = 4; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    break;
                // 6 Liste
                case 6:
                    // 1° Lista
                    a = new TTZone();
                    GetZone(ref a, 20, 230, 300, 430); a.expr = 0; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // 2° Lista
                    a = new TTZone();
                    GetZone(ref a, 340, 230, 650, 430); a.expr = 1; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // 3° Lista
                    a = new TTZone();
                    GetZone(ref a, 690, 230, 980, 430); a.expr = 2; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // seconda riga
                    // 4° Lista
                    a = new TTZone();
                    GetZone(ref a, 20, 480, 300, 680); a.expr = 3; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // 5° Lista
                    a = new TTZone();
                    GetZone(ref a, 340, 480, 650, 680); a.expr = 4; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    // 6° Lista
                    a = new TTZone();
                    GetZone(ref a, 690, 480, 980, 680); a.expr = 5; a.pag = 0; a.Multi = 0;
                    a.Text = ""; a.ev = TTEvento.steVotoValido;
                    TouchZoneVoto.Add(a);
                    break;
            }

            // Le schede Speciali
            MettiSchedeSpecialiDiVoto();

            // nella classe base c'è il bottone di uscita
            base.GetTouchVoteZone();
        }


    }
}
