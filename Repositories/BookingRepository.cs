using System.Data;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class BookingRepository : BaseRepository
{
    public BookingRepository()
    {
        tableName = "booking";
    }

    private BookingItem? getItemFromRow(DataRow? row)
    {
        if (row == null)
        {
            return null;
        }

        BookingItem item = new();

        item.Id = Int32.Parse(row["id"].ToString());
        item.ResourceId = Int32.Parse(row["hw_resourse_id"].ToString());
        item.UserId = Int32.Parse(row["user_id"].ToString());
        item.BeginDate = ((DateTime)row["begin_date"]).ToString("yyyy-MM-dd HH:mm:ss");
        item.EndDate = ((DateTime)row["end_date"]).ToString("yyyy-MM-dd HH:mm:ss");
        item.Rented = Int32.Parse(row["rented"].ToString());
        item.Returned = Int32.Parse(row["returned"].ToString());
        item.Canceled = Int32.Parse(row["canceled"].ToString());
        try
        {
            item.ResourceTypeTitle = row["title"].ToString();
        }
        catch {}

        return item;
    }

    public List<BookingItem> getBookings(UserItem currentUser, string? tags = null, string? search="")
    {
        string query = $@"SELECT * FROM (
            SELECT rt.title, b.* FROM {tableName} as b
            JOIN resources as r ON r.id = b.hw_resourse_id
            JOIN resource_types as rt ON rt.id = r.resource_type_id";
        List<string> conditions = [];
        if (currentUser.Admin != 1)
        {
            conditions.Add("user_id = " + currentUser.Id);
        }
        if (search != "") {
            conditions.Add($"(b.id = '{search}' OR user_id = '{search}' OR rt.title LIKE @search)");
            search = $"%{search}%";
        }
        if (!string.IsNullOrEmpty(tags))
        {
            string[] tagValues = tags.Split(",");
            List<string> tagsConditions = [];
            if (tagValues.Contains("overdue"))
            {
                Console.WriteLine("overdue");
                tagsConditions.Add("end_date < curdate()");
                tagValues = tagValues.Where(val => val != "overdue").ToArray();
                Console.WriteLine(string.Join(", ", tagValues));
            }
            foreach (var tag in tagValues)
            {
                tagsConditions.Add($"{tag} = 0");
            }
            conditions.Add("(" + string.Join(" OR ", tagsConditions) + ")");
        }
        if (conditions.Count > 0)
        {
            query += " WHERE " + string.Join(" AND ", conditions);
        }
        Console.WriteLine(query);
        query += ") as t";

        var ds = getDataSet(query, search);

        List<BookingItem> bookingItems = new List<BookingItem>();
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            bookingItems.Add(getItemFromRow(row));
        }

        return bookingItems;
    }

    public BookingItem? getBookingItem(int id)
    {
        DataRow? row = getRowById(id);

        return getItemFromRow(row);
    }

    public BookingItem addBookingItem(BookingItem bookingItem)
    {
        bookingItem.Id = addRow(bookingItem.getAsDictionary());

        Console.WriteLine("added booking");
        return bookingItem;
    }

    public BookingItem updateBookingItem(BookingItem bookingItem, int id)
    {
        updateRow(bookingItem.getAsDictionary(), id);

        Console.WriteLine("updated booking");
        return bookingItem;
    }

    public bool deleteBookingItem(int id)
    {
        return deleteRow(id);
    }

}
