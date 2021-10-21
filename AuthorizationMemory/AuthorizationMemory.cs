using System;

namespace Memory
{
    public class AuthorizationMemory
    {
        public Status Status { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }

        

        public AuthorizationMemory(Status Status, string Email, string Login, string Password, string FullName)
        {
            this.Status = Status;
            this.Email = Email;
            this.Login = Login;
            this.Password = Password;
            this.FullName = FullName;
        }
    }

    public enum Status
    {
        Authorization,
        Registration,
        Authorized,
        Registered,
        AuthorizationFail,
        RegistraionFail,
        Admin
    }
}
