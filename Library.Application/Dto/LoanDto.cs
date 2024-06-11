using System.ComponentModel.DataAnnotations;
using Library.Application.Model;

namespace Library.Application.Dto;

class ValidLoanDate : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime loanDate)
        {
            if (loanDate > DateTime.Now)
            {
                return new ValidationResult("LoanDate cannot be in the future.");
            }
            return ValidationResult.Success;
        }
        return new ValidationResult("Invalid LoanDate.");
    }
}

public record LoanDto(
    Guid Guid,
    [ValidLoanDate] DateTime LoanDate,
    DateTime DueDate,
    Guid BookGuid,
    Guid LibraryGuid
);

