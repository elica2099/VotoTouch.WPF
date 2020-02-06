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
    public enum TGroupSubVotoExpr {nessuno, favorevole, contrario, astenuto, non_votante}

    public class CGroupSubVoto: INotifyPropertyChanged
    {
        public int NumSubVotaz { get; set; }
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
        public int VotoExpr { get; set; }
        private TGroupSubVotoExpr _VotoExprEnum;
        public TGroupSubVotoExpr VotoExprEnum
        {
            get => _VotoExprEnum;
            set
            {
                _VotoExprEnum = value;
                OnPropertyChanged("VotoExprEnum");
            }
        }

        public bool SkNonVoto { get; set; }

        public CGroupSubVoto(CSubVotazione s)
        {
            NumSubVotaz = s.NumSubVotaz;
            TipoSubVoto = s.TipoSubVoto;
            Argomento = s.Argomento;
            Descrizione_aggiuntiva = s.Descrizione_aggiuntiva;
            VotoExpr = -1;
            VotoExprEnum = TGroupSubVotoExpr.nessuno;
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
            ListSubVoto =  new ObservableCollection<CGroupSubVoto>();
            CVListSubVoto = new CollectionViewSource { Source = ListSubVoto }.View;
            CVListSubVoto.CurrentChanged += new EventHandler(CVListSubVotoCurrentChanged);

            DataContext = this;
        }

        // selezione ---------------------------------------------------------------------------------

        public override void StartVote()
        {
            foreach (CGroupSubVoto subVoto in ListSubVoto)
            {
                subVoto.VotoExprEnum = TGroupSubVotoExpr.nessuno;
            }
            TxtHaiVotato = false;
        }

        public override void EndVote()
        {
            string Storyboard = "PleaseContinue";
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb?.Stop();
        }

        public override void SetVoteParameters(List<CSubVotazione> ASubVoti)
        {
            ListSubVoto.Clear();
            foreach (CSubVotazione subVotazione in ASubVoti)
            {
                ListSubVoto.Add(new CGroupSubVoto(subVotazione));
            }
            ShowNoVote = ListSubVoto.Count > 0 && ListSubVoto[0].SkNonVoto;
        }

        // selezione ---------------------------------------------------------------------------------

        private void CVListSubVotoCurrentChanged(object sender, EventArgs e)
        {
            CGroupSubVoto currentItem = (CGroupSubVoto)(CVListSubVoto.CurrentItem);
            Selected = currentItem ?? null;
        }

        private void CheckAndAnimatePleaseContinue()
        {
            string Storyboard = "PleaseContinue";
            Storyboard sb = Resources[Storyboard] as Storyboard;
            int tot = ListSubVoto.Count(x => x.VotoExprEnum != TGroupSubVotoExpr.nessuno);
            if (tot == ListSubVoto.Count)
            {
                TxtHaiVotato = true;
                sb?.Begin(btnContinua);
                sb?.Begin(txtHaiVotato);
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
                subVoto.VotoExprEnum = (TGroupSubVotoExpr)Tipovoto;
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

        
        #endregion


    }
}
