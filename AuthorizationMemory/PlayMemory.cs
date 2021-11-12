namespace Memory
{
    public class PlayMemory
    {
        public string PlayName { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int? TicketAmount { get; set; }
        public int Id { get; set; }


        public PlayMemory(string PlayName, string StartDate, string StartTime, string EndTime, int? TicketAmount, int Id)
        {
            this.PlayName = PlayName;
            this.StartDate = StartDate;
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            this.TicketAmount = TicketAmount;
            this.Id = Id;
          
        }


    }
}
