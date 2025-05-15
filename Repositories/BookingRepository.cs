using System.Data;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class BookingRepository : BaseRepository
{
    public BookingRepository() {
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
        item.BeginDate = ((DateTime) row["begin_date"]).ToString("yyyy-MM-dd HH:mm:ss");
        item.EndDate = ((DateTime) row["end_date"]).ToString("yyyy-MM-dd HH:mm:ss");
        item.Rented = Int32.Parse(row["rented"].ToString());
        item.Returned = Int32.Parse(row["returned"].ToString());
        item.Canceled = Int32.Parse(row["canceled"].ToString());
        
        return item;
    }

    public List<BookingItem> getBookings() {
        string query = "SELECT * FROM " + tableName;
        var ds = getDataSet(query);
    
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