namespace TodoApi.Models;

public class UserItem
{
    public int Id { get; set; }
    public string? Login { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Document_id { get; set; }
    public int? Admin { get; set; } = 0;
    public string? Token { get; set; }
    public string? Number { get; set; }

    public override string ToString()
    {
        return Id + ":" + Login + ":" + Name;
    }

    public Dictionary<string, string> getAsDictionary()
    {
        return new Dictionary<string, string>()
        {
            { "login", Login },
            { "password", Password },
            { "name", Name },
            { "email", Email },
            { "document_id", Document_id },
            { "admin", Admin.ToString() },
            { "token", Token },
            { "number", Number }
        };
    }
}
