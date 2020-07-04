using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Database;
using ProjectManager.Database.Models;
using ProjectManager.Utils;

namespace ProjectManager.Users
{
    public class UserRegistry
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IPasswordPolicyValidator _passwordPolicyValidator;
        private readonly IPasswordHash _passwordHash;

        public UserRegistry(DatabaseContext databaseContext, IPasswordPolicyValidator passwordPolicyValidator, IPasswordHash passwordHash)
        {
            _databaseContext = databaseContext;
            _passwordPolicyValidator = passwordPolicyValidator;
            _passwordHash = passwordHash;
        }

        public void Create(string name, string password)
        {
            Validate.NotNullOrBlank(name, "Name cannot be blank.");
            Validate.NotNullOrBlank(password, "Password cannot be blank.");
            Validate.ShouldThrow<UserNotExistsException>(() =>
            {
                Find(name);
            }, "User " + name + " already exists.");
            ValidatePassword(password);
            
            var hashedPassword = _passwordHash.Hash(password);
            var user = new User
            {
                Name = name,
                PasswordHash = hashedPassword
            };
            _databaseContext.Add(user);
            _databaseContext.SaveChanges();
        }

        private void ValidatePassword(string password)
        {
            try
            {
                _passwordPolicyValidator.Validate(password);
            }
            catch (PasswordValidationError e)
            {
                throw new ArgumentException(e.Message, e);
            }

        }

        public User Find(string name)
        {
            var user = _databaseContext.Users.AsEnumerable().SingleOrDefault(u => u.Name == name);
            if (user == null)
            {
                throw new UserNotExistsException("User " + name + " does not exist");
            }
            return user;
        }

        public User Find(int id)
        {
            var user = _databaseContext.Users
                .Include(u => u.Projects)
                .AsEnumerable()
                .SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                throw new UserNotExistsException("User with Id " + id + " does not exist");
            }
            return user;
        }
    }

    public class UserNotExistsException : Exception
    {
        public UserNotExistsException(string message)
            : base(message)
        {
        }
    }
}