using Memory;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.Json;

namespace Client
{
    public partial class TheatreWindow : Window
    {
        int Iter = 0;

        int TicketIter = 0;

        int PageIterator = 0;

        AdminPanel adminPanel;

        public static int port = 8005;
        public IMemory DerivedMemory { get; set; }

        public static string address = "127.0.0.1";
        private List<PlayMemory> Plays { get; set; }
        private List<TicketMemory> Tickets { get; set; }
        public TheatreWindow(IMemory memory)
        {
            InitializeComponent();
            if (memory.Authorization.Status == Status.Admin)
            {       
                AdminPanel.Visibility = Visibility.Visible;
            }
            else
            {
                UserName.Content = "Добро пожаловать " + memory.Account.Nickname + " !";
            }
            DerivedMemory = memory;


            HideCanvas();
            NextViewButton.Visibility = Visibility.Hidden;
            NextButton.Visibility = Visibility.Hidden;
        }


        private void HideCanvas()
        {
            foreach (Canvas canvas in Canv.Children)
                canvas.Visibility = Visibility.Hidden;
        }

        private void AdminPanel_Click(object sender, RoutedEventArgs e)
        {
            adminPanel = new AdminPanel();
            adminPanel.TheGrid.ItemsSource = DerivedMemory.PlaysList;
            adminPanel.Show();
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            HideCanvas();

            if (SearchBox.Text == "")
                return;
            
            Iter = 0;
            TicketIter = 0;
            IMemory memory = new IMemory();
            memory.Query = SearchBox.Text;
            memory.State = State.PlayState;
            string message = JsonSerializer.Serialize<IMemory>(memory);
            Connection connection = new Connection(message, address, port);

            string serverResponse = connection.OutMessage.ToString();
            Plays = JsonSerializer.Deserialize<List<PlayMemory>>(serverResponse);
            if (Plays.Count == 0)
            {
                MessageBox.Show("По данному запросу ничего не найдено");
                return;
            }

            if(Plays.Count > Canv.Children.Count)
                NextButton.Visibility = Visibility.Hidden;
            PageIterator = 0;
            Iter = 0;
            if (Plays.Count != 0)
                DisplayPlays(ref Iter, Plays);
        }


        private void DisplayPlays(ref int Iter, List<PlayMemory> plays)
        {
            HideCanvas();

            NextViewButton.Visibility = Visibility.Hidden;
            foreach (Canvas canvas in Canv.Children)
            {

                if (plays.Count > Iter)
                {
                    canvas.Visibility = Visibility.Visible;
                    Button button = (Button)canvas.Children[0];
                    Button button_2 = (Button)canvas.Children[1];
                    button.Visibility = Visibility.Visible;
                    button_2.Visibility = Visibility.Hidden;
                    Label lablel_1 = (Label)canvas.Children[2];
                    Label lablel_2 = (Label)canvas.Children[3];
                    Label lablel_3 = (Label)canvas.Children[4];
                    Label lablel_4 = (Label)canvas.Children[5];
                    Label lablel_5 = (Label)canvas.Children[6];


                    lablel_1.Content = plays[Iter].PlayName;
                    lablel_2.Content = "С " + plays[Iter].StartDate;
                    lablel_3.Content = "Начало: " + plays[Iter].StartTime;
                    lablel_4.Content = "Конец: " + plays[Iter].EndTime;
                    lablel_5.Content = "Билеты: " + plays[Iter].TicketAmount;
                    button.Click -= ActionButton;
                    button.Click += ActionButton;
                    Iter++;
  
                }

            }
            NextButton.Visibility = (Iter >= Canv.Children.Count) ? Visibility.Visible : Visibility.Hidden;
        }

