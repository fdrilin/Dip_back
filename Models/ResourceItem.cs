namespace TodoApi.Models;

public class ResourceItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public override string ToString() 
    {
        return Id + ":" + Title + ":" + Description;
    }
    
    public Dictionary<string, string> getAsDictionary() 
    {
        return new Dictionary<string, string>()
        {
            { "title", Title },
            { "description", Description }
        };
    }
}