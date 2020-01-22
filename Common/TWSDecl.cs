// --------------------------------------------------------------
//  FILE DELLE DICHIARAZIONI DI VOTO SEGRATO
// --------------------------------------------------------------

// --------------------------------------------------------------
//  CRONOLOGIA DELLE VERSIONI
//
//  V.3.0 10/04/2011                ASSEMBLEA BPER
//  V.3.1 10/10/2011                ASSEMBLEA BPM
//                                  Aggiunto tasto Non Votante, aggiunto tema esterno label
//                                  migliorata randomizzazione id,
//  V.3.2 15/03/2012                Multivotazioni, Rivisto TouchScreen

using System;

namespace VotoTouch.WPF
{
	
	public enum TTipoVoto:  int  {stvNormale, stvLista, stvMulti};

	public enum TAppStato: int {ssvBadge, ssvVotoStart, ssvVoto, ssvVotoConferma,ssvVotoContinua, ssvSalvaVoto, 
                                ssvVotoFinito, ssvPreIntermezzo, ssvConfermaNonVoto};
    public enum TStatoSemaforo: int {stsNulla, stsLibero, stsOccupato, stsErrore, stsFineoccupato, stsChiusoVoto};

    // configurazione del programma
    //    static public class TTotemConfig
    public static class VTConfig
    {
        // CONFIGURAZIONE DINAMICA
        public static bool IsDemoMode;
        public static string NomeTotem;
        public static bool IsDebugMode;
        public static bool IsPaintTouch;

        // CONFIGURAZIONE GENERALE
        public static int ModoAssemblea;
        public static string ValAssemblea;
        public static bool SalvaLinkVoto;
        public static bool SalvaVotoNonConfermato;
        public static bool SalvaVotoInGeas;
        public static int MaxDeleghe;
        public static bool AbilitaDifferenziatoSuRichiesta;
        public static int IDSchedaUscitaForzata;
        public static int ModoPosizioneAreeTouch;
        public static int ControllaPresenze;
        public static bool AbilitaBottoneUscita;
        public static bool AttivaAutoRitornoVoto;
        public static int TimeAutoRitornoVoto;
        public static bool AbilitaDirittiNonVoglioVotare;
        
        // CONFIGURAZIONE LOCALE
        public static string Postazione;
        public static string Descrizione;
        public static int IDSeggio;
        public static bool Attivo;
        public static bool VotoAperto;
        public static int Sala;
        // Semaforo
        public static bool UsaSemaforo;
        public static string IP_Com_Semaforo;
        public static int TipoSemaforo;
        // Variabili di configurazione Lettore
        public static bool UsaLettore;
        public static int PortaLettore;
        public static string CodiceUscita;
        // codici impianto
        public static int BadgeLen;
        public static string CodImpianto;

        public static bool IsOrdinaria; // = ValAssemblea.Contains("O"); //CheckOrdinariaValAssem();
        public static bool IsStraordinaria; // = ValAssemblea.Contains("S"); //CheckStraordinariaValAssem();

        // Diciture Votazioni
        public static string ContrarioATutti;
        public static string AstenutoATutti;



        static VTConfig()
        {
            SalvaLinkVoto = true;
            SalvaVotoNonConfermato = false;
            IDSchedaUscitaForzata = VSDecl.VOTO_NONVOTO;
            ModoPosizioneAreeTouch = VSDecl.MODO_POS_TOUCH_NORMALE;
            ControllaPresenze = VSDecl.PRES_CONTROLLA;
            AbilitaBottoneUscita = false;
            AttivaAutoRitornoVoto = false;
            AbilitaDifferenziatoSuRichiesta = false;
            TimeAutoRitornoVoto = VSDecl.TIME_AUTOCLOSEVOTO;
            AbilitaDirittiNonVoglioVotare = false;
            IsDemoMode = false;
            NomeTotem = "";
            ContrarioATutti = App.Instance.getLang("SAPP_SKCONTRARIOTUTTI");
            AstenutoATutti = App.Instance.getLang("SAPP_SKASTENUTOTUTTI");
        }
    }