        public void ActionButton(object sender, RoutedEventArgs e)
        {
            if (DerivedMemory.Authorization.Status == Status.Admin)
            {
                MessageBox.Show("Кассир не может бронировать билеты");
                return;
            }

            var button = (Button)sender;
            if (MessageBox.Show("Вы действительно хотите забронировать место",
                    "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            IMemory memory = new IMemory();
 
            switch (button.Name)    
            {
                case "FirstButton":
                    memory.Play = Plays[0 + PageIterator];
                    break;
                case "SecondButton":
                    memory.Play = Plays[1 + PageIterator];
                    break;
                case "ThirdButton":
                    memory.Play = Plays[2 + PageIterator];
                    break;
                case "ForthButton":
                    memory.Play = Plays[3 + PageIterator];
                    break;
                case "FifthButton":
                    memory.Play = Plays[4 + PageIterator];
                    break;
                case "SixthButton":
                    memory.Play = Plays[5 + PageIterator];
                    break;
                case "SeventhButton":
                    memory.Play = Plays[6 + PageIterator];
                    break;
                case "EigthButton":
                    memory.Play = Plays[7 + PageIterator];
                    break;
                default:
                    break;
            }
            memory.State = State.BuyTicket;
            memory.Account = DerivedMemory.Account;
            //not implemented price/place
            int price = 500;
            int place = 5;

            memory.Ticket = new TicketMemory(price, place, memory.Play.PlayName, memory.Account.Nickname, memory.Play.StartDate, memory.Play.StartTime, 0);

            string message = JsonSerializer.Serialize(memory);
            Connection connection = new Connection(message, address, port);
            string serverResponse = connection.OutMessage.ToString();
            var response = JsonSerializer.Deserialize<IMemory>(serverResponse);

            if(response.State == State.SuccessfulTicketBuy)
            {
                SearchButton_Click(sender, e);
                PageIterator = 0;
                MessageBox.Show("Успешное бронирование билета");
            }
            else
            {
                MessageBox.Show("Билетов больше нет!");
            }

        }


        private void DisplayTicketsButton_Click(object sender, RoutedEventArgs e)
        {
            NextViewButton.Visibility = Visibility.Hidden;
            NextButton.Visibility = Visibility.Hidden;
            if (DerivedMemory.Authorization.Status == Status.Admin)
            {
                MessageBox.Show("Кассир не может смотреть свои билеты");
                return;
            }
            IMemory memory = new IMemory();
            memory.Account = DerivedMemory.Account;
            memory.State = State.GetTickets;
            string message = JsonSerializer.Serialize<IMemory>(memory);

            Connection connection = new Connection(message, address, port);

            string serverResponse = connection.OutMessage.ToString();
            var response = JsonSerializer.Deserialize<IMemory>(serverResponse);

            if(response.State == State.GetTicketsFail)
            {
                HideCanvas();
                MessageBox.Show("У вас нет ни одного купленного билета!");
                return;
            }
            PageIterator = 0;
            TicketIter = 0;
            Tickets = response.TicketList;
            DisplayTickets(ref TicketIter, Tickets);


        }

        private void DisplayTickets(ref int TicketIter, List<TicketMemory> tickets)
        {
            NextButton.Visibility = Visibility.Hidden;
            HideCanvas();
            
            foreach (Canvas canvas in Canv.Children)
            {
                if (tickets.Count > TicketIter)
                {
                    canvas.Visibility = Visibility.Visible;

                    Button button = (Button)canvas.Children[0];
                    Button button_2 = (Button)canvas.Children[1];
                    button.Visibility = Visibility.Hidden;
                    button_2.Visibility = Visibility.Visible;

                    Label lablel_1 = (Label)canvas.Children[2];
                    Label lablel_2 = (Label)canvas.Children[3];
                    Label lablel_3 = (Label)canvas.Children[4];
                    Label lablel_4 = (Label)canvas.Children[5];
                    Label lablel_5 = (Label)canvas.Children[6];


                    lablel_1.Content = tickets[TicketIter].Play;
                    lablel_2.Content = "Дата: " + tickets[TicketIter].DateStart;
                    lablel_3.Content = "Начало: " + tickets[TicketIter].TimeStart;
                    lablel_4.Content = "Стоимость: " + tickets[TicketIter].Price;
                    lablel_5.Content = "Место: " + tickets[TicketIter].Place;

                    TicketIter++;
                    button_2.Click -= ReturnTicket;
                    button_2.Click += ReturnTicket;
                }

            }
            NextViewButton.Visibility = (Iter >= Canv.Children.Count) ? Visibility.Visible : Visibility.Hidden;
        }

        private void ReturnTicket(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (MessageBox.Show("Вы действительно хотите вернуть билет",
                    "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            IMemory memory = new IMemory();

            switch (button.Name)
            {
                case "CFirstButton":
                    memory.Query = Tickets[0 + PageIterator].Id.ToString();
                    break;
                case "CSecondButton":
                    memory.Query = Tickets[1 + PageIterator].Id.ToString();
                    break;
                case "CThirdButton":
                    memory.Query = Tickets[2 + PageIterator].Id.ToString();
                    break;
                case "CForthButton":
                    memory.Query = Tickets[3 + PageIterator].Id.ToString();
                    break;
                case "CFifthButton":
                    memory.Query = Tickets[4 + PageIterator].Id.ToString();
                    break;
                case "CSixthButton":
                    memory.Query = Tickets[5 + PageIterator].Id.ToString();
                    break;
                case "CSeventhButton":
                    memory.Query = Tickets[6 + PageIterator].Id.ToString();
                    break;
                case "CEigthButton":
                    memory.Query = Tickets[7 + PageIterator].Id.ToString();
                    break;
                default:
                    break;
            }
            memory.State = State.ReturnTicket;
            string message = JsonSerializer.Serialize(memory);
            Connection connection = new Connection(message, address, port);
            string serverResponse = connection.OutMessage.ToString();
            var response = JsonSerializer.Deserialize<IMemory>(serverResponse);



            if (response.State == State.SuccessfulTicketReturn)
            {
                MessageBox.Show("Успешный возврат билета");
                DisplayTicketsButton_Click(sender, e);
                PageIterator = 0;
            }
            else
                MessageBox.Show("Возвратить билет не удалось");
            
        }

        private void NextViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (Tickets != null && TicketIter < Tickets.Count)
            {
                PageIterator += 8;
                DisplayTickets(ref TicketIter, Tickets);
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (Plays != null && Iter < Plays.Count)
            {
                PageIterator += 8;
                DisplayPlays(ref Iter, Plays);
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            SearchBox.Opacity = 0.70;
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

