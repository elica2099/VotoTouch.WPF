using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VotoTouch.WPF.Touchscreen;

namespace VotoTouch.WPF.Models
{
    public class CVotazione : CTouch
    {
        public int NumVotaz;
        public int MozioneRealeGeas;
        public int IDGruppoVoto;
        public string Argomento;
        public int TipoVoto;                
        public int TipoSubVoto;            
        public bool SkBianca;              
        public bool SkNonVoto;             
        public bool SkContrarioTutte;      
        public bool SkAstenutoTutte;       
        public int MaxScelte;              
        public int MinScelte;              
        //public bool NeedConferma;        
        public bool AbilitaBottoneUscita;
        public bool SelezionaTuttiCDA;

        public List<CSubVotazione> SubVotazioni;
        public List<TTZone> TouchZoneVoto;
        public TAreaVotazione AreaVoto;
        public List<TLista> Liste;     
        public ArrayList Pagine;    

        // user control di voto
        public CBaseVoto_UserControl UserControlVoto = null;

        public int NListe => Liste?.Count ?? 0;
        public int NPresentatoCDA => Liste?.Count(a => a.PresentatodaCDA == true) ?? 0;             
        public int NMultiSelezioni => TouchZoneVoto.Count(item => item.Multi > 0);
        public bool HaSubVotazioni => SubVotazioni.Count > 0;
        public bool HaUserControl => UserControlVoto != null;

        // variabili interne
        protected bool CustomPaint = false;

        public CVotazione(Rect AFormRect) :base(AFormRect)
        {
            SubVotazioni = new List<CSubVotazione>();
            Liste = new List<TLista>();
            Pagine = new ArrayList();
            TouchZoneVoto = new List<TTZone>();
            AbilitaBottoneUscita = false;
        }

        ~CVotazione()
        {
            // Distruttore
            Liste.Clear();
            Pagine.Clear();
            TouchZoneVoto.Clear();
            SubVotazioni.Clear();
        }

        //  FUNZIONI VARIE ----------------------------------------------------------------------

        #region Funzioni Varie

        public int DammiMaxMultiCandSelezionabili()
        {
            return TipoVoto == VSDecl.VOTO_MULTICANDIDATO ? MaxScelte : 0;
        }

        public int DammiMinMultiCandSelezionabili()
        {
            return TipoVoto == VSDecl.VOTO_MULTICANDIDATO ? MinScelte : 0;
        }

        public void CopyFromDB_Votazione(CDB_Votazione AVotaz)
        {
            NumVotaz = AVotaz.DB_NumVotaz;
            MozioneRealeGeas = AVotaz.DB_MozioneRealeGeas;
            IDGruppoVoto = AVotaz.DB_IDGruppoVoto;
            Argomento = AVotaz.DB_Argomento;
            TipoVoto = AVotaz.DB_TipoVoto;
            TipoSubVoto = AVotaz.DB_TipoSubVoto;
            SkBianca = AVotaz.DB_SkBianca;
            SkNonVoto = AVotaz.DB_SkNonVoto;
            SkContrarioTutte = AVotaz.DB_SkContrarioTutte;
            SkAstenutoTutte = AVotaz.DB_SkAstenutoTutte;
            MaxScelte = AVotaz.DB_MaxScelte;
            MinScelte = AVotaz.DB_MinScelte;
            AbilitaBottoneUscita = AVotaz.DB_AbilitaBottoneUscita;
            SelezionaTuttiCDA = AVotaz.DB_SelezionaTuttiCDA;
        }

        #endregion

        //  FUNZIONI VIRTUALI TOUCHSCREEN ----------------------------------------------------------------------

        public virtual void GetTouchVoteZone() 
        {
            // l'implementazione è nelle varie classi

            //c 'è una parte comune: il Bottone Uscita
            if (CustomPaint || !AbilitaBottoneUscita) return;
            TTZone a = new TTZone();
            GetZone(ref a, 760, 0, 980, 120); // in alto a dx
            a.expr = VSDecl.VOTO_BTN_USCITA;
            a.Text = ""; a.ev = TTEvento.steBottoneUscita; a.pag = 0; a.Multi = 0; a.MultiNoPrint = true;
            TouchZoneVoto.Add(a);
        }

        //  SCHEDE SPECIALI TOUCH --------------------------------------------------------------

        #region schede speciali

