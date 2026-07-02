using AutoService.Application.Auth;
using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Application.Repositories;
using AutoService.Application.Serviseri.Commands;
using AutoService.Domain.Models;

namespace AutoService.Application.Serviseri.Handlers
{
    public class CreateServiserCommandHandler : IRequestHandler<CreateServiserCommand>
    {
        private readonly IRepository<Serviser> _serviserRepository;
        private readonly IRepository<Vlasnik> _vlasnikRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public CreateServiserCommandHandler(
            IRepository<Serviser> serviserRepository,
            IRepository<Vlasnik> vlasnikRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _serviserRepository = serviserRepository;
            _vlasnikRepository = vlasnikRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(CreateServiserCommand request)
        {
            var dto = request.Dto;
            ValidateAdminCredentials(dto, requirePassword: dto.IsAdmin);

            var serviser = new Serviser
            {
                Ime = dto.Ime.Trim(),
                Prezime = dto.Prezime.Trim(),
                Specijalizacija = dto.Specijalizacija.Trim(),
                Aktivan = dto.Aktivan,
                IsAdmin = dto.IsAdmin
            };

            ApplyAdminCredentials(serviser, dto, updatePassword: true);

            await _serviserRepository.AddAsync(serviser);
            await _unitOfWork.SaveChangesAsync();
        }

        private void ValidateAdminCredentials(ServiserDto dto, bool requirePassword)
        {
            if (!dto.IsAdmin)
            {
                return;
            }

            string email = dto.Email?.Trim().ToLowerInvariant() ?? string.Empty;
            string userName = dto.UserName?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(email) ||
                (requirePassword && string.IsNullOrWhiteSpace(dto.Password)))
            {
                throw new InvalidOperationException(
                    "Admin serviser mora imati korisnicko ime, email i lozinku.");
            }

            if (_serviserRepository.GetAll().Any(s => s.UserName == userName) ||
                _vlasnikRepository.GetAll().Any(v => v.UserName == userName))
            {
                throw new InvalidOperationException("Korisnicko ime je vec zauzeto.");
            }

            if (_serviserRepository.GetAll().Any(s => s.Email == email) ||
                _vlasnikRepository.GetAll().Any(v => v.Email == email))
            {
                throw new InvalidOperationException("Email je vec zauzet.");
            }
        }

        private void ApplyAdminCredentials(
            Serviser serviser,
            ServiserDto dto,
            bool updatePassword)
        {
            if (!dto.IsAdmin)
            {
                serviser.UserName = null;
                serviser.Email = null;
                serviser.PasswordHash = null;
                return;
            }

            serviser.UserName = dto.UserName?.Trim();
            serviser.Email = dto.Email?.Trim().ToLowerInvariant();

            if (updatePassword && !string.IsNullOrWhiteSpace(dto.Password))
            {
                serviser.PasswordHash = _passwordHasher.Hash(dto.Password);
            }
        }
    }

    public class UpdateServiserCommandHandler : IRequestHandler<UpdateServiserCommand>
    {
        private readonly IRepository<Serviser> _serviserRepository;
        private readonly IRepository<Vlasnik> _vlasnikRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UpdateServiserCommandHandler(
            IRepository<Serviser> serviserRepository,
            IRepository<Vlasnik> vlasnikRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _serviserRepository = serviserRepository;
            _vlasnikRepository = vlasnikRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(UpdateServiserCommand request)
        {
            var dto = request.Dto;
            var serviser = await _serviserRepository.GetByIdAsync(dto.ServiserId);

            if (serviser == null)
            {
                throw new InvalidOperationException("Izabrani serviser ne postoji.");
            }

            ValidateAdminCredentials(dto, serviser);

            serviser.Ime = dto.Ime.Trim();
            serviser.Prezime = dto.Prezime.Trim();
            serviser.Specijalizacija = dto.Specijalizacija.Trim();
            serviser.Aktivan = dto.Aktivan;
            serviser.IsAdmin = dto.IsAdmin;
            ApplyAdminCredentials(serviser, dto);

            _serviserRepository.Update(serviser);
            await _unitOfWork.SaveChangesAsync();
        }

        private void ValidateAdminCredentials(ServiserDto dto, Serviser current)
        {
            if (!dto.IsAdmin)
            {
                return;
            }

            string email = dto.Email?.Trim().ToLowerInvariant() ?? string.Empty;
            string userName = dto.UserName?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(email) ||
                (string.IsNullOrWhiteSpace(current.PasswordHash) &&
                    string.IsNullOrWhiteSpace(dto.Password)))
            {
                throw new InvalidOperationException(
                    "Admin serviser mora imati korisnicko ime, email i lozinku.");
            }

            if (_serviserRepository.GetAll().Any(s =>
                    s.UserName == userName &&
                    s.ServiserId != current.ServiserId) ||
                _vlasnikRepository.GetAll().Any(v => v.UserName == userName))
            {
                throw new InvalidOperationException("Korisnicko ime je vec zauzeto.");
            }

            if (_serviserRepository.GetAll().Any(s =>
                    s.Email == email &&
                    s.ServiserId != current.ServiserId) ||
                _vlasnikRepository.GetAll().Any(v => v.Email == email))
            {
                throw new InvalidOperationException("Email je vec zauzet.");
            }
        }

        private void ApplyAdminCredentials(Serviser serviser, ServiserDto dto)
        {
            if (!dto.IsAdmin)
            {
                serviser.UserName = null;
                serviser.Email = null;
                serviser.PasswordHash = null;
                return;
            }

            serviser.UserName = dto.UserName?.Trim();
            serviser.Email = dto.Email?.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                serviser.PasswordHash = _passwordHasher.Hash(dto.Password);
            }
        }
    }

    public class DeleteServiserCommandHandler : IRequestHandler<DeleteServiserCommand>
    {
        private readonly IRepository<Serviser> _serviserRepository;
        private readonly ITerminRepository _terminRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteServiserCommandHandler(
            IRepository<Serviser> serviserRepository,
            ITerminRepository terminRepository,
            IUnitOfWork unitOfWork)
        {
            _serviserRepository = serviserRepository;
            _terminRepository = terminRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteServiserCommand request)
        {
            var serviser = _serviserRepository
                .GetAll()
                .FirstOrDefault(s => s.ServiserId == request.ServiserId);

            if (serviser == null)
            {
                throw new InvalidOperationException("Izabrani serviser ne postoji.");
            }

            if (_terminRepository.GetAll().Any(t => t.ServiserId == request.ServiserId))
            {
                serviser.Aktivan = false;
                _serviserRepository.Update(serviser);
            }
            else
            {
                _serviserRepository.Delete(serviser);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
