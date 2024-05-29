namespace Library.Application.Dto;

public record LoanDto(
    Guid Guid,
    DateTime LoanDate,
    DateTime DueDate,
    Guid BookGuid,
    Guid LibraryGuid
);