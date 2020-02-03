using System;
using System.Drawing;
//using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Media;
using System.Windows;
using VotoTouch.WPF.Models;

namespace VotoTouch.WPF.Touchscreen
{
    public class CBaseSpecialTouch : CTouch
    {

        protected ArrayList Tz;

        public ArrayList TouchZone => Tz ?? null;

        public CBaseSpecialTouch(Rect AFormRect): base(AFormRect)		
        {
            // costruttore
            Tz = new ArrayList();
        }

        //  FUNZIONI VIRTUALI --------------------------------------------------------------------------

        /*
        public virtual void GetTouchVoteZone(TVotazione AVotazione) //ref ArrayList Tz)
        {
            // l'implementazione è nelle varie classi

            //c 'è una parte comune
            // il Bottone Uscita
            if (!CustomPaint && AVotazione.AbilitaBottoneUscita)
            {
                TTZone a = new TTZone();
                GetZone(ref a, 760, 0, 980, 120); // in alto a dx
                a.expr = VSDecl.VOTO_BTN_USCITA;
                a.Text = ""; a.ev = TTEvento.steBottoneUscita; a.pag = 0; a.Multi = 0; a.MultiNoPrint = true;
                Tz.Add(a);
            }
        }
        */

        public virtual void GetTouchSpecialZone(TAppStato AStato, bool ADiffer, bool ABtnUscita) //, ref ArrayList Tz
        {
            // l'implementazione è nelle varie classi

            //c 'è una parte comune
            // il Bottone Uscita
            if (ABtnUscita)
            {
                TTZone a = new TTZone();
                GetZone(ref a, 760, 0, 980, 120); // in alto a dx
                a.expr = VSDecl.VOTO_BTN_USCITA;
                a.Text = ""; a.ev = TTEvento.steBottoneUscita; a.pag = 0; a.Multi = 0; a.MultiNoPrint = true;
                Tz.Add(a);
            }
        }




    }
}
