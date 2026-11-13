namespace backend.DTOs.Player;

public class CreatePlayerDto
{
    public string Name { get; set; } = string.Empty;

    public string FName { get; set; } = string.Empty;

    public string Age { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string Contact { get; set; } = string.Empty;

    //public string Belt { get; set; } = "White";

    public string History { get; set; } = "Fresh Member";

    public string Reason { get; set; } = string.Empty;

    public string Fee { get; set; } = string.Empty;

    //public string Status { get; set; } = "Active";
}