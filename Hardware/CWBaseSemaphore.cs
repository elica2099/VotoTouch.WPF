using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace VotoTouch.WPF
{

    public delegate void ehChangeSemaphore(object source, TStatoSemaforo ASemStato);

    public class CBaseSemaphore
    {
        // costanti
        public const char COLOR_RED = 'R';
        public const char COLOR_GREEN = 'V';

        public const char COLOR_FLASH = '1';
        public const char COLOR_FIXED = '0';

        public const char EFFECT_TIME = '1';
        public const char EFFECT_NO = '1';

        public const char CHAR_ROUND = '!';
        public const char CHAR_ROUND_EMPTY = (char)34;
        public const char CHAR_X = '#';

        public const char CHAR_LEFT = '$';
        public const char CHAR_RIGHT = '%';
        public const char CHAR_UP = '&';
        public const char CHAR_DOWN = (char)0x27;
        
        //
        public event ehChangeSemaphore ChangeSemaphore;

        public TStatoSemaforo SemStato;
        public bool SemaforoAttivo;
        public string ConnAddress;
        //public string LogNomeFile;
      
        public CBaseSemaphore()
        {
            // nulla
        }

        // semaforo ---------------------------------------------------

        public virtual void timSemaforo_tick(object sender, EventArgs e)
        {
            //
        }

        // connessione ---------------------------------------------------

        public virtual bool AttivaSemaforo(bool AAttiva)
        {
            return true;
        }

        public virtual void SemaforoOccupato()
        {
            // chiamo l'evento
            if (ChangeSemaphore != null) { ChangeSemaphore(this, SemStato); }
        }

        public virtual void SemaforoLibero()
        {
            // chiamo l'evento
            if (ChangeSemaphore != null) { ChangeSemaphore(this, SemStato); }
        }

        public virtual void SemaforoErrore()
        {
            // chiamo l'evento
            if (ChangeSemaphore != null) { ChangeSemaphore(this, SemStato); }
        }

        public virtual void SemaforoFineOccupato()
        {
            // chiamo l'evento
            if (ChangeSemaphore != null) { ChangeSemaphore(this, SemStato); }
        }

        public virtual void SemaforoChiusoVoto()
        {
            // chiamo l'evento
            if (ChangeSemaphore != null) { ChangeSemaphore(this, SemStato); }
        }

    
    }



}
