using System.Data;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class ResourceTypeRepository : BaseRepository
{
    public ResourceTypeRepository()
    {
        tableName = "resource_types";
    }

    private ResourceTypeItem? getItemFromRow(DataRow? row)
    {
        if (row == null)
        {
            return null;
        }

        ResourceTypeItem item = new();

        item.Id = Int32.Parse(row["id"].ToString() ?? "");
        item.Title = row["title"].ToString();
        item.Description = row["description"].ToString();
        item.Software = row["software"].ToString();
        item.Tags = row["tags"].ToString();
        item.Available = Int32.Parse(row["available"].ToString() ?? "");

        return item;
    }

    public bool validateUnique(string title, int? excludeId = null)
    {
        string query = "SELECT * FROM " + tableName + " WHERE title = @search";
        var ds = getDataSet(query, title, excludeId);

        return ds.Tables[0].Rows.Count == 0;
    }

    public List<ResourceTypeItem> getResourceTypes(string? search, string? tags, bool isAdmin = false)
    {
        string query = "SELECT *, (SELECT COUNT(*) FROM resources WHERE resource_type_id = resource_types.id and available = 1) as available FROM " + tableName;
        List<string> conditions = [];
        if (search != null)
        {
            conditions.Add("(title LIKE @search OR description LIKE @search)");
        }
        if (!string.IsNullOrEmpty(tags))
        {
            string[] tagValues = tags.Split(",");
            List<string> tagsConditions = [];
            foreach (var tag in tagValues)
            {
                tagsConditions.Add($"tags LIKE '%{tag}%'");
            }
            conditions.Add("(" + string.Join(" OR ", tagsConditions) + ")");
        }
        if (conditions.Count > 0)
        {
            query += " WHERE " + string.Join(" AND ", conditions);
        }
        Console.WriteLine(query);

        var ds = getDataSet(query, "%" + search + "%");

        List<ResourceTypeItem> items = new List<ResourceTypeItem>();
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            items.Add(getItemFromRow(row));
        }

        return items;
    }

    public ResourceTypeItem? getResourceTypeItem(int id)
    {
        DataRow? row = getRowById(id);

        return getItemFromRow(row);
    }

    public ResourceTypeItem addResourceTypeItem(ResourceTypeItem item)
    {
        item.Id = addRow(item.getAsDictionary());

        Console.WriteLine("added resource");
        return item;
    }

    public ResourceTypeItem updateResourceTypeItem(ResourceTypeItem item, int id)
    {
        updateRow(item.getAsDictionary(), id);

        Console.WriteLine("updated resource");
        return item;
    }

    public bool deleteResourceTypeItem(int id)
    {
        return deleteRow(id);
    }
}
