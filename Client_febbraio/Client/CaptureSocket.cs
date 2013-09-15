using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

public class CaptureSocket
{
    TcpClient socket;
    private double cont;

    // oggetto socket utilizzato per la cattura
    public CaptureSocket(string host, int port)
    {
        socket = new TcpClient(host, port);
        cont = 0;
    }

    // trasforma da bytes a bitmap
    public int receiveCapture(ref Bitmap bmp)
    {
        // -1: non connesso, 0: immagine non disponibile, 1: immagine disponibile

        if (!socket.Connected)
            return -1;

        if (socket.GetStream().DataAvailable)
        {
            cont = 0;
            Stream socketStream = socket.GetStream();
            IFormatter formatter = new BinaryFormatter();
            bmp = (Bitmap)formatter.Deserialize(socketStream);
            return 1;
        }
        else
        {
            // se non e' disponibile, attendo un po' prima di segnalarlo (con un contatore)
            if (cont >= 50)
            {
                //bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Height, Screen.PrimaryScreen.Bounds.Width);
                return 0;
            }
            cont++;
            return 1;
        }
    }

    public void closeCaptureConnection()
    {
        socket.Close();
    }


}