namespace Domain.Entities;

public class School
{
    public string Id { get; set; } = new Guid().ToString();
    public string Name { get; set; } = string.Empty;
    public DateTime EstablishedDate { get; set; }
}
