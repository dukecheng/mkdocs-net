using AutoMapper;
using Niusys.Docs.Core.Projects;

namespace Niusys.Docs.Web.ApiModels
{
    public class LoginResult
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public long ExpiresIn { get; set; }
        public string Token { get; set; }
    }

    public class TokenInfo
    {
        public long ExpiresIn { get; set; }
        public string Token { get; set; }
    }

    public class Mappping : Profile
    {
        public Mappping()
        {
            CreateMap<User, LoginResult>();
            CreateMap<TokenInfo, LoginResult>();
        }
    }
}
