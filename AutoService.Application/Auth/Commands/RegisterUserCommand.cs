using AutoService.Application.DTOs;
using AutoService.Application.Mediator;

namespace AutoService.Application.Auth.Commands
{
    public class RegisterUserCommand : IRequest<AuthResultDto>
    {
        public RegisterUserCommand(RegisterDto dto)
        {
            Dto = dto;
        }

        public RegisterDto Dto { get; }
    }
}
