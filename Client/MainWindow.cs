using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using Memory;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        int port = 8005;
        string address = "127.0.0.1";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

            if (LoginBox.Text == "" || PasswordBox.Password == "")
            {
                System.Windows.MessageBox.Show("Пожалуйста, заполните все поля для авторизации");
                return;
            }
            IMemory memory = new IMemory();
            memory.Authorization = new AuthorizationMemory(Status.Authorization, "", LoginBox.Text, PasswordBox.Password, "");
            memory.State = State.AuthorizationState;
            string message = JsonSerializer.Serialize(memory);
            Connection connection = new Connection(message, address, port);
            string serverResponse = connection.OutMessage.ToString();

            var response = JsonSerializer.Deserialize<IMemory>(serverResponse);


            switch (response.Authorization.Status)
            {
                case Status.Authorized:
                    TheatreWindow theatre = new TheatreWindow(response);
                    theatre.Show();
                    this.Close();
                    break;
                case Status.AuthorizationFail:
                    MessageBox.Show("Введены неправильные данные");
                    break;
                case Status.Admin:
                    TheatreWindow theatreA = new TheatreWindow(response);
                    theatreA.Show();
                    this.Close();
                    break;
                default:
                    break;
            }
        }
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow regWindow = new RegistrationWindow();
            this.Close();
            regWindow.Show();

        }
        private void LoginBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginBox.Text = "";
        }
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = "";
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
