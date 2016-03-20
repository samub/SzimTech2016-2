using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace teszt
{
    class MessageHandler
    {
        private TextBox TextBoxMessages;

        public MessageHandler(TextBox TextBoxMessages) {
            this.TextBoxMessages = TextBoxMessages;
        }
        public void Write(String Message) {
            if (TextBoxMessages.Text != "") TextBoxMessages.AppendText("\n");
            TextBoxMessages.AppendText(DateTime.Now + ": " + Message);
            TextBoxMessages.Focus();
            TextBoxMessages.CaretIndex = TextBoxMessages.Text.Length;
            TextBoxMessages.ScrollToEnd();
        }
    }
}
