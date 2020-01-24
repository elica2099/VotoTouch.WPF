using System;
using System.IO.Ports;
using System.Linq;

namespace VotoTouch.WPF
{
    public delegate void DataRead(object source, string data);
    public delegate void SerialError(object source, System.IO.Ports.SerialErrorReceivedEventArgs e);

    public class CNETActiveReader : Object
    {
        // classe della porta seriale
        private SerialPort serial;

        // evento
        public event DataRead ADataRead;
        public event SerialError ASerialError;

        public string PortName;
        private string buffer;
        private int pos;

        public CNETActiveReader()
        {
            serial = new SerialPort
            {
                PortName = "COM1",
                BaudRate = 9600,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };
            PortName = "COM1";
            buffer = "";

            serial.DataReceived += serial_DataReceived;

            pos = 0;
        }

        public bool Open()
        {
            bool ret;
            try
            {
                // prima testo se è presente la com, se no esco dicendo che non va
                // vedi pistole usb che si creano le com e non sono collegate
                bool found = SerialPort.GetPortNames().Any(sysportname => sysportname == PortName);

                if (found)
                {
                    // se è aperta la chiudoi
                    if (serial.IsOpen)
                        serial.Close();
                    // setto la porta
                    serial.PortName = PortName;
                    // apro la porta
                    serial.Open();
                    ret = true;
                }
                else
                    ret = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ret = false;
            }

            return ret;
        }

        public void Close()
        {
            serial.Close();
        }

        public void Flush()
        {
            serial.Close();
            buffer = "";
            pos = 0;
            serial.Open();
        }

        private void serial_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int i;

            string msg = serial.ReadExisting();

            buffer += msg;

            // ciclo lungo il buffer per vedere se riesco a riconoscere un codice valido
            // che finisce con un #13#10, poi shifto il buffer di quel pacchetto
            for (i = pos; i < (buffer.Length - 1); i++)
            {
                // devo cercare il carattere 
                if (buffer[i] == 13 && buffer[i + 1] == 10)
                {
                    // ok, ho trovato, devo copiare la stringa
                    string dato = buffer.Substring(pos, i);
                    // devo posizionare i a uno e togliere il tutto
                    if ((i + 2) == buffer.Length)
                        buffer = "";
                    else
                    {
                        string gg = buffer.Substring(i + 2, (buffer.Length - i - 2));
                        buffer = gg;
                    }

                    pos = 0;

                    //if (ADataRead != null) { ADataRead(this, dato); }
                    ADataRead?.Invoke(this, dato);
                }
            }
        }

        private void serial_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            // rispedisco al mittente
            //if (ASerialError != null) { ASerialError(this, e); }
            ASerialError?.Invoke(this, e);
        }


        // -------------------------------------------------------------------------------
        // AUTODISCOVER
        // -------------------------------------------------------------------------------

        public bool AutodiscoverBarcode(ref string APorta, ref string ADescription, ref int APortaInt)
        {
            bool found = false;
            // carico le seriali
            foreach (COMPortInfo comPort in COMPortInfo.GetCOMPortsInfo())
            {
                string ss = comPort.Description.ToLower();

                if (ss.IndexOf("datalogic", System.StringComparison.Ordinal) >= 0 ||
                    ss.IndexOf("barcode", System.StringComparison.Ordinal) >= 0 ||
                    ss.IndexOf("scanner", System.StringComparison.Ordinal) >= 0 ||
                    ss.IndexOf("hyperion", System.StringComparison.Ordinal) >= 0 ||
                    ss.IndexOf("honeywell bidirectional", System.StringComparison.Ordinal) >= 0
                )
                {
                    // la porta va bene, setto
                    APorta = comPort.Name;
                    ADescription = comPort.Description;
                    // ora devo trovare la stringa COM
                    string ss2 = comPort.Name;
                    const string removeString = "COM";
                    int index = ss2.LastIndexOf(removeString, System.StringComparison.Ordinal);
                    if (index >= 0)
                    {
                        string st = ss2.Remove(ss2.IndexOf(removeString, System.StringComparison.Ordinal),
                            removeString.Length);
                        //string st = ss.Substring(index +3, ss.Length - index +3); // ss[index + 3].ToString();
                        APortaInt = Convert.ToInt16(st);
                    }

                    found = true;
                    break;
                }
            }

            return found;
        }
    }
}