using System.ComponentModel.DataAnnotations;

namespace Library.Application.Dto;

public record LibraryDto(
    Guid Guid,
    [StringLength(100, MinimumLength = 2, ErrorMessage = "The Name has to be at least 2 characters and a maximum of 100 characters long.")]
    string Name
    );