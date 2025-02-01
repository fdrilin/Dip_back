using System.Data;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class ResourceRepository : BaseRepository
{
    public ResourceRepository() {
        tableName = "hw_resourses";
    }

    private ResourceItem getItemFromRow(DataRow? row) 
    {
        ResourceItem item = new();
        if(row != null)
        {
            item.Id = Int32.Parse(row["id"].ToString() ?? "");
            item.Title = row["title"].ToString();
            item.Description = row["description"].ToString();
        }
        
        return item;
    }

    public List<ResourceItem> getResources() {
        string query = "SELECT * FROM " + tableName;
        var ds = getDataSet(query);
    
        List<ResourceItem> resourceItems = new List<ResourceItem>();
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            resourceItems.Add(getItemFromRow(row));
        }

        return resourceItems; 
    }

    public ResourceItem getResourceItem(int id)
    {
        DataRow? row = getRowById(id);

        return getItemFromRow(row);
    }

    public ResourceItem addResourceItem(ResourceItem resourceItem)
    {
        resourceItem.Id = addRow(resourceItem.getAsDictionary());
        
        Console.WriteLine("added resource");
        return resourceItem;
    }
    
    public ResourceItem updateResourceItem(ResourceItem resourceItem, int id)
    {
        updateRow(resourceItem.getAsDictionary(), id);
        
        Console.WriteLine("updated resource");
        return resourceItem;
    }

    public bool deleteResourceItem(int id)
    {
        return deleteRow(id);
    }

}