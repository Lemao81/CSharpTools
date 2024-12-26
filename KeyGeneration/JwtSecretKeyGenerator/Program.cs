namespace JwtSecretKeyGenerator;

internal class Program
{
    private static void Main(string[] _)
    {
        var rng   = System.Security.Cryptography.RandomNumberGenerator.Create();
        var bytes = new byte[256 / 8];
        rng.GetBytes(bytes);
        Console.WriteLine(Convert.ToBase64String(bytes));
    }
}
