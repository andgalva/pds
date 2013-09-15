using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class CaptureWorker
    {
        private int captureType;
        private volatile bool mustStop;
        private int width_s, height_s;
        private Point point_s;
        Pen pen;
        protected static IntPtr m_HBitmap;
        Cursor arrow = Cursors.Arrow;
        Point p;
        public const Int32 CURSOR_SHOWING = 0x00000001;

        /* Data la pesantezza di spedire una bitmap in streaming,
         * prima di tutto impongo un intervallo di cattura allo scadere del quale viene spedita l'immagine.
         * Ad ogni invio confronto la cattura attuale con l'ultima inviata e, raggiunto un numero di
         * uguaglianze, aumento (*10) l'intervallo di cattura per alleggerire il carico; alla prima
         * immagine diversa dall'ultima l'intervallo ritorna al suo valore iniziale.
        */
        private int captureSleepInterval;
        private byte[] rgbPrevious;
        private int equalCaptures;

        public CaptureWorker(int type)
        {
            captureType = type;

            pen = new Pen(Brushes.Red);
            pen.Width = 2.0F;
            forceStart();
            captureSleepInterval = 30;
            equalCaptures = 0;
        }

        #region GETTERS & SETTERS

        public void forceStart()
        {
            mustStop = false;
        }
        public void forceStop()
        {
            mustStop = true;
        }

        public void setScreenParameters(int p_x, int p_y, int w, int h)
        {
            point_s = new Point(p_x, p_y);
            width_s = w;
            height_s = h;
        }
        public void setCaptureType(int type)
        {
            captureType = type;
        }
        private bool cursorInArea(ref Point cursor, int x_new, int y_new, int w_new, int h_new)
        {
            if ((cursor.X >= x_new) && (cursor.Y >= y_new) && (cursor.X <= (x_new + w_new)) && (cursor.Y <= (y_new + h_new)))
            {
                cursor.X = cursor.X - x_new;
                cursor.Y = cursor.Y - y_new;
                return true;
            }
            return false;
        }
        public static RECT getForegroundWindow()
        {
            IntPtr hwnd = WIN32_API.GetForegroundWindow();
            RECT rct;
            WIN32_API.GetWindowRect(hwnd, out rct);

            return rct;

        }

        #endregion

        // entry-point del thread!
        public void DoWork()
        {
            /* CAPTURE TYPE: 1 = PARTE DELLO SCHERMO, 2 = FINESTRA ATTIVA, 3 = SCHERMO INTERO */

            while (!mustStop)
            {
                p = Cursor.Position;
                Graphics gfxScreenshot = null;
                Bitmap screenShot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                 Screen.PrimaryScreen.Bounds.Height,
                                 PixelFormat.Format32bppArgb);

                #region 1) PARTE DELLO SCHERMO
                if (captureType == 2)
                {
                    Size sz = new Size(width_s, height_s);

                    gfxScreenshot = Graphics.FromImage(screenShot);

                    gfxScreenshot.CopyFromScreen(point_s.X,
                                                point_s.Y,
                                                0,
                                                0,
                                                sz,
                                                CopyPixelOperation.SourceCopy);

                    if (cursorInArea(ref p, point_s.X, point_s.Y, sz.Width, sz.Height))
                        paintCursor(ref gfxScreenshot, p.X, p.Y);
                }
                #endregion

                #region 2) FINESTRA ATTIVA
                if (captureType == 1)
                {
                    RECT rct = getForegroundWindow();

                    Size sz = new Size(Math.Abs(rct.Left - rct.Right), Math.Abs(rct.Bottom - rct.Top));
                    if (rct.Bottom > Screen.PrimaryScreen.WorkingArea.Bottom)
                        sz.Height = Math.Abs(Screen.PrimaryScreen.WorkingArea.Bottom - rct.Top);

                    gfxScreenshot = Graphics.FromImage(screenShot);
                    // ritaglio il rettangolo della finestra attiva...
                    gfxScreenshot.CopyFromScreen(rct.Left,
                                                rct.Top,
                                                0,
                                                0,
                                                sz,
                                                CopyPixelOperation.SourceCopy);

                    gfxScreenshot = Graphics.FromImage(screenShot); //?????
                    if (cursorInArea(ref p, rct.Left, rct.Top, sz.Width, sz.Height))
                        paintCursor(ref gfxScreenshot, p.X, p.Y);
                }
                #endregion

                #region 3) SCHERMO INTERO
                if (captureType == 3)
                {
                    gfxScreenshot = Graphics.FromImage(screenShot);

                    try
                    {
                        gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                                    Screen.PrimaryScreen.Bounds.Y,
                                                    0,
                                                    0,
                                                    Screen.PrimaryScreen.Bounds.Size,
                                                    CopyPixelOperation.SourceCopy);
                    }
                    catch
                    {
                        continue;
                    }

                    paintCursor(ref gfxScreenshot, p.X, p.Y);
                }
                #endregion

                gfxScreenshot.Dispose();

                compareBitmaps(screenShot);

                MainServer.sendFrameCapture(screenShot);
                Thread.Sleep(captureSleepInterval);
            }

        }

        // devo disegnare anche il cursore quando mando la cattura!
        private void paintCursor(ref Graphics graphics, int cursorX, int cursorY)
        {
            Rectangle rCursor = new Rectangle(cursorX, cursorY, arrow.Size.Width, arrow.Size.Height);
            arrow.Draw(graphics, rCursor);
        }

        private void compareBitmaps(Bitmap bitmap)
        {
            try
            {
                /* { */

                // Create a new bitmap.
                Bitmap bmp = bitmap;
                // Lock the bitmap's bits.  
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);
                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;
                // Declare an array to hold the bytes of the bitmap.
                int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                byte[] rgbValues = new byte[bytes];
                // Copy the RGB values into the array.
                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
                // Unlock the bits.
                bmp.UnlockBits(bmpData);
                
                /* } */

                // confronto...
                if (rgbPrevious != null)
                {
                    if (compareArrays<byte>(rgbValues, rgbPrevious))
                    {
                        equalCaptures++;
                        if (equalCaptures > 30)
                        {
                            captureSleepInterval *= 2;
                            if (captureSleepInterval > 2000)
                                captureSleepInterval = 2000;

                            equalCaptures = 0;
                        }
                    }
                    else
                    {
                        captureSleepInterval = 30;
                        equalCaptures = 0;
                    }
                }

                rgbPrevious = rgbValues;
            }
            catch
            {
                captureSleepInterval = 30;
            }
         }
        static bool compareArrays<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;
            if (a1 == null || a2 == null)
                return false;
            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
                if (!comparer.Equals(a1[i], a2[i]))
                    return false;

            return true;
        }

    }
}

