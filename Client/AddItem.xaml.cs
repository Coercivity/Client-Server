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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Memory;
using System.Text.Json;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для AddItem.xaml
    /// </summary>
    public partial class AddItem : Window
    {
        public AddItem()
        {
            InitializeComponent();
        }




        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if(NameBox.Text == "" || !DatePicker.IsEnabled || TimeStartBox.Text == "" || TimeEndBox.Text == "" || AmountBox.Text == "")
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            if(Int32.Parse(AmountBox.Text) <= 0)
            {
                MessageBox.Show("Количество билетов должно быть больше нуля");
                return;
            }
            if (!Regex.IsMatch(TimeStartBox.Text, "[0-2][0-9]:[0-6][0-9]") || !Regex.IsMatch(TimeEndBox.Text, "[0-2][0-9]:[0-6][0-9]"))
            {
                MessageBox.Show("Введите корректный формат времени вида XX:XX");
                return;
            }

            



            IMemory memory = new IMemory();
            memory.Play = new PlayMemory(NameBox.Text, DatePicker.Text, TimeStartBox.Text, TimeEndBox.Text, Int32.Parse(AmountBox.Text), 0);
            memory.State = State.InputState;
            string message = JsonSerializer.Serialize(memory);

            Connection connection = new Connection(message, TheatreWindow.address, TheatreWindow.port);

            string serverResponse = connection.OutMessage.ToString();
            MessageBox.Show(serverResponse);
            Close();

        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
