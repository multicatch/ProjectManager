using System;
using ProjectManager.Database.Models;
using ProjectManager.Utils;

namespace ProjectManager.Users
{
    public class IdentityProvider
    {
        private readonly UserRegistry _userRegistry;
        private readonly IPasswordHash _passwordHash;

        public IdentityProvider(UserRegistry userRegistry, IPasswordHash passwordHash)
        {
            _userRegistry = userRegistry;
            _passwordHash = passwordHash;
        }

        public User Authenticate(string username, string password)
        {
            Validate.NotNullOrBlank(username, "Username cannot not be blank.");
            Validate.NotNullOrBlank(password, "Password cannot not be blank.");
            var user = Functional
                .Try(() =>
                    _userRegistry.Find(username)
                ).On<UserNotExistsException>(e =>
                    throw new AuthenticationException("Username or password is incorrect.")
                ).OrElseThrow();

            var passwordCorrect = _passwordHash.Verify(password, user.PasswordHash);
            if (!passwordCorrect)
            {
                throw new AuthenticationException("Username or password is incorrect.");
            }

            return user;
        }
    }

    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message)
        {
        }
    }
}