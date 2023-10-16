public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateRegistered { get; set; } = DateTime.Now;
    public bool IsInstructor { get; set; } = false;

    // Navigation properties
    public int? ShikshaGuruId { get; set; }
    public User ShikshaGuru { get; set; }
    public ICollection<User> Devotees { get; set; }
    public ICollection<ChantingRecord> ChantingRecords { get; set; }
}