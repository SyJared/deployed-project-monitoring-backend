
namespace Models;

public class Users
{
    public int id { get; set; }

    public string first_name { get; set; } = string.Empty;
    public string last_name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string username { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public ICollection<Projects> Projects { get; set; } = new List<Projects>();
}
