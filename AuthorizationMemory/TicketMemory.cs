using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory
{
    public class TicketMemory
    {
        public decimal? Price { get; set; }
        public int? Place { get; set; }
        public string Play { get; set; }
        public string Client { get; set; }
        
        public string DateStart { get; set; }
        public string TimeStart { get; set; }
        
        public int Id { get; set; }
        

        public TicketMemory(decimal? Price, int? Place, string Play, string Client, string DateStart, string TimeStart, int Id)
        {
            this.Price = Price;
            this.Place = Place;
            this.Play = Play;
            this.Client = Client;
            this.DateStart = DateStart;
            this.TimeStart = TimeStart;
            this.Id = Id;

        }
    }
}