    // struttura di configurazione del database
    public struct ConfigDbData
    {
        public bool DB_ConfigOK;
        public string DB_Type;
        public string DB_Dsn;
        public string DB_Name;
        public string DB_Uid;
        public string DB_Pwd;
        public string DB_Server;
    }

    // ------------------------------------------------------------------
    // STRUTTURE PER LE VOTAZIONI
    // ------------------------------------------------------------------
    
    public struct TAreaVotazione
    {
        // Area di Voto
        public int XVt;
        public int YVt;
        public int WVt;
        public int HVt;

        // N.candidati per pagina in caso di multi o candidato
        public int CandidatiPerPagina;
        // Uso o meno delle linguette (in caso di pochi candidati sono inutili)
        public bool NeedTabs;

        // AreaCandidatiCDA
        public int XCda;
        public int YCda;
        public int WCda;
        public int HCda;
        public int RCda() { return XCda + WCda; }
        public int BCda() { return YCda + HCda; }
        // AreaCandidatiAlt
        public int XAlt;
        public int YAlt;
        public int WAlt;
        public int HAlt;
        public int RAlt() { return XAlt + WAlt; }
        public int BAlt() { return YAlt + HAlt; }
    }

    public struct TIndiceListe
    {
        public int pag;
        public string sp;
        public string ep;
        public string indice;
        public int idx_start;
        public int idx_end;
    }

    public struct TVotoEspresso
    {
        public int NumVotaz;
        public int TipoCarica;
        public int VotoExp_IDScheda;
        //public string Str_ListaElenco;
        //public string StrUp_DescrLista;
    }

    public class VSDecl
    {
        // Classe che mantiene tutte le costanti
        public const string VTS_VERSION = "4.22  10/05/2018 vs17 **";

        // language constants
        internal const int LANGUAGE_IT = 0;
        internal const int LANGUAGE_EN = 1;
        internal const int LANGUAGE_DE = 2;

        public const string RIPETIZ_VOTO = "88889999";
        public const string ABILITA_DIFFERENZIATO = "88889900";
        public const string CONFIGURA = "88889990";
        public const string PANNELLO_STATO = "88889991";
        public const string PANNELLO_AZION = "88889992";

        public const string MSG_RIPETIZ_VOTO = "ATTENZIONE! Questa operazione annuller� la votazione corrente " +
                                               "\nI voti espressi fino ad ora NON saranno salvati." +
                                               "\n NON sar� salvata la consegna scheda, a tutti gli effetti " +
                                               "il badge non avr� votato." +
                                               "\nSar� quindi possibile in seguito ripetere la votazione. NON ci saranno voti doppi." +
                                               "\nConfermi l'annullamento? (Si = annulla la votazione, No = continua a votare)" +
                                               "\n ----> Badge corrente : ";
        public const string MSG_RIPETIZ_VOTO_C = "Sei proprio sicuro di Annullare la votazione ? \n Badge: ";
        public const string MSG_CANC_VOTO = "ATTENZIONE! Questa operazione canceller� i voti di questo badge sul database." +
                                               "\nI voti salvati fino ad ora saranno CANCELLATI." +
                                               "\n Sar� CANCELLATA la consegna scheda, quindi a tutti gli effetti " +
                                               "il badge non avr� votato." +
                                               "\nSar� quindi possibile in seguito ripetere la votazione. NON ci saranno voti doppi." +
                                               "\nConfermi la cancellazione? (Si = cancella la votazione, No = non fare nulla)" +
                                               "\n ----> Badge da cancellare : ";
        public const string MSG_CANC_VOTO_C = "Sei proprio sicuro di Cancellare i voti dal DB ? \n Badge: ";

