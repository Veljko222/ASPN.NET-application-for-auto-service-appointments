using AutoService.Application.Auth.Commands;
using AutoService.Application.Auth.Queries;
using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Application.Repositories;
using AutoService.Domain.Models;

namespace AutoService.Application.Auth.Handlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
    {
        private readonly IRepository<Vlasnik> _vlasnikRepository;
        private readonly IRepository<Serviser> _serviserRepository;
        private readonly IRepository<Vozilo> _voziloRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public RegisterUserCommandHandler(
            IRepository<Vlasnik> vlasnikRepository,
            IRepository<Serviser> serviserRepository,
            IRepository<Vozilo> voziloRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _vlasnikRepository = vlasnikRepository;
            _serviserRepository = serviserRepository;
            _voziloRepository = voziloRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResultDto> Handle(RegisterUserCommand request)
        {
            var dto = request.Dto;
            string email = dto.Email.Trim().ToLowerInvariant();
            string userName = dto.UserName.Trim();
            string registracija = dto.Registracija.Trim().ToUpperInvariant();

            if (_vlasnikRepository.GetAll().Any(v => v.Email == email) ||
                _serviserRepository.GetAll().Any(s => s.Email == email))
            {
                throw new InvalidOperationException("Vlasnik sa ovim emailom vec postoji.");
            }

            if (_vlasnikRepository.GetAll().Any(v => v.UserName == userName) ||
                _serviserRepository.GetAll().Any(s => s.UserName == userName))
            {
                throw new InvalidOperationException("Korisnicko ime je vec zauzeto.");
            }

            if (_voziloRepository.GetAll().Any(v => v.Registracija == registracija))
            {
                throw new InvalidOperationException("Vozilo sa ovom registracijom vec postoji.");
            }

            var vlasnik = new Vlasnik
            {
                Ime = dto.Ime.Trim(),
                Prezime = dto.Prezime.Trim(),
                Email = email,
                Telefon = dto.Telefon.Trim(),
                UserName = userName,
                PasswordHash = _passwordHasher.Hash(dto.Password)
            };

            vlasnik.Vozila.Add(new Vozilo
            {
                Marka = dto.Marka.Trim(),
                Model = dto.Model.Trim(),
                GodinaProizvodnje = dto.GodinaProizvodnje,
                Registracija = registracija
            });

            await _vlasnikRepository.AddAsync(vlasnik);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResultDto
            {
                Token = _jwtTokenService.CreateToken(
                    vlasnik.VlasnikId,
                    vlasnik.UserName,
                    vlasnik.Email,
                    "User"),
                UserName = vlasnik.UserName,
                Email = vlasnik.Email,
                Role = "User"
            };
        }
    }

    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, AuthResultDto>
    {
        private readonly IRepository<Vlasnik> _vlasnikRepository;
        private readonly IRepository<Serviser> _serviserRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginUserQueryHandler(
            IRepository<Vlasnik> vlasnikRepository,
            IRepository<Serviser> serviserRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _vlasnikRepository = vlasnikRepository;
            _serviserRepository = serviserRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public Task<AuthResultDto> Handle(LoginUserQuery request)
        {
            string email = request.Dto.Email.Trim().ToLowerInvariant();

            var adminServiser = _serviserRepository
                .GetAll()
                .FirstOrDefault(s =>
                    s.IsAdmin &&
                    s.Aktivan &&
                    s.Email == email &&
                    s.PasswordHash != null);

            if (adminServiser != null &&
                _passwordHasher.Verify(request.Dto.Password, adminServiser.PasswordHash!))
            {
                return Task.FromResult(new AuthResultDto
                {
                    Token = _jwtTokenService.CreateToken(
                        adminServiser.ServiserId,
                        adminServiser.UserName ?? adminServiser.Email ?? "admin",
                        adminServiser.Email ?? email,
                        "Admin"),
                    UserName = adminServiser.UserName ?? adminServiser.Email ?? "admin",
                    Email = adminServiser.Email ?? email,
                    Role = "Admin"
                });
            }

            var vlasnik = _vlasnikRepository
                .GetAll()
                .FirstOrDefault(v => v.Email == email);

            if (vlasnik == null ||
                !_passwordHasher.Verify(request.Dto.Password, vlasnik.PasswordHash))
            {
                throw new InvalidOperationException("Email ili lozinka nisu ispravni.");
            }

            var result = new AuthResultDto
            {
                Token = _jwtTokenService.CreateToken(
                    vlasnik.VlasnikId,
                    vlasnik.UserName,
                    vlasnik.Email,
                    "User"),
                UserName = vlasnik.UserName,
                Email = vlasnik.Email,
                Role = "User"
            };

            return Task.FromResult(result);
        }
    }
}
