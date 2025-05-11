using System.Data;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class UserRepository : BaseRepository
{
    public UserRepository() {
        tableName = "users";
    }

    private UserItem? getItemFromRow(DataRow? row) 
    {   
        if(row == null)
        {
            return null;
        }
        UserItem item = new();
        
        item.Id = Int32.Parse(row["id"].ToString() ?? "");
        item.Login = row["login"].ToString();
        item.Password = row["password"].ToString();
        item.Name = row["name"].ToString();
        item.Email = row["email"].ToString();
        item.Document_id = row["document_id"].ToString();
        
        return item;
    }

    public bool validateUnique(string login, int? excludeId = null) {
        string query = "SELECT * FROM " + tableName + " WHERE login = @search";

        var ds = getDataSet(query, login, excludeId);

        return ds.Tables[0].Rows.Count == 0; 
    }

    public List<UserItem> getUsers(string? search) {
        string query = "SELECT * FROM " + tableName;
        if(search != null) {
            query += " WHERE (login LIKE @search OR name LIKE @search)";
        }
        var ds = getDataSet(query, "%"+search+"%");
    
        List<UserItem> userItems = new List<UserItem>();
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            userItems.Add(getItemFromRow(row));
        }

        return userItems; 
    }

    public UserItem? getUserItem(int id)
    {
        DataRow? row = getRowById(id);

        return getItemFromRow(row);
    }

    public UserItem addUserItem(UserItem userItem)
    {
        userItem.Id = addRow(userItem.getAsDictionary());
        
        Console.WriteLine("added user");
        return userItem;
    }
    
    public UserItem updateUserItem(UserItem userItem, int id)
    {
        updateRow(userItem.getAsDictionary(), id);
        
        Console.WriteLine("updated user");
        return userItem;
    }

    public bool deleteUserItem(int id)
    {
        return deleteRow(id);
    }

}