namespace MinimalAPI.Application.DTOs.Users;

public sealed record UserMAPI
{
    public required ulong Id { get; set; }
    public required bool Active { get; set; }
    public required ulong Role { get; set; }
    public required string FirstName { get; set; }
    public required string? MidName { get; set; }
    public required string FatherLastName { get; set; }
    public required string? MotherLastname { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
