public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(UserRegisterDto dto);
    Task<AuthResultDto> LoginAsync(UserLoginDto dto);
}