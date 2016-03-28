using System.Windows;

namespace teszt {
    public partial class Dialog {
        public Dialog() {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public string ResponseText => TextBox.Text;

        public string ResponseText1 => TextBox1.Text;

        private void button_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
    }
}