namespace ProjectManager.Users
{
    public interface IPasswordHash
    {
        public string Hash(string password);
        public bool Verify(string password, string hash);
    }

    public class BCryptPasswordHash : IPasswordHash
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
    
}