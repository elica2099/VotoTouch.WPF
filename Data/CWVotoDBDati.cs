using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Collections; 
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Common;
using System.Reflection;
using System.Windows;
using Microsoft.Win32;
using System.Threading;
using VotoTouch.WPF.Models;

// -----------------------------------------------------------------------
//			VOTO TOUCH - TSTSQLCONN ClassE
//  Classe di gestione del database e task relativi
// -----------------------------------------------------------------------
//		AUTH	: M.Binello
//		VER		: 4.0 
//		DATE	: Mar 2016
// -----------------------------------------------------------------------
//	History
//  Aggiunto controllo versione db
// -----------------------------------------------------------------------

namespace VotoTouch.WPF
{
	/// <summary>
	/// Summary description for CVotoDBDati.
	/// </summary>
    public class CVotoDBDati : CVotoBaseDati
	{
	    private const string DriveM = @"M:\";
	    // connessione database
		private SqlConnection STDBConn;

        // stringhe sql
	    private readonly string qry_DammiDirittiDiVoto_Titolare;
	    private readonly string qry_DammiDirittiDiVoto_Deleganti;
        private readonly string qry_DammiVotazioniTotem;
        private readonly string qry_DammiBadgePresenteGeas;
        private readonly string qry_MettiBadgePresenteGeas;

        public CVotoDBDati(ConfigDbData AFDBConfig, Boolean AADataLocal, string AAData_path) : 
            base(AFDBConfig, AADataLocal, AAData_path)
		{
			STDBConn = new SqlConnection();
			FConnesso = false;
			// setto i parametri di default di DBConfig
			FDBConfig.DB_ConfigOK = false;
			FDBConfig.DB_Type = "ODBC";
			FDBConfig.DB_Dsn = "GEAS";
			FDBConfig.DB_Name = "GEAS_BPER";
			FDBConfig.DB_Uid = "geas";
			FDBConfig.DB_Pwd = "geas";
			FDBConfig.DB_Server = @"TOGTA-SRVSQL2k\SQL2kGTA";
			//
			FIDSeggio = 2;

            // load the query
            qry_DammiDirittiDiVoto_Titolare = getModelsQueryProcedure("DammiDirittiDiVoto_Titolare.sql");
            qry_DammiDirittiDiVoto_Deleganti = getModelsQueryProcedure("DammiDirittiDiVoto_Deleganti.sql");
            qry_DammiVotazioniTotem = getModelsQueryProcedure("DammiVotazioniTotem.sql");
            qry_DammiBadgePresenteGeas = getModelsQueryProcedure("DammiBadgePresenteGeas.sql");
            qry_MettiBadgePresenteGeas = getModelsQueryProcedure("MettiBadgePresenteGeas.sql");
        }

        ~CVotoDBDati()
        {
            // Destructor
        }

        //  METODI DATABASE --------------------------------------------------------------------------

        #region Metodi Database

        public override object DBConnect()
        {
            // connessione al DB in funzione dei parametri che ci sono in TSTConfig
            STDBConn.ConnectionString = DammiStringaConnessione();
            try
            {
                STDBConn.Open();
                FConnesso = true;
                return STDBConn;
            }
            catch
            {
                FConnesso = false;
                return null;
            }
        }

        public override object DBDisconnect()
        {
            // disconnessione al DB
            STDBConn.Close();
            FConnesso = false;
            return STDBConn;
        }

        public override string DammiStringaConnessione()
        {
            // devo aggiungere dei controlli
            // compone la stringa di connessione in funzione di TSTConfig
            string ssconn = "server=" + FDBConfig.DB_Server + ";database=" + FDBConfig.DB_Name +
                ";uid=" + FDBConfig.DB_Uid + ";pwd=" + FDBConfig.DB_Pwd;
            return ssconn;
        }

