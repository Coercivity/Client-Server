using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using System.Linq;
using Memory;
using System.Collections.Generic;

namespace Server
{
    public class Server
    {

        static void Main(string[] args)
        {
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IMemory.IP), IMemory.port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных
                    string messageFromClient = "";
                    string messageToClient = "";

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    messageFromClient = builder.ToString();
                    string logJson = messageFromClient;
                    IMemory clientResponse = JsonSerializer.Deserialize<IMemory>(messageFromClient);
                    messageFromClient = clientResponse.Query;

                    switch (clientResponse.State)
                    {
                        case State.ReturnTicket:
                            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + clientResponse.State);
                            Console.WriteLine(logJson);
                            using (PlayDBContext db = new PlayDBContext())
                            {
                                Ticket ticket = db.Tickets.Where(p => p.Id.Equals(Int32.Parse(clientResponse.Query))).FirstOrDefault();
                                Play play = db.Plays.Where(p => p.Id.Equals(ticket.Play)).FirstOrDefault();

                                if (ticket != null)
                                {
                                    play.TicketAmount++;
                                    db.Plays.Update(play);
                                    db.Tickets.Remove(ticket);
                                    db.SaveChanges();
                                    clientResponse.State = State.SuccessfulTicketReturn;

                                }
                                messageToClient = JsonSerializer.Serialize<IMemory>(clientResponse);
                                data = Encoding.Unicode.GetBytes(messageToClient);
                                handler.Send(data);
                            }
                            break;
                        case State.BuyTicket:

                            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + clientResponse.State);
                            Console.WriteLine(logJson);

