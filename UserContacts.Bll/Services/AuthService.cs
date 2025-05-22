using System.Security.Claims;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserContacts.Bll.Dtos;
using UserContacts.Bll.Helpers;
using UserContacts.Bll.Helpers.Security;
using UserContacts.Bll.Settings;
using UserContacts.Core.Errors;
using UserContacts.Dal;
using UserContacts.Dal.Entities;

namespace UserContacts.Bll.Services;

public class AuthService(MainContext _context, IValidator<UserCreateDto> _validator, ITokenService _tokenService, JwtAppSettings _jwtSetting, IValidator<UserCreateDto> _validatorForLogin, ILogger<AuthService> _logger) : IAuthService
{
    private readonly int Expires = int.Parse(_jwtSetting.Lifetime);
    public async Task<long> SignUpUserAsync(UserCreateDto userCreateDto)
    {
        var validatorResult = await _validator.ValidateAsync(userCreateDto);
        if (!validatorResult.IsValid)
        {
            string errorMessages = string.Join("; ", validatorResult.Errors.Select(e => e.ErrorMessage));
            throw new AuthException(errorMessages);
        }

        var tupleFromHasher = PasswordHasher.Hasher(userCreateDto.Password);
        var user = new User()
        {
            FirstName = userCreateDto.FirstName,
            LastName = userCreateDto.LastName,
            UserName = userCreateDto.UserName,
            Email = userCreateDto.Email,
            PhoneNumber = userCreateDto.PhoneNumber,
            Password = tupleFromHasher.Hash,
            Salt = tupleFromHasher.Salt,
        };
        user.RoleId = await GetRoleIdAsync("User");

        await _context.Users.AddAsync(user);

        _logger.LogInformation($"UserId : {user.UserId} -- SignUp in the server");
        await _context.SaveChangesAsync();
        return user.UserId;
    }

    public async Task LogOut(string token)
    {
        var foundToken = await _context.RefreshTokens.FirstOrDefaultAsync(x=>x.Token == token);
        if (foundToken is null)
        {
            throw new EntityNotFoundException(token);
        }
        _context.RefreshTokens.Remove(foundToken);
        _context.SaveChanges();
    }


    public async Task<LoginResponseDto> LoginUserAsync(UserLoginDto userLoginDto)
    {
        var user = await SelectUserByUserNameAync(userLoginDto.UserName);

        var checkUserPassword = PasswordHasher.Verify(userLoginDto.Password, user.Password, user.Salt);

        if (checkUserPassword == false)
        {
            throw new UnauthorizedException($"UserId -- {user.UserId} -- UserName or password incorrect");
        }

        var userGetDto = new UserGetDto()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.Name,
        };

        var token = _tokenService.GenerateToken(userGetDto);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenToDB = new RefreshToken()
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(21),
            IsRevoked = false,
            UserId = user.UserId
        };

        await AddRefreshToken(refreshTokenToDB);

        var loginResponseDto = new LoginResponseDto()
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            Expires = 24
        };

        _logger.LogInformation($"UserId : {user.UserId} -- Login in the server");

        return loginResponseDto;
    }



    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshRequestDto request)
    {
        ClaimsPrincipal? principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null) throw new ForbiddenException("Invalid access token.");


        var userClaim = principal.FindFirst(c => c.Type == "UserId");

        if (userClaim is null)
        {
            throw new ForbiddenException();
        }

        var userId = long.Parse(userClaim.Value);


        var refreshToken = await SelectRefreshToken(request.RefreshToken, userId);
        if (refreshToken == null || refreshToken.Expires < DateTime.UtcNow || refreshToken.IsRevoked)
            throw new UnauthorizedException($"UserId : {userId} --- Invalid or expired refresh token.");

        refreshToken.IsRevoked = true;

        var user = await SelectUserByIdAync(userId);

        var userGetDto = new UserGetDto()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.Name,
        };

        var newAccessToken = _tokenService.GenerateToken(userGetDto);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenToDB = new RefreshToken()
        {
            Token = newRefreshToken,
            Expires = DateTime.UtcNow.AddDays(21),
            IsRevoked = false,
            UserId = user.UserId
        };

        await AddRefreshToken(refreshTokenToDB);

        _logger.LogInformation($"UserId : {userId} -- refreshed His token");

        return new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            TokenType = "Bearer",
            Expires = 24
        };
    }

    private async Task<User> SelectUserByUserNameAync(string userName)
    {
        var user = await _context.Users.Include(_ => _.Role).FirstOrDefaultAsync(x => x.UserName == userName);
        if (user == null)
        {
            throw new EntityNotFoundException($"Entity with {userName} not found");
        }
        return user;
    }

    private async Task<RefreshToken> SelectRefreshToken(string refreshToken, long userId)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);
        return token;
    }
    private async Task AddRefreshToken(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    private async Task<long> GetRoleIdAsync(string role)
    {
        var foundRole = await _context.UserRoles.FirstOrDefaultAsync(_ => _.Name == role);
        if (foundRole is null)
        {
            throw new EntityNotFoundException(role + " - not found");
        }
        return foundRole.Id;
    }

    private async Task<User> SelectUserByIdAync(long id)
    {
        var user = await _context.Users.Include(_ => _.Role).FirstOrDefaultAsync(x => x.UserId == id);
        if (user == null)
        {
            throw new EntityNotFoundException($"UserId : {id} --- Entity not found");
        }
        return user;
    }
}
