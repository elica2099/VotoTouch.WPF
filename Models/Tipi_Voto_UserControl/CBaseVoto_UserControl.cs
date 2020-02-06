using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VotoTouch.WPF.Models
{
    public class CBaseVoto_UserControl : UserControl, INotifyPropertyChanged
    {

        public CBaseVoto_UserControl()
        {
            //
            DataContext = this;
        }

        // METODI VIRTUALI configurazione ----------------------------------------------------------------------

        public virtual void SetVoteParameters(List<CSubVotazione> ASubVoti, bool ASkNonVoto)
        {
            //
        }

        // METODI VIRTUALI voto ----------------------------------------------------------------------

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

        //  inotify  ---------------------------------------------------------------------------------

        #region INotifyPropertyChanged
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string strPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(strPropertyName));
        }
        #endregion

    }
}
