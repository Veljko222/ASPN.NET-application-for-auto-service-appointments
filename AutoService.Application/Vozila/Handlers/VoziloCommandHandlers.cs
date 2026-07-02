using AutoService.Application.Mediator;
using AutoService.Application.Repositories;
using AutoService.Application.Vozila.Commands;
using AutoService.Domain.Models;

namespace AutoService.Application.Vozila.Handlers
{
    public class CreateVoziloCommandHandler : IRequestHandler<CreateVoziloCommand>
    {
        private readonly IRepository<Vozilo> _voziloRepository;
        private readonly IRepository<Vlasnik> _vlasnikRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateVoziloCommandHandler(
            IRepository<Vozilo> voziloRepository,
            IRepository<Vlasnik> vlasnikRepository,
            IUnitOfWork unitOfWork)
        {
            _voziloRepository = voziloRepository;
            _vlasnikRepository = vlasnikRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateVoziloCommand request)
        {
            var dto = request.Dto;
            int vlasnikId = request.IsAdmin
                ? dto.VlasnikId
                : request.VlasnikId ?? 0;

            if (!request.IsAdmin && vlasnikId <= 0)
            {
                throw new InvalidOperationException("Vlasnik nije prijavljen.");
            }

            if (!_vlasnikRepository.GetAll().Any(v => v.VlasnikId == vlasnikId))
            {
                throw new InvalidOperationException("Izabrani vlasnik ne postoji.");
            }

            string registracija = dto.Registracija.Trim().ToUpperInvariant();

            if (_voziloRepository.GetAll().Any(v => v.Registracija == registracija))
            {
                throw new InvalidOperationException(
                    "Vozilo sa ovom registracijom veÄ‡ postoji.");
            }

            var vozilo = new Vozilo
            {
                Marka = dto.Marka.Trim(),
                Model = dto.Model.Trim(),
                GodinaProizvodnje = dto.GodinaProizvodnje,
                Registracija = registracija,
                VlasnikId = vlasnikId
            };

            await _voziloRepository.AddAsync(vozilo);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public class UpdateVoziloCommandHandler : IRequestHandler<UpdateVoziloCommand>
    {
        private readonly IRepository<Vozilo> _voziloRepository;
        private readonly IRepository<Vlasnik> _vlasnikRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateVoziloCommandHandler(
            IRepository<Vozilo> voziloRepository,
            IRepository<Vlasnik> vlasnikRepository,
            IUnitOfWork unitOfWork)
        {
            _voziloRepository = voziloRepository;
            _vlasnikRepository = vlasnikRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateVoziloCommand request)
        {
            var dto = request.Dto;
            var vozilo = await _voziloRepository.GetByIdAsync(dto.VoziloId);

            if (vozilo == null)
            {
                throw new InvalidOperationException("Izabrano vozilo ne postoji.");
            }

            if (!request.IsAdmin && vozilo.VlasnikId != request.VlasnikId)
            {
                throw new InvalidOperationException("Mozete menjati samo svoja vozila.");
            }

            int vlasnikId = request.IsAdmin
                ? dto.VlasnikId
                : vozilo.VlasnikId;

            if (!_vlasnikRepository.GetAll().Any(v => v.VlasnikId == vlasnikId))
            {
                throw new InvalidOperationException("Izabrani vlasnik ne postoji.");
            }

            string registracija = dto.Registracija.Trim().ToUpperInvariant();

            if (_voziloRepository.GetAll().Any(v =>
                v.Registracija == registracija &&
                v.VoziloId != dto.VoziloId))
            {
                throw new InvalidOperationException(
                    "Drugo vozilo veÄ‡ koristi ovu registraciju.");
            }

            vozilo.Marka = dto.Marka.Trim();
            vozilo.Model = dto.Model.Trim();
            vozilo.GodinaProizvodnje = dto.GodinaProizvodnje;
            vozilo.Registracija = registracija;
            vozilo.VlasnikId = vlasnikId;

            _voziloRepository.Update(vozilo);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public class DeleteVoziloCommandHandler : IRequestHandler<DeleteVoziloCommand>
    {
        private readonly IRepository<Vozilo> _voziloRepository;
        private readonly ITerminRepository _terminRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteVoziloCommandHandler(
            IRepository<Vozilo> voziloRepository,
            ITerminRepository terminRepository,
            IUnitOfWork unitOfWork)
        {
            _voziloRepository = voziloRepository;
            _terminRepository = terminRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteVoziloCommand request)
        {
            var vozilo = _voziloRepository
                .GetAll()
                .FirstOrDefault(v => v.VoziloId == request.VoziloId);

            if (vozilo == null)
            {
                throw new InvalidOperationException("Izabrano vozilo ne postoji.");
            }

            if (!request.IsAdmin && vozilo.VlasnikId != request.VlasnikId)
            {
                throw new InvalidOperationException("Mozete obrisati samo svoja vozila.");
            }

            if (_terminRepository.GetAll().Any(t => t.VoziloId == request.VoziloId))
            {
                throw new InvalidOperationException(
                    "Vozilo ne moÅ¾e biti obrisano jer ima evidentirane termine.");
            }

            _voziloRepository.Delete(vozilo);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

