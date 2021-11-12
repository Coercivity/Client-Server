using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory
{
    public class IMemory
    {
        public PlayMemory Play { get; set; }
        public TicketMemory Ticket { get; set; }
        public AuthorizationMemory Authorization { get; set; }
        public AccountMemory Account { get; set; }
        public List<PlayMemory> PlaysList { get; set; }
        public List<TicketMemory> TicketList { get; set; }
        public State State { get; set; }
        public string Query { get; set; }

        //Порт для сокета
        public static int port = 8005; 
        //IP для сокета
        public static string IP = "127.0.0.1";

    }

    public enum State
    {
        PlayState,
        TicketState,
        AuthorizationState,
        InputState, 
        GetAllPlaysState, 
        BuyTicket,
        GetTickets,
        SuccessfulTicketBuy,
        GetTicketsFail,
        ReturnTicket,
        SuccessfulTicketReturn

    }

}
