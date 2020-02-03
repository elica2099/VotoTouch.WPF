﻿using System;
using System.Collections.Generic;
using System.Data;

namespace VotoTouch.WPF
{

    public class CVotoFileDati : CVotoBaseDati
    {

        public CVotoFileDati(ConfigDbData AFDBConfig, Boolean AADataLocal, string AAData_path) : 
            base(AFDBConfig, AADataLocal, AAData_path)
        {
            //
        }

        // --------------------------------------------------------------------------
        //  METODI DATABASE
        // --------------------------------------------------------------------------

        public override object DBConnect()
        {
            return this;
        }

        public override object DBDisconnect()
        {
            return this;
        }

        // --------------------------------------------------------------------------
        //  LETTURA CONFIGURAZIONE NEL DATABASE
        // --------------------------------------------------------------------------

        #region Lettura/Scrittura Configurazione

        public override int CaricaConfigDB(ref int ABadgeLen, ref string ACodImpianto)
        {
            ABadgeLen = 8;
            ACodImpianto = "00";
            return 0;
        }

        public override int DammiConfigTotem() //, ref TTotemConfig TotCfg)
        {
            VTConfig.Postazione = VTConfig.NomeTotem;
            // faccio un  ulteriore controllo
            VTConfig.IDSeggio = 99;
            FIDSeggio = 99;
            VTConfig.Attivo = true;
            VTConfig.VotoAperto = true;
            //VTConfig.ControllaPresenze = 1;
            VTConfig.UsaSemaforo = false;
            VTConfig.IP_Com_Semaforo = "127.0.0.1";
            VTConfig.TipoSemaforo = 1;
            VTConfig.ValAssemblea = "O";
            VTConfig.AbilitaBottoneUscita = true;
            //VTConfig.SalvaLinkVoto = true;
            //VTConfig.SalvaVotoNonConfermato = true;
            //VTConfig.IDSchedaUscitaForzata = VSDecl.VOTO_SCHEDABIANCA;
            //TotCfg.UsaSemaforo = true;
            //TotCfg.IP_Com_Semaforo = "10.178.6.16";
            //TotCfg.IP_Com_Semaforo = "192.168.0.32";
            //TotCfg.UsaSemaforo = true;
            //TotCfg.IP_Com_Semaforo = "COM3";           
            //TotCfg.TipoSemaforo = 2;
            VTConfig.UsaLettore = false;
            VTConfig.PortaLettore = 0;
            VTConfig.CodiceUscita = "999999";
            VTConfig.MaxDeleghe = 1000;

            VTConfig.ContrarioATutti = "Contrario";
            VTConfig.AstenutoATutti = "Astenuto";

            //TotCfg.UsaController = false;
            //TotCfg.IPController = "127.0.0.1";
            return 0; 
            //VTConfig.Postazione = VTConfig.NomeTotem;
            //// faccio un  ulteriore controllo
            //VTConfig.IDSeggio = 99;
            //FIDSeggio = 99;
            //VTConfig.Attivo = true;
            //VTConfig.VotoAperto = true;
            //VTConfig.ControllaPresenze = 1;
            //VTConfig.UsaSemaforo = false;
            //VTConfig.IP_Com_Semaforo = "127.0.0.1";
            //VTConfig.TipoSemaforo = 1;
            //VTConfig.SalvaLinkVoto = true;
            //VTConfig.SalvaVotoNonConfermato = true;
            //VTConfig.IDSchedaUscitaForzata = VSDecl.VOTO_SCHEDABIANCA;            
            ////TotCfg.UsaSemaforo = true;
            ////TotCfg.IP_Com_Semaforo = "10.178.6.16";
            ////TotCfg.IP_Com_Semaforo = "192.168.0.32";
            ////TotCfg.UsaSemaforo = true;
            ////TotCfg.IP_Com_Semaforo = "COM3";           
            ////TotCfg.TipoSemaforo = 2;
            //VTConfig.UsaLettore = false;
            //VTConfig.PortaLettore = 0;
            //VTConfig.CodiceUscita = "999999";
            ////TotCfg.UsaController = false;
            ////TotCfg.IPController = "127.0.0.1";
            //return 0;
        }

