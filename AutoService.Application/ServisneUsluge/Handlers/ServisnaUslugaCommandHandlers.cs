using AutoService.Application.Mediator;
using AutoService.Application.Repositories;
using AutoService.Application.ServisneUsluge.Commands;
using AutoService.Domain.Models;

namespace AutoService.Application.ServisneUsluge.Handlers
{
    public class CreateServisnaUslugaCommandHandler : IRequestHandler<CreateServisnaUslugaCommand>
    {
        private readonly IRepository<ServisnaUsluga> _uslugaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateServisnaUslugaCommandHandler(
            IRepository<ServisnaUsluga> uslugaRepository,
            IUnitOfWork unitOfWork)
        {
            _uslugaRepository = uslugaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateServisnaUslugaCommand request)
        {
            var dto = request.Dto;

            if (_uslugaRepository.GetAll().Any(u => u.Naziv == dto.Naziv))
            {
                throw new InvalidOperationException(
                    "Servisna usluga sa ovim nazivom veÄ‡ postoji.");
            }

            var usluga = new ServisnaUsluga
            {
                Naziv = dto.Naziv.Trim(),
                Opis = dto.Opis.Trim(),
                Cena = dto.Cena,
                TrajanjeUMinutima = dto.TrajanjeUMinutima,
                Aktivna = dto.Aktivna
            };

            await _uslugaRepository.AddAsync(usluga);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public class UpdateServisnaUslugaCommandHandler : IRequestHandler<UpdateServisnaUslugaCommand>
    {
        private readonly IRepository<ServisnaUsluga> _uslugaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateServisnaUslugaCommandHandler(
            IRepository<ServisnaUsluga> uslugaRepository,
            IUnitOfWork unitOfWork)
        {
            _uslugaRepository = uslugaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateServisnaUslugaCommand request)
        {
            var dto = request.Dto;
            var usluga = await _uslugaRepository.GetByIdAsync(dto.ServisnaUslugaId);

            if (usluga == null)
            {
                throw new InvalidOperationException("Izabrana servisna usluga ne postoji.");
            }

            if (_uslugaRepository.GetAll().Any(u =>
                u.Naziv == dto.Naziv &&
                u.ServisnaUslugaId != dto.ServisnaUslugaId))
            {
                throw new InvalidOperationException(
                    "Druga servisna usluga veÄ‡ koristi ovaj naziv.");
            }

            usluga.Naziv = dto.Naziv.Trim();
            usluga.Opis = dto.Opis.Trim();
            usluga.Cena = dto.Cena;
            usluga.TrajanjeUMinutima = dto.TrajanjeUMinutima;
            usluga.Aktivna = dto.Aktivna;

            _uslugaRepository.Update(usluga);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public class DeleteServisnaUslugaCommandHandler : IRequestHandler<DeleteServisnaUslugaCommand>
    {
        private readonly IRepository<ServisnaUsluga> _uslugaRepository;
        private readonly ITerminRepository _terminRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteServisnaUslugaCommandHandler(
            IRepository<ServisnaUsluga> uslugaRepository,
            ITerminRepository terminRepository,
            IUnitOfWork unitOfWork)
        {
            _uslugaRepository = uslugaRepository;
            _terminRepository = terminRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteServisnaUslugaCommand request)
        {
            var usluga = _uslugaRepository
                .GetAll()
                .FirstOrDefault(u => u.ServisnaUslugaId == request.ServisnaUslugaId);

            if (usluga == null)
            {
                throw new InvalidOperationException("Izabrana servisna usluga ne postoji.");
            }

            if (_terminRepository.GetAll().Any(t =>
                t.ServisnaUslugaId == request.ServisnaUslugaId))
            {
                usluga.Aktivna = false;
                _uslugaRepository.Update(usluga);
            }
            else
            {
                _uslugaRepository.Delete(usluga);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}

