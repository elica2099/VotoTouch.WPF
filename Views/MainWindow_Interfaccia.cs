﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VotoTouch.WPF.Views.UserControls;

namespace VotoTouch.WPF
{
    // INTERFACCIA

    public partial class MainWindow : Window
    {
        // DR16 - Classe intera

        //	GESTIONE DELL'AREA SENSIBILE ----------------------------------------------------------

        private void frmMain_MouseUp(object sender, MouseEventArgs e)
        {
            // chiamo il metodo in CTouchscreen che mi ritornerà eventi diversi a seconda del caso
            oVotoTouch.TastoPremuto(sender, e, Stato);
        }

        //   Creazione dei controlli ----------------------------------------------------------------

        private void CreaControlli()
        {
            // qua creo i controlli dell'applicazone

            // innanzitutto creo in caso di debug
            if (VTConfig.IsDebugMode)
            {
                //creo la label del mouse
                TextBlock lblMouse = new TextBlock()
                {
                    Name = "lblMouse",
                    Text = "0 : 0",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    TextAlignment = TextAlignment.Right,
                    Margin = new Thickness(0,10,10,0),
                    Visibility = Visibility.Visible
                };
                mainGrid.Children.Add(lblMouse);
                mainGrid.RegisterName(lblMouse.Name, lblMouse);
                // creo il pannello del badge
                UBadgePanel badgePanel = new UBadgePanel()
                {
                    Name = "badgePanel",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(40,40,0,0),
                    Visibility = Visibility.Visible
                };
                mainGrid.Children.Add(badgePanel);
                mainGrid.RegisterName(badgePanel.Name, badgePanel);
            }



        }


        //   Caricamento Tema da VotoTheme ----------------------------------------------------------------

        private void CaricaTemaInControlli()
        {
            // carico il tema da vototouch, semplicemente richiamando le singole label
            oVotoTheme.SetTheme_lbNomeDisgiunto(ref lbNomeDisgiunto);
            oVotoTheme.SetTheme_lbDisgiuntoRimangono(ref lbDisgiuntoRimangono);
            oVotoTheme.SetTheme_lbDirittiStart(ref lbDirittiStart);
            oVotoTheme.SetTheme_lbDirittiDiVoto(ref lbDirittiDiVoto);
            oVotoTheme.SetTheme_lbConferma(ref lbConferma);
            oVotoTheme.SetTheme_lbConfermaUp(ref lbConfermaUp);
            oVotoTheme.SetTheme_lbConfermaNVoti(ref lbConfermaNVoti);
            oVotoTheme.SetTheme_lbNomeAzStart(ref lbNomeAzStart);
        }

        // Inizializzazione dei controlli ----------------------------------------------------------------

        private void InizializzaControlli()
        {
            //Font MyFont = new Font(VSDecl.BTN_FONT_NAME, VSDecl.BTN_FONT_SIZE, FontStyle.Bold);

            lbDirittiStart.BackColor = VTConfig.IsPaintTouch ? Color.Tan : Color.Transparent;
            lbDirittiDiVoto.BackColor = VTConfig.IsPaintTouch ? Color.Coral : Color.Transparent;
            // il pannello della conferma
            lbConferma.BackColor = VTConfig.IsPaintTouch ? Color.Red : Color.Transparent;
            lbConfermaUp.BackColor = VTConfig.IsPaintTouch ? Color.Turquoise : Color.Transparent;
            lbConfermaNVoti.BackColor = VTConfig.IsPaintTouch ? Color.GreenYellow : Color.Transparent;

            if (VTConfig.IsDebugMode) pnBadge.Visible = true;
        }

        //  SETTAGGIO DEI COMPONENTI A INIZIO CAMBIO STATO ----------------------------------------------------------------

