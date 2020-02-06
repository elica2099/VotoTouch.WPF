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
using System.Xml.Serialization;
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
                UTextBlock lblMouse = new UTextBlock()
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
                    Margin = new Thickness(40,10,0,0),
                    Visibility = Visibility.Visible
                };
                mainGrid.Children.Add(badgePanel);
                mainGrid.RegisterName(badgePanel.Name, badgePanel);
            }

            // ok ora i label normali
            // nome disgiunto
            UTextBlock lblNomeDisgiunto = new UTextBlock()
            {
                Name = "lblNomeDisgiunto",
                Text = "lblNomeDisgiunto",
                ToolTip = "lblNomeDisgiunto",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = VTConfig.IsPaintTouch ? Brushes.DarkSalmon : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            lblNomeDisgiunto.PreviewMouseRightButtonUp += AllLabelStartOnPreviewMouseRightButtonDown;
            BindingOperations.SetBinding(lblNomeDisgiunto, UTextBlock.TextProperty, new Binding("TxtNomeDisgiunto"));
            BindingOperations.SetBinding(lblNomeDisgiunto, UTextBlock.VisibilityProperty, 
                new Binding("TxtNomeDisgiuntoVis"){ Converter = new BooleanToHiddenVisibility() });
            _TxtNomeDisgiuntoVis = false;
            mainGrid.Children.Add(lblNomeDisgiunto);
            mainGrid.RegisterName(lblNomeDisgiunto.Name, lblNomeDisgiunto);
            
            // nome lblDisgiuntoRimangono
            UTextBlock lblDisgiuntoRimangono = new UTextBlock()
            {
                Name = "lblDisgiuntoRimangono",
                Text = "lblDisgiuntoRimangono",
                ToolTip = "lblDisgiuntoRimangono",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = VTConfig.IsPaintTouch ? Brushes.Khaki : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            lblDisgiuntoRimangono.PreviewMouseRightButtonUp += AllLabelStartOnPreviewMouseRightButtonDown;
            BindingOperations.SetBinding(lblDisgiuntoRimangono, UTextBlock.TextProperty, new Binding("TxtDisgiuntoRimangono"));
            BindingOperations.SetBinding(lblDisgiuntoRimangono, UTextBlock.VisibilityProperty, 
                new Binding("TxtDisgiuntoRimangonoVis"){ Converter = new BooleanToHiddenVisibility() });
            _TxtDisgiuntoRimangonoVis = false;
            mainGrid.Children.Add(lblDisgiuntoRimangono);
            mainGrid.RegisterName(lblDisgiuntoRimangono.Name, lblDisgiuntoRimangono);
            
            // nome lblDirittiStart
            UTextBlock lblDirittiStart = new UTextBlock()
            {
                Name = "lblDirittiStart",
                Text = "lblDirittiStart",
                ToolTip = "lblDirittiStart",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = VTConfig.IsPaintTouch ? Brushes.LemonChiffon : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            lblDirittiStart.PreviewMouseRightButtonUp += AllLabelStartOnPreviewMouseRightButtonDown;
            BindingOperations.SetBinding(lblDirittiStart, UTextBlock.TextProperty, new Binding("TxtDirittiStart"));
            BindingOperations.SetBinding(lblDirittiStart, UTextBlock.VisibilityProperty, 
                new Binding("TxtDirittiStartVis"){ Converter = new BooleanToHiddenVisibility() });
            _TxtDirittiStartVis = false;
            mainGrid.Children.Add(lblDirittiStart);
            mainGrid.RegisterName(lblDirittiStart.Name, lblDirittiStart);

            // nome lblDirittiStartMin
            UTextBlock lblDirittiStartMin = new UTextBlock()
            {
                Name = "lblDirittiStartMin",
                Text = "lblDirittiStartMin",
                ToolTip = "lblDirittiStartMin",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = VTConfig.IsPaintTouch ? Brushes.LightSteelBlue : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            lblDirittiStartMin.PreviewMouseRightButtonUp += AllLabelStartOnPreviewMouseRightButtonDown;
            BindingOperations.SetBinding(lblDirittiStartMin, UTextBlock.TextProperty, new Binding("TxtDirittiStartMin"));
            BindingOperations.SetBinding(lblDirittiStartMin, UTextBlock.VisibilityProperty, 
                new Binding("TxtDirittiStartMinVis"){ Converter = new BooleanToHiddenVisibility() });
            _TxtDirittiStartMinVis = false;
            mainGrid.Children.Add(lblDirittiStartMin);
            mainGrid.RegisterName(lblDirittiStartMin.Name, lblDirittiStartMin);

            // lblDirittiDiVoto
            UTextBlock lblDirittiDiVoto = new UTextBlock()
            {
                Name = "lblDirittiDiVoto",
                Text = "lblDirittiDiVoto",
                ToolTip = "lblDirittiDiVoto",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = VTConfig.IsPaintTouch ? Brushes.LightCoral : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            lblDirittiDiVoto.PreviewMouseRightButtonUp += AllLabelStartOnPreviewMouseRightButtonDown;
            BindingOperations.SetBinding(lblDirittiDiVoto, UTextBlock.TextProperty, new Binding("TxtDirittiDiVoto"));
            BindingOperations.SetBinding(lblDirittiDiVoto, UTextBlock.VisibilityProperty, 
                new Binding("TxtDirittiDiVotoVis"){ Converter = new BooleanToHiddenVisibility() });
            _TxtDirittiDiVotoVis = false;
            mainGrid.Children.Add(lblDirittiDiVoto);
            mainGrid.RegisterName(lblDirittiDiVoto.Name, lblDirittiDiVoto);

            // lbConferma
            ULabelCandidati lblConferma = new ULabelCandidati()
            {
                Name = "lblConferma",
                ToolTip = "lblConferma",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = VTConfig.IsPaintTouch ? Brushes.MistyRose : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            lblConferma.PreviewMouseRightButtonUp += AllLabelStartOnPreviewMouseRightButtonDown;
            BindingOperations.SetBinding(lblConferma, ULabelCandidati.SetTextProperty, new Binding("TxtConferma"));
            BindingOperations.SetBinding(lblConferma, ULabelCandidati.VisibilityProperty, 
                new Binding("TxtConfermaVis"){ Converter = new BooleanToHiddenVisibility() });
            _TxtConfermaVis = false;
            mainGrid.Children.Add(lblConferma);
            mainGrid.RegisterName(lblConferma.Name, lblConferma);
            
            // lbConfermaUp
            UTextBlock lblConfermaUp = new UTextBlock()
            {
                Name = "lblConfermaUp",
                Text = "lblConfermaUp",
                ToolTip = "lblConfermaUp",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = VTConfig.IsPaintTouch ? Brushes.Aquamarine : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            lblConfermaUp.PreviewMouseRightButtonUp += AllLabelStartOnPreviewMouseRightButtonDown;
            BindingOperations.SetBinding(lblConfermaUp, UTextBlock.TextProperty, new Binding("TxtConfermaUp"));
            BindingOperations.SetBinding(lblConfermaUp, UTextBlock.VisibilityProperty, 
                new Binding("TxtConfermaUpVis"){ Converter = new BooleanToHiddenVisibility() });
            _TxtConfermaUpVis = false;
            mainGrid.Children.Add(lblConfermaUp);
            mainGrid.RegisterName(lblConfermaUp.Name, lblConfermaUp);
            
            // lbConfermaNVoti
            UTextBlock lblConfermaNVoti = new UTextBlock()
            {
                Name = "lblConfermaNVoti",
                Text = "lblConfermaNVoti",
                ToolTip = "lblConfermaNVoti",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = VTConfig.IsPaintTouch ? Brushes.GreenYellow : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            lblConfermaNVoti.PreviewMouseRightButtonUp += AllLabelStartOnPreviewMouseRightButtonDown;
            BindingOperations.SetBinding(lblConfermaNVoti, UTextBlock.TextProperty, new Binding("TxtConfermaNVoti"));
            BindingOperations.SetBinding(lblConfermaNVoti, UTextBlock.VisibilityProperty, 
                new Binding("TxtConfermaNVotiVis"){ Converter = new BooleanToHiddenVisibility() });
            _TxtConfermaNVotiVis = false;
            mainGrid.Children.Add(lblConfermaNVoti);
            mainGrid.RegisterName(lblConfermaNVoti.Name, lblConfermaNVoti);
            
            // lblNomeAzStart
            UTextBlock lblNomeAzStart = new UTextBlock()
            {
                Name = "lblNomeAzStart",
                Text = "lblNomeAzStart",
                ToolTip = "lblNomeAzStart",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = VTConfig.IsPaintTouch ? Brushes.Plum : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            lblNomeAzStart.PreviewMouseRightButtonUp += AllLabelStartOnPreviewMouseRightButtonDown;
            BindingOperations.SetBinding(lblNomeAzStart, UTextBlock.TextProperty, new Binding("TxtNomeAzStart"));
            BindingOperations.SetBinding(lblNomeAzStart, UTextBlock.VisibilityProperty, 
                new Binding("TxtNomeAzStartVis"){ Converter = new BooleanToHiddenVisibility() });
            _TxtNomeAzStartVis = false;
            mainGrid.Children.Add(lblNomeAzStart);
            mainGrid.RegisterName(lblNomeAzStart.Name, lblNomeAzStart);

            // multiselezione/candidati
            // lblNSelezioni
            UTextBlock lblNSelezioni = new UTextBlock()
            {
                Name = "lblNSelezioni",
                Text = "lblNSelezioni",
                ToolTip = "lblNSelezioni",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = VTConfig.IsPaintTouch ? Brushes.Wheat : Brushes.Transparent,
                Margin = new Thickness(20,20,0,0)
            };
            lblNSelezioni.PreviewMouseRightButtonUp += AllLabelStartOnPreviewMouseRightButtonDown;
            BindingOperations.SetBinding(lblNSelezioni, UTextBlock.TextProperty, new Binding("TxtNSelezioni"));
            BindingOperations.SetBinding(lblNSelezioni, UTextBlock.VisibilityProperty, 
                new Binding("TxtNSelezioniVis"){ Converter = new BooleanToHiddenVisibility() });
            _TxtNSelezioniVis = false;
            mainGrid.Children.Add(lblNSelezioni);
            mainGrid.RegisterName(lblNSelezioni.Name, lblNSelezioni);

            // lblCandidati_PresCDA
            UTextBlock lblCandidati_PresCDA = new UTextBlock()
            {
                Name = "lblCandidati_PresCDA",
                Text = "lblCandidati_PresCDA",
                ToolTip = "lblCandidati_PresCDA",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20,20,0,0)
            };
            lblCandidati_PresCDA.PreviewMouseRightButtonUp += AllLabelStartOnPreviewMouseRightButtonDown;
            BindingOperations.SetBinding(lblCandidati_PresCDA, UTextBlock.TextProperty, new Binding("TxtCandidati_PresCDA"));
            BindingOperations.SetBinding(lblCandidati_PresCDA, UTextBlock.VisibilityProperty, 
                new Binding("TxtCandidati_PresCDAVis"){ Converter = new BooleanToHiddenVisibility() });
            _TxtCandidati_PresCDAVis = false;
            mainGrid.Children.Add(lblCandidati_PresCDA);
            mainGrid.RegisterName(lblCandidati_PresCDA.Name, lblCandidati_PresCDA);

            // lblCandidati_Altern
            UTextBlock lblCandidati_Altern = new UTextBlock()
            {
                Name = "lblCandidati_Altern",
                Text = "lblCandidati_Altern",
                ToolTip = "lblCandidati_Altern",
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20,20,0,0)
            };
            lblCandidati_Altern.PreviewMouseRightButtonUp += AllLabelStartOnPreviewMouseRightButtonDown;
            BindingOperations.SetBinding(lblCandidati_Altern, UTextBlock.TextProperty, new Binding("TxtCandidati_Altern"));
            BindingOperations.SetBinding(lblCandidati_Altern, UTextBlock.VisibilityProperty, 
                new Binding("TxtCandidati_AlternVis"){ Converter = new BooleanToHiddenVisibility() });
            _TxtCandidati_AlternVis = false;
            mainGrid.Children.Add(lblCandidati_Altern);
            mainGrid.RegisterName(lblCandidati_Altern.Name, lblCandidati_Altern);
        }

        private void AllLabelStartOnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // qua dovrebbe esserci la finestra di personalizzazione della label

            //MessageBox.Show("sdfsdfs");
            //throw new NotImplementedException();
        }

        //   Caricamento Tema da VotoTheme ----------------------------------------------------------------

        private void CaricaTemaInControlli()
        {
            UTextBlock lblDirittiStart = (UTextBlock) this.mainGrid.FindName("lblDirittiStart");
            if (lblDirittiStart != null) oVotoTheme.SetTheme_lbDirittiStart(ref lblDirittiStart);

            UTextBlock lblDirittiStartMin = (UTextBlock) this.mainGrid.FindName("lblDirittiStartMin");
            if (lblDirittiStartMin != null) oVotoTheme.SetTheme_lbDirittiStartMin(ref lblDirittiStartMin);

            UTextBlock lblDirittiDiVoto = (UTextBlock) this.mainGrid.FindName("lblDirittiDiVoto");
            if (lblDirittiDiVoto != null) oVotoTheme.SetTheme_lbDirittiDiVoto(ref lblDirittiDiVoto);

            // carico il tema da vototouch, semplicemente richiamando le singole label
            UTextBlock lblNomeDisgiunto = (UTextBlock) this.mainGrid.FindName("lblNomeDisgiunto");
            if (lblNomeDisgiunto != null) oVotoTheme.SetTheme_lbNomeDisgiunto(ref lblNomeDisgiunto);

            UTextBlock lblDisgiuntoRimangono = (UTextBlock) this.mainGrid.FindName("lblDisgiuntoRimangono");
            if (lblDisgiuntoRimangono != null) oVotoTheme.SetTheme_lbDisgiuntoRimangono(ref lblDisgiuntoRimangono);

            ULabelCandidati lblConferma = (ULabelCandidati) this.mainGrid.FindName("lblConferma");
            if (lblConferma != null) oVotoTheme.SetTheme_lbConferma(ref lblConferma);

            UTextBlock lblConfermaUp = (UTextBlock) this.mainGrid.FindName("lblConfermaUp");
            if (lblConfermaUp != null) oVotoTheme.SetTheme_lbConfermaUp(ref lblConfermaUp);

            UTextBlock lblConfermaNVoti = (UTextBlock) this.mainGrid.FindName("lblConfermaNVoti");
            if (lblConfermaNVoti != null) oVotoTheme.SetTheme_lbConfermaNVoti(ref lblConfermaNVoti);

            UTextBlock lblNomeAzStart = (UTextBlock) this.mainGrid.FindName("lblNomeAzStart");
            if (lblNomeAzStart != null) oVotoTheme.SetTheme_lbNomeAzStart(ref lblNomeAzStart);

            UTextBlock lblNSelezioni = (UTextBlock) this.mainGrid.FindName("lblNSelezioni");
            if (lblNSelezioni != null) oVotoTheme.SetTheme_lbNSelezioni(ref lblNSelezioni);

            UTextBlock lblCandidati_PresCDA = (UTextBlock) this.mainGrid.FindName("lblCandidati_PresCDA");
            if (lblCandidati_PresCDA != null) oVotoTheme.SetTheme_lbCandidati_PresCDA(ref lblCandidati_PresCDA);

            UTextBlock lblCandidati_Altern = (UTextBlock) this.mainGrid.FindName("lblCandidati_Altern");
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
                //TextBlock lblConfermaUp = (TextBlock) this.mainGrid.FindName("lblConfermaUp");
                //if (lblConfermaUp != null) oVotoTheme.SetTheme_lbConfermaUp(ref lblConfermaUp);
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
