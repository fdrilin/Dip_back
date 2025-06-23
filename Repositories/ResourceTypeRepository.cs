using System.Data;
using Google.Protobuf.WellKnownTypes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        if (row.Table.Columns.Contains("available")) {
            item.Available = Int32.Parse(row["available"].ToString() ?? "");
        }
        item.PictureLink = row["picture_link"].ToString();

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
        string queryWhere = "";
        if (!isAdmin)
        {
            //queryWhere = "WHERE r.available = 1 AND b.hw_resourse_id is null";
        }
        string query = $@"
            SELECT rt.*, available FROM 
            (
                SELECT resource_type_id, count(*) as available #sum(case WHEN r.available = 1 AND b.hw_resourse_id is null then 1 else 0 end) as available 
                FROM resources as r 
                #LEFT JOIN booking as b on b.hw_resourse_id = r.id AND end_date >= current_date() AND b.canceled = 0 AND b.returned = 0 
                {queryWhere} 
                GROUP BY r.resource_type_id
            ) as rtAv 
            JOIN resource_types as rt on rt.id = rtAv.resource_type_id ";
        List<string> conditions = [];
        if (search != null)
        {
            conditions.Add($"(id = '{search}' OR title LIKE @search OR description LIKE @search OR software LIKE @search)");
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
        query = $"SELECT * FROM ({query}) as t";
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
        ResourceTypeItem oldItem = getResourceTypeItem(id);
        
        item.PictureLink = oldItem.PictureLink;
        updateRow(item.getAsDictionary(), id);

        Console.WriteLine("updated resource");
        return item;
    }

    public bool deleteResourceTypeItem(int id)
    {
        return deleteRow(id);
    }
}