        private void SettaComponenti(bool AVisibile)
        {
            lbConferma.Visible = AVisibile;
            lbConfermaUp.Visible = AVisibile;
            lbConfermaNVoti.Visible = AVisibile;
            // label del differenziato
            lbDirittiStart.Visible = AVisibile;

            //lbNome.Visible = AVisibile;
            lbNomeDisgiunto.Visible = AVisibile;
            lbDisgiuntoRimangono.Visible = AVisibile;
            lbNomeAzStart.Visible = AVisibile;

            if (VTConfig.IsDemoMode)
            {
                Button btnBadgeUnVoto = (Button) this.mainGrid.FindName("btnBadgeUnVoto");
                if (btnBadgeUnVoto != null)
                    btnBadgeUnVoto.Visibility = Stato == TAppStato.ssvBadge ? Visibility.Visible : Visibility.Hidden;
                Button btnBadgePiuVoti = (Button) this.mainGrid.FindName("btnBadgePiuVoti");
                if (btnBadgePiuVoti != null)
                    btnBadgePiuVoti.Visibility = Stato == TAppStato.ssvBadge ? Visibility.Visible : Visibility.Hidden;
                Button btnFineVotoDemo = (Button) this.mainGrid.FindName("btnFineVotoDemo");
                if (btnFineVotoDemo != null)
                    btnFineVotoDemo.Visibility = Stato == TAppStato.ssvVotoFinito ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void TornaInizio()
        {
            // dall'inizio
            timAutoRitorno.Stop();
            Stato = TAppStato.ssvBadge;
            CambiaStato();
        }

        // STATO DI START VOTE ----------------------------------------------------------

        private void MettiComponentiStartVoto()
        {
            string PrefNomeAz = "";
            // start del voto
            SettaComponenti(false);
            // le labels
            // nome azionista
            PrefNomeAz = Azionisti.Titolare_Badge.RaSo_Sesso;

            PrefNomeAz = UppercaseWords(PrefNomeAz.ToLower());
            lbNomeAzStart.Text = PrefNomeAz;
            lbNomeAzStart.Visible = true;
            // diritti di voto  
            if (VTConfig.ModoAssemblea == VSDecl.MODO_AGM_POP)
            {
                string ss = string.Format("{0:N0}", Azionisti.DammiMaxNumeroDirittiDiVotoTotali());
                lbDirittiDiVoto.Text = ss + App.Instance.getLang("SAPP_VOTE_D_DIRITTI"); // " Diritti di voto";
                lbDirittiStart.Text = ss;
            }
            else
            {
                string ss = string.Format("{0:N0}", Azionisti.DammiMaxNumeroVotiTotali());
                lbDirittiDiVoto.Text = ss;
                lbDirittiStart.Text = ss;
            }
            // diritti di voto
            lbDirittiStart.Visible = true;
            // in funzione del n. di deleghe metto
            if (Azionisti.HaDirittiDiVotoMultipli())
            {
                // verifico se ho AbilitaDifferenziatoSuRichiesta Attivato
                if (VTConfig.AbilitaDifferenziatoSuRichiesta)
                {
                    oVotoImg.LoadImages(LocalAbilitaVotazDifferenziataSuRichiesta
                                            ? VSDecl.IMG_VotostartD
                                            : VSDecl.IMG_Votostart1);
                }
                else
                {
                    // si comporta normalmente
                    oVotoImg.LoadImages(VSDecl.IMG_VotostartD);
                }
            }
            else
            {
                // immagine di 1 voto
                oVotoImg.LoadImages(VSDecl.IMG_Votostart1);
            }
        }


        // CONFERMA/SALVATAGGIO DEL VOTO ----------------------------------------------------------------

        private void MettiComponentiConferma()
        {
            //bool NODirittiLabel = false;

            // crea la pagina di conferma
            //SettaComponenti(false);
            lbDirittiDiVoto.Visible = true;
            // Sistemo la label dei diritti di voto
            int NDirittiAzioniConferma = Azionisti.DammiDirittiAzioniDiVotoConferma(IsVotazioneDifferenziata);
            lbConfermaNVoti.Text = string.Format("{0:N0}", NDirittiAzioniConferma) + " voti per"; // +rm.GetString("SAPP_VOTE_DIRITTIPER");

            if (VTConfig.ModoAssemblea == VSDecl.MODO_AGM_POP)
            {
                if (IsVotazioneDifferenziata)
                    lbConfermaNVoti.Text = App.Instance.getLang("SAPP_VOTE_1DIRITTOPER"); //"1 diritto di voto per";
                else
                {
                    if (!Azionisti.HaDirittiDiVotoMultipli())
                        lbConfermaNVoti.Text = App.Instance.getLang("SAPP_VOTE_1DIRITTOPER"); //"1 diritto di voto per";
                    else
                        lbConfermaNVoti.Text = Azionisti.DammiCountDirittiDiVoto_VotoCorrente() +
                                               App.Instance.getLang("SAPP_VOTE_DIRITTIPER"); //" diritti di voto per";
                }
            }
            else
            {
                // TODO: lbConfermaNVoti METTERE AZIONI????
            }

            // ok, per ora distinguiamo tra i due metodi di voto, quello normale e quello multicandidato
            // che ha i voti salvati in una collection
            // in un secondo tempo dovrà essere unificato
            if (Votazioni.VotoCorrente.TipoVoto == VSDecl.VOTO_MULTICANDIDATO)
            {
                /*
                // ciclo e metto i candidati
                TVotoEspresso vt;
                bool acapo = false;
                lbConfermaUp.Text = "";
                lbConferma.Text = "";
                int cnt = FVotiExpr.Count;
                // ok, ora riempio la collection di voti
                for (int i = 0; i < cnt; i++)
                {
                    vt = (TVotoEspresso)FVotiExpr[i];

                    // se è sk bianca o non voto non metto i diritti
                    NODirittiLabel = (vt.VotoExp_IDScheda == VSDecl.VOTO_SCHEDABIANCA || vt.VotoExp_IDScheda == VSDecl.VOTO_NONVOTO);

                    lbConfermaUp.Text += vt.StrUp_DescrLista;
                    if (acapo)
                        lbConfermaUp.Text += "\n";
                    else
                    {
                        if (i < (cnt - 1))   // per evitarmi l'ultimo " - "
                            lbConfermaUp.Text += "  -  ";
                    }
                    acapo = !acapo;
                }
                 * */
                lbConferma.Text = VotoEspressoStrUp;
                lbConferma.TextNote = VotoEspressoStrNote;
                lbConferma.Visible = true;
                //oVotoTheme.SetTheme_lbConfermaUp_Cand(ref lbConfermaUp);
            }
            else
            {
                // se è sk bianca o non voto non metto i diritti
                //NODirittiLabel = (VotoEspresso == VSDecl.VOTO_SCHEDABIANCA || VotoEspresso == VSDecl.VOTO_NONVOTO);
                if (VotoEspresso == VSDecl.VOTO_SCHEDABIANCA || VotoEspresso == VSDecl.VOTO_NONVOTO)
                    lbConfermaNVoti.Text = "-";
                // voto di lista/candidato              
                lbConfermaUp.Text = VotoEspressoStrUp;
                lbConferma.Text = VotoEspressoStr;
                lbConferma.TextNote = VotoEspressoStrNote;
                oVotoTheme.SetTheme_lbConfermaUp(ref lbConfermaUp);
            }

            // attenzione, se ho una sk bianca o non voto non metto i diritii
            //if (NODirittiLabel)
            //{
            //    lbConfermaNVoti.Text = "";
            //}

            // ok, ora le mostro
            lbConferma.Visible = true;
            lbConfermaNVoti.Visible = true;
            lbConfermaUp.Visible = true;
        }

		//    PARTE DEMO MODE ------------------------------------------------------------------

        #region Demo mode

        private void InizializzaControlliDemo()
        {
            Button btnBadgeUnVoto = new Button
            {
                Content = App.Instance.getLang("SAPP_DEMO_1DIR"),
                Name = "btnBadgeUnVoto",
                Width = 300,
                Height = 100,
                Background = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(4),
                FontSize = 20,
                FontWeight = FontWeights.DemiBold,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Visibility = Visibility.Hidden,
                Margin = new Thickness(100, 0, 0, 100)
            };
            btnBadgeUnVoto.Click += new RoutedEventHandler(this.btnBadgeUnVoto_Click);
            mainGrid.Children.Add(btnBadgeUnVoto);
            mainGrid.RegisterName(btnBadgeUnVoto.Name, btnBadgeUnVoto);

            Button btnBadgePiuVoto = new Button
            {
                Content = App.Instance.getLang("SAPP_DEMO_3DIR"),
                Name = "btnBadgePiuVoto",
                Width = 300,
                Height = 100,
                Background = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(4),
                FontSize = 20,
                FontWeight = FontWeights.DemiBold,
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 100, 100)
            };
            btnBadgePiuVoto.Click += new RoutedEventHandler(this.btnBadgePiuVoti_Click);
            mainGrid.Children.Add(btnBadgePiuVoto);
            mainGrid.RegisterName(btnBadgePiuVoto.Name, btnBadgePiuVoto);

            Button btnFineVotoDemo = new Button
            {
                Content = App.Instance.getLang("SAPP_DEMO_3DIR"),
                Name = "btnFineVotoDemo",
                Width = 300,
                Height = 100,
                Background = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(4),
                FontSize = 20,
                FontWeight = FontWeights.DemiBold,
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 100)
            };
            btnFineVotoDemo.Click += new RoutedEventHandler(this.btnFineVotoDemo_Click);
            mainGrid.Children.Add(btnFineVotoDemo);
            mainGrid.RegisterName(btnFineVotoDemo.Name, btnFineVotoDemo);
        }

