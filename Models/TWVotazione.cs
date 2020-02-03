using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoTouch.WPF.Models
{
    public class TVotazione
    {
        public int IDVoto;
        public int MozioneRealeGeas;
        public int IDGruppoVoto;
        public string Descrizione;
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

        public int NListe => Liste?.Count ?? 0;
        public int NPresentatoCDA => Liste?.Count(a => a.PresentatodaCDA == true) ?? 0;             
        public int NMultiSelezioni => TouchZoneVoto.TouchZone.Cast<TTZone>().Count(item => item.Multi > 0);

        public CBaseTipoVoto TouchZoneVoto;
        public TAreaVotazione AreaVoto;

        public List<TLista> Liste;     
        public ArrayList Pagine;    

        public TVotazione()
        {
            Liste = new List<TLista>();
            Pagine = new ArrayList();
            TouchZoneVoto = null;
            AbilitaBottoneUscita = false;
        }

        public int DammiMaxMultiCandSelezionabili()
        {
            return TipoVoto == VSDecl.VOTO_MULTICANDIDATO ? MaxScelte : 0;
        }

        public int DammiMinMultiCandSelezionabili()
        {
            return TipoVoto == VSDecl.VOTO_MULTICANDIDATO ? MinScelte : 0;
        }

        ~TVotazione()
        {
            // Distruttore
            Liste.Clear();
            Pagine.Clear();
        }
    }

    public class TSubVotazione
    {
    }

    public class TLista
    {
        public int NumVotaz;
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

}
