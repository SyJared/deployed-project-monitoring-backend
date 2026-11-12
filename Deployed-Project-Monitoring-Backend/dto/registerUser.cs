using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class RegisterUser
{
    [Required]
    public string first_name { get; set; } = string.Empty;

    [Required]
    public string last_name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string email { get; set; } = string.Empty;

    [Required]
    public string username { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string password { get; set; } = string.Empty;
}
public class LoginUser
{
    [Required]
    public string username { get; set; } = string.Empty;
    [Required]
    public string password { get; set; } = string.Empty;
}