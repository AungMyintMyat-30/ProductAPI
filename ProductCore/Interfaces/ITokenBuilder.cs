namespace ProductCore.Interfaces
{
    public interface ITokenBuilder
    {
        string GenerateAccessToken(string user);
    }
}
