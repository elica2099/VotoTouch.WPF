using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VotoTouch.WPF.Models
{
    public enum TGroupSubVotoExpr {nessuno, favorevole, contrario, astenuto, non_votante}

    public class CGroupSubVoto
    {
        public int NumSubVotaz { get; set; }
        public int TipoSubVoto { get; set; }
        public string Argomento { get; set; }
        public string Descrizione_aggiuntiva { get; set; }
        public int VotoExpr { get; set; }
        public TGroupSubVotoExpr VotoExprEnum { get; set; }

        public CGroupSubVoto(CSubVotazione s)
        {
            NumSubVotaz = s.NumSubVotaz;
            TipoSubVoto = s.TipoSubVoto;
            Argomento = s.Argomento;
            Descrizione_aggiuntiva = s.Descrizione_aggiuntiva;
            VotoExpr = -1;
            VotoExprEnum = TGroupSubVotoExpr.nessuno;
        }
    }

    public partial class UCWVotazione_GruppoVoto : CBaseVoto_UserControl
    {

        #region Model
        private ICollectionView cvListSubVoto;
        public ICollectionView CVListSubVoto
        {
            get => cvListSubVoto;
            set
            {
                cvListSubVoto = value;
                OnPropertyChanged("CVListSubVoto");
            }
        }
        private ObservableCollection<CGroupSubVoto> ListSubVoto;
        #endregion
        private bool SkNonVoto = false;

        public UCWVotazione_GruppoVoto() : base()
        {
            InitializeComponent();
            ListSubVoto =  new ObservableCollection<CGroupSubVoto>();
            CVListSubVoto = new CollectionViewSource { Source = ListSubVoto }.View;
            CVListSubVoto.CurrentChanged += new EventHandler(CVListSubVotoCurrentChanged);

            DataContext = this;
        }

        public override void SetVoteParameters(List<CSubVotazione> ASubVoti, bool ASkNonVoto)
        {
            ListSubVoto.Clear();
            foreach (CSubVotazione subVotazione in ASubVoti)
            {
                ListSubVoto.Add(new CGroupSubVoto(subVotazione));
            }

            // sk non voto
            SkNonVoto = ASkNonVoto;
        }

        // selezione ---------------------------------------------------------------------------------

        private void CVListSubVotoCurrentChanged(object sender, EventArgs e)
        {
            //CBindRadiovoter currentItem = (CBindRadiovoter)(CVListRvt.CurrentItem);
            //Selected = currentItem ?? null;
        }


    }
}
