using AutoService.Application.DTOs;
using AutoService.Application.Mediator;

namespace AutoService.Application.Serviseri.Commands
{
    public class CreateServiserCommand : IRequest
    {
        public CreateServiserCommand(ServiserDto dto)
        {
            Dto = dto;
        }

        public ServiserDto Dto { get; }
    }

    public class UpdateServiserCommand : IRequest
    {
        public UpdateServiserCommand(ServiserDto dto)
        {
            Dto = dto;
        }

        public ServiserDto Dto { get; }
    }

    public class DeleteServiserCommand : IRequest
    {
        public DeleteServiserCommand(int serviserId)
        {
            ServiserId = serviserId;
        }

        public int ServiserId { get; }
    }
}

