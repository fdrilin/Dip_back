namespace TodoApi.Models;

public class ResourceItem
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public override string ToString() 
    {
        return Title;
    }
}