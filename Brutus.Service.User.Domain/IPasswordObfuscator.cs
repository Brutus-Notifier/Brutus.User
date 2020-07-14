namespace Brutus.Service.User.Domain
{
    public interface IPasswordObfuscator
    {
        string Obfuscate(string value);
    }
}