using System.Text.Json;
using Thiskord_Back.Models.Account;

namespace Thiskord_Back.Models.Auth
{
    public class AuthenticatedUser 
    {
        public UserAccount user { get; set; }
        public string token { get; set; }


        // constructeur avec objet Account | On part du principe que l'on a fait appel au constructeur User en amont
        public AuthenticatedUser(UserAccount _user, string _token)
        {
            this.user = _user;
            this.token = _token;
        }

    }

    public class AuthRequest
    {
        public string user_auth { get; set; }
        public string password { get; set; }
    }
}
