using AutoService.Application.DTOs;
using AutoService.Application.Mediator;

namespace AutoService.Application.Auth.Queries
{
    public class LoginUserQuery : IRequest<AuthResultDto>
    {
        public LoginUserQuery(LoginDto dto)
        {
            Dto = dto;
        }

        public LoginDto Dto { get; }
    }
}

