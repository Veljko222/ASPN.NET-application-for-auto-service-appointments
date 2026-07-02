using AutoService.Application.Vlasnici.Commands;
using AutoService.Application.Mediator;
using AutoService.Application.Repositories;
using AutoService.Application.Auth;
using AutoService.Domain.Models;

namespace AutoService.Application.Vlasnici.Handlers
{
    public class CreateVlasnikCommandHandler : IRequestHandler<CreateVlasnikCommand>
    {
        private readonly IRepository<Vlasnik> _vlasnikRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public CreateVlasnikCommandHandler(
            IRepository<Vlasnik> vlasnikRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _vlasnikRepository = vlasnikRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(CreateVlasnikCommand request)
        {
            var dto = request.Dto;

            if (_vlasnikRepository.GetAll().Any(k => k.Email == dto.Email))
            {
                throw new InvalidOperationException(
                    "Vlasnik sa ovim emailom veÄ‡ postoji.");
            }

            if (_vlasnikRepository.GetAll().Any(k => k.UserName == dto.UserName))
            {
                throw new InvalidOperationException("KorisniÄko ime je veÄ‡ zauzeto.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new InvalidOperationException("Lozinka je obavezna.");
            }

            var vlasnik = new Vlasnik
            {
                Ime = dto.Ime.Trim(),
                Prezime = dto.Prezime.Trim(),
                Email = dto.Email.Trim(),
                Telefon = dto.Telefon.Trim(),
                UserName = dto.UserName.Trim(),
                PasswordHash = _passwordHasher.Hash(dto.Password)
            };

            await _vlasnikRepository.AddAsync(vlasnik);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public class UpdateVlasnikCommandHandler : IRequestHandler<UpdateVlasnikCommand>
    {
        private readonly IRepository<Vlasnik> _vlasnikRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UpdateVlasnikCommandHandler(
            IRepository<Vlasnik> vlasnikRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _vlasnikRepository = vlasnikRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(UpdateVlasnikCommand request)
        {
            var dto = request.Dto;
            var vlasnik = await _vlasnikRepository.GetByIdAsync(dto.VlasnikId);

            if (vlasnik == null)
            {
                throw new InvalidOperationException("Izabrani vlasnik ne postoji.");
            }

            if (_vlasnikRepository.GetAll().Any(k =>
                k.Email == dto.Email &&
                k.VlasnikId != dto.VlasnikId))
            {
                throw new InvalidOperationException(
                    "Drugi vlasnik veÄ‡ koristi ovaj email.");
            }

            if (_vlasnikRepository.GetAll().Any(k =>
                    k.UserName == dto.UserName &&
                    k.VlasnikId != dto.VlasnikId))
            {
                throw new InvalidOperationException("KorisniÄko ime je veÄ‡ zauzeto.");
            }

            vlasnik.Ime = dto.Ime.Trim();
            vlasnik.Prezime = dto.Prezime.Trim();
            vlasnik.Email = dto.Email.Trim();
            vlasnik.Telefon = dto.Telefon.Trim();
            vlasnik.UserName = dto.UserName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                vlasnik.PasswordHash = _passwordHasher.Hash(dto.Password);
            }

            _vlasnikRepository.Update(vlasnik);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public class DeleteVlasnikCommandHandler : IRequestHandler<DeleteVlasnikCommand>
    {
        private readonly IRepository<Vlasnik> _vlasnikRepository;
        private readonly IRepository<Vozilo> _voziloRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteVlasnikCommandHandler(
            IRepository<Vlasnik> vlasnikRepository,
            IRepository<Vozilo> voziloRepository,
            IUnitOfWork unitOfWork)
        {
            _vlasnikRepository = vlasnikRepository;
            _voziloRepository = voziloRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteVlasnikCommand request)
        {
            var vlasnik = _vlasnikRepository
                .GetAll()
                .FirstOrDefault(k => k.VlasnikId == request.VlasnikId);

            if (vlasnik == null)
            {
                throw new InvalidOperationException("Izabrani vlasnik ne postoji.");
            }

            if (_voziloRepository.GetAll().Any(v => v.VlasnikId == request.VlasnikId))
            {
                throw new InvalidOperationException(
                    "Vlasnik ne moÅ¾e biti obrisan jer ima evidentirana vozila.");
            }

            _vlasnikRepository.Delete(vlasnik);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

