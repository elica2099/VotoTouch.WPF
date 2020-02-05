using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;

namespace VotoTouch.WPF.Models
{
    public class CVotazione_GruppoVoto: CVotazione
    {

        // CLASSE DELLA votazione con usercontrol
        
        public CVotazione_GruppoVoto(Rect AFormRect) : base(AFormRect)
        {
            // costruttore
        }

        // TOUCH ---------------------------------------------------------

        public override void GetTouchVoteZone()
        {
            TouchZoneVoto.Clear();

            // non serve i touch screen perchè uso lo UserControl
            // non metto la scheda di voto non votante perchè è dentro lo user control
            SkNonVoto = false;
            // devo aggiungere il tasto avanti con evento           
            TTZone a = new TTZone();
            GetZone(ref a, 700, 870, 1000, 1000); // in bass a sx
            a.expr = VSDecl.VOTO_GRUPPOAVANTI;
            a.Text = ""; a.ev = TTEvento.steGruppoAvanti; a.pag = 0; a.cda = false; a.Multi = 0;
            TouchZoneVoto.Add(a);

            // Le schede Speciali
            MettiSchedeSpecialiDiVoto();

            // nella classe base c'è il bottone di uscita
            base.GetTouchVoteZone();
        }

        // USERCONTROL ---------------------------------------------------------

        public override void GetVotoUserControl()
        {
            // questa funzione può essere chiamata per creare o per spostare lo usercontrol
            // es. resize quindi deve capire se è stato creato o no

            // mi costruisco l'usercontrol se è diverso da null
            if (UserControlVoto == null)
            {
                UserControlVoto = new UCWVotazione_GruppoVoto()
                {
                    Name = "UVotazione_GruppoVoto",
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Visibility = Visibility.Hidden,
                    Margin = new Thickness(10,80,10,120)
                };

                // resize dell'area

                // devo fare il binding delle subvotazioni


            }


        }


    }
}
