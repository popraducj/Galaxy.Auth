using Galaxy.Auth.Core.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Galaxy.Auth.Core.Helpers
{
    public class UrlEncoder: IUrlEncoder
    {
        public string Encode(string input)
        {
            return Base64UrlEncoder.Encode(input);
        }
     
        public string Decode(string input)
        {
            return Base64UrlEncoder.Decode(input);
        }
    }
}