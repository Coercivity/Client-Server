
using System.Windows;
using System.Windows.Input;
using Memory;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Window.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        int port = 8005;
        string address = "127.0.0.1";
        public RegistrationWindow()
        {
            InitializeComponent();
        }
     

        private void RequestRegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            if (REmailBox.Text == "" || RLoginBox.Text == "" || RPasswordBox.Password == "" || RNameBox.Text == "")
            {
                System.Windows.MessageBox.Show("Пожалуйста, заполните все поля для регистрации");
                return;
            }
            if (!Regex.IsMatch(REmailBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) 
            {
                MessageBox.Show("Введите корректный Email");
                return;
            }


            IMemory memory = new IMemory();
            memory.Authorization = new AuthorizationMemory(Status.Registration, REmailBox.Text, RLoginBox.Text, RPasswordBox.Password, RNameBox.Text);
            memory.State = State.AuthorizationState;
            string message = JsonSerializer.Serialize(memory);

            Connection connection = new Connection(message, address, port);

            string serverResponse = connection.OutMessage.ToString();
            var response = JsonSerializer.Deserialize<IMemory>(serverResponse);


            if (response.Authorization.Status == Status.Registered)
            {
                MainWindow w = new MainWindow();
                MessageBox.Show("Вы успешно зарегистрировались");
                w.Show();
                Close();
            }
            else if(response.Authorization.Status == Status.RegistraionFail)
            {
                MessageBox.Show("Такой логин уже занят");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            Close();
            mainWindow.Show();
        }

        private void RLoginBox_OnFocus(object sender, RoutedEventArgs e)
        {
            RLoginBox.Text = "";
        }
        private void REmailBox_OnFocus(object sender, RoutedEventArgs e)
        {
            REmailBox.Text = "";
        }
        private void RPasswordBox_OnFocus(object sender, RoutedEventArgs e)
        {
            RPasswordBox.Password = "";
        }
        private void RNameBox_OnFocus(object sender, RoutedEventArgs e)
        {
            RNameBox.Text = "";
        }
        private void RWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
