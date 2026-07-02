using AutoService.Application.DTOs;
using AutoService.Application.Mediator;

namespace AutoService.Application.Vozila.Commands
{
    public class CreateVoziloCommand : IRequest
    {
        public CreateVoziloCommand(VoziloDto dto, int? vlasnikId, bool isAdmin)
        {
            Dto = dto;
            VlasnikId = vlasnikId;
            IsAdmin = isAdmin;
        }

        public VoziloDto Dto { get; }

        public int? VlasnikId { get; }

        public bool IsAdmin { get; }
    }

    public class UpdateVoziloCommand : IRequest
    {
        public UpdateVoziloCommand(VoziloDto dto, int? vlasnikId, bool isAdmin)
        {
            Dto = dto;
            VlasnikId = vlasnikId;
            IsAdmin = isAdmin;
        }

        public VoziloDto Dto { get; }

        public int? VlasnikId { get; }

        public bool IsAdmin { get; }
    }

    public class DeleteVoziloCommand : IRequest
    {
        public DeleteVoziloCommand(int voziloId, int? vlasnikId, bool isAdmin)
        {
            VoziloId = voziloId;
            VlasnikId = vlasnikId;
            IsAdmin = isAdmin;
        }

        public int VoziloId { get; }

        public int? VlasnikId { get; }

        public bool IsAdmin { get; }
    }
}

