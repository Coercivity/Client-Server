using Memory;
using System;
using System.Collections.Generic;

using System.Text.Json;

using System.Windows;

using System.Windows.Input;


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

            Connection connection = new Connection(message, IMemory.IP, IMemory.port);

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