        private void ResizeControlliDemo()
        {
            // per ora non serve in wpf

            // bottone un voto
            //if (btnBadgeUnVoto != null)
            //{
            //    btnBadgeUnVoto.Left = this.Width / 7;
            //    btnBadgeUnVoto.Top = (this.Height / 10) * 6;
            //    btnBadgeUnVoto.Width = (this.Width / 7) * 2;
            //    btnBadgeUnVoto.Height = (this.Height / 10) *2;
            //}
            //// bottone più voto
            //if (btnBadgePiuVoti != null)
            //{
            //    btnBadgePiuVoti.Left = (this.Width / 7) * 4;
            //    btnBadgePiuVoti.Top = (this.Height / 10) * 6;
            //    btnBadgePiuVoti.Width = (this.Width / 7) * 2;
            //    btnBadgePiuVoti.Height = (this.Height / 10) * 2;
            //}
            //// bottone finevotodemo
            //if (btnFineVotoDemo != null)
            //{
            //    btnFineVotoDemo.Left = (this.Width / 7) * 2;
            //    btnFineVotoDemo.Top = (this.Height / 10) * 6;
            //    btnFineVotoDemo.Width = (this.Width / 7) * 3;
            //    btnFineVotoDemo.Height = (this.Height / 10) * 2;
            //}
        }

        public void onChangeSemaphore(object source, TStatoSemaforo ASemStato)
        {
            // evento inutile
        }

        void btnBadgeUnVoto_Click(object sender, RoutedEventArgs e)
        {
            //1 voto
            BadgeLetto("1000");
        }

        void btnBadgePiuVoti_Click(object sender, RoutedEventArgs e)
        {
            //3 voti
            BadgeLetto("1001");
        }

        void btnFineVotoDemo_Click(object sender, RoutedEventArgs e)
        {
            BadgeLetto("999999");
        }


        #endregion

    }
}
