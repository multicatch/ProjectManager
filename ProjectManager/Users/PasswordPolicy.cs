using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ProjectManager.Users
{
    public interface IPasswordPolicyValidator
    {
        public void Validate(string password);
    }

    public class CompositePolicyValidator : IPasswordPolicyValidator
    {
        private static readonly List<IPasswordPolicyValidator> Validators = new List<IPasswordPolicyValidator>
        {
            new PasswordLengthPolicy(), 
            new UppercasePasswordPolicy(), 
            new LowercasePasswordPolicy(), 
            new PasswordWithNumbersPolicy()
        };
        
        public void Validate(string password)
        {
            Validators.ForEach(validator => validator.Validate(password));
        }
    }

    public class PasswordLengthPolicy : IPasswordPolicyValidator
    {
        public void Validate(string password)
        {
            if (password.Length <= 8)
            {
                throw new PasswordValidationError("Password must be longer than 8 characters.");
            }
        }
    }

    public class UppercasePasswordPolicy : IPasswordPolicyValidator
    {
        public void Validate(string password)
        {
            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                throw new PasswordValidationError("Password must contain uppercase characters.");
            }
        }
    }

    public class LowercasePasswordPolicy : IPasswordPolicyValidator
    {
        public void Validate(string password)
        {
            if (!Regex.IsMatch(password, "[a-z]"))
            {
                throw new PasswordValidationError("Password must contain lowercase characters.");
            }
        }
    }

    public class PasswordWithNumbersPolicy : IPasswordPolicyValidator
    {
        public void Validate(string password)
        {
            if (!Regex.IsMatch(password, "[0-9]"))
            {
                throw new PasswordValidationError("Password must contain numbers.");
            }
        }
    }

    public class PasswordValidationError : Exception
    {
        public PasswordValidationError(string message) : base(message)
        {
        }
    }
}