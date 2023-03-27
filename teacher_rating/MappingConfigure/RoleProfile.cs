using AutoMapper;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;

namespace teacher_rating.MappingConfigure;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<SelfCriticism, SelfCriticismViewModel>().ReverseMap();
    }
}

