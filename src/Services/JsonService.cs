using System.Text.Json;
using System.Text.Json.Serialization;
using Thiskord_Back.Models.Auth;
namespace Thiskord_Back.Services
{
    //referentiel pour le switch case de toObject 
    //TODO passer l'enum en paramètre global
    //public enum OBJET
    //{
    //    USER = 0,
    //    AUTHENTICATED_USER = 1,
    //}
    public class JsonService
    {
        public string toJson(Object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public Object toObject(string obj, string objet)
        {
            Object returnObject = null;
            //switch (objet)
            //{
            //    case "0":
            //        returnObject = JsonSerializer.Deserialize<User>(obj);
            //        break;
            //    case "1":
            //        returnObject = JsonSerializer.Deserialize<AuthenticatedUser>(obj);
            //        break;
            //    default: 
            //        returnObject = null;
            //        break;
            //}
            return returnObject;
        }
    }
}
