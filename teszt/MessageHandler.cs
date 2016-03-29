using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RobotMoverGUI
{
    /*
        Használata:
        Write() osztálymetódussal kiíratunk egy üzenetet a boxba. Az üzeneteket egymás után fűzi.
        ToLog() osztálymetódussal fileba íratjuk a tartalmát, majd ürítjük, majd kijelezzük hogy hova írta ki. Ez a GUI-ból is használható.
        Start előtt bepipálható, hogy logoljon e, ilyenkor Stop gombot kell használni, hogy ez tényleg meg is történjen.
        Start gomb kiüríti a tartalmát.
        Stop gomb fileba írja(ToLog()) a tartalmát, és kiírja, hogy hova mentette   
    */
    class MessageHandler
    {
        public static TextBox TextBoxMessages;

        public static void Write(String Message) {
            if (TextBoxMessages.Text != "") TextBoxMessages.AppendText("\r\n");
            TextBoxMessages.AppendText(DateTime.Now + ": " + Message);
            TextBoxMessages.Focus();
            TextBoxMessages.CaretIndex = TextBoxMessages.Text.Length;
            TextBoxMessages.ScrollToEnd();
        }
        public static void ToLog(string fileName)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(fileName+".txt");
            file.Write(TextBoxMessages.Text);
            file.Close();
            TextBoxMessages.Text = "Üzenetek logolva ide: " + fileName + ".txt";
        }
    }
}
