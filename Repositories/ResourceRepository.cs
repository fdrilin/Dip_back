using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class ResourceRepository : BaseRepository
{
    public List<ResourceItem> getResources() {
        string query = "SELECT * FROM hw_resourses";
        var ds = getDataSet(query);
    
        List<ResourceItem> resourceItems = new List<ResourceItem>();
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            ResourceItem item = new ResourceItem();

            var row = ds.Tables[0].Rows[i];

            item.Id = Int32.Parse(row["id"].ToString());
            item.Title = row["title"].ToString();
            item.Description = row["description"].ToString();

            resourceItems.Add(item);
        }

        return resourceItems; 
    }
}