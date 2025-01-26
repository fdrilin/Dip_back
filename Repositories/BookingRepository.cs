using System.Data;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class BookingRepository : BaseRepository
{
    public BookingRepository() {
        tableName = "booking";
    }

    public List<BookingItem> getBookings() {
        string query = "SELECT * FROM " + tableName;
        var ds = getDataSet(query);
    
        List<BookingItem> bookingItems = new List<BookingItem>();
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            BookingItem item = new BookingItem();

            var row = ds.Tables[0].Rows[i];

            item.Id = Int32.Parse(row["id"].ToString());
            item.ResourceId = Int32.Parse(row["hw_resourse_id"].ToString());
            item.UserId = Int32.Parse(row["user_id"].ToString());
            item.BeginDate = row["begin_date"].ToString();
            item.EndDate = row["end_date"].ToString();

            bookingItems.Add(item);
        }

        return bookingItems; 
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