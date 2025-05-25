namespace TodoApi.Models;

public class ErrorItem
{
    public string message { get; set; }
    public ErrorItem(string aMessage)
    {
        message = aMessage;
    }
    public override string ToString()
    {
        return message;
    }
}
