namespace TodoApi.Models;

public class BookingItem
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public int UserId { get; set; }
    public string? BeginDate { get; set; }
    public string? EndDate { get; set; }
    public override string ToString() 
    {
        return Id + ":" + ResourceId + ":" + UserId + ":" + BeginDate + ":" + EndDate;
    }
    
    public Dictionary<string, string> getAsDictionary() 
    {
        return new Dictionary<string, string>()
        {
            { "hw_resourse_Id", ResourceId.ToString() },
            { "user_Id", UserId.ToString() },
            { "begin_date", BeginDate },
            { "end_date", EndDate }
        };
    }
}