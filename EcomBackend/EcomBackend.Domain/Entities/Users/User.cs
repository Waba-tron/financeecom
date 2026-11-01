using EcomBackend.Domain.Common.Constants;

namespace EcomBackend.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = RoleConstants.Customer;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
