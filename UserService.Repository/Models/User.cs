public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Role { get; set; } = "User";// string:0 Admin | 1 Manager | 2 User
    public DateTime CreatedAt { get; set; }
}
