using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.IO;

namespace VotoTouch.WPF
{

    //public delegate void ehProgressoSalvaTutto(object source, int ATot, int AProg);
    
    public class CVotoBaseDati
    {
        //public event ehProgressoSalvaTutto ProgressoSalvaTutto;

        public ConfigDbData FDBConfig;
        public Boolean FConnesso;
        public int FIDSeggio;
        //public string LogNomeFile;
        //public string NomeTotem;

        public string AData_path;
        public Boolean ADataLocal;

        public CVotoBaseDati(ConfigDbData AFDBConfig, Boolean AADataLocal, string AAData_path)
        {
            FDBConfig = AFDBConfig;
            AData_path = AAData_path;
            //NomeTotem = ANomeTotem;
            ADataLocal = AADataLocal;

            // i file devono essere in locale nella cartella Data
            //AData_path = "c:" + VSDecl.DATA_PATH_ABS; // "c:\\data\\";
            //ADataLocal = false;
        }

        // --------------------------------------------------------------------------
        //  EVENTI
        // --------------------------------------------------------------------------

        //protected void OnProgressoSalvaTutto(object source, int ATot, int AProg)
        //{
        //    if (ProgressoSalvaTutto != null) { ProgressoSalvaTutto(this, ATot, AProg); }
        //}

        // --------------------------------------------------------------------------
        //  LETTURA CONFIGURAZIONE NEL DATABASE
        // --------------------------------------------------------------------------

        public virtual int CaricaConfigDB(ref int ABadgeLen, ref string ACodImpianto)
        {
            return 0;
        }

        public virtual int DammiConfigTotem() //, ref TTotemConfig TotCfg)
        {
            return 0;
        }

        public virtual int DammiConfigDatabase() //ref TTotemConfig TotCfg)
        {
            return 0;
        }
        
        public virtual int SalvaConfigurazione() //, ref TTotemConfig ATotCfg)
        {
            return 0;
        }

        public virtual int SalvaConfigurazionePistolaBarcode() //, ref TTotemConfig ATotCfg)
        {
            return 0;
        }

        // --------------------------------------------------------------------------
        //  CARICAMENTO DATI VOTAZIONI
        // --------------------------------------------------------------------------

        public virtual bool CaricaVotazioniDaDatabase(ref List<TNewVotazione> AVotazioni)
        {
            return true;
        }

        public virtual bool CaricaListeDaDatabase(ref List<TNewVotazione> AVotazioni)

        {
            return true;
        }

        // --------------------------------------------------------------------------
        //  METODI SUI BADGE
        // --------------------------------------------------------------------------

//        virtual public bool ControllaBadge(int AIDBadge, TTotemConfig TotCfg, ref int AReturnFlags)
        public virtual bool ControllaBadge(int AIDBadge, ref int AReturnFlags)
        {
            return true;
        }

        public virtual bool BadgeAnnullato(int AIDBadge)
        {
            return false;
        }

        public virtual bool BadgePresente(int AIDBadge, bool ForzaTimbr)
        {
            return false;
        }

        public virtual bool BadgeHaGiaVotato(int AIDBadge)
        {
            return false;
        }

        public virtual bool HaVotato(int ANVotaz, int AIDBadge, int ProgDelega)
        {
            return false;
        }

		// --------------------------------------------------------------------------
        //  LETTURA DATI AZIONISTA 
		// --------------------------------------------------------------------------

        public virtual bool CaricaDirittidiVotoDaDatabase(int AIDBadge, ref List<TAzionista> AAzionisti,
                                                          ref TAzionista ATitolare_badge, ref TListaVotazioni AVotazioni)
        {
            return true;
        }

        // --------------------------------------------------------------------------
        //  CONTROLLO DELLA VOTAZIONE
        // --------------------------------------------------------------------------

//        virtual public int SalvaTutto(int AIDBadge, TTotemConfig ATotCfg, ref TListaAzionisti FAzionisti)
        public virtual int SalvaTutto(int AIDBadge, ref TListaAzionisti FAzionisti)
        {
            return 0;
        }

        public virtual int SalvaTuttoInGeas(int AIDBadge, ref TListaAzionisti FAzionisti)
        {
            return 0;
        }

        public virtual int NumAzTitolare(int AIDBadge)
        {
            return 0;
        }

        public virtual int CheckStatoVoto(string ANomeTotem)
        {
            return 1;
        }

        public virtual bool CancellaBadgeVotazioni(int AIDBadge)
        {
            return true;
        }

        public virtual Boolean CancellaTuttiVoti()
        {
            return true;
        }

        // --------------------------------------------------------------------------
        //  METODI DATABASE
        // --------------------------------------------------------------------------

        public virtual object DBConnect()
        {
            // ritorna l'oggetto connessione
            return null;
        }

        public virtual object DBDisconnect()
        {
            // ritorna l'oggetto connessione
            return null;
        }

        // --------------------------------------------------------------------------
        //  REGISTRAZIONE NEL DATABASE
        // --------------------------------------------------------------------------

        public virtual int RegistraTotem(string ANomeTotem)
        {
            return 0;
        }

        public virtual int UnregistraTotem(string ANomeTotem)
        {
            return 0;
        }

        // --------------------------------------------------------------
        //  METODI PRIVATI
        // --------------------------------------------------------------

        public virtual string DammiStringaConnessione()
        {
            return "";
        }

        // --------------------------------------------------------------
        //  METODI DI CONFIGURAZIONE
        // --------------------------------------------------------------

        // carica la configurazione 
        public virtual Boolean CaricaConfig()
        {
                return true;
        }

        // --------------------------------------------------------------------------
        //  FUNZIONE DI RETRIEVE DI UNA STRINGA SQL DALLE RISORSE
        // --------------------------------------------------------------------------

        public string getModelsQueryProcedure(string ANameSqlFile)
        {
            string ret;
            // load from resources the query strings
            Stream stream;
            StreamReader reader;
            // -> detailsByIDShareholder
            stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("VotoTouch.WPF.Data.Query." + ANameSqlFile);
            if (stream == null)
                ret = "";
            else
            {
                reader = new StreamReader(stream);
                ret = reader.ReadToEnd();
                reader = null;
            }
            stream = null;
            // replacing the newline with spaces for query syntax
            ret = ret.Replace("\r", " ");
            ret = ret.Replace("\n", " ");
            ret = ret.Replace("\t", " ");

            return ret;
        }

        // --------------------------------------------------------------------------
        //  METODI Di TEST
        // --------------------------------------------------------------------------

        public virtual bool DammiTuttiIBadgeValidi(ref ArrayList badgelist)
        {

            return true;
        }

     }
}