        public override int DammiConfigDatabase() //ref TTotemConfig TotCfg)
        {
            DataTable dt = new DataTable();

            dt.ReadXml(AData_path + "CONFIG_CfgVotoSegreto.xml");

            VTConfig.CodImpianto = "78";

            foreach (DataRow a in dt.Rows)
            {
                VTConfig.ModoAssemblea = Convert.ToInt32(a["ModoAssemblea"]);
                // il link del voto
                VTConfig.SalvaLinkVoto = Convert.ToBoolean(a["SalvaLinkVoto"]);
                // il salvataggio del voto anche se non ha confermato
                VTConfig.SalvaVotoNonConfermato = Convert.ToBoolean(a["SalvaVotoNonConfermato"]);
                // l'id della scheda che deve essere salvata in caso di 999999
                VTConfig.IDSchedaUscitaForzata = Convert.ToInt32(a["IDSchedaUscitaForzata"]);
                // ModoPosizioneAreeTouch
                VTConfig.ModoPosizioneAreeTouch = Convert.ToInt32(a["ModoPosizioneAreeTouch"]); ;
                // controllo delle presenze
                VTConfig.ControllaPresenze = Convert.ToInt32(a["ControllaPresenze"]); 
                // AbilitaBottoneUscita
                VTConfig.AbilitaBottoneUscita = Convert.ToBoolean(a["AbilitaBottoneUscita"]);
                // AttivaAutoRitornoVoto
                VTConfig.AttivaAutoRitornoVoto = Convert.ToBoolean(a["AttivaAutoRitornoVoto"]);
                // TimeAutoRitornoVoto
                VTConfig.TimeAutoRitornoVoto = Convert.ToInt32(a["TimeAutoRitornoVoto"]); ; ;
                // AbilitaDirittiNonVoglioVotare
                VTConfig.AbilitaDirittiNonVoglioVotare = Convert.ToBoolean(a["AbilitaDirittiNonVoglioVotare"]); ;
            }
            return 0;
        }

        public override int SalvaConfigurazione() //, ref TTotemConfig ATotCfg)
        {
            return 0;
        }

        public override int SalvaConfigurazionePistolaBarcode() //, ref TTotemConfig ATotCfg)
        {
            return 0;
        }

        #endregion

        // --------------------------------------------------------------------------
        //  CARICAMENTO DATI VOTAZIONI
        // --------------------------------------------------------------------------

        #region CARICAMENTO DATI VOTAZIONI

        public override bool CaricaVotazioniDaDatabase(ref List<TVotazione> AVotazioni)
        {
            //int z;
            DataTable dt = new DataTable();
            TVotazione v;

            dt.ReadXml(AData_path + "VS_MatchVot_Totem.xml");

            foreach (DataRow a in dt.Rows)
            {
                v = new TVotazione();

                v.IDVoto = Convert.ToInt32(a["NumVotaz"]);
                v.IDGruppoVoto = Convert.ToInt32(a["GruppoVotaz"]);
                v.TipoVoto = Convert.ToInt32(a["TipoVotaz"]);
                v.TipoSubVoto = 0;
                v.Descrizione = a["Argomento"].ToString();
                v.SkBianca = Convert.ToBoolean(a["SchedaBianca"]);
                v.SkNonVoto = Convert.ToBoolean(a["SchedaNonVoto"]);
                v.SkContrarioTutte = Convert.ToBoolean(a["SchedaContrarioTutte"]);
                v.SkAstenutoTutte = Convert.ToBoolean(a["SchedaAstenutoTutte"]);
                v.SelezionaTuttiCDA = Convert.ToBoolean(a["SelezTuttiCDA"]);
                //PreIntermezzo = false,
                v.MaxScelte = Convert.ToInt32(a["MaxScelte"]);
                v.AbilitaBottoneUscita = Convert.ToBoolean(a["AbilitaBottoneUscita"]);
                
                AVotazioni.Add(v);
            }

            dt.Dispose();

            return true;
        }

        public override bool CaricaListeDaDatabase(ref List<TVotazione> AVotazioni)
        {
            DataTable dt = new DataTable();
            TLista Lista;

            dt.ReadXml(AData_path + "VS_Liste_Totem.xml");
            string ASort = "idlista asc";
            // cicla lungo le votazioni e carica le liste
            foreach (TVotazione votaz in AVotazioni)
            {
                // faccio un sorting delle liste
                switch (votaz.TipoVoto)
                {
                    // se è lista ordino per l'id
                    case VSDecl.VOTO_LISTA:
                        ASort = "idlista asc";
                        break;
                    // se è candidato ordino in modo alfabetico
                    case VSDecl.VOTO_CANDIDATO:
                    case VSDecl.VOTO_CANDIDATO_SING:
                    case VSDecl.VOTO_MULTICANDIDATO:
                        ASort = "PresentatoDaCdA desc, OrdineCarica, DescrLista asc";
                        break;
                }

                foreach (DataRow riga in dt.Select("NumVotaz = " +
                    votaz.IDVoto.ToString(), ASort))
                {
                    Lista = new TLista
                    {
                        NumVotaz = Convert.ToInt32(riga["NumVotaz"]),
                        IDLista = Convert.ToInt32(riga["idLista"]),
                        IDScheda = Convert.ToInt32(riga["idScheda"]),
                        DescrLista = riga["DescrLista"].ToString(),
                        TipoCarica = Convert.ToInt32(riga["TipoCarica"]),
                        PresentatodaCDA = Convert.ToBoolean(riga["PresentatodaCDA"]),
                        Presentatore = riga["Presentatore"].ToString(),
                        Capolista = riga["Capolista"].ToString(),
                        ListaElenco = riga["ListaElenco"].ToString()
                    };
                    // aggiungo
                    votaz.Liste.Add(Lista);
                }
            }

            dt.Dispose();

            return true;
        }

        #endregion

        // --------------------------------------------------------------------------
        //  METODI SUI BADGE
        // --------------------------------------------------------------------------

