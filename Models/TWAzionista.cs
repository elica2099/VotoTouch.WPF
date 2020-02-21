using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoTouch.WPF.Models
{
    public class TAzionista
    {
        // dati dell'utente
        public int IDBadge;
        public string CoAz;
        public int IDAzion;
        public int ProgDeleg;
        public string RaSo;
        public double NVoti;
        public double Voti1;
        public double Voti2;
        public double NAzioni;
        public double Azioni1;
        public double Azioni2;
        public string Sesso;
        public int HaVotato;
        // dati del voto 
        public int IDVotaz;
        // voti
        public List<CVotoEspresso> VotiEspressi;

        // test se sk nonvoto
        public bool HaNonVotato => VTConfig.AbilitaDirittiNonVoglioVotare && 
                                   VotiEspressi.Count(v => v.VotoExp_IDScheda == VSDecl.VOTO_NONVOTO) > 0;
        public string RaSo_Sesso => Sesso == "M" ? ("Sig. " + RaSo) : ("Sig.ra " + RaSo);

        public TAzionista()
        {
            HaVotato = TListaAzionisti.VOTATO_NO;
            VotiEspressi = new List<CVotoEspresso>();
        }

        public void CopyFrom(ref TAzionista cp)
        {
            IDBadge = cp.IDBadge; CoAz = cp.CoAz; IDAzion = cp.IDAzion; ProgDeleg = cp.ProgDeleg;
            RaSo = cp.RaSo; NVoti = cp.NVoti; Sesso = cp.Sesso; HaVotato = cp.HaVotato;
            IDVotaz = cp.IDVotaz; Voti1 = cp.Voti1; Voti2 = cp.Voti2;
            Azioni1 = cp.Azioni1; Azioni2 = cp.Azioni2;
        }
    }
}