        public bool OpenConnection(string NomeFunzione)
        {
            // apro la connessione se � chiusa
            if (STDBConn.State == ConnectionState.Open) return true;
            try
            {
                STDBConn.Open();
                return true;
            }
            catch (Exception objExc)
            {
                Logging.WriteToLog("<dberror> Errore fn " + NomeFunzione + " - OpenConnection: " + objExc.Message);
                MessageBox.Show("Errore fn " + NomeFunzione + " - OpenConnection: " + objExc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool CloseConnection(string NomeFunzione)
        {
            if (STDBConn.State == ConnectionState.Closed) return true;
            try
            {
                STDBConn.Close();
                return true;
            }
            catch (Exception objExc)
            {
                Logging.WriteToLog("<dberror> Errore fn " + NomeFunzione + " - CloseConnection: " + objExc.Message);
                MessageBox.Show("Errore fn " + NomeFunzione + " - CloseConnection: " + objExc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        #endregion

        //  LETTURA TIPO DI DATABASE  --------------------------------------------------------------------------

        #region Lettura tipo database

        public override int getDatabaseMode()
        {
            // this function return the type of the database spa/pop
            int ret = -1;
            if (!OpenConnection("getDatabaseMode")) return -1;
            SqlCommand qryStd = new SqlCommand { Connection = STDBConn, CommandText = "select VotaTesta from CONFIG_CfgParametri" };
            try
            {
                SqlDataReader a = qryStd.ExecuteReader();
                if (a.HasRows && a.Read())
                {
                    ret = Convert.ToBoolean(a["VotaTesta"]) ? VSDecl.DBMODE_POP : VSDecl.DBMODE_SPA;
                }
            }
            catch (Exception objExc)
            {
                Utils.errorCall("getDatabaseMode", objExc.Message);
            }
            finally
            {
                qryStd?.Dispose();
                CloseConnection("");
            }
            return ret;
        }

        public override int getDatabaseVersion()
        {
            // this function return the type of the database spa/pop
            return TableExists("GEAS_Titolari_Voti") ? VSDecl.DBVERS_12 : VSDecl.DBVERS_10;
        }

        private static bool ColumnExists(SqlDataReader reader, string columnName)
        {
            using (var schemaTable = reader.GetSchemaTable())
            {
                if (schemaTable != null)
                    schemaTable.DefaultView.RowFilter = $"ColumnName= '{columnName}'";

                return schemaTable != null && (schemaTable.DefaultView.Count > 0);
            }
        }

        private bool TableExists(string SQLTableName)
        {
            Int32 newProdID = 0;
            string sql = "SELECT count(*) as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[" + SQLTableName + "]')";
            using (SqlConnection conn = new SqlConnection(DammiStringaConnessione()))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                    conn.Open();
                    newProdID = (Int32)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //get the result value: 1-exist; 0-not exist;
            return newProdID == 1;
        }

        #endregion

        //  LETTURA CONFIGURAZIONE NEL DATABASE  --------------------------------------------------------------------------

        #region Lettura/Scrittura Configurazione

        public override int CaricaConfigDB(ref int ABadgeLen, ref string ACodImpianto)
        {
            // mi dice la lunghezza del badge e il codice impianto per il lettore
            if (!OpenConnection("CaricaConfigDB")) return 0;

            ABadgeLen = 8;
            ACodImpianto = "00";
            int Tok = 0;
            SqlCommand qryStd = new SqlCommand() { Connection = STDBConn };
            try
            {
                // Leggo ora da CONFIG_cfgParametri	
                qryStd.CommandText = "select * from CONFIG_cfgParametri with (nolock)";
                SqlDataReader a = qryStd.ExecuteReader();
                if (a.HasRows)
                {
                    // devo verificare 
                    a.Read();
                    ABadgeLen = a.IsDBNull(a.GetOrdinal("LenNumBadge")) ? 8 : Convert.ToInt32(a["LenNumBadge"]);
                    ACodImpianto = a.IsDBNull(a.GetOrdinal("CodImpRea")) ? "00" : (a["CodImpRea"]).ToString();
                }
                else
                {
                    ABadgeLen = 8;
                    ACodImpianto = "00";
                }
                a.Close();

                // ora devo leggere il modo assemblea
                // Leggo ora da CONFIG_cfgParametri	
                qryStd.CommandText = "select ValAssem from CONFIG_DatiAssemblea with (nolock)";
                a = qryStd.ExecuteReader();
                if (a.HasRows)
                {
                    // devo verificare 
                    a.Read();
                    VTConfig.ValAssemblea = a.IsDBNull(a.GetOrdinal("ValAssem")) ? "O" : (a["ValAssem"]).ToString();
                }
                else
                {
                    VTConfig.ValAssemblea = "O";
                }

                VTConfig.IsOrdinaria = VTConfig.ValAssemblea.Contains("O");
                VTConfig.IsStraordinaria = VTConfig.ValAssemblea.Contains("S");

                a.Close();
            }
            catch (Exception objExc)
            {
                Tok = 1;
                Utils.errorCall("CaricaConfigDB", objExc.Message);
            }
            finally
            {
                qryStd.Dispose();
                CloseConnection("");
            }
            return Tok;
        }

        public override int DammiConfigTotem() //, ref TTotemConfig TotCfg)
        {
            if (!OpenConnection("DammiConfigTotem")) return 0;
            int result = 0;
            // preparo gli oggetti
            SqlCommand qryStd = new SqlCommand
                {
                    Connection = STDBConn,
                    CommandText =
                        "select * from CONFIG_POSTAZIONI_TOTEM with (nolock) where Postazione = '" + VTConfig.NomeTotem + "'"
                };
            // registra il totem aggiungendo il record in CONFIG_POSTAZIONI, e chiaramente verifica che ci sia gi�
            SqlTransaction traStd = STDBConn.BeginTransaction();
            qryStd.Transaction = traStd;
            bool inserisci = false;

            try
            {
                SqlDataReader a = qryStd.ExecuteReader();
                // se c'� il record
                if (a.HasRows)
                {
                    // devo verificare 
                    a.Read();
                    // carico
                    VTConfig.Postazione = a["Postazione"].ToString();
                    // faccio un  ulteriore controllo
                    if (VTConfig.NomeTotem != VTConfig.Postazione) VTConfig.Postazione = VTConfig.NomeTotem;

                    VTConfig.Descrizione = a.IsDBNull(a.GetOrdinal("Descrizione")) ? VTConfig.NomeTotem : a["Descrizione"].ToString();
                    VTConfig.IDSeggio = Convert.ToInt32(a["IdSeggio"]);
                    FIDSeggio = Convert.ToInt32(a["IdSeggio"]);

                    VTConfig.Attivo = Convert.ToBoolean(a["Attivo"]);
                    VTConfig.VotoAperto = Convert.ToBoolean(a["VotoAperto"]);

                    VTConfig.UsaSemaforo = Convert.ToBoolean(a["UsaSemaforo"]);
                    VTConfig.IP_Com_Semaforo = a["IPCOMSemaforo"].ToString();
                    VTConfig.TipoSemaforo = Convert.ToInt32(a["TipoSemaforo"]);

                    VTConfig.UsaLettore = Convert.ToBoolean(a["UsaLettore"]);
                    VTConfig.PortaLettore = Convert.ToInt32(a["PortaLettore"]);
                    VTConfig.CodiceUscita = a["CodiceUscita"].ToString();

                    VTConfig.Sala = a.IsDBNull(a.GetOrdinal("Sala")) ? 1 : Convert.ToInt32(a["Sala"]);
                }
                else
                    inserisci = true;
                // chiudo
                a.Close();

                // ok, se inserisci � true, vuol dire che non ha trovato record e devo inserirlo
                if (inserisci)
                {
                    // non c'� configurazione, devo inserirla
                    qryStd.CommandText = @"INSERT into CONFIG_POSTAZIONI_TOTEM
                                                (Postazione, Descrizione, IdSeggio, Attivo, VotoAperto, UsaSemaforo,
                                                IPCOMSemaforo, TipoSemaforo, UsaLettore, PortaLettore, CodiceUscita,
                                                UsaController, IPController, Sala)
                                            VALUES
                                                (@NomeTotem, @NomeTotem2,  999, 1, 0, 0, 
                                                    '127.0.0.1', 2, 0, 1, '999999', 0, '127.0.0.1', 1)";
                    qryStd.Parameters.Clear();
                    qryStd.Parameters.Add("@NomeTotem", System.Data.SqlDbType.VarChar).Value = VTConfig.NomeTotem;
                    qryStd.Parameters.Add("@NomeTotem2", System.Data.SqlDbType.VarChar).Value = "Desc_" + VTConfig.NomeTotem;

                    //qryStd.CommandText = "INSERT into CONFIG_POSTAZIONI_TOTEM " +
                    //    "(Postazione, Descrizione, IdSeggio, Attivo, VotoAperto, UsaSemaforo, "+
                    //    " IPCOMSemaforo, TipoSemaforo, UsaLettore, PortaLettore, CodiceUscita, " +
                    //    " UsaController, IPController, Sala) " +
                    //    " VALUES ('" + VTConfig.NomeTotem + "', 'Desc_" + VTConfig.NomeTotem + "', 999, 1, 0, 0, " +
                    //    "'127.0.0.1', 2, 0, 1, '999999', 0, '127.0.0.1', 1)";
                    // metto in quadro i valori
                    VTConfig.Postazione = VTConfig.NomeTotem;
                    VTConfig.Descrizione = VTConfig.NomeTotem;
                    VTConfig.IDSeggio = 999;
                    FIDSeggio = 999;
                    VTConfig.Attivo = true;
                    VTConfig.VotoAperto = false;
                    VTConfig.UsaSemaforo = false;
                    VTConfig.IP_Com_Semaforo = "127.0.0.1";
                    VTConfig.UsaLettore = false;
                    VTConfig.PortaLettore = 1;
                    VTConfig.CodiceUscita = "999999";
                    VTConfig.Sala = 1;
                    // parte come semaforo com per facilitare gli esterni,
                    // poi bisogner� fare un wizard di configurazione
                    VTConfig.TipoSemaforo = VSDecl.SEMAFORO_COM;
                    // ora scrivo
                    qryStd.ExecuteNonQuery();
                }

                // chiudo la transazione
                traStd.Commit();
                result = 0;
            }
            catch (Exception objExc)
            {
                result = 1;
                traStd.Rollback();
                Utils.errorCall("DammiConfigTotem", objExc.Message);
            }
            finally
            {
                qryStd.Dispose();
                traStd.Dispose();
                CloseConnection("");
            }

            return result;
        }

        public override int DammiConfigDatabase() //ref TTotemConfig TotCfg)
        {
            if (!OpenConnection("DammiConfigDatabase")) return 0;
            int result = 0;
            // preparo gli oggetti
            SqlCommand qryStd = new SqlCommand
                {
                    Connection = STDBConn,
                    CommandText = "select * from CONFIG_CfgVotoSegreto with (nolock) where attivo = 1"
                };
            // la configurazione ci deve essere, non � necessario inserirla
            try
            {
                SqlDataReader a = qryStd.ExecuteReader();
                // se c'� il record
                if (a.HasRows)
                {
                    // devo verificare 
                    a.Read();
                    // carico
                    VTConfig.ModoAssemblea = Convert.ToInt32(a["ModoAssemblea"]);
                    // il link del voto
                    VTConfig.SalvaLinkVoto = Convert.ToBoolean(a["SalvaLinkVoto"]);
                    // il salvataggio del voto anche se non ha confermato
                    VTConfig.SalvaVotoNonConfermato = Convert.ToBoolean(a["SalvaVotoNonConfermato"]);
                    // il salvataggio del voto su Geas
                    VTConfig.SalvaVotoInGeas = Convert.ToBoolean(a["SalvaVotoInGeas"]);
                    // Massimo n. di deleghe accettate
                    VTConfig.MaxDeleghe = Convert.ToInt32(a["MaxDeleghe"]);
                    // l'id della scheda che deve essere salvata in caso di 999999
                    VTConfig.IDSchedaUscitaForzata = Convert.ToInt32(a["IDSchedaUscitaForzata"]);
                    // ModoPosizioneAreeTouch
                    VTConfig.ModoPosizioneAreeTouch = Convert.ToInt32(a["ModoPosizioneAreeTouch"]);
                    // controllo delle presenze
                    VTConfig.ControllaPresenze = Convert.ToInt32(a["ControllaPresenze"]);
                    // AbilitaBottoneUscita
                    VTConfig.AbilitaBottoneUscita = Convert.ToBoolean(a["AttivaAutoRitornoVoto"]);
                    // AttivaAutoRitornoVoto
                    VTConfig.AttivaAutoRitornoVoto = Convert.ToBoolean(a["AttivaAutoRitornoVoto"]);
                    // TimeAutoRitornoVoto
                    VTConfig.TimeAutoRitornoVoto = Convert.ToInt32(a["TimeAutoRitornoVoto"]);
                    // AbilitaDirittiNonVoglioVotare
                    VTConfig.AbilitaDirittiNonVoglioVotare = Convert.ToBoolean(a["AbilitaDirittiNonVoglioVotare"]);
                    // AbilitaDifferenziatoSuRichiesta
                    VTConfig.AbilitaDifferenziatoSuRichiesta = Convert.ToBoolean(a["AbilitaDifferenziatoSuRichiesta"]);
                }
                a.Close();

                // chiudo la transazione
                result = 0;
            }
            catch (Exception objExc)
            {
                result = 1;
                Utils.errorCall("DammiConfigDatabase", objExc.Message);
            }
            finally
            {
                qryStd.Dispose();
                CloseConnection("");
            }

            return result;
        }
        
        public override int SalvaConfigurazione() //, ref TTotemConfig ATotCfg)
        {
            if (!OpenConnection("SalvaConfigurazione")) return 0;
            int result = 0;
            // preparo gli oggetti
            int usal = VTConfig.UsaLettore ? 1 : 0;
            int usas = VTConfig.UsaSemaforo ? 1 : 0;
            SqlCommand qryStd = new SqlCommand {Connection = STDBConn};
            // devo inserirlo
            SqlTransaction traStd = STDBConn.BeginTransaction();
            try
            {
                qryStd.Transaction = traStd;
                qryStd.CommandText = "update CONFIG_POSTAZIONI_TOTEM with (rowlock) set " +
                        "  UsaLettore = " + usal.ToString() +
                        ", PortaLettore = " + VTConfig.PortaLettore.ToString() +
                        ", UsaSemaforo = " + usas.ToString() +
                        ", IPCOMSemaforo = '" + VTConfig.IP_Com_Semaforo + "'" +
                        " where Postazione = '" + VTConfig.NomeTotem + "'";
                qryStd.ExecuteNonQuery();
                traStd.Commit();
                result = 1;
            }
            catch (Exception objExc)
            {
                traStd.Rollback();
                result = 0;
                Utils.errorCall("SalvaConfigurazioneLettore", objExc.Message);
            }
            finally
            {
                qryStd.Dispose();
                traStd.Dispose();
                CloseConnection("");
            }

            return result;
        }

        public override int SalvaConfigurazionePistolaBarcode() //, ref TTotemConfig ATotCfg)
        {
            if (!OpenConnection("SalvaConfigurazionePistolaBarcode")) return 0;
            int result = 0;
            // preparo gli oggetti
            int usal = VTConfig.UsaLettore ? 1 : 0;
            int usas = VTConfig.UsaSemaforo ? 1 : 0;
            SqlCommand qryStd = new SqlCommand { Connection = STDBConn };
            // devo inserirlo
            SqlTransaction traStd = STDBConn.BeginTransaction();
            try
            {
                qryStd.Transaction = traStd;
                qryStd.CommandText = "update CONFIG_POSTAZIONI_TOTEM with (rowlock) set " +
                        "  UsaLettore = " + usal.ToString() +
                        ", PortaLettore = " + VTConfig.PortaLettore.ToString() +
                        " where Postazione = '" + VTConfig.NomeTotem + "'";
                qryStd.ExecuteNonQuery();
                traStd.Commit();
                result = 1;
            }
            catch (Exception objExc)
            {
                traStd.Rollback();
                result = 0;
                Utils.errorCall("SalvaConfigurazionePistolaBarcode", objExc.Message);
            }
            finally
            {
                qryStd.Dispose();
                traStd.Dispose();
                CloseConnection("");
            }

            return result;
        }

        #endregion       
        
        //  CARICAMENTO DATI VOTAZIONI  --------------------------------------------------------------------------

        #region Caricamento dati votazioni

        public override List<CDB_Votazione> CaricaVotazioniDaDatabase()
        {
            // testo la connessione
            if (!OpenConnection("CaricaVotazioniDaDatabase")) return null;

            List<CDB_Votazione> votaz =  new List<CDB_Votazione>();

            SqlCommand qryStd = new SqlCommand { Connection = STDBConn };
            try
            {
                // ok ora carico le votazioni
                qryStd.Parameters.Clear();
                qryStd.CommandText = qry_DammiVotazioniTotem;
                SqlDataReader a = qryStd.ExecuteReader();
                if (a.HasRows)
                {
                    while (a.Read())
                    {
                        CDB_Votazione v = new CDB_Votazione
                        {
                            DB_NumVotaz = Convert.ToInt32(a["NumVotaz"]),
                            DB_MozioneRealeGeas = Convert.ToInt32(a["MozioneRealeGeas"]),
                            DB_IDGruppoVoto = Convert.ToInt32(a["GruppoVotaz"]),
                            DB_TipoVoto = Convert.ToInt32(a["TipoVotaz"]),
                            DB_TipoSubVoto = Convert.ToInt32(a["TipoSubVotaz"]),
                            DB_Argomento = a["Argomento"].ToString(),
                            DB_SkBianca = Convert.ToBoolean(a["VotoSchedaBianca"]),
                            DB_SkNonVoto = Convert.ToBoolean(a["VotoNonVotante"]),
                            DB_SkContrarioTutte = Convert.ToBoolean(a["SchedaContrarioTutte"]),
                            DB_SkAstenutoTutte = Convert.ToBoolean(a["SchedaAstenutoTutte"]),
                            DB_SelezionaTuttiCDA = Convert.ToBoolean(a["SelezTuttiCDA"]),
                            //PreIntermezzo = Convert.ToBoolean(a["PreIntermezzo"]),
                            DB_MaxScelte = a.IsDBNull(a.GetOrdinal("MaxScelte")) ? 1 : Convert.ToInt32(a["MaxScelte"]),
                            DB_MinScelte = a.IsDBNull(a.GetOrdinal("MinScelte")) ? 1 : Convert.ToInt32(a["MinScelte"]),
                            DB_AbilitaBottoneUscita = Convert.ToBoolean(a["VotoBottoneUscita"])
                        };
                        votaz.Add(v);
                    }
                }
                a.Close();
            }
            catch (Exception objExc)
            {
                votaz = null;
                Utils.errorCall("CaricaVotazioniDaDatabase", objExc.Message);
            }
            finally
            {
                qryStd.Dispose();
                CloseConnection("");
            }
            return votaz;
        }

        /*
        public override bool CaricaVotazioniDaDatabase(ref List<TVotazione> AVotazioni)
        {
            bool result = false; 

            // testo la connessione
            if (!OpenConnection("CaricaVotazioniDaDatabase")) return false;

            AVotazioni.Clear();

            SqlCommand qryStd = new SqlCommand { Connection = STDBConn };
            try
            {
                // ok ora carico le votazioni
                qryStd.Parameters.Clear();
                qryStd.CommandText = qry_DammiVotazioniTotem;
                //qryStd.CommandText =   "SELECT * from VS_MatchVot_Totem with (NOLOCK)  where GruppoVotaz < 999 order by NumVotaz";
                SqlDataReader a = qryStd.ExecuteReader();
                if (a.HasRows)
                {
                    while (a.Read())
                    {
                        TVotazione v = new TVotazione
                        {
                            NumVotaz = Convert.ToInt32(a["NumVotaz"]),
                            MozioneRealeGeas = Convert.ToInt32(a["MozioneRealeGeas"]),
                            IDGruppoVoto = Convert.ToInt32(a["GruppoVotaz"]),
                            TipoVoto = Convert.ToInt32(a["TipoVotaz"]),
                            TipoSubVoto = Convert.ToInt32(a["TipoSubVotaz"]),
                            Argomento = a["Argomento"].ToString(),
                            SkBianca = Convert.ToBoolean(a["VotoSchedaBianca"]),
                            SkNonVoto = Convert.ToBoolean(a["VotoNonVotante"]),
                            SkContrarioTutte = Convert.ToBoolean(a["SchedaContrarioTutte"]),
                            SkAstenutoTutte = Convert.ToBoolean(a["SchedaAstenutoTutte"]),
                            SelezionaTuttiCDA = Convert.ToBoolean(a["SelezTuttiCDA"]),
                            //PreIntermezzo = Convert.ToBoolean(a["PreIntermezzo"]),
                            MaxScelte = a.IsDBNull(a.GetOrdinal("MaxScelte")) ? 1 : Convert.ToInt32(a["MaxScelte"]),
                            MinScelte = a.IsDBNull(a.GetOrdinal("MinScelte")) ? 1 : Convert.ToInt32(a["MinScelte"]),
                            AbilitaBottoneUscita = Convert.ToBoolean(a["VotoBottoneUscita"])
                        };
                        AVotazioni.Add(v);
                    }
                }
                a.Close();

                // ok ora carico le subvotazioni
                foreach (TVotazione votazione in AVotazioni)
                {
                    qryStd.Parameters.Clear();
                    qryStd.CommandText = @"select * from VS_MatchVot_Gruppo_Totem 
                                where gruppovotaz = @gruppovotaz order by numsubvotaz";
                    qryStd.Parameters.Add("@gruppovotaz", System.Data.SqlDbType.Int).Value = votazione.IDGruppoVoto;
                    SqlDataReader b = qryStd.ExecuteReader();
                    if (b.HasRows)
                    {
                        while (b.Read())
                        {
                            TSubVotazione subvoto = new TSubVotazione()
                            {
                                NumSubVotaz = Convert.ToInt32(b["NumSubVotaz"]),
                                MozioneRealeGeas = Convert.ToInt32(b["MozioneRealeGeas"]),
                                IDGruppoVoto = Convert.ToInt32(b["GruppoVotaz"]),
                                TipoSubVoto = Convert.ToInt32(b["TipoVotaz"]),
                                Argomento = b["Argomento"].ToString()
                            };
                            votazione.SubVotazioni.Add(subvoto);
                        }
                    }
                    b.Close();
                }

                result = true;
            }
            catch (Exception objExc)
            {
                Utils.errorCall("CaricaVotazioniDaDatabase", objExc.Message);
            }
            finally
            {
                qryStd.Dispose();
                CloseConnection("");
            }
            return result;
        }
        */

        public override bool CaricaListeDaDatabase(ref List<CVotazione> AVotazioni)
        {
            bool result = false; //, naz;

            // testo la connessione
            if (!OpenConnection("CaricaVotazioniDaDatabase")) return false;

            SqlCommand qryStd = new SqlCommand { Connection = STDBConn };
            try
            {
                // TODO: CaricaListeDaDatabase da vedere in futuro di fare un solo ciclo di caricamento senza ordine
                // ciclo sulle votazioni e carico le liste
                foreach (CVotazione votaz in AVotazioni)
                {
                    // ok ora carico le votazioni
                    qryStd.Parameters.Clear();
                    qryStd.CommandText = "SELECT * from VS_Liste_Totem with (NOLOCK) " +
                                         "where NumVotaz = @IDVoto and Attivo = 1 ";
                    // ecco, in funzione del tipo di voto
                    switch (votaz.TipoVoto)
                    {
                        // se � lista ordino per l'id
                        case VSDecl.VOTO_LISTA:
                            qryStd.CommandText += " order by idlista";
                            break;
                        // se � candidato ordino in modo alfabetico
                        case VSDecl.VOTO_CANDIDATO:
                        //case VSDecl.VOTO_CANDIDATO_SING:
                        case VSDecl.VOTO_MULTICANDIDATO:
                            qryStd.CommandText += " order by PresentatoDaCdA desc, OrdineCarica, DescrLista "; //DescrLista ";
                            break;
                        default:
                            qryStd.CommandText += " order by idlista";
                            break;
                    }
                    qryStd.Parameters.Add("@IDVoto", System.Data.SqlDbType.Int).Value = votaz.NumVotaz;
                    SqlDataReader a = qryStd.ExecuteReader();
                    if (a.HasRows)
                    {
                        while (a.Read())
                        {
                            TLista l = new TLista
                            {
                                NumVotaz = Convert.ToInt32(a["NumVotaz"]),
                                NumSubVotaz = Convert.ToInt32(a["NumSubVotaz"]),
                                IDLista = Convert.ToInt32(a["idLista"]),
                                IDScheda = Convert.ToInt32(a["idScheda"]),
                                DescrLista = a.IsDBNull(a.GetOrdinal("DescrLista")) ? "DESCRIZIONE" : a["DescrLista"].ToString(),
                                TipoCarica = Convert.ToInt32(a["TipoCarica"]),
                                PresentatodaCDA = Convert.ToBoolean(a["PresentatodaCDA"]),
                                Presentatore = a.IsDBNull(a.GetOrdinal("Presentatore")) ? "" : a["Presentatore"].ToString(),
                                Capolista = a.IsDBNull(a.GetOrdinal("Capolista")) ? "" : a["Capolista"].ToString(),
                                ListaElenco = a.IsDBNull(a.GetOrdinal("ListaElenco")) ? "DESCRIZIONE" : a["ListaElenco"].ToString()
                            };
                            votaz.Liste.Add(l);
                        }
                    }
                    a.Close();
                
                    // TODO: DA RIVEDERE CHE FUNZIONA SOLO SU UN VOTO SOLO
                    // carica la dicitira astenuto e contrario
                    qryStd.Parameters.Clear();
                    qryStd.CommandText = @"select distinct IDscheda, descrLista from VS_Liste_Totem
                                            where IdScheda = 226 or IdScheda = 227 ";
                    a = qryStd.ExecuteReader();
                    if (a.HasRows)
                    {
                        while (a.Read())
                        {
                            int IDScheda = Convert.ToInt32(a["idScheda"]);
                            string DescrLista = a.IsDBNull(a.GetOrdinal("DescrLista")) ? "DESCRIZIONE" : a["DescrLista"].ToString();
                            switch (IDScheda)
                            {
                                case VSDecl.VOTO_CONTRARIO_TUTTI:
                                    VTConfig.ContrarioATutti = DescrLista;
                                    break;
                                case VSDecl.VOTO_ASTENUTO_TUTTI:
                                    VTConfig.AstenutoATutti = DescrLista;
                                    break;
                            }
                        }
                    }
                    a.Close();

                }
                result = true;
            }
            catch (Exception objExc)
            {
                Utils.errorCall("CaricaListeDaDatabase", objExc.Message);
            }
            finally
            {
                qryStd.Dispose();
                CloseConnection("");
            }
            return result;
        }

        #endregion

		//  METODI SUI BADGE  --------------------------------------------------------------------------

        #region Metodi sui Badge (Presenza, ha gi� votato...)

        //override public bool ControllaBadge(int AIDBadge, TTotemConfig ATotCfg, ref int AReturnFlags)
        public override bool ControllaBadge(int AIDBadge, ref int AReturnFlags)
        {
            // questa procedura effettua in un colpo solo tutti i controlli relativi al badge
            // 1 - Se il badge � annullato
            // 2 - Controlla se � presente e in caso di forzatura mette il movimento
            // 3 - Controlla se ha gi� votato
            // Il tutto in un unica transazione
            // naturalmente true indica che il controllo � andato a buon fine e pu� continuare

            SqlTransaction traStd = null;
            bool Presente = false, resCons, BAnnull = true, BNonEsiste = true, BAbilitato = false;

            // testo la connessione
            if (!OpenConnection("ControllaBadge")) return false;

            bool result = true;
            SqlCommand qryStd = new SqlCommand {Connection = STDBConn};
            // apro una transazione atomica
            // metto sotto try
            try
            {
                traStd = STDBConn.BeginTransaction();
                qryStd.Transaction = traStd;
                
                // -------------------------------------------------
                // ok, ora testo se � annullato
                BAnnull = false;
                qryStd.Parameters.Clear();
                qryStd.CommandText = "SELECT Annullato FROM GEAS_Titolari with (NOLOCK) WHERE Badge = @Badge"; //'" + AIDBadge.ToString() + "'";
                qryStd.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                SqlDataReader a = qryStd.ExecuteReader();
                if (a.HasRows)
                {
                    // annullato non pu� essere null 
                    a.Read();
                    int ii = Convert.ToInt32(a["Annullato"]);
                    BAnnull = (ii > 0);
                    BNonEsiste = false;
                }
                else
                {
                    BAnnull = true;     // se non ha record non esiste
                    BNonEsiste = true;
                }
                a.Close();

                // OK, se � annullato non ci posso fare nulla e skippo direttamente, se invece � ok
                // continuo
                if (!BAnnull)
                {

                    // -------------------------------------------------
                    // se sono in geas mode devo controllare se il badge � abilitato a inizio votazione corrente
                    // se non lo � devo mettere il movimento di ingresso uscita all'ora della votazione
                    if (VTConfig.ControllaPresenze == VSDecl.PRES_MODO_GEAS)
                    {
                        BAbilitato = false;
                        qryStd.CommandText = qry_DammiBadgePresenteGeas;
                        qryStd.Parameters.Clear();
                        qryStd.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                        a = qryStd.ExecuteReader();
                        if (a.HasRows)
                        {
                            // devo verificare, il campo non pu� essere null 
                            a.Read();
                            string mv = a["TipoMov"].ToString();
                            BAbilitato = (mv == "E");
                        }
                        a.Close();

                        // se non � abilitato, forzo il movimento 1s prima ingresso e 1s dopo uscita
                        qryStd.CommandText = qry_MettiBadgePresenteGeas;
                        qryStd.Parameters.Clear();
                        qryStd.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                        qryStd.Parameters.Add("@Sala", System.Data.SqlDbType.VarChar).Value = VTConfig.Sala;
                        qryStd.ExecuteNonQuery();

                        BAbilitato = true;
                    }
                    else
                        BAbilitato = true;

                    // -------------------------------------------------
                    // ok ora testo se � presente a questo momento
                    Presente = false;
                    qryStd.CommandText = @"SELECT TipoMov FROM GEAS_TimbInOut with (NOLOCK) 
                                        WHERE Badge = @Badge AND GEAS_TimbInOut.Reale=1 
                                        AND DataOra=(SELECT MAX(DataOra) FROM GEAS_TimbInOut with (NOLOCK)
                                        WHERE Badge = @Badge AND GEAS_TimbInOut.Reale=1)";
                    qryStd.Parameters.Clear();
                    qryStd.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                    a = qryStd.ExecuteReader();
                    if (a.HasRows)
                    {
                        // devo verificare, il campo non pu� essere null 
                        a.Read();
                        string mv = a["TipoMov"].ToString();
                        Presente = (mv == "E");
                    }
                    a.Close();

                    // ok, se non � presente e il flag � VSDecl.PRES_FORZA_INGRESSO o VSDecl.PRES_MODO_GEAS
                    // forzo un movimento di ingresso a questo momento
                    if (!Presente && (VTConfig.ControllaPresenze == VSDecl.PRES_FORZA_INGRESSO ||
                          VTConfig.ControllaPresenze == VSDecl.PRES_MODO_GEAS))
                    {
                        // forzo il movimento
                        qryStd.CommandText = @"insert into Geas_TimbinOut with (ROWLOCK) 
                                                (DataOra, Badge, TipoMov, Reale, Classe, Terminale, DataIns)
                                                values ({ fn NOW() } , @Badge, 'E', 1, 99, @Sala, { fn NOW() })";
                        // eseguo
                        qryStd.Parameters.Clear();
                        qryStd.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                        qryStd.Parameters.Add("@Sala", System.Data.SqlDbType.Int).Value = VTConfig.Sala;
                        qryStd.ExecuteNonQuery();
                        Presente = true;
                    }
                }

                // qua faccio un elaborazione successsiva in funzione del flag ControllaPresenze
                // per avere un valore assoluto nel confronto finale
                // perch� se Presente = true va tutto bene, ma se Presente � a false
                // bisogna testare il flag ControllaPresenze perch� nel caso "PRES_NON_CONTROLLARE"
                // � ok lo stesso e bisogna mettere Presente a true x il confronto finale
                if (!Presente && VTConfig.ControllaPresenze == VSDecl.PRES_NON_CONTROLLARE)
                    Presente = true;

                // -------------------------------------------------
                // ok, ora testo se ha votato

                // modifiche AbilitaDirittiNonVoglioVotare.
                if (VTConfig.AbilitaDirittiNonVoglioVotare)
                {
                    // se � abilitato il controllo su non voglio votare vuol dire che non salvo in 
                    // vs_votanti_totem, ma controllo i residui diritti di voto
                    // e quindi forza il controllo a false
                    resCons = false;
                }
                else
                {
                    // si comporta normalmente
                    qryStd.CommandText = "SELECT * from VS_ConSchede with (NOLOCK) where Badge = '" + AIDBadge.ToString() + "'";
                    a = qryStd.ExecuteReader();
                    resCons = a.HasRows;
                    a.Close();
                    // se non ha consegnato schede allora verifico se ha gi� votato
                    if (!resCons)
                    {
                        qryStd.CommandText = "SELECT * from VS_Votanti_Totem with (NOLOCK) where Badge = '" + AIDBadge.ToString() + "'";
                        a = qryStd.ExecuteReader();
                        resCons = a.HasRows;
                        a.Close();
                    }
                }               
                traStd.Commit();

                // ok, ora devo elaborare il risultato che deve essere
                //  BAnnull = false, Presente = true (ma solo se il controllo � attivato), resCons = false
                // naturalmente true indica che il controllo � andato a buon fine e pu� continuare
                // � un and quindi tutti i valori devono essere a true
                result = (!BAnnull &&   // se non � annullato � a true
                          Presente &&   // � presente
                          !resCons &&   // se non ha schede consegnate � a true
                          BAbilitato);  // Se � abilitato (solo geas) 

                // ok ora compongo il flag degli eventuali errori
                AReturnFlags = 0;
                if (BAnnull)
                {
                    if (BNonEsiste)
                        AReturnFlags = AReturnFlags | 0x40;
                    else
                        AReturnFlags = AReturnFlags | 0x01;
                }
                if (!Presente) AReturnFlags = AReturnFlags | 0x02;
                if (resCons) AReturnFlags = AReturnFlags | 0x04;
                if (!BAbilitato) AReturnFlags = AReturnFlags | 0x80;

            }
            catch (Exception objExc)
            {
                traStd?.Rollback();
                Utils.errorCall("ControllaBadge", objExc.Message);
                result = false;
            }
            finally
            {
                qryStd.Dispose();
                traStd?.Dispose();
                CloseConnection("");
            }

            return result;
        }

        //override public string DammiNomeAzionista(int AIDBadge)
        //{
        //    // mi dice la lunghezza del badge e il codice impianto per il lettore
        //    SqlDataReader a;
        //    SqlCommand qryStd;
        //    string NomeAz = "", Sesso = "";

        //    // testo la connessione
        //    if (!OpenConnection("DammiNomeAzionista")) return "";

        //    qryStd = new SqlCommand();
        //    try
        //    {
        //        qryStd.Connection = STDBConn;
        //        // Leggo ora da GEAS_Titolari	
        //        qryStd.CommandText = "select T.badge, T.idazion, A.Sesso, " + 
        //                             " CASE WHEN A.FisGiu ='F' THEN A.Cognome+ ' ' + A.Nome ELSE A.Raso END as Raso1 " +
        //                             " from geas_titolari T " + 
        //                             " INNER JOIN GEAS_Anagrafe As A  with (NOLOCK) ON T.IdAzion = A.IdAzion " + 
        //                             " WHERE T.Badge = @Badge AND T.Reale=1";
        //        qryStd.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
        //        a = qryStd.ExecuteReader();
        //        if (a.HasRows)
        //        {
        //            // devo verificare 
        //            a.Read();
        //            NomeAz = a.IsDBNull(a.GetOrdinal("Raso1")) ? "" : (a["Raso1"]).ToString();
        //            Sesso = a.IsDBNull(a.GetOrdinal("Sesso")) ? "" : (a["Sesso"]).ToString();
        //        }
        //        a.Close();

        //        if (Sesso == "M")
        //            NomeAz = "Sig. " + NomeAz;
        //        if (Sesso == "F")
        //            NomeAz = "Sig.ra " + NomeAz;

        //    }
        //    catch (Exception objExc)
        //    {
        //        Logging.WriteToLog("<dberror> Errore nella funzione DammiNomeAzionista: " + objExc.Message);
        //        MessageBox.Show("Errore nella funzione DammiNomeAzionista" + "\n" +
        //            "Eccezione : \n" + objExc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    finally
        //    {
        //        qryStd.Dispose();
        //        CloseConnection("");
        //    }

        //    return NomeAz;
        //}

        #endregion

		//  LETTURA DATI AZIONISTA  --------------------------------------------------------------------------

        #region Caricamento dati Azionista 

        public override bool CaricaDirittidiVotoDaDatabase(int AIDBadge, ref List<TAzionista> AAzionisti,
                                                                ref TAzionista ATitolare_badge, ref CListaVotazioni AVotazioni)
        {
            // ok, questa funziomne carica i diritti di voto in funzione
            // del idbadge, in pratica alla fine avr� una lista di diritti *per ogni votazione*
            // con l'indicazione se sono stati gi� espressi o no

            // ok, questa procedura mi carica tutti i dati
            //SqlConnection STDBConn = null;
            SqlDataReader a = null;
            SqlCommand qryStd = null;
            TAzionista c;
            int IDVotazione = -1;
            bool result = false; //, naz;

            // testo la connessione
            if (!OpenConnection("CaricaDirittidiVotoDaDatabase")) return false;

            AAzionisti.Clear();

            qryStd = new SqlCommand { Connection = STDBConn };
            try
            {
                // ciclo sul voto per crearmi l'array dei diritti di voto per ogni singola votazione
                //for (int i = 0  ; i < NVoti; i++)
                // TODO: CVotoDBDati|CaricaDirittidiVotoDaDatabase - Inutile chiamare n volte la query
                foreach (CVotazione voto in AVotazioni.Votazioni)
                {
                    IDVotazione = voto.NumVotaz;

                    // resetto la query
                    qryStd.Parameters.Clear();

                    // ok ora carico il titolare
                    qryStd.CommandText = qry_DammiDirittiDiVoto_Titolare;
                    qryStd.Parameters.Add("@IDVotaz", System.Data.SqlDbType.Int).Value = IDVotazione;
                    qryStd.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                    a = qryStd.ExecuteReader();
                    // in teoria non pu� non avere righe, testa anche se ha azioni, se no � un rappr
                    if (a.HasRows && a.Read())
                    {
                        c = new TAzionista();
                        c.CoAz = a.IsDBNull(a.GetOrdinal("CoAz")) ? "0000000" : a["CoAz"].ToString();
                        c.IDAzion = Convert.ToInt32(a["IdAzion"]);
                        c.IDBadge = AIDBadge;
                        c.ProgDeleg = 0;
                        c.RaSo = a["Raso1"].ToString();
                        // TODO: GEAS VERSIONE
                        if (VTConfig.IsOrdinaria) // becca O, O/S o S/O
                        {
                            c.Voti1 = Convert.ToDouble(a["VtOrd1"]);
                            c.Voti2 = Convert.ToDouble(a["VtOrd2"]);
                            c.NVoti = c.Voti1 + c.Voti2;
                        }
                        else
                        {
                            if (!VTConfig.IsOrdinaria && VTConfig.IsStraordinaria) // BECCA S
                            {
                                c.Voti1 = Convert.ToDouble(a["VtStr1"]);
                                c.Voti2 = Convert.ToDouble(a["VtStr2"]);
                                c.NVoti = c.Voti1 + c.Voti2;   
                            }
                        }
                        c.Sesso = a.IsDBNull(a.GetOrdinal("Sesso")) ? "N" : a["Sesso"].ToString();
                        c.HaVotato = Convert.ToInt32(a["TitIDVotaz"]) >= 0
                                         ? TListaAzionisti.VOTATO_DBASE
                                         : TListaAzionisti.VOTATO_NO;
                        c.IDVotaz = IDVotazione;

                        // ok, ora se � titolare e ha azioni l'aggiungo alla lista
                        if (c.NVoti > 0)
                        //if ((Convert.ToInt32(a["AzOrd"]) + Convert.ToInt32(a["AzStr"])) > 0)
                            AAzionisti.Add(c);

                        // poi lo salvo come titolare
                        ATitolare_badge.CopyFrom(ref c);
                    }
                    a.Close();

                    // resetto la query
                    qryStd.Parameters.Clear();

                    // ora carico i deleganti
                    qryStd.CommandText = qry_DammiDirittiDiVoto_Deleganti;
                    qryStd.Parameters.Add("@IDVotaz", System.Data.SqlDbType.Int).Value = IDVotazione;
                    qryStd.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                    a = qryStd.ExecuteReader();
                    if (a.HasRows)
                    {
                        while (a.Read())        // qua posso avere pi� righe
                        {
                            // anche qua devo testare se ha azioni 0, potrebbe essere un badge banana
                            if ((Convert.ToInt32(a["VtOrd1"]) + Convert.ToInt32(a["VtStr1"]) +
                                Convert.ToInt32(a["VtOrd2"]) + Convert.ToInt32(a["VtStr2"])) > 0)
                            {
                                c = new TAzionista();
                                c.CoAz = a.IsDBNull(a.GetOrdinal("CoAz")) ? "0000000" : a["CoAz"].ToString();
                                c.IDAzion = Convert.ToInt32(a["IdAzion"]);
                                c.IDBadge = AIDBadge;
                                c.ProgDeleg = Convert.ToInt32(a["ProgDeleg"]);
                                c.RaSo = a["Raso1"].ToString();
                                // TODO: GEAS VERSIONE
                                if (VTConfig.IsOrdinaria)
                                {
                                    c.Voti1 = Convert.ToDouble(a["VtOrd1"]);
                                    c.Voti2 = Convert.ToDouble(a["VtOrd2"]);
                                    c.NVoti = c.Voti1 + c.Voti2;                                    
                                }
                                else
                                {
                                    if (!VTConfig.IsOrdinaria && VTConfig.IsStraordinaria)
                                    {
                                        c.Voti1 = Convert.ToDouble(a["VtStr1"]);
                                        c.Voti2 = Convert.ToDouble(a["VtStr2"]);
                                        c.NVoti = c.Voti1 + c.Voti2;
                                    }
                                }
                                c.Sesso = "N"; // a.IsDBNull(a.GetOrdinal("Sesso")) ? "N" : a["Sesso"].ToString();
                                c.HaVotato = Convert.ToInt32(a["ConIDVotaz"]) >= 0 ? TListaAzionisti.VOTATO_DBASE : TListaAzionisti.VOTATO_NO;
                                c.IDVotaz = IDVotazione;
                                // aggiungo
                                AAzionisti.Add(c);
                            }
                        }   //while (a.Read()) 
                    }   //if (a.HasRows)
                    a.Close();

                }   //for (int i = 0...
                result = true;

            }
            catch (Exception objExc)
            {
                Utils.errorCall("CaricaDirittidiVotoDaDatabaseDBDATI", objExc.Message);
            }
            finally
            {
                qryStd.Dispose();
                CloseConnection("");
            }
            return result;
        }

	    #endregion

        //  CONTROLLO DELLA VOTAZIONE --------------------------------------------------------------------------

        #region Salvataggio Voti

        //override public int SalvaTutto(int AIDBadge, TTotemConfig ATotCfg, ref TListaAzionisti AAzionisti)
        public override int SalvaTutto(int AIDBadge, ref TListaAzionisti AAzionisti)
        {
            // questa funzione viene chhiamata alla fine della votazione ed effettua le operazioni 
            // IN UN UNICA TRANSAZIONE:
            //
            //  1. un record in VS_Votanti_Totem che indica che il badge ha votato, per il controlo iniziale
            //  2. tanti record quanti sono gli azionisti con azioni > 0 in VS_ConSchede
            //  3. l'arraylist FVotiDaSalvare in VS_Intonse_Totem, i voti veri e propri

            SqlCommand qryStd = null, qryVoti = null;
            SqlTransaction traStd = null;
            int result = 0; const int TopRand = VSDecl.MAX_ID_RANDOM;
            //double PNAzioni1 = 0, PNAzioni2 = 0;

            // testo la connessione
            if (!OpenConnection("SalvaTutto")) return 0;

            qryStd = new SqlCommand {Connection = STDBConn};
            qryVoti = new SqlCommand { Connection = STDBConn };
            try
            {
                // abilito la transazione
                traStd = STDBConn.BeginTransaction();
                qryStd.Transaction = traStd;
                qryVoti.Transaction = traStd;

                // 1. scrivo che ha votato in VS_Votanti_Totem
                // se non � abilitato il non voto si comporta normalmente, quindi salva in vs_votanti_totem
                if (!VTConfig.AbilitaDirittiNonVoglioVotare)
                {
                    qryStd.Parameters.Clear();
                    qryStd.CommandText = @"insert into VS_Votanti_Totem with (ROWLOCK) 
                                                (Badge, idSeggio, DataOraVotaz, NomeComputer)
                                            VALUES 
                                                (@Badge, @idSeggio, { fn NOW() }, @NomeComputer)";
                    qryStd.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                    qryStd.Parameters.Add("@idSeggio", System.Data.SqlDbType.Int).Value = FIDSeggio;
                    qryStd.Parameters.Add("@NomeComputer", System.Data.SqlDbType.VarChar).Value = VTConfig.NomeTotem;
                    qryStd.ExecuteNonQuery();
                }

                // 2. ora scrivo vs_conschede e vs_intonse_totem insieme
                Random random = new Random();
                foreach (TAzionista az in AAzionisti.Azionisti)
                {
                    // salva solo se ha votato
                    if (az.HaVotato == TListaAzionisti.VOTATO_SESSIONE && !az.HaNonVotato)
                    {
                        // conschede
                        qryStd.Parameters.Clear();
                        qryStd.CommandText = @"INSERT INTO VS_ConSchede with (ROWLOCK) 
                                                (Badge, NumVotaz, IdAzion, ProgDeleg, IdSeggio, DataOraVotaz, NomeComputer, voti, voti2) 
                                              VALUES 
                                                (@Badge, @NumVotaz, @IdAzion, @ProgDeleg, @IdSeggio, { fn NOW() }, @NomeComputer, @voti, @voti2) ";
                        qryStd.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                        qryStd.Parameters.Add("@NumVotaz", System.Data.SqlDbType.Int).Value = az.IDVotaz;
                        qryStd.Parameters.Add("@IdAzion", System.Data.SqlDbType.Int).Value = az.IDAzion;
                        qryStd.Parameters.Add("@ProgDeleg", System.Data.SqlDbType.Int).Value = az.ProgDeleg;
                        qryStd.Parameters.Add("@idSeggio", System.Data.SqlDbType.Int).Value = FIDSeggio;
                        qryStd.Parameters.Add("@NomeComputer", System.Data.SqlDbType.VarChar).Value = VTConfig.NomeTotem;
                        qryStd.Parameters.Add("@voti", System.Data.SqlDbType.Int).Value = az.Voti1;
                        qryStd.Parameters.Add("@voti2", System.Data.SqlDbType.Int).Value = az.Voti2;
                        qryStd.ExecuteNonQuery();
                        // 
                        foreach (TVotoEspresso vt in az.VotiEspressi)
                        {
                            // intonse_totem, salvo il voto, ma prima devo fare qualche elaborazione
                            // 1. testo se devo togliere il link voto-azionista
                            int AIDBadge_OK = AIDBadge;
                            if (!VTConfig.SalvaLinkVoto)
                                AIDBadge_OK = random.Next(1, TopRand);

                            // salvo nel db
                            qryVoti.Parameters.Clear();
                            qryVoti.CommandText = @"insert into VS_Intonse_Totem  with (rowlock) 
                                                   (NumVotaz, idTipoScheda, idSeggio, voti, voti2, Badge, ProgDeleg, IdCarica) 
                                                   VALUES 
                                                   (@NumVotaz, @idTipoScheda, @idSeggio, @voti, @voti2, @Badge, @ProgDeleg, @IdCarica) ";
                            qryVoti.Parameters.Add("@NumVotaz", System.Data.SqlDbType.Int).Value = az.IDVotaz;
                            qryVoti.Parameters.Add("@idTipoScheda", System.Data.SqlDbType.Int).Value = vt.VotoExp_IDScheda;
                            qryVoti.Parameters.Add("@idSeggio", System.Data.SqlDbType.Int).Value = FIDSeggio;
                            qryVoti.Parameters.Add("@voti", System.Data.SqlDbType.Float).Value = az.Voti1;
                            qryVoti.Parameters.Add("@voti2", System.Data.SqlDbType.Float).Value = az.Voti2;
                            qryVoti.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge_OK.ToString();
                            qryVoti.Parameters.Add("@ProgDeleg", System.Data.SqlDbType.Int).Value = az.ProgDeleg;
                            qryVoti.Parameters.Add("@IdCarica", System.Data.SqlDbType.Int).Value = vt.TipoCarica;
                            qryVoti.ExecuteNonQuery();
                        }
                    }
                }

                // chiudo la transazione
                traStd.Commit();
                result = 1;
            }
            catch (Exception objExc)
            {
                traStd?.Rollback();
                result = 0;
                Utils.errorCall("SalvaTutto", objExc.Message);
            }
            finally
            {
                qryStd.Dispose();
                qryVoti.Dispose();
                traStd?.Dispose();
                CloseConnection("");
            }
            return result;
        }

        public override int SalvaTuttoInGeas(int AIDBadge, ref TListaAzionisti AAzionisti)
        {
            SqlCommand qryStd = null, qryVoti = null;
            SqlTransaction traStd = null;
            int result = 0;
            //double PNAzioni = 0;
            string TipoAsse = "";

            // TODO: GEAS VERSIONE (Salvataggio voti Geas NOTA: FUNZIONA SOLO CON UN VOTO)

            // testo la connessione
            if (!OpenConnection("SalvaTuttoInGeas")) return 0;

            qryStd = new SqlCommand {Connection = STDBConn};
            qryVoti = new SqlCommand { Connection = STDBConn };
            try
            {
                //  1. Mi calcolo il progmozione
                int ProgMozione = -1;
                qryStd.Parameters.Clear();
                qryStd.CommandText = @"select isnull(GEAS_MatchVot.ProgMozione, -1) as ProgMozione, TipoAsse from GEAS_MatchVot
					                        where GEAS_MatchVot.VotoSegretoDettaglio > 0";
                SqlDataReader a = qryStd.ExecuteReader();
                if (a.HasRows)
                {
                    // devo verificare, il campo non pu� essere null 
                    a.Read();
                    ProgMozione = Convert.ToInt32(a["ProgMozione"]);
                    TipoAsse = a["TipoAsse"].ToString();
                }
                a.Close();

                if (ProgMozione >= 0)
                {
                    // abilito la transazione
                    traStd = STDBConn.BeginTransaction();
                    qryStd.Transaction = traStd;
                    qryVoti.Transaction = traStd;

                    // devo salvare i voti in geas
                    // 2. salvo il titolare in geas_voti con voto 6, lo salvo comunque anche se ha azioni 0
                    qryStd.Parameters.Clear();
                    qryStd.CommandText = @"INSERT INTO Geas_Voti with (ROWLOCK) 
                                                (ProgMozione, ProgSubVotaz, Reale, Badge, MozioneRea, SubVotaz, TipoVoto, DataOraVoto, NumFav, IsSelection) 
                                              VALUES 
                                                (@ProgMozione, -1, 1, @Badge, 0, 0, 6, { fn NOW() }, 0, 0) ";
                    qryStd.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                    qryStd.Parameters.Add("@ProgMozione", System.Data.SqlDbType.Int).Value = ProgMozione;
                    qryStd.ExecuteNonQuery();

                    // occhio che devo inserire il titolare se ha azioni 0
                    if (AAzionisti.Titolare_Badge.NVoti == 0)
                    {
                        qryVoti.Parameters.Clear();
                        qryVoti.CommandText = @"insert into GEAS_VotiDiff with (rowlock)
                                                        (ProgMozione, ProgSubVotaz, Badge, ProgDeleg, ValAssem, TipoVoto, AzioniSi, VotiSi,
                                                            PercSi, AzioniNo, VotiNo, PercNo, AzioniAst, VotiAst, PercAst,AzioniCi, VotiCi,
                                                            AzioniNv, VotiNv, PercNv, AzioniNq, VotiNq, PercNq)
                                                    Values
                                                        (@ProgMozione, -1, @Badge, 0, @ValAssem, 0, 0, 0,
                                                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";

                        qryVoti.Parameters.Add("@ProgMozione", System.Data.SqlDbType.Int).Value = ProgMozione;
                        qryVoti.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                        //qryVoti.Parameters.Add("@ProgDeleg", System.Data.SqlDbType.Int).Value = az.ProgDeleg;
                        qryVoti.Parameters.Add("@ValAssem", System.Data.SqlDbType.VarChar).Value = TipoAsse;
                        qryVoti.ExecuteNonQuery();
                    }

                    // 3 . ok ora salvo i singoli voti in geas_diff
                    foreach (TAzionista az in AAzionisti.Azionisti)
                    {
                                               
                        foreach (TVotoEspresso vt in az.VotiEspressi)
                        {
                            double ASi = 0, VSi = 0, PSi = 0, ANo = 0, VNo = 0, PNo = 0, AAst = 0, VAst = 0,
                                   PAst = 0, ANv = 0, VNv = 0, PNv = 0;
                            
                            int TipoVoto = vt.VotoExp_IDScheda;

                            switch (vt.VotoExp_IDScheda)
                            {
                                // li metto hardcoded
                                //Fav e anche le liste
                                case 1:
                                case 129:
                                case 130:
                                case 131:
                                case 132:
                                case 133:
                                case 134:
                                case 135:
                                case 137:
                                case 138:
                                case 139:
                                case 140:
                                    ASi = az.NVoti;
                                    VSi = 1;
                                    PSi = 100;
                                    break;
                                // contr
                                case 2:
                                case 227:
                                    ANo = az.NVoti;
                                    VNo = 1;
                                    PNo = 100;
                                    break;
                                // ast
                                case 3:
                                case 226:
                                    AAst = az.NVoti;
                                    VAst = 1;
                                    PAst = 100;
                                    break;
                                // nv
                                case -2:
                                case -3:
                                case -4:
                                    TipoVoto = 0;
                                    ANv = az.NVoti;
                                    VNv = 1;
                                    PNv = 100;
                                    break;
                            }

                            qryVoti.Parameters.Clear();
                            qryVoti.CommandText = @"insert into GEAS_VotiDiff with (rowlock)
                                                        (ProgMozione, ProgSubVotaz, Badge, ProgDeleg, ValAssem, TipoVoto, AzioniSi, VotiSi,
                                                            PercSi, AzioniNo, VotiNo, PercNo, AzioniAst, VotiAst, PercAst,AzioniCi, VotiCi,
                                                            AzioniNv, VotiNv, PercNv, AzioniNq, VotiNq, PercNq)
                                                    Values
                                                        (@ProgMozione, -1, @Badge, @ProgDeleg, @ValAssem, @TipoVoto, @Azionisi, @VotiSi,
                                                            @PercSi, @AzioniNo, @VotiNo, @PercNo, @AzioniAst, @VotiAst, @PercAst, 0, 0,
                                                            @AzioniNv, @VotiNv, @PercNv, 0, 0, 0)";

                            qryVoti.Parameters.Add("@ProgMozione", System.Data.SqlDbType.Int).Value = ProgMozione;
                            qryVoti.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                            qryVoti.Parameters.Add("@ProgDeleg", System.Data.SqlDbType.Int).Value = az.ProgDeleg;
                            qryVoti.Parameters.Add("@ValAssem", System.Data.SqlDbType.VarChar).Value = TipoAsse;
                            qryVoti.Parameters.Add("@TipoVoto", System.Data.SqlDbType.Int).Value = TipoVoto; // vt.VotoExp_IDScheda;

                            qryVoti.Parameters.Add("@AzioniSi", System.Data.SqlDbType.Decimal).Value = ASi;
                            qryVoti.Parameters.Add("@VotiSi", System.Data.SqlDbType.Decimal).Value = VSi;
                            qryVoti.Parameters.Add("@PercSi", System.Data.SqlDbType.Decimal).Value = PSi;

                            qryVoti.Parameters.Add("@AzioniNo", System.Data.SqlDbType.Decimal).Value = ANo;
                            qryVoti.Parameters.Add("@VotiNo", System.Data.SqlDbType.Decimal).Value = VNo;
                            qryVoti.Parameters.Add("@PercNo", System.Data.SqlDbType.Decimal).Value = PNo;

                            qryVoti.Parameters.Add("@AzioniAst", System.Data.SqlDbType.Decimal).Value = AAst;
                            qryVoti.Parameters.Add("@VotiAst", System.Data.SqlDbType.Decimal).Value = VAst;
                            qryVoti.Parameters.Add("@PercAst", System.Data.SqlDbType.Decimal).Value = PAst;

                            qryVoti.Parameters.Add("@AzioniNv", System.Data.SqlDbType.Decimal).Value = ANv;
                            qryVoti.Parameters.Add("@VotiNv", System.Data.SqlDbType.Decimal).Value = VNv;
                            qryVoti.Parameters.Add("@PercNv", System.Data.SqlDbType.Decimal).Value = PNv;

                            //qryVoti.Parameters.Add("@idSeggio", System.Data.SqlDbType.Int).Value = FIDSeggio;
                            //qryVoti.Parameters.Add("@voti", System.Data.SqlDbType.Float).Value = PNAzioni;
                            //qryVoti.Parameters.Add("@IdCarica", System.Data.SqlDbType.Int).Value = vt.TipoCarica;
                            qryVoti.ExecuteNonQuery();
                        }
                    }
                    // chiudo la transazione
                    traStd.Commit();
                    result = 1;
                }
            }
            catch (Exception objExc)
            {
                traStd?.Rollback();
                result = 0;
                Utils.errorCall("SalvaTuttoInGeas", objExc.Message);
            }
            finally
            {
                qryStd.Dispose();
                qryVoti.Dispose();
                traStd?.Dispose();
                CloseConnection("");
            }
            return result;
        }

        #endregion

        //  ALTRE FUNZIONI DELLA VOTAZIONE  --------------------------------------------------------------------------

        #region Altre funzioni votazione

        public override int NumAzTitolare(int AIDBadge)
        {
            //  mi da quante azioni ha un titolare
            SqlDataReader ab;
            SqlCommand qryStd1;
            int result = 0;

            // testo la connessione
            if (!OpenConnection("NumAzTitolare")) return 0;

            qryStd1 = new SqlCommand();
            qryStd1.Connection = STDBConn;
            // apro la query
            qryStd1.CommandText = " SELECT A.TipoAssemblea,COALESCE(Azioni1Ord,0)+COALESCE(Azioni2Ord,0) AS AzOrd,COALESCE(Azioni1Str,0)+COALESCE(Azioni2Str,0) AS AzStr " +
                                 " FROM GEAS_Titolari AS T with (nolock), CONFIG_AppoggioR AS A with (nolock) WHERE (T.ValAssem Like '%'+A.TipoAssemblea + '%') " +
                                 " AND T.Badge='" + AIDBadge.ToString() + "' AND T.Reale=1";
            ab = qryStd1.ExecuteReader();
            if (ab.HasRows)
            {
                ab.Read();
                // possono essere nulli
                if (VTConfig.IsOrdinaria)
                    result = ab.IsDBNull(ab.GetOrdinal("AzOrd")) ? 0 : Convert.ToInt32(ab["AzOrd"]);
                //c.NAzioni = Convert.ToDouble(a["AzOrd"]);
                else
                {
                    if (!VTConfig.IsOrdinaria && VTConfig.IsStraordinaria)
                        result = ab.IsDBNull(ab.GetOrdinal("AzStr")) ? 0 : Convert.ToInt32(ab["AzStr"]);
                    //c.NAzioni = Convert.ToDouble(a["AzStr"]);
                }
                
                //if (ab.IsDBNull(ab.GetOrdinal("AzOrd"))) AzO = 0;
                //else AzO = Convert.ToInt32(ab["AzOrd"]);

                //if (ab.IsDBNull(ab.GetOrdinal("AzStr"))) AzS = 0;
                //else AzS = Convert.ToInt32(ab["AzStr"]);

                //result = AzO + AzS; //Convert.ToInt32( ab["AzOrd"] ) + Convert.ToInt32( ab["AzStr"] );
                //ab.Close();
            }
            ab.Close();
            qryStd1.Dispose();

            return result;
        }

        public override int CheckStatoVoto(string ANomeTotem)
        {
            //  mi da quante azioni ha un titolare
            SqlDataReader ab;
            SqlCommand qryStd1;
            int result;

            result = 1;
            // 0: chiuso
            // 1: aperto
            // -1 errore

            // testo la connessione
            if (STDBConn.State != ConnectionState.Open)
            {
                try
                {
                    STDBConn.Open();
                }
                catch (Exception objExc)
                {
                    Logging.WriteToLog("<dberror> Errore fn Open CheckStatoVoto err: " + objExc.Message);
                    //MessageBox.Show("Errore nella funzione CheckStatoVoto" + "\n\n" +
                    //    "Eccezione : \n" + objExc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    result = -1;
                    return -1;
                }
            }
            
            qryStd1 = new SqlCommand();
            qryStd1.Connection = STDBConn;
            try
            {
                // apro la query
                qryStd1.CommandText = "select * from CONFIG_POSTAZIONI_TOTEM with (nolock) where Postazione = '" + ANomeTotem + "'";
                ab = qryStd1.ExecuteReader();
                if (ab.HasRows)
                {
                    ab.Read();
                    // possono essere nulli
                    if (ab.IsDBNull(ab.GetOrdinal("VotoAperto")))
                        result = 0;
                    else
                    {
                        bool pippo = Convert.ToBoolean(ab["VotoAperto"]);
                        if (pippo) result = 1; else  result = 0;
                    }
                }
                ab.Close();
            }
            catch (Exception objExc)
            {
                result = -1;
                Utils.errorCall("CheckStatoVoto", objExc.Message);
            }
            finally
            {
                qryStd1.Dispose();
#if _DBClose
                if (STDBConn.State == ConnectionState.Open)
                    STDBConn.Close();
#endif
            }

            return result;
        }

        public override bool CancellaBadgeVotazioni(int AIDBadge)
        {
            // questa routine cancella i dati di un badge

            SqlCommand qryStd;
            SqlTransaction traStd;
            int NumberofRows;
            bool result;

            // questa procedura cancella i dati del badge dalle tre tabelle

            // testo la connessione
            if (!OpenConnection("CancellaBadgeVotazioni")) return false;

            result = false;
            qryStd = new SqlCommand();
            qryStd.Connection = STDBConn;
            // devo cancellarlo
            traStd = STDBConn.BeginTransaction();
            try
            {
                qryStd.Transaction = traStd;
                qryStd.CommandText = "delete from vs_votanti_totem with (ROWLOCK) where badge = " + AIDBadge.ToString();
                NumberofRows = qryStd.ExecuteNonQuery();

                qryStd.CommandText = "delete from vs_intonse_totem with (ROWLOCK) where badge = " + AIDBadge.ToString();
                NumberofRows = qryStd.ExecuteNonQuery();

                qryStd.CommandText = "delete from vs_ConSchede with (ROWLOCK) where badge = " + AIDBadge.ToString();
                NumberofRows = qryStd.ExecuteNonQuery();
                traStd.Commit();
                result = true;

                // TODO: GEAS VERSIONE
                /*
                qryStd.Parameters.Clear();
                qryStd.CommandText = @"delete Geas_voti with (ROWLOCK) 
                                       where badge = @Badge 
                                       and ProgMozione = (select isnull(GEAS_MatchVot.ProgMozione, 0) as ProgMozione from GEAS_MatchVot
		                               where GEAS_MatchVot.VotoSegretoDettaglio > 0) ";
                qryStd.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                NumberofRows = qryStd.ExecuteNonQuery();

                qryStd.Parameters.Clear();
                qryStd.CommandText = @"delete Geas_votiDiff with (ROWLOCK) 
                                       where badge = @Badge 
                                       and ProgMozione = (select isnull(GEAS_MatchVot.ProgMozione, 0) as ProgMozione from GEAS_MatchVot
		                               where GEAS_MatchVot.VotoSegretoDettaglio > 0) ";
                qryStd.Parameters.Add("@Badge", System.Data.SqlDbType.VarChar).Value = AIDBadge.ToString();
                NumberofRows = qryStd.ExecuteNonQuery();
                */
                //
                MessageBox.Show("I Voti sono stati cancellati", "Exclamation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception objExc)
            {
                traStd.Rollback();
                result = false;
                Utils.errorCall("CancellaBadgeVotazioni, badge:" +AIDBadge, objExc.Message);
            }
            finally
            {
                qryStd.Dispose();
                traStd.Dispose();
#if _DBClose
                STDBConn.Close();
#endif
            }
            return result;
        }

        public override Boolean CancellaTuttiVoti()
        {
            // questa routine cancella tutti i voti

            SqlCommand qryStd;
            SqlTransaction traStd;
            int NumberofRows;
            bool result;

            // testo la connessione
            if (STDBConn.State != ConnectionState.Open) STDBConn.Open();

            result = false;
            qryStd = new SqlCommand();
            qryStd.Connection = STDBConn;
            // devo cancellarlo
            traStd = STDBConn.BeginTransaction();
            try
            {
                qryStd.Transaction = traStd;
                qryStd.CommandText = "delete from vs_votanti_totem with (ROWLOCK)";
                NumberofRows = qryStd.ExecuteNonQuery();

                qryStd.CommandText = "delete from vs_intonse_totem with (ROWLOCK) ";
                NumberofRows = qryStd.ExecuteNonQuery();

                qryStd.CommandText = "delete from vs_ConSchede with (ROWLOCK) ";
                NumberofRows = qryStd.ExecuteNonQuery();
                traStd.Commit();
                result = true;
                //
                MessageBox.Show("TUTTI I Voti sono stati cancellati", "Exclamation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception objExc)
            {
                traStd.Rollback();
                result = false;
                Utils.errorCall("CancellaTuttiVoti", objExc.Message);
            }
            qryStd.Dispose();
            traStd.Dispose();
#if _DBClose
            STDBConn.Close();
#endif
            return result;
        }

        #endregion

		//  METODI DI CONFIGURAZIONE --------------------------------------------------------------------------
		
		// carica la configurazione 
        public override Boolean CaricaConfig()
		{
			string GeasFileName = "";
            // verifica se � locale oppure no
            if (ADataLocal)
            {
                if (File.Exists(AData_path + "geas.sql"))
                    GeasFileName = AData_path + "geas.sql";
                else
                    return false;
            }
            else
            {
                if (Directory.Exists(DriveM) && File.Exists(DriveM + "geas.sql"))
                    GeasFileName = DriveM + "geas.sql";
                else
                    return false;
            }

            // leggo cosa c'� dentro
            try
            {               
                StreamReader file1 = File.OpenText(GeasFileName);
                string ss = file1.ReadLine();
                // testo se il file � giusto
                if (ss.IndexOf("GEAS") >= 0)
                //if (ss == "GEAS 2000 -- Stringa Connesione a SQL")
                {
                    // tutto ok leggo
                    FDBConfig.DB_Type = file1.ReadLine();
                    FDBConfig.DB_Dsn = file1.ReadLine();
                    FDBConfig.DB_Name = file1.ReadLine();
                    FDBConfig.DB_Uid = file1.ReadLine();
                    FDBConfig.DB_Pwd = file1.ReadLine();
                    FDBConfig.DB_Server = file1.ReadLine();
                    FDBConfig.DB_ConfigOK = true;
                    file1.Close();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                Utils.errorCall("CaricaConfig", e.Message);
                return false;
            }		

        }

        // --------------------------------------------------------------------------
        //  METODI Di TEST
        // --------------------------------------------------------------------------

        public override bool DammiTuttiIBadgeValidi(ref ArrayList badgelist)
        {
            if (badgelist == null) return false;
            badgelist.Clear();
            if (!OpenConnection("DammiTuttiIBadgeValidi")) return false;
            SqlCommand qryStd = new SqlCommand { Connection = STDBConn };
            try
            {
                // Leggo ora da GEAS_Titolari	
                qryStd.CommandText = "select T.badge, T.idazion from geas_titolari T  where T.Reale=1 and T.Annullato = 0 order by Badge";
                SqlDataReader a = qryStd.ExecuteReader();
                if (a.HasRows)
                {
                    while (a.Read()) // qua posso avere pi� righe
                    {
                        string bdg = a.IsDBNull(a.GetOrdinal("Badge")) ? "" : (a["Badge"]).ToString();
                        badgelist.Add(bdg);
                    }
                }
                a.Close();
            }
            catch (Exception objExc)
            {
                Utils.errorCall("DammiTuttiIBadgeValidi", objExc.Message);
            }
            finally
            {
                qryStd.Dispose();
                CloseConnection("");
            }

            return true;
        }

	}
}
