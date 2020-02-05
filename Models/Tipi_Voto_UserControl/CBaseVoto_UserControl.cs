using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VotoTouch.WPF.Models
{
    public class CBaseVoto_UserControl : UserControl
    {

        public CBaseVoto_UserControl()
        {
            //
        }

        // METODI VIRTUALI ----------------------------------------------------------------------

        public virtual void StartVote()
        {
            // inizio del voto, le faranno le classi singole, può servire per inizializzare o resettare i voti
        }

        public virtual void EndVote()
        {
            // fine voto, vedi sopra
        }

        public virtual List<TVotoEspresso> GetVotes()
        {
            // ritorna come ha votato
            return null;
        }

    }
}
