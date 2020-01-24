using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace VotoTouch.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for UStatusPanel.xaml
    /// </summary>
    public partial class UStatusPanel : UserControl
    {
        public UStatusPanel(TListaVotazioni AVotaz, TListaAzionisti AAzion)
        {
            InitializeComponent();

            //carica la lista
            CaricaListaStato(AVotaz, AAzion);
        }

        public void CaricaListaStato(TListaVotazioni AVotaz, TListaAzionisti AAzion)
        {
            // inizio con il caricare lo stato
            ObservableCollection<string> stato = new ObservableCollection<string>
            {
                VSDecl.VTS_VERSION,
#if _DBClose
                "DBClose version",
#endif
                "Configurazione",
                "Usalettore: " + VTConfig.UsaLettore.ToString() + " Porta: " + VTConfig.PortaLettore.ToString(),
                "UsaSemaforo: " + VTConfig.UsaSemaforo.ToString() + " IP: " + VTConfig.IP_Com_Semaforo.ToString(),
                "IDSeggio: " + VTConfig.IDSeggio.ToString() + " NomeComputer: " + VTConfig.NomeTotem,
                "ControllaPresenze: " + VTConfig.ControllaPresenze.ToString(),
                "     0: Non controllare, 1: Blocca",
                "     2: Forza Ingresso (ora) 3: Forza ingresso Geas",
                "MaxDeleghe: " + VTConfig.MaxDeleghe,
                "AbilitaDifferenziataSuRichiesta: " + VTConfig.AbilitaDifferenziatoSuRichiesta.ToString(),
                "CodiceUscita: " + VTConfig.CodiceUscita,
                "SalvaLinkVoto: " + VTConfig.SalvaLinkVoto.ToString(),
                "SalvaVotoNonConfermato: " + VTConfig.SalvaVotoNonConfermato.ToString(),
                "SalvaVoto In geas: " + VTConfig.SalvaVotoInGeas.ToString(),
                "IDSchedaUscitaForzata: " + VTConfig.IDSchedaUscitaForzata.ToString(),
                "AbilitaDirittiNonVoglioVotare: " + VTConfig.AbilitaDirittiNonVoglioVotare.ToString(),
                "TimerAutoritorno: " + VTConfig.AttivaAutoRitornoVoto.ToString(),
                "Tempo TimerAutoritorno (ms): " + VTConfig.TimeAutoRitornoVoto.ToString(),
                ""
            };
            if (AVotaz != null)
            {
                // ok ora le votazioni
                stato.Add("=== Votazioni ===");
                foreach (TNewVotazione fVoto in AVotaz.Votazioni)
                {
                    stato.Add("Voto: " + fVoto.IDVoto.ToString() + ", Tipo: " +
                              fVoto.TipoVoto.ToString() + ", " + fVoto.Descrizione);
                    stato.Add("   NListe: " + fVoto.NListe + ", MaxScelte: " +
                              fVoto.MaxScelte);
                    stato.Add("   SKBianca: " + fVoto.SkBianca.ToString() +
                              ", SKNonVoto: " + fVoto.SkNonVoto);
                    // Le liste
                    foreach (TNewLista a in fVoto.Liste)
                    {
                        stato.Add("    Lista:" + a.IDLista.ToString() + ", IdSk:" +
                                  a.IDScheda.ToString() + ", " + a.DescrLista + ", p" +
                                  a.Pag.ToString() + " " + a.PagInd + "  cda: " + a.PresentatodaCDA.ToString());
                    }
                }
                stato.Add("");
            }

            // ora gli azionisti
            if (AAzion != null)
            {
                stato.Add("=== Azionisti ===");
                foreach (TAzionista c in AAzion.Azionisti)
                {
                    stato.Add("Badge: " + c.IDBadge.ToString() + " " + c.RaSo.Trim());
                    stato.Add("   IDazion:" + c.IDAzion.ToString() + " *** IDVotaz: " + c.IDVotaz.ToString());
                    stato.Add("   ProgDeleg:" + c.ProgDeleg.ToString() + " Coaz:" + c.CoAz +
                              " AzOrd: " + c.NVoti.ToString());
                }
            }
            // bindo
            listBox.ItemsSource = stato;
        }

        private void CancellaVoti_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RicaricaListe_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Test_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ChiudiApp_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
