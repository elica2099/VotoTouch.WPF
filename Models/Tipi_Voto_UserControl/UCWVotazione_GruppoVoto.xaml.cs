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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VotoTouch.WPF.Models
{
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
        private CGroupSubVoto Selected;

        #endregion

        public UCWVotazione_GruppoVoto() : base()
        {
            InitializeComponent();
            ListSubVoto = new ObservableCollection<CGroupSubVoto>();
            CVListSubVoto = new CollectionViewSource {Source = ListSubVoto}.View;
            CVListSubVoto.CurrentChanged += new EventHandler(CVListSubVotoCurrentChanged);

            DataContext = this;
        }

        // get dei voti

        public override List<CVotoEspresso> GetVotes()
        {
            // ritorna i voti espressi
            return ListSubVoto.Select(subVoto => new CVotoEspresso()
            {
                NumVotaz = UNumVotaz, NumSubVotaz = subVoto.NumSubVotaz, VotoExp_IDScheda = subVoto.VotoExpr
            }).ToList();
        }

        public override List<string> GetVotesDescr()
        {
            // ritorna come ha votato
            return ListSubVoto.Select(subVoto => subVoto.Argomento + ": " + subVoto.VotoExprDescr).ToList();
        }

        // selezione ---------------------------------------------------------------------------------

        public override void StartVote()
        {
            foreach (CGroupSubVoto subVoto in ListSubVoto)
            {
                subVoto.VotoExprEnum = TSubVotoExpr.nessuno;
            }
            PleaseContinue(false);
            TxtHaiVotato = false;
        }

        public override void EndVote()
        {
            PleaseContinue(false);
        }

        public override void SetVoteParameters(int ANumVotaz, List<CSubVotazione> ASubVoti)
        {
            UNumVotaz = ANumVotaz;
            ListSubVoto.Clear();
            foreach (CSubVotazione subVotazione in ASubVoti)
            {
                ListSubVoto.Add(new CGroupSubVoto(subVotazione));
            }

            ShowNoVote = ListSubVoto.Count > 0 && ListSubVoto[0].SkNonVoto;
        }

        private void PleaseContinue(bool iswork)
        {
            string Storyboard = "PleaseContinue";
            Storyboard sb = Resources[Storyboard] as Storyboard;
            if (iswork)
            {
                sb?.Begin(btnContinua, true);
                sb?.Begin(txtHaiVotato, true);
            }
            else
            {
                sb?.Stop(btnContinua);
                sb?.Stop(txtHaiVotato);
               
            }
        }

        // selezione ---------------------------------------------------------------------------------

        private void CVListSubVotoCurrentChanged(object sender, EventArgs e)
        {
            CGroupSubVoto currentItem = (CGroupSubVoto) (CVListSubVoto.CurrentItem);
            Selected = currentItem ?? null;
        }

        private void CheckAndAnimatePleaseContinue()
        {
            int tot = ListSubVoto.Count(x => x.VotoExprEnum != TSubVotoExpr.nessuno);
            if (tot == ListSubVoto.Count)
            {
                TxtHaiVotato = true;
                PleaseContinue(true);
            }
        }

        // Property OF UI -------------------------------------------------------------------------

        #region property ui

        private bool _ShowNoVote;

        public bool ShowNoVote
        {
            get => _ShowNoVote;
            set
            {
                _ShowNoVote = value;
                OnPropertyChanged("ShowNoVote");
            }
        }

        private bool _TxtHaiVotato;

        public bool TxtHaiVotato
        {
            get => _TxtHaiVotato;
            set
            {
                _TxtHaiVotato = value;
                OnPropertyChanged("TxtHaiVotato");
            }
        }

        #endregion

        // COMMANDS OF UI -------------------------------------------------------------------------

        #region Commands

        private ICommand _cmdVotatutto;
        private ICommand _cmdVotaSingolo;
        private ICommand _cmdGruppoContinua;

        #region _cmdVotatutto

        // _cmdCambiaVisualizzazione
        public ICommand Votatutto =>
            _cmdVotatutto ??
            (_cmdVotatutto = new RelayCommand(this.Votatutto_Execute, this.Votatutto_CanExecute));

        private bool Votatutto_CanExecute(object param) => true;

        private void Votatutto_Execute(object param)
        {
            int Tipovoto = Convert.ToInt32(param);
            foreach (CGroupSubVoto subVoto in ListSubVoto)
            {
                subVoto.VotoExprEnum = (TSubVotoExpr) Tipovoto;
            }

            CheckAndAnimatePleaseContinue();
        }

        #endregion

        #region _cmdVotaSingolo

        // _cmdCambiaVisualizzazione
        public ICommand VotaSingolo =>
            _cmdVotaSingolo ??
            (_cmdVotaSingolo = new RelayCommand(this.VotaSingolo_Execute, this.VotaSingolo_CanExecute));

        private bool VotaSingolo_CanExecute(object param) => true;

        private void VotaSingolo_Execute(object param)
        {
            int Tipovoto = Convert.ToInt32(param);
            CheckAndAnimatePleaseContinue();
        }

        #endregion

        #region _cmdGruppoContinua

        // _cmdCambiaVisualizzazione
        public ICommand GruppoContinua =>
            _cmdGruppoContinua ??
            (_cmdGruppoContinua = new RelayCommand(this.GruppoContinua_Execute, this.GruppoContinua_CanExecute));

        private bool GruppoContinua_CanExecute(object param) => true;

        private void GruppoContinua_Execute(object param)
        {
            App.ICMsn.NotifyColleaguesAsync(VSDecl.ICM_TOUCH_GROUPCONTINUE, null);
        }

        #endregion

        #endregion
    }
}