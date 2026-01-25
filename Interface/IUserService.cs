public interface IUserService
{
    Task<UserDto> GetUserProfileAsync(string userId);
    Task<UserDto> UpdateUserProfileAsync(string userId, UserDto dto);
    Task<UserDto> UpdatePhotoPathAsync(string userId, string photoPath);

}