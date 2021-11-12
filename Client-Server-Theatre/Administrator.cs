
#nullable disable

namespace Server
{
    public partial class Administrator
    {
        public int Id { get; set; }

        public virtual Account IdNavigation { get; set; }
    }
}
