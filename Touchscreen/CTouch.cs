using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VotoTouch.WPF.Touchscreen
{
    public class CTouch
    {
        public Rect FFormRect;

        public CTouch(Rect AFormRect)
        {
            // inizializzo
            FFormRect = new Rect();
            FFormRect = AFormRect;
        }

        // --------------------------------------------------------------
        //  UTILITA DI RICALCOLO SCHERMO
        // --------------------------------------------------------------

        #region UTILITA DI RICALCOLO SCHERMO

        protected int GetX(int n)
        {
            return (int)(FFormRect.Width / VSDecl.VSCREEN_DIV_WIDTH) * n;
        }

        protected int GetY(int n)
        {
            return (int)(FFormRect.Height / VSDecl.VSCREEN_DIV_HEIGHT) * n;
        }

        protected void GetZone(ref TTZone a, int qx, int qy, int qr, int qb)
        {
            // prendo le unità di misura
            double x = (FFormRect.Width / VSDecl.VSCREEN_DIV_WIDTH) * qx;
            double y = (FFormRect.Height / VSDecl.VSCREEN_DIV_HEIGHT) * qy;
            double r = (FFormRect.Width / VSDecl.VSCREEN_DIV_WIDTH) * qr;
            double b = (FFormRect.Height / VSDecl.VSCREEN_DIV_HEIGHT) * qb;
            a.x = (int)x;
            a.y = (int)y;
            a.r = (int)r;
            a.b = (int)b;
        }

        protected void GetZoneFloat(ref TTZone a, float qx, float qy, float qr, float qb)
        {
            // prendo le unità di misura
            double x = (FFormRect.Width / VSDecl.VSCREEN_DIV_WIDTH) * qx;
            double y = (FFormRect.Height / VSDecl.VSCREEN_DIV_HEIGHT) * qy;
            double r = (FFormRect.Width / VSDecl.VSCREEN_DIV_WIDTH) * qr;
            double b = (FFormRect.Height / VSDecl.VSCREEN_DIV_HEIGHT) * qb;
            a.x = (int)x;
            a.y = (int)y;
            a.r = (int)r;
            a.b = (int)b;
        }

        #endregion

    }
}
