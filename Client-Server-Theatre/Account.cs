#nullable disable

namespace Server
{
    public partial class Account
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string Parol { get; set; }

        public virtual Administrator Administrator { get; set; }
        public virtual Spectator Spectator { get; set; }
    }
}