        protected void MettiSchedeSpecialiDiVoto()
        {
            TTZone a;
            
            // le schede speciali possono essere 4
            // - Bianca - Contrario Tutti - Astenuto tutti - Cotinua (Multivoto)
            // solo  bianca : in centro
            // solo contrario e astenuto vanno in fila a sx
            // tutti e 3 : in teoria non dovrebbero andare

            // per evitarmi 1000 casi metto i casi + usati

            // solo sk Bianca
            if (SkBianca && !SkContrarioTutte && !SkAstenutoTutte)
            {
                a = new TTZone();
                // se c'è anche non voto devo spostarla
                if (VTConfig.ModoPosizioneAreeTouch == VSDecl.MODO_POS_TOUCH_NORMALE)
                    GetZone(ref a, 280, 720, 720, 930); // non la sposto sta in centro
                else
                    GetZone(ref a, 350, 760, 640, 930); // non la sposto sta in centro
                //if (!AVotazione.SkNonVoto)
                //    GetZone(ref a, 28, 74, 73, 90); // non la sposto sta in centro
                //else
                //    GetZone(ref a, 10, 72, 44, 90); //la sposto a sinistra
                a.expr = VSDecl.VOTO_SCHEDABIANCA;
                a.Text = ""; a.ev = TTEvento.steSkBianca; a.pag = 0; a.Multi = 0; a.MultiNoPrint = true;
                TouchZoneVoto.Add(a);
            }

            // ora solo Contrario + Astenuto
            if (!SkBianca && SkContrarioTutte && SkAstenutoTutte)
            {
                // Contrario A Tutti
                a = new TTZone();
                GetZone(ref a, 60, 760, 290, 930); // non la sposto sta in centro
                a.expr = VSDecl.VOTO_CONTRARIO_TUTTI;
                a.Text = ""; a.ev = TTEvento.steSkContrarioTutti; a.pag = 0; a.Multi = 0; a.MultiNoPrint = true;
                TouchZoneVoto.Add(a);
                // Astenuti A Tutti
                a = new TTZone();
                if (SkContrarioTutte)
                    GetZone(ref a, 340, 760, 570, 930); 
                else
                    GetZone(ref a, 60, 760, 290, 930); 
                a.expr = VSDecl.VOTO_ASTENUTO_TUTTI;
                a.Text = ""; a.ev = TTEvento.steSkAstenutoTutti; a.pag = 0; a.Multi = 0; a.MultiNoPrint = true;
                TouchZoneVoto.Add(a);
            }

            // il non voto, se presente (caso BPM)
            if (SkNonVoto)
            {
                a = new TTZone();
                // nella nuova versione è in basso a dx
                //if (!AVotazione.SkBianca)
                //    GetZone(ref a, 32, 72, 67, 90); // non la sposto, sta in centro
                //else
                //    GetZone(ref a, 55, 72, 89, 90); //la sposto a destra
                GetZone(ref a, 760, 870, 980, 1000); // in bass a sx
                a.expr = VSDecl.VOTO_NONVOTO;
                a.Text = ""; a.ev = TTEvento.steSkNonVoto; a.pag = 0; a.Multi = 0; a.MultiNoPrint = true;
                TouchZoneVoto.Add(a);
            }
        }
        
        #endregion

        //  USERControl --------------------------------------------------------------

        public virtual void GetVotoUserControl()
        {
            UserControlVoto = null;
        }

    }

    public class CSubVotazione
    {
        public int NumSubVotaz;
        public int MozioneRealeGeas;
        public int IDGruppoVoto;
        public int TipoSubVoto;                
        public string Argomento;

        public CSubVotazione()
        {
            // normale
        }

        public CSubVotazione(CDB_Votazione AVotaz)
        {
            NumSubVotaz = AVotaz.DB_NumVotaz;
            MozioneRealeGeas = AVotaz.DB_MozioneRealeGeas;
            IDGruppoVoto = AVotaz.DB_IDGruppoVoto;
            TipoSubVoto = AVotaz.DB_TipoVoto;
            Argomento = AVotaz.DB_Argomento;
        }
    }

    public class TLista
    {
        public int NumVotaz;
        public int NumSubVotaz;
        public int IDLista;
        public int IDScheda;
        public string DescrLista;
        public int TipoCarica;
        public bool PresentatodaCDA;
        public string Presentatore;
        public string Capolista;
        public string ListaElenco;
        public int Pag;
        public string PagInd;

        public TLista()
        {
        }

    }

    public class CDB_Votazione
    {
        // classe che mi serve come "ponte" per il db verso lista votazioni
        public int DB_NumVotaz;
        public int DB_MozioneRealeGeas;
        public int DB_IDGruppoVoto;
        public string DB_Argomento;
        public int DB_TipoVoto;
        public int DB_TipoSubVoto;
        public bool DB_SkBianca;
        public bool DB_SkNonVoto;
        public bool DB_SkContrarioTutte;
        public bool DB_SkAstenutoTutte;
        public int DB_MaxScelte;
        public int DB_MinScelte;
        //public bool NeedConferma;        
        public bool DB_AbilitaBottoneUscita;
        public bool DB_SelezionaTuttiCDA;
    }

}
