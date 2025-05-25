namespace TodoApi.Models;

public class ResourceItem
{
    public int Id { get; set; }
    public int ResourceTypeId { get; set; }
    public string? SerialNo { get; set; }
    public int? Available { get; set; }
    public override string ToString()
    {
        return Id + ":" + SerialNo + ":available-" + Available;
    }

    public Dictionary<string, string> getAsDictionary()
    {
        return new Dictionary<string, string>()
        {
            { "resource_type_id", ResourceTypeId.ToString() },
            { "serial_no", SerialNo },
            { "available", Available.ToString() }
        };
    }
}
