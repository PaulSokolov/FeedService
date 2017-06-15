using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FeedService
{
    public class AuthOptions
    {
        public const string ISSUER = "FeedServiceTokenServer"; // издатель токена
        public const string AUDIENCE = "http://localhost:60840/"; // потребитель токена
        const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
        public const int LIFETIME = 5; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
