using Microsoft.AspNetCore.Identity;

namespace appdev_sprint.Data;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
