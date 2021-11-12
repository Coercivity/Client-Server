namespace Memory
{
    public class AccountMemory
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string Parol { get; set; }


        public AccountMemory(int Id, string Nickname, string Email, string Parol)
        {
            this.Id = Id;
            this.Nickname = Nickname;
            this.Email = Email;
            this.Parol = Parol;
        }
    }
}
