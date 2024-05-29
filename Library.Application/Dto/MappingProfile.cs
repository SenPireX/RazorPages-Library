using AutoMapper;
using Library.Application.Model;

namespace Library.Application.Dto;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<LibraryDto, Model.Library>();
        CreateMap<Model.Library, LibraryDto>();
        CreateMap<LoanDto, Loan>()
            .ForMember(
                l => l.Guid,
                opt => opt.MapFrom(l => l.Guid == default ? Guid.NewGuid() : l.Guid));
        CreateMap<Loan, LoanDto>();
    }
}