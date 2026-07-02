using AutoService.Application.DTOs;
using AutoService.Application.Mediator;

namespace AutoService.Application.ServisneUsluge.Commands
{
    public class CreateServisnaUslugaCommand : IRequest
    {
        public CreateServisnaUslugaCommand(ServisnaUslugaDto dto)
        {
            Dto = dto;
        }

        public ServisnaUslugaDto Dto { get; }
    }

    public class UpdateServisnaUslugaCommand : IRequest
    {
        public UpdateServisnaUslugaCommand(ServisnaUslugaDto dto)
        {
            Dto = dto;
        }

        public ServisnaUslugaDto Dto { get; }
    }

    public class DeleteServisnaUslugaCommand : IRequest
    {
        public DeleteServisnaUslugaCommand(int servisnaUslugaId)
        {
            ServisnaUslugaId = servisnaUslugaId;
        }

        public int ServisnaUslugaId { get; }
    }
}

