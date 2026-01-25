using AutoMapper;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserDto> GetUserProfileAsync(string userId)
    {
        var user = await _context.Users.FindAsync(int.Parse(userId));

        if (user == null)
            throw new Exception("Пользователь не найден");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateUserProfileAsync(string userId, UserDto dto)
    {
        var user = await _context.Users.FindAsync(int.Parse(userId));
        if (user == null)
            throw new Exception("Пользователь не найден");

        _mapper.Map(dto, user);
        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdatePhotoPathAsync(string userId, string photoPath)
    {
        var user = await _context.Users.FindAsync(int.Parse(userId));
        if (user == null)
            throw new Exception("Пользователь не найден");

        user.PhotoUrl = photoPath;
        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }
}
