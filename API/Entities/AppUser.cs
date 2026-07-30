namespace API.Entities;

public class AppUser
{
    //the ID -->represent the rows
    public  string Id { get; set; }=Guid.NewGuid().ToString();

//these are represent the columns
    public required string DisplayName { get; set; } 
    public required string Email { get; set; }
}