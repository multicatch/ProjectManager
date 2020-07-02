using System;
using System.Linq;
using ProjectManager.Database;
using ProjectManager.Database.Models;

namespace ProjectManager.Users
{
    public class UserRegistry
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IPasswordPolicyValidator _passwordPolicyValidator;

        public UserRegistry(DatabaseContext databaseContext, IPasswordPolicyValidator passwordPolicyValidator)
        {
            _databaseContext = databaseContext;
            _passwordPolicyValidator = passwordPolicyValidator;
        }

        public void Create(string name, string password)
        {
            if (String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Name and password cannot be null or empty.");
            }

            var exists = _databaseContext.Users.AsEnumerable().Any(u => u.Name == name);
            if (exists)
            {
                throw new UserAlreadyExistsException("User " + name + " already exists.");
            }

            try
            {
                _passwordPolicyValidator.Validate(password);
            }
            catch (PasswordValidationError e)
            {
                throw new ArgumentException(e.Message, e);
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                Name = name,
                PasswordHash = hashedPassword
            };
            _databaseContext.Add(user);
            _databaseContext.SaveChanges();
        }
    }

    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string message)
            : base(message)
        {
        }
    }
}