        #region METODI SUI BADGE
        //        override public bool ControllaBadge(int AIDBadge, TTotemConfig TotCfg, ref int AReturnFlags)
        public override bool ControllaBadge(int AIDBadge, ref int AReturnFlags)
        {
            AReturnFlags = 0;
            return true;
        }
        
        public override bool BadgeAnnullato(int AIDBadge)
        {
            return false;
        }

        public override bool BadgePresente(int AIDBadge, bool ForzaTimbr)
        {
            return true;
        }

        public override bool BadgeHaGiaVotato(int AIDBadge)
        {
            return false;
        }

        public override bool HaVotato(int ANVotaz, int AIDBadge, int ProgDelega)
        {
            return false;
        }

        #endregion

        // --------------------------------------------------------------------------
		//  LETTURA DATI AZIONISTA
        // --------------------------------------------------------------------------

        #region CaricaDirittidiVotoDaDatabase

        public override bool CaricaDirittidiVotoDaDatabase(int AIDBadge, ref List<TAzionista> AAzionisti,
                                                  ref TAzionista ATitolare_badge, ref TListaVotazioni AVotazioni)
        {
            int IDVotazione = -1;
            AAzionisti.Clear();
            TAzionista a;

            foreach (TVotazione voto in AVotazioni.Votazioni)
            {
                IDVotazione = voto.IDVoto;
                // un voto
                if (AIDBadge == 1000)
                {
                    a = new TAzionista
                        {
                            CoAz = "10000",
                            IDAzion = 10000,
                            IDBadge = 1000,
                            ProgDeleg = 0,
                            RaSo = "Mario Rossi",
                            Sesso = "M",
                            NVoti = VTConfig.ModoAssemblea == VSDecl.MODO_AGM_POP ? 1 : 10000,
                            IDVotaz = IDVotazione,
                            HaVotato = TListaAzionisti.VOTATO_NO
                        };
                    AAzionisti.Add(a);
                    // poi lo salvo come titolare
                    ATitolare_badge.CopyFrom(ref a);
                }
                // tre voti
                if (AIDBadge == 1001)
                {
                    a = new TAzionista
                        {
                            CoAz = "10001",
                            IDAzion = 10001,
                            IDBadge = 1001,
                            ProgDeleg = 0,
                            RaSo = "Mario Rossi",
                            Sesso = "M",
                            NVoti = VTConfig.ModoAssemblea == VSDecl.MODO_AGM_POP ? 1 : 5000,
                            IDVotaz = IDVotazione,
                            HaVotato = TListaAzionisti.VOTATO_NO
                        };
                    AAzionisti.Add(a);
                    // poi lo salvo come titolare
                    ATitolare_badge.CopyFrom(ref a);

                    a = new TAzionista
                        {
                            CoAz = "10002",
                            IDAzion = 10002,
                            IDBadge = 1001,
                            ProgDeleg = 1,
                            Sesso = "M",
                            RaSo = "Mario Rossi - Delega 1",
                            NVoti = VTConfig.ModoAssemblea == VSDecl.MODO_AGM_POP ? 1 : 300,
                            IDVotaz = IDVotazione,
                            HaVotato = TListaAzionisti.VOTATO_NO
                        };
                    AAzionisti.Add(a);

                    a = new TAzionista
                        {
                            CoAz = "10003",
                            IDAzion = 10003,
                            IDBadge = 1003,
                            ProgDeleg = 0,
                            NVoti = VTConfig.ModoAssemblea == VSDecl.MODO_AGM_POP ? 1 : 1500,
                            Sesso = "M",
                            RaSo = "Mario Rossi - Delega 2",
                            IDVotaz = IDVotazione,
                            HaVotato = TListaAzionisti.VOTATO_NO
                        };
                    AAzionisti.Add(a);
                }
            }

            return true;
        }

        public override int SalvaTutto(int AIDBadge, ref TListaAzionisti FAzionisti)
        {
            return 0;
        }

        public override int SalvaTuttoInGeas(int AIDBadge, ref TListaAzionisti AAzionisti)
        {
            return 0;
        }

        public override int NumAzTitolare(int AIDBadge)
        {
            return 0;
        }

        public override int CheckStatoVoto(string ANomeTotem)
        {
            return 1;
        }

        public override bool CancellaBadgeVotazioni(int AIDBadge)
        {
            return true;
        }

        public override Boolean CancellaTuttiVoti()
        {
            return true;
        }

        #endregion

        // --------------------------------------------------------------------------
        //  REGISTRAZIONE NEL DATABASE
        // --------------------------------------------------------------------------

        public override int RegistraTotem(string ANomeTotem)
        {
            return 0;
        }

        public override int UnregistraTotem(string ANomeTotem)
        {
            return 0;
        }

        // --------------------------------------------------------------
        //  METODI PRIVATI
        // --------------------------------------------------------------

        public override string DammiStringaConnessione()
        {
            return "";
        }

        // --------------------------------------------------------------
        //  METODI DI CONFIGURAZIONE
        // --------------------------------------------------------------

        // carica la configurazione 
        public override Boolean CaricaConfig()
        {
                return true;
        }

    }

}
