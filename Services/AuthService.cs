using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly IPasswordHasher<User> _hasher;

    public AuthService(ApplicationDbContext context, IConfiguration config, IPasswordHasher<User> hasher)
    {
        _context = context;
        _config = config;
        _hasher = hasher;
    }

    public async Task<AuthResultDto> RegisterAsync(UserRegisterDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            throw new Exception("Пароли не совпадают");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new Exception("Пользователь с таким Email уже существует");

        var user = new User
        {
            FullName = dto.FullName,
            UserName = dto.UserName,
            Email = dto.Email,
            RoleId = dto.RoleId,
            Specialization = dto.RoleId == 2 ? dto.Specialization : null,
            City = dto.RoleId == 2 ? dto.City : null
        };

        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return GenerateJwt(user);
    }

    public async Task<AuthResultDto> LoginAsync(UserLoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Identifier ||
                                                                 u.UserName == dto.Identifier);
        if (user == null)
            throw new Exception("Неверный Email или пароль");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result != PasswordVerificationResult.Success)
            throw new Exception("Неверный Email или пароль");

        return GenerateJwt(user);
    }

    private AuthResultDto GenerateJwt(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.RoleId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

        return new AuthResultDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserName = user.UserName,
            UserId = user.Id,
            RoleId = user.RoleId
        };
    }
}
