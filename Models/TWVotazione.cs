using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoTouch.WPF
{
    public class TNewVotazione
    {
        public int IDVoto { get; set; }
        public int MozioneRealeGeas { get; set; }
        public int IDGruppoVoto { get; set; }
        public string Descrizione { get; set; }
        public int TipoVoto { get; set; }                //1.norm, 2.Lista, 3.Multi
        public int TipoSubVoto { get; set; }             // a seconda del tipo principale 
        public bool SkBianca { get; set; }               // ha scheda bianca
        public bool SkNonVoto { get; set; }              // ha il non voto
        public bool SkContrarioTutte { get; set; }       // ha il contraio a tutte       
        public bool SkAstenutoTutte { get; set; }       // ha il astenuto a tutte       
        public int MaxScelte { get; set; }               // n scelte max nel caso di multi
        public int MinScelte { get; set; }               // n scelte min nel caso di multi
        //public bool NeedConferma { get; set; }           // indica che dopo questa votazione necessita la conferma
        public bool AbilitaBottoneUscita { get; set; }
        public bool SelezionaTuttiCDA;

        public int NListe => Liste?.Count ?? 0;
        public int NPresentatoCDA => Liste?.Count(a => a.PresentatodaCDA == true) ?? 0;             
        public int NMultiSelezioni => TouchZoneVoto.TouchZone.Cast<TTZone>().Count(item => item.Multi > 0);

        public CBaseTipoVoto TouchZoneVoto;
        public TAreaVotazione AreaVoto;

        public List<TNewLista> Liste;     // collection di strutture Tliste
        public ArrayList Pagine;    // collection delle pagine (per le votazioni candidato)

        public TNewVotazione()
        {
            Liste = new List<TNewLista>();
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

        ~TNewVotazione()
        {
            // Distruttore
            Liste.Clear();
            Pagine.Clear();
        }
    }

    public class TNewLista
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

        public TNewLista()
        {
        }
    }

}
