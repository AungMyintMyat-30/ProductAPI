namespace ProductCore.Interfaces
{
    public interface ITokenBuilder
    {
        public string GenerateAccessToken(string user);
    }
}
