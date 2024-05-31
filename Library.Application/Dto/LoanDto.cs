namespace Library.Application.Dto;

public record LoanDto(
    Guid Guid,
    Guid BookGuid,
    Guid LibraryGuid,
    Guid MemberGuid,
    DateTime LoanDate,
    DateTime DueDate
);