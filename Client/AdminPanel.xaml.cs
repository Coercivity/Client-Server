using Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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

namespace Client
{
    public partial class AdminPanel : Window
    {
        public AdminPanel()
        {
            InitializeComponent();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddItem addItem = new AddItem();
            addItem.Show();
        }
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            IMemory memory = new IMemory();
            memory.State = State.GetAllPlaysState;
            string message = JsonSerializer.Serialize(memory);

            Connection connection = new Connection(message, TheatreWindow.address, TheatreWindow.port);

            string serverResponse = connection.OutMessage.ToString();
            try
            {
                var response = JsonSerializer.Deserialize<List<PlayMemory>>(serverResponse);

                    TheGrid.ItemsSource = response;
                
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
            
        }
    }
}
