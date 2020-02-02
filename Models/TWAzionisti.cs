using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoTouch.WPF
{
    public class TAzionista
    {
        // dati dell'utente
        public int IDBadge { get; set; }
        public string CoAz { get; set; }
        public int IDAzion { get; set; }
        public int ProgDeleg { get; set; }
        public string RaSo { get; set; }
        public double NVoti { get; set; }
        public double Voti1 { get; set; }
        public double Voti2 { get; set; }
        public string Sesso { get; set; }
        public int HaVotato { get; set; }
        
        // dati del voto 
        public int IDVotaz { get; set; }
        // voti
        public List<TVotoEspresso> VotiEspressi;

        // test se sk nonvoto
        public bool HaNonVotato { 
            get
            {
                if (VTConfig.AbilitaDirittiNonVoglioVotare)
                    return VotiEspressi.Count(v => v.VotoExp_IDScheda == VSDecl.VOTO_NONVOTO) > 0;
                else
                    return false;
            } 
        }
        // Raso con Sig
        public string RaSo_Sesso
        {
            get
            {
                if (Sesso == "M")
                    return "Sig. " + RaSo;
                if (Sesso == "F")
                    return "Sig.ra " + RaSo;
                return RaSo;
            }
        }

        public TAzionista()
        {
            HaVotato = TListaAzionisti.VOTATO_NO;
            VotiEspressi = new List<TVotoEspresso>();
        }

        public void CopyFrom(ref TAzionista cp)
        {
            IDBadge = cp.IDBadge; CoAz = cp.CoAz; IDAzion = cp.IDAzion; ProgDeleg = cp.ProgDeleg;
            RaSo = cp.RaSo; NVoti = cp.NVoti; Sesso = cp.Sesso; HaVotato = cp.HaVotato;
            IDVotaz = cp.IDVotaz; Voti1 = cp.Voti1; Voti2 = cp.Voti2;
        }
    }
}
