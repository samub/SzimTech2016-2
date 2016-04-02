using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace teszt {
    public partial class Dialog {
        public Dialog() {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Button.IsEnabled = false;
        }

        public string ResponseText => TextBox.Text;

        public string ResponseText1 => TextBox1.Text;

        public string ResponseText2 => TextBox2.Text;

        private void button_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        private void IntegerValidationTextBox(object sender, TextCompositionEventArgs e) {
            var regex = new Regex(@"^\d$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (TextBox1.Visibility == Visibility.Hidden) Button.IsEnabled = TextBox.Text.Length > 0;
            else Button.IsEnabled = TextBox.Text.Length > 0 && TextBox1.Text.Length > 0;
        }
    }
}