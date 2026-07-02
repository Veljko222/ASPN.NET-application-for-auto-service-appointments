using AutoService.Application.DTOs;
using AutoService.Application.Mediator;

namespace AutoService.Application.Vlasnici.Commands
{
    public class CreateVlasnikCommand : IRequest
    {
        public CreateVlasnikCommand(VlasnikDto dto)
        {
            Dto = dto;
        }

        public VlasnikDto Dto { get; }
    }

    public class UpdateVlasnikCommand : IRequest
    {
        public UpdateVlasnikCommand(VlasnikDto dto)
        {
            Dto = dto;
        }

        public VlasnikDto Dto { get; }
    }

    public class DeleteVlasnikCommand : IRequest
    {
        public DeleteVlasnikCommand(int vlasnikId)
        {
            VlasnikId = vlasnikId;
        }

        public int VlasnikId { get; }
    }
}

