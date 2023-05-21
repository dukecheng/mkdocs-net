using Niusys.Docs.Core.Configuration;

namespace Niusys.Docs.Core.Projects
{
    public class User : IConfig
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
}
