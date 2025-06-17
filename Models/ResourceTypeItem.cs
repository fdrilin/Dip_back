namespace TodoApi.Models;

public class ResourceTypeItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Software { get; set; }
    public string? Tags { get; set; }
    public string[]? TagsArray { get; set; }
    public int? Available { get; set; }
    public string? PictureLink { get; set; }
    public override string ToString()
    {
        return Id + ":" + Title + ":" + Description;
    }

    public Dictionary<string, string> getAsDictionary()
    {
        return new Dictionary<string, string>()
        {
            { "title", Title },
            { "description", Description },
            { "software", Software },
            { "tags", Tags },
            { "picture_link", PictureLink },
        };
    }
}
