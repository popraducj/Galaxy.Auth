namespace Galaxy.Auth.Core.Interfaces
{
    public interface IUrlEncoder
    {
        string Encode(string input);
        string Decode(string input);
    }
}