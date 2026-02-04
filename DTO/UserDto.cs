public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
    public string? Specialization {  get; set; }
    public string? City {  get; set; }
    public string? Description { get; set; }
    public string? PhotoUrl { get; set; }
}

public class UserRegisterDto
{
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
    public string? Specialization { get; set; }
    public string? City { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class UserLoginDto
{
    public string Identifier { get; set; }
    public string Password { get; set; }
}
