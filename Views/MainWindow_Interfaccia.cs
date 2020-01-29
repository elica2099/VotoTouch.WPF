using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using VotoTouch.WPF.Converters;
using VotoTouch.WPF.Views.UserControls;

namespace VotoTouch.WPF
{
    // INTERFACCIA

    public partial class MainWindow : Window
    {
        // DR16 - Classe intera


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

            // ok ora i label normali
            // nome disgiunto
            TextBlock lblNomeDisgiunto = new TextBlock()
            {
                Name = "lblNomeDisgiunto",
                Text = "lblNomeDisgiunto",
                Visibility = Visibility.Hidden,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20,20,0,0)
            };
            BindingOperations.SetBinding(lblNomeDisgiunto, TextBlock.TextProperty, new Binding("TxtNomeDisgiunto"));
            BindingOperations.SetBinding(lblNomeDisgiunto, TextBlock.VisibilityProperty, 
                new Binding("TxtNomeDisgiuntoVis"){ Converter = new BooleanToHiddenVisibility() });
            mainGrid.Children.Add(lblNomeDisgiunto);
            mainGrid.RegisterName(lblNomeDisgiunto.Name, lblNomeDisgiunto);
            
            // nome lblDisgiuntoRimangono
            TextBlock lblDisgiuntoRimangono = new TextBlock()
            {
                Name = "lblDisgiuntoRimangono",
                Text = "lblDisgiuntoRimangono",
                Visibility = Visibility.Hidden,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20,20,0,0)
            };
            BindingOperations.SetBinding(lblDisgiuntoRimangono, TextBlock.TextProperty, new Binding("TxtDisgiuntoRimangono"));
            BindingOperations.SetBinding(lblDisgiuntoRimangono, TextBlock.VisibilityProperty, 
                new Binding("TxtDisgiuntoRimangonoVis"){ Converter = new BooleanToHiddenVisibility() });
            mainGrid.Children.Add(lblDisgiuntoRimangono);
            mainGrid.RegisterName(lblDisgiuntoRimangono.Name, lblDisgiuntoRimangono);
            
            // nome lblDirittiStart
            TextBlock lblDirittiStart = new TextBlock()
            {
                Name = "lblDirittiStart",
                Text = "lblDirittiStart",
                Visibility = Visibility.Hidden,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = VTConfig.IsPaintTouch ? Brushes.Tan : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            BindingOperations.SetBinding(lblDirittiStart, TextBlock.TextProperty, new Binding("TxtDirittiStart"));
            BindingOperations.SetBinding(lblDirittiStart, TextBlock.VisibilityProperty, 
                new Binding("TxtDirittiStartVis"){ Converter = new BooleanToHiddenVisibility() });
            mainGrid.Children.Add(lblDirittiStart);
            mainGrid.RegisterName(lblDirittiStart.Name, lblDirittiStart);

            // nome lblDirittiStartMin
            TextBlock lblDirittiStartMin = new TextBlock()
            {
                Name = "lblDirittiStartMin",
                Text = "lblDirittiStartMin",
                Visibility = Visibility.Hidden,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = VTConfig.IsPaintTouch ? Brushes.Tan : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            BindingOperations.SetBinding(lblDirittiStartMin, TextBlock.TextProperty, new Binding("TxtDirittiStartMin"));
            BindingOperations.SetBinding(lblDirittiStartMin, TextBlock.VisibilityProperty, 
                new Binding("TxtDirittiStartMinVis"){ Converter = new BooleanToHiddenVisibility() });
            mainGrid.Children.Add(lblDirittiStartMin);
            mainGrid.RegisterName(lblDirittiStartMin.Name, lblDirittiStartMin);

            // lblDirittiDiVoto
            TextBlock lblDirittiDiVoto = new TextBlock()
            {
                Name = "lblDirittiDiVoto",
                Text = "lblDirittiDiVoto",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TextWrapping = TextWrapping.Wrap,
                Background = VTConfig.IsPaintTouch ? Brushes.Coral : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            BindingOperations.SetBinding(lblDirittiDiVoto, TextBlock.TextProperty, new Binding("TxtDirittiDiVoto"));
            BindingOperations.SetBinding(lblDirittiDiVoto, TextBlock.VisibilityProperty, 
                new Binding("TxtDirittiDiVotoVis"){ Converter = new BooleanToHiddenVisibility() });
            mainGrid.Children.Add(lblDirittiDiVoto);
            mainGrid.RegisterName(lblDirittiDiVoto.Name, lblDirittiDiVoto);

            // lbConferma
            TextBlock lblConferma = new TextBlock()
            {
                Name = "lblConferma",
                Text = "lblConferma",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TextWrapping = TextWrapping.Wrap,
                Background = VTConfig.IsPaintTouch ? Brushes.Red : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            BindingOperations.SetBinding(lblConferma, TextBlock.TextProperty, new Binding("TxtConferma"));
            BindingOperations.SetBinding(lblConferma, TextBlock.VisibilityProperty, 
                new Binding("TxtConfermaVis"){ Converter = new BooleanToHiddenVisibility() });
            mainGrid.Children.Add(lblConferma);
            mainGrid.RegisterName(lblConferma.Name, lblConferma);
            
            // lbConfermaUp
            TextBlock lblConfermaUp = new TextBlock()
            {
                Name = "lblConfermaUp",
                Text = "lblConfermaUp",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TextWrapping = TextWrapping.Wrap,
                Background = VTConfig.IsPaintTouch ? Brushes.Turquoise : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            BindingOperations.SetBinding(lblConfermaUp, TextBlock.TextProperty, new Binding("TxtConfermaUp"));
            BindingOperations.SetBinding(lblConfermaUp, TextBlock.VisibilityProperty, 
                new Binding("TxtConfermaUpVis"){ Converter = new BooleanToHiddenVisibility() });
            mainGrid.Children.Add(lblConfermaUp);
            mainGrid.RegisterName(lblConfermaUp.Name, lblConfermaUp);
            
            // lbConfermaNVoti
            TextBlock lblConfermaNVoti = new TextBlock()
            {
                Name = "lblConfermaNVoti",
                Text = "lblConfermaNVoti",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TextWrapping = TextWrapping.Wrap,
                Background = VTConfig.IsPaintTouch ? Brushes.GreenYellow : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            BindingOperations.SetBinding(lblConfermaNVoti, TextBlock.TextProperty, new Binding("TxtConfermaNVoti"));
            BindingOperations.SetBinding(lblConfermaNVoti, TextBlock.VisibilityProperty, 
                new Binding("TxtConfermaNVotiVis"){ Converter = new BooleanToHiddenVisibility() });
            mainGrid.Children.Add(lblConfermaNVoti);
            mainGrid.RegisterName(lblConfermaNVoti.Name, lblConfermaNVoti);
            
            // lblNomeAzStart
            TextBlock lblNomeAzStart = new TextBlock()
            {
                Name = "lblNomeAzStart",
                Text = "lblNomeAzStart",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20,20,0,0)
            };
            BindingOperations.SetBinding(lblNomeAzStart, TextBlock.TextProperty, new Binding("TxtNomeAzStart"));
            BindingOperations.SetBinding(lblNomeAzStart, TextBlock.VisibilityProperty, 
                new Binding("TxtNomeAzStartVis"){ Converter = new BooleanToHiddenVisibility() });
            mainGrid.Children.Add(lblNomeAzStart);
            mainGrid.RegisterName(lblNomeAzStart.Name, lblNomeAzStart);

            // multiselezione/candidati
            // lblNSelezioni
            TextBlock lblNSelezioni = new TextBlock()
            {
                Name = "lblNSelezioni",
                Text = "lblNSelezioni",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20,20,0,0)
            };
            BindingOperations.SetBinding(lblNSelezioni, TextBlock.TextProperty, new Binding("TxtNSelezioni"));
            BindingOperations.SetBinding(lblNSelezioni, TextBlock.VisibilityProperty, 
                new Binding("TxtNSelezioniVis"){ Converter = new BooleanToHiddenVisibility() });
            mainGrid.Children.Add(lblNSelezioni);
            mainGrid.RegisterName(lblNSelezioni.Name, lblNSelezioni);

            // lblCandidati_PresCDA
            TextBlock lblCandidati_PresCDA = new TextBlock()
            {
                Name = "lblCandidati_PresCDA",
                Text = "lblCandidati_PresCDA",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20,20,0,0)
            };
            BindingOperations.SetBinding(lblCandidati_PresCDA, TextBlock.TextProperty, new Binding("TxtCandidati_PresCDA"));
            BindingOperations.SetBinding(lblCandidati_PresCDA, TextBlock.VisibilityProperty, 
                new Binding("TxtCandidati_PresCDAVis"){ Converter = new BooleanToHiddenVisibility() });
            mainGrid.Children.Add(lblCandidati_PresCDA);
            mainGrid.RegisterName(lblCandidati_PresCDA.Name, lblCandidati_PresCDA);

            // lblCandidati_Altern
            TextBlock lblCandidati_Altern = new TextBlock()
            {
                Name = "lblCandidati_Altern",
                Text = "lblCandidati_Altern",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20,20,0,0)
            };
            BindingOperations.SetBinding(lblCandidati_Altern, TextBlock.TextProperty, new Binding("TxtCandidati_Altern"));
            BindingOperations.SetBinding(lblCandidati_Altern, TextBlock.VisibilityProperty, 
                new Binding("TxtCandidati_AlternVis"){ Converter = new BooleanToHiddenVisibility() });
            mainGrid.Children.Add(lblCandidati_Altern);
            mainGrid.RegisterName(lblCandidati_Altern.Name, lblCandidati_Altern);
        }

        //   Caricamento Tema da VotoTheme ----------------------------------------------------------------

        private void CaricaTemaInControlli()
        {
            TextBlock lblDirittiStart = (TextBlock) this.mainGrid.FindName("lblDirittiStart");
            if (lblDirittiStart != null) oVotoTheme.SetTheme_lbDirittiStart(ref lblDirittiStart);

            TextBlock lblDirittiStartMin = (TextBlock) this.mainGrid.FindName("lblDirittiStartMin");
            if (lblDirittiStartMin != null) oVotoTheme.SetTheme_lbDirittiStartMin(ref lblDirittiStartMin);

            TextBlock lblDirittiDiVoto = (TextBlock) this.mainGrid.FindName("lblDirittiDiVoto");
            if (lblDirittiDiVoto != null) oVotoTheme.SetTheme_lbDirittiDiVoto(ref lblDirittiDiVoto);

            // carico il tema da vototouch, semplicemente richiamando le singole label
            TextBlock lblNomeDisgiunto = (TextBlock) this.mainGrid.FindName("lblNomeDisgiunto");
            if (lblNomeDisgiunto != null) oVotoTheme.SetTheme_lbNomeDisgiunto(ref lblNomeDisgiunto);

            TextBlock lblDisgiuntoRimangono = (TextBlock) this.mainGrid.FindName("lblDisgiuntoRimangono");
            if (lblDisgiuntoRimangono != null) oVotoTheme.SetTheme_lbDisgiuntoRimangono(ref lblDisgiuntoRimangono);

            TextBlock lblConferma = (TextBlock) this.mainGrid.FindName("lblConferma");
            if (lblConferma != null) oVotoTheme.SetTheme_lbConferma(ref lblConferma);

            TextBlock lblConfermaUp = (TextBlock) this.mainGrid.FindName("lblConfermaUp");
            if (lblConfermaUp != null) oVotoTheme.SetTheme_lbConfermaUp(ref lblConfermaUp);

            TextBlock lblConfermaNVoti = (TextBlock) this.mainGrid.FindName("lblConfermaNVoti");
            if (lblConfermaNVoti != null) oVotoTheme.SetTheme_lbConfermaNVoti(ref lblConfermaNVoti);

            TextBlock lblNomeAzStart = (TextBlock) this.mainGrid.FindName("lblNomeAzStart");
            if (lblNomeAzStart != null) oVotoTheme.SetTheme_lbNomeAzStart(ref lblNomeAzStart);

            TextBlock lblNSelezioni = (TextBlock) this.mainGrid.FindName("lblNSelezioni");
            if (lblNSelezioni != null) oVotoTheme.SetTheme_lbNSelezioni(ref lblNSelezioni);

            TextBlock lblCandidati_PresCDA = (TextBlock) this.mainGrid.FindName("lblCandidati_PresCDA");
            if (lblCandidati_PresCDA != null) oVotoTheme.SetTheme_lbCandidati_PresCDA(ref lblCandidati_PresCDA);

            TextBlock lblCandidati_Altern = (TextBlock) this.mainGrid.FindName("lblCandidati_Altern");
            if (lblCandidati_Altern != null) oVotoTheme.SetTheme_lbCandidati_Altern(ref lblCandidati_Altern);

        }

        //  SETTAGGIO DEI COMPONENTI A INIZIO CAMBIO STATO ----------------------------------------------------------------

        private void SettaComponenti(bool AVisibile)
        {
            // setto la visibilità
            TxtConfermaVis = AVisibile;
            TxtConfermaUpVis = AVisibile;
            TxtConfermaNVotiVis = AVisibile;
            // label del differenziato
            TxtDirittiStartVis = AVisibile;
            TxtDirittiStartMinVis = AVisibile;
            //TxtNomeVis = AVisibile;
            TxtNomeDisgiuntoVis = AVisibile;
            TxtDisgiuntoRimangonoVis = AVisibile;
            TxtNomeAzStartVis = AVisibile;

            TxtNSelezioniVis = AVisibile;
            TxtCandidati_PresCDAVis = AVisibile;
            TxtCandidati_AlternVis = AVisibile;

            // se non è in demo esco
            if (!VTConfig.IsDemoMode) return;
            // demo mode
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
            TxtNomeAzStart = PrefNomeAz;
            TxtNomeAzStartVis = true;
            // diritti di voto  
            if (VTConfig.ModoAssemblea == VSDecl.MODO_AGM_POP)
            {
                string ss = $"{Azionisti.DammiMaxNumeroDirittiDiVotoTotali():N0}";
                TxtDirittiDiVoto = ss + App.Instance.getLang("SAPP_VOTE_D_DIRITTI"); // " Diritti di voto";
                TxtDirittiStart = ss;
            }
            else
            {
                string ss = $"{Azionisti.DammiMaxNumeroVotiTotali():N0}";
                TxtDirittiDiVoto = ss;
                TxtDirittiStart = ss;
            }
            // diritti di voto
            TxtDirittiStartVis = true;
            // faccio il paint del numero di diritti di voto nel bottone in basso a sx , 
            // in questo caso uso un paint e non una label per un problema grafico di visibilità
            int VVoti = VTConfig.ModoAssemblea == VSDecl.MODO_AGM_POP
                ? Azionisti.DammiMaxNumeroDirittiDiVotoTotali()
                : Azionisti.DammiMaxNumeroVotiTotali();
            TxtDirittiStartMin = $"{VVoti:N0}";
            if (Azionisti.HaDirittiDiVotoMultipli())
            {
                TxtDirittiStartMin += "(d)";
            }
            TxtDirittiStartMinVis = true;
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
            TxtDirittiDiVotoVis = true;
            // Sistemo la label dei diritti di voto
            int NDirittiAzioniConferma = Azionisti.DammiDirittiAzioniDiVotoConferma(IsVotazioneDifferenziata);
            TxtConfermaNVoti = $"{NDirittiAzioniConferma:N0}" + " voti per"; // +rm.GetString("SAPP_VOTE_DIRITTIPER");

            if (VTConfig.ModoAssemblea == VSDecl.MODO_AGM_POP)
            {
                if (IsVotazioneDifferenziata)
                {
                    TxtConfermaNVoti = App.Instance.getLang("SAPP_VOTE_1DIRITTOPER"); //"1 diritto di voto per";
                }
                else
                {
                    if (!Azionisti.HaDirittiDiVotoMultipli())
                    {
                        TxtConfermaNVoti = App.Instance.getLang("SAPP_VOTE_1DIRITTOPER"); //"1 diritto di voto per";
                    }
                    else
                    {
                        TxtConfermaNVoti = Azionisti.DammiCountDirittiDiVoto_VotoCorrente() +
                                                    App.Instance.getLang("SAPP_VOTE_DIRITTIPER"); //" diritti di voto per";
                    }
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
                TxtConferma = VotoEspressoStrUp;
                //lblConferma.TextNote = VotoEspressoStrNote;
                TxtConfermaVis = true;

                //oVotoTheme.SetTheme_lbConfermaUp_Cand(ref lbConfermaUp);
            }
            else
            {
                // se è sk bianca o non voto non metto i diritti
                //NODirittiLabel = (VotoEspresso == VSDecl.VOTO_SCHEDABIANCA || VotoEspresso == VSDecl.VOTO_NONVOTO);
                if (VotoEspresso == VSDecl.VOTO_SCHEDABIANCA || VotoEspresso == VSDecl.VOTO_NONVOTO)
                {
                    TxtConfermaNVoti = "-";
                }
                // voto di lista/candidato              
                TxtConfermaUp = VotoEspressoStrUp;
                TxtConferma = VotoEspressoStr;
                //lbConferma.TextNote = VotoEspressoStrNote;
                TextBlock lblConfermaUp = (TextBlock) this.mainGrid.FindName("lblConfermaUp");
                if (lblConfermaUp != null) oVotoTheme.SetTheme_lbConferma(ref lblConfermaUp);
            }

            // attenzione, se ho una sk bianca o non voto non metto i diritii
            //if (NODirittiLabel)
            //{
            //    lbConfermaNVoti.Text = "";
            //}

            // ok, ora le mostro
            TxtConfermaVis = true;
            TxtConfermaUpVis = true;
            TxtConfermaNVotiVis = true;
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
