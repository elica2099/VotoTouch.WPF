using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VotoTouch.WPF.Models
{
    public class CGroupSubVoto: INotifyPropertyChanged
    {
        public int NumSubVotaz { get; set; }
        public int MozioneRealeGeas { get; set; }
        public int TipoSubVoto { get; set; }
        private string _Argomento;
        public string Argomento
        {
            get => _Argomento;
            set
            {
                _Argomento = value;
                OnPropertyChanged("Argomento");
            }
        }
        private string _Descrizione_aggiuntiva;
        public string Descrizione_aggiuntiva
        {
            get => _Descrizione_aggiuntiva;
            set
            {
                _Descrizione_aggiuntiva = value;
                OnPropertyChanged("Descrizione_aggiuntiva");
            }
        }
        private TSubVotoExpr _VotoExprEnum;
        public TSubVotoExpr VotoExprEnum
        {
            get => _VotoExprEnum;
            set
            {
                _VotoExprEnum = value;
                OnPropertyChanged("VotoExprEnum");
            }
        }
        public bool SkNonVoto { get; set; }

        public int VotoExpr
        {
            get
            {
                switch (VotoExprEnum)
                {
                    case TSubVotoExpr.astenuto:
                        return VSDecl.GEAS_VOTO_ABS;
                    case TSubVotoExpr.contrario:
                        return VSDecl.GEAS_VOTO_CON;
                    case TSubVotoExpr.favorevole:
                        return VSDecl.GEAS_VOTO_FAV;
                    case TSubVotoExpr.non_votante:
                        return VSDecl.GEAS_VOTO_NV;
                }

                return VSDecl.GEAS_VOTO_NV;
            }
        }

        public string VotoExprDescr
        {
            get
            {
                switch (VotoExprEnum)
                {
                    case TSubVotoExpr.astenuto:
                        return "Astenuto";
                    case TSubVotoExpr.contrario:
                        return "Contrario";
                    case TSubVotoExpr.favorevole:
                        return "Favorevole";
                    case TSubVotoExpr.non_votante:
                        return "non_votante";
                }
                return "?";
            }
        }

        public CGroupSubVoto(CSubVotazione s)
        {
            NumSubVotaz = s.NumSubVotaz;
            MozioneRealeGeas = s.MozioneRealeGeas;
            TipoSubVoto = s.TipoSubVoto;
            Argomento = s.Argomento;
            Descrizione_aggiuntiva = s.Descrizione_aggiuntiva;
            VotoExprEnum = TSubVotoExpr.nessuno;
            SkNonVoto = s.SKNonVoto;
        }

        #region INotifyPropertyChanged
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string strPropertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(strPropertyName));
        }
        #endregion

    }


    public class CBaseVoto_UserControl : UserControl, INotifyPropertyChanged
    {
        protected int UNumVotaz;

        public CBaseVoto_UserControl()
        {
            //
            DataContext = this;
        }

        // METODI VIRTUALI configurazione ----------------------------------------------------------------------

        public virtual void SetVoteParameters(int ANumVotaz, List<CSubVotazione> ASubVoti)
        {
            //
        }

        // METODI VIRTUALI ritorno voto ----------------------------------------------------------------------

        public virtual List<CVotoEspresso> GetVotes()
        {
            // ritorna come ha votato
            return null;
        }

        public virtual List<string> GetVotesDescr()
        {
            // ritorna come ha votato
            return null;
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
