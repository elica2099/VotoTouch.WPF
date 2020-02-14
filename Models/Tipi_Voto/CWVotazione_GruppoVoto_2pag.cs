using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;

namespace VotoTouch.WPF.Models
{
    public class CVotazione_GruppoVoto_2Pag: CVotazione
    {
        // CLASSE DELLA votazione con usercontrol del gruppo voto ma con 2 pagine

        private bool OriginalSkNoVoto;

        public CVotazione_GruppoVoto_2Pag(Rect AFormRect) : base(AFormRect)
        {
            // costruttore
        }

        // TOUCH ---------------------------------------------------------

        public override void GetTouchVoteZone()
        {
            TouchZoneVoto.Clear();

            // non serve i touch screen perchè uso lo UserControl
            // non metto la scheda di voto non votante perchè è dentro lo user control, però la salvo;
            // TODO: Gestione della sk non voto: brutta da rifare
            OriginalSkNoVoto = SkNonVoto;
            SkNonVoto = false;
            // ora devo aggiungere il 
            TTZone a = new TTZone();
            GetZone(ref a, 0, 110, 1000, 1000); // in bass a sx
            a.expr = 0;
            a.Text = ""; a.ev = TTEvento.steUserControl; a.pag = 0; a.cda = false; a.Multi = 0;
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
            if (UserControlVoto != null) return;
            UserControlVoto = new UCWVotazione_GruppoVoto_2Pag()
            {
                Name = "UVotazione_GruppoVoto_2Pag"+ "_" + NumVotaz.ToString(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Visibility = Visibility.Hidden,
                Margin = new Thickness(10,100,10,20)
            };

            // carico le subvotazioni e gli eventuali parametri
            UserControlVoto.SetVoteParameters(NumVotaz, SubVotazioni);
        }


    }
}