        // path Immagini assoluti
        public const string DATA_PATH_ABS = "\\Data\\";
        public const string IMG_PATH_ABS = "\\Data\\VtsNETImg\\";
        // path immagine da server
        public const string SOURCE_IMG_PATH = "\\Data\\VtsNETImg\\";
        // path immagine locale
        public const string IMG_PATH_LOC = "\\VtsNETImgLocali\\";
        public const string IMG_PATH_LOC_ABS = "\\Data\\VtsNETImgLocali\\";
        public const string IMG_type = ".png";

        public const string IMG_Badge = "badge";
        public const string IMG_VotostartD = "votostart_D";
        public const string IMG_Votostart1 = "votostart_1";
        public const string IMG_fine = "fine";
        public const string IMG_intermezzo = "intermezzo";
        public const string IMG_Votochiuso = "votochiuso";
        public const string IMG_Salva = "salvataggio";

        public const string IMG_voto = "voto_";
        public const string IMG_voto_c = "_conf";
        public const string IMG_voto_pre = "_pre";

        // Modo Assemblea
        public const int MODO_AGM_POP = 0;            // popolari
        public const int MODO_AGM_SPA = 1;            // spa

        // tipi di Votazione
        public const int VOTO_LISTA = 1;            // voto di lista
        public const int VOTO_CANDIDATO = 2;        // voto per candidato a pagine
        public const int VOTO_CANDIDATO_SING = 3;   // voto per candidato singola pagina (da cancellare)
        public const int VOTO_MULTICANDIDATO = 4;   // voto multicandidato

        // tipo di sottovoto
        public const int SUBVOTO_NORMAL = 0;
        public const int SUBVOTO_NEW = 1;
        public const int SUBVOTO_CUSTOM_MANUTENCOOP = 40;

        // Voti
        public const int LISTA_1 = 0;
        public const int LISTA_2 = 1;
        public const int LISTA_3 = 2;
        public const int LISTA_4 = 3;
        public const int LISTA_5 = 4;
        public const int LISTA_6 = 5;
        public const int VOTO_SCHEDABIANCA = -1;
        public const int VOTO_NONVOTO = -2;
        public const int VOTO_MULTIAVANTI = -10;
        public const int VOTO_ASTENUTO_TUTTI = 226;
        public const int VOTO_CONTRARIO_TUTTI = 227;
        public const int VOTO_BTN_USCITA = -3;

        // n. di selezioni per pagina in caso di VOTO_CANDIDATO / alfabeto
        public const int CANDIDATI_PER_PAGINA = 10;
        public const int CANDXPAG_10 = 10;
        public const int CANDXPAG_8 = 8;
        public const int CANDXPAG_6 = 6;
        public static readonly string[] abt ={ 
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", 
                "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};

        // presenze
        public const int PRES_NON_CONTROLLARE = 0;
        public const int PRES_CONTROLLA = 1;
        public const int PRES_FORZA_INGRESSO = 2;
        public const int PRES_MODO_GEAS = 3;

        // TOUCHSCREEN **********************

        public const int PM_NORMAL = 0;
        public const int PM_NONE = 1;
        public const int PM_ONLYCHECK = 2;

        // Modo Touch aree 
        public const int MODO_POS_TOUCH_NORMALE = 0;
        public const int MODO_POS_TOUCH_MODERN = 1;
        public const int MODO_POS_TOUCH_BIG_BTN = 2;
        
        // paint mode TTZone
        public const float VOTESCREEN_DIVIDE_WIDTH = 1000;
        public const float VOTESCREEN_DIVIDE_HEIGHT = 1000;

        public const int SEMAFORO_IP = 1;
        public const int SEMAFORO_COM = 2;

        public const int MAX_ID_RANDOM = 9999999;
        public const bool SALVAVOTISULOG = true;
        public const int MINVOTI_PROGRESSIVO = 30;
        public const int BTN_FONT_SIZE = 14;
        public const string BTN_FONT_NAME = "Arial";
        public const int TIM_CKVOTO_MIN = 15000;   // 15 secondi
        public const int TIM_CKVOTO_MAX = 40000;   // 50 secondi
        public const int TIME_AUTOCLOSEVOTO = 20;


    }
}