namespace MoneyCalc.Web.Services;

public interface IBoardPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string storedHash);
}
