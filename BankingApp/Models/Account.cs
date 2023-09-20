using BankingApp.Areas.Identity.Data;
using BankingApp.Data;
using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [UniqueAccountName(ErrorMessage = "An account with this name already exists.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Balance is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Balance cannot be negative")]
        public int Balance { get; set; }

        public string UserId { get; set; }
        public BankingAppUser User { get; set; }
    }

    public class UniqueAccountNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var dbContext = (BankingAppContext?)validationContext.GetService(typeof(BankingAppContext));

            var accountId = (int?)validationContext.ObjectType.GetProperty("Id")?.GetValue(validationContext.ObjectInstance);
            var userId = (string?)validationContext.ObjectType.GetProperty("UserId")?.GetValue(validationContext.ObjectInstance);
            var name = (string?)value;

            if (dbContext.Accounts.Any(a => a.UserId == userId && a.Name == name))
            {
                return new ValidationResult("Account name must be unique for the same user.");
            }

            return ValidationResult.Success;
        }
    }
}
