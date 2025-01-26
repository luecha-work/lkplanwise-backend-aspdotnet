using AutoMapper;
using Entities;
using Shared.DTOs;

namespace PlanWiseBackend
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Account, AccountDto>();
            CreateMap<SingUpDto, Account>();
        }
    }
}