                            using (PlayDBContext db = new PlayDBContext())
                            {
                                Play play = db.Plays.Where(p => p.PlayName.Equals(clientResponse.Play.PlayName)).FirstOrDefault();
                                if (play != null && play.TicketAmount > 0)
                                {
                                    play.TicketAmount--;
                                    db.Plays.Update(play);
                                    db.SaveChanges();

                                    var client = db.Spectators.Where(c => c.Id.Equals(clientResponse.Account.Id)).FirstOrDefault();
                                    Ticket ticket = new Ticket()
                                    {
                                        ClientNavigation = client,
                                        PlayNavigation = play,
                                        Place = clientResponse.Ticket.Place,
                                        Price = clientResponse.Ticket.Price,
                                    };
                                    db.Tickets.Add(ticket);
                                    db.SaveChanges();

                                    clientResponse.State = State.SuccessfulTicketBuy;

                                }

                                messageToClient = JsonSerializer.Serialize<IMemory>(clientResponse);
                                data = Encoding.Unicode.GetBytes(messageToClient);
                                handler.Send(data);
                            }
                            break;
                        case State.GetTickets:
                            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + clientResponse.State);
                            Console.WriteLine(logJson);

                            using (PlayDBContext db = new PlayDBContext())
                            {

                                List<TicketMemory> TicketList = new List<TicketMemory>();

                                //var clientTickets = from ticket in db.Tickets
                                //                    join play in db.Plays
                                //                    on ticket.Play equals play.Id
                                //                    where ticket.Client == clientResponse.Account.Id
                                //                    select new
                                //                    {
                                //                        Price = ticket.Price,
                                //                        Place = ticket.Place,
                                //                        PlayName = play.PlayName,
                                //                        DateStart = play.StartDate,
                                //                        StartTime = play.StartTime
                                //                    };

                                var query = db.Tickets.Join(db.Plays,
                                     t => t.Play, // свойство-селектор объекта из первого набора
                                     p => p.Id,
                                     (t, p) => new
                                     {
                                         Price = t.Price,
                                         Place = t.Place,
                                         PlayName = p.PlayName,
                                         DateStart = p.StartDate,
                                         StartTime = p.StartTime,
                                         Client = t.Client,
                                         Id = t.Id

                                     }
                                    ).Where(t => t.Client.Equals(clientResponse.Account.Id)).ToList();
                                if (query.Count == 0)
                                {
                                    clientResponse.State = State.GetTicketsFail;
                                }
                                else
                                {

                                    foreach (var item in query)
                                    {
                                        TicketList.Add(new TicketMemory(item.Price, item.Place, item.PlayName,
                                            clientResponse.Account.Nickname, item.DateStart, item.StartTime, item.Id));
                                    }
                                    clientResponse.TicketList = TicketList;

                                }

                                messageToClient = JsonSerializer.Serialize<IMemory>(clientResponse);

                                data = Encoding.Unicode.GetBytes(messageToClient);
                                handler.Send(data);
                            }
                            break;

                        case State.GetAllPlaysState:
                            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + clientResponse.State);
                            Console.WriteLine(logJson);
                            using (PlayDBContext db = new PlayDBContext())
                            {
                                List<PlayMemory> PlaysList = new List<PlayMemory>();
                                var query = db.Plays.ToList();
                                foreach (var item in query)
                                {
                                    PlaysList.Add(new PlayMemory(item.PlayName, item.StartDate, item.StartTime, item.EndTime, item.TicketAmount, item.Id));
                                }
                                messageToClient = JsonSerializer.Serialize<List<PlayMemory>>(PlaysList);

                                data = Encoding.Unicode.GetBytes(messageToClient);
                                handler.Send(data);
                            }
                            break;

                        case State.InputState:
                            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + clientResponse.State);
                            Console.WriteLine(logJson);
                            using (PlayDBContext db = new PlayDBContext())
                            {
                                Play play = new Play()
                                {
                                    PlayName = clientResponse.Play.PlayName,
                                    StartDate = clientResponse.Play.StartDate,
                                    EndDate = "",
                                    StartTime = clientResponse.Play.StartTime,
                                    EndTime = clientResponse.Play.EndTime,
                                    TicketAmount = clientResponse.Play.TicketAmount,


                                };
                                db.Plays.Add(play);
                                db.SaveChanges();
                                messageToClient = "Запись добавлена!";
                                data = Encoding.Unicode.GetBytes(messageToClient);
                                handler.Send(data);
                            }


                            break;

                        case State.PlayState:
                            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + clientResponse.State);
                            Console.WriteLine(logJson);
                            messageFromClient = clientResponse.Query;

                            using (PlayDBContext db = new PlayDBContext())
                            {
                                List<PlayMemory> PlaysList = new List<PlayMemory>();
                                var query = db.Plays.Where(p => p.PlayName.Contains(messageFromClient)).ToList();
                                foreach (var item in query)
                                {
                                    PlaysList.Add(new PlayMemory(item.PlayName, item.StartDate, item.StartTime, item.EndTime, item.TicketAmount, item.Id));
                                }
                                messageToClient = JsonSerializer.Serialize<List<PlayMemory>>(PlaysList);
                                data = Encoding.Unicode.GetBytes(messageToClient);
                                handler.Send(data);
                            }
                            break;

                        case State.AuthorizationState:
                            switch (clientResponse.Authorization.Status)
                            {



                                case Status.Authorization:
                                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + clientResponse.Authorization.Status);
                                    Console.WriteLine(logJson);

                                    using (PlayDBContext db = new PlayDBContext())
                                    {

                                        var query = db.Accounts.Where(a => a.Nickname.Equals(clientResponse.Authorization.Login) &&
                                        a.Parol.Equals(clientResponse.Authorization.Password)).FirstOrDefault();
                                        if (query != null)
                                        {

                                            if (db.Administrators.Where(a => a.Id.Equals(query.Id)).FirstOrDefault() != null)
                                            {
                                                clientResponse.Authorization.Status = Status.Admin;
                                                var playsList = db.Plays.ToList();
                                                List<PlayMemory> playMemories = new List<PlayMemory>();
                                                foreach (var play in playsList)
                                                {
                                                    playMemories.Add(new PlayMemory(play.PlayName, play.StartDate,
                                                        play.StartTime, play.EndTime, play.TicketAmount, play.Id));
                                                }
                                                clientResponse.PlaysList = playMemories;
                                            }
                                            else if (query != null)
                                            {
                                                clientResponse.Authorization.Status = Status.Authorized;
                                                clientResponse.Account = new AccountMemory(query.Id, query.Nickname, query.Email, query.Parol);
                                            }
                                            else
                                                clientResponse.Authorization.Status = Status.AuthorizationFail;

                                            //var adminQuery = db.Accounts.Join(db.Administrators, u => u.Id, c => c.Id, (u, c) => new
                                            //{
                                            //    NickName = u.Nickname,
                                            //    Pass = u.Parol,
                                            //    Email = u.Email
                                            //}).Where(u => u.NickName.Equals(clientResponse.Authorization.Login) 
                                            //               && u.Pass.Equals(clientResponse.Authorization.Password));


                                        }

                                        else
                                        {
                                            clientResponse.Authorization.Status = Status.AuthorizationFail;
                                        }
                                        messageToClient = JsonSerializer.Serialize<IMemory>(clientResponse);
                                        data = Encoding.Unicode.GetBytes(messageToClient);
                                        handler.Send(data);

                                    }
                                    break;
                                case Status.Registration:
                                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + clientResponse.Authorization.Status);
                                    Console.WriteLine(logJson);
                                    using (PlayDBContext db = new PlayDBContext())
                                    {
                                        var query = db.Accounts.Where(a => a.Nickname.Equals(clientResponse.Authorization.Login)).FirstOrDefault();
                                        if (query != null)
                                            clientResponse.Authorization.Status = Status.RegistraionFail;
                                        else
                                        {
                                            Account acc = new Account()
                                            {
                                                Email = clientResponse.Authorization.Email,
                                                Nickname = clientResponse.Authorization.Login,
                                                Parol = clientResponse.Authorization.Password
                                            };
                                            Spectator spec = new Spectator()
                                            {
                                                FullName = clientResponse.Authorization.FullName,
                                                IdNavigation = acc
                                            };

                                            db.Spectators.Add(spec);
                                            db.Accounts.Add(acc);

                                            db.SaveChanges();
                                            clientResponse.Authorization.Status = Status.Registered;
                                        }
                                        messageToClient = JsonSerializer.Serialize<IMemory>(clientResponse);
                                        Console.WriteLine(messageToClient);
                                        data = Encoding.Unicode.GetBytes(messageToClient);
                                        handler.Send(data);
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }

                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

