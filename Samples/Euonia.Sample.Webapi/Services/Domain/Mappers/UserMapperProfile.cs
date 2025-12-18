using AutoMapper;
using Nerosoft.Euonia.Sample.Domain.Aggregates;
using Nerosoft.Euonia.Sample.Domain.Commands;
using Nerosoft.Euonia.Sample.Domain.Dtos;

namespace Nerosoft.Euonia.Sample.Domain.Mappers;

public class UserMapperProfile : Profile
{
	public UserMapperProfile()
	{
		CreateMap<UserCreateDto, UserCreateCommand>();
		CreateMap<UserUpdateDto, UserUpdateCommand>();

		CreateMap<User, UserDetailDto>();
		CreateMap<User, UserListDto>();
	}
}
