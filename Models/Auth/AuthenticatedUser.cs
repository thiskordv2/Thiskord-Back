using System.Text.Json;
namespace Thiskord_Back.Models.Auth
{

    public class User
    {
        public string user_name { get; set; }
        public string user_mail { get; set; }
        public string user_picture { get; set; }

        public User(string _user_name, string _user_mail, string _user_picture)
        {
            this.user_name = _user_name;
            this.user_mail = _user_mail;
            this.user_picture = _user_picture;
        }
    }
    public class AuthenticatedUser
    {
        public User user { get; set; }
        public string token { get; set; }


        // constructeur avec objet User | On part du principe que l'on a fait appel au constructeur User en amont
        public AuthenticatedUser(User _user, string _token)
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
