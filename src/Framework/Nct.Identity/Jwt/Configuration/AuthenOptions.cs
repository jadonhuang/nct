namespace Nct.Identity.Jwt.Configuration
{
    public class AuthenOptions
    {
        public AuthenConfiguration Authentication { get; set; }
    }

    public class AuthenConfiguration
    {

        public JwtConfiguration JwtBearer { get; set; }

    }


    public class JwtConfiguration
    {

        public string SecurityKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }


        public int? Expire { get; set; } = 24 * 60;

    }


}
