namespace SWP391_Project.Helpers
{
    public class HashHelper
    {
        public static string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        
        public static bool Verify(string inputPassword, string dbHash)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, dbHash);
        }
    }
}
