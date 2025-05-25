using System.Data;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class ResourceRepository : BaseRepository
{
    public ResourceRepository()
    {
        tableName = "resources";
    }

    private ResourceItem? getItemFromRow(DataRow? row)
    {
        if (row == null)
        {
            return null;
        }

        ResourceItem item = new();

        item.Id = Int32.Parse(row["id"].ToString() ?? "");
        item.ResourceTypeId = Int32.Parse(row["resource_type_id"].ToString() ?? "0");
        item.SerialNo = row["serial_no"].ToString();
        item.Available = Int32.Parse(row["available"].ToString());

        return item;
    }

    public bool validateUnique(string serial_No, int? excludeId = null)
    {
        string query = "SELECT * FROM " + tableName + " WHERE serial_no = @search";
        var ds = getDataSet(query, serial_No, excludeId);

        return ds.Tables[0].Rows.Count == 0;
    }

    public List<ResourceItem> getResources(string? search, bool isAdmin = false, int? resourceTypeId = null)
    {
        string query = "SELECT * FROM " + tableName;
        List<string> conditions = [];
        if (search != null)
        {
            conditions.Add("serial_no LIKE @search");
        }
        if (!isAdmin)
        {
            conditions.Add("available = 1");
        }
        if (resourceTypeId != null)
        {
            conditions.Add($"resource_type_id = {resourceTypeId}");
        }
        if (conditions.Count > 0)
        {
            query += " WHERE " + string.Join(" AND ", conditions);
        }
        Console.WriteLine(query);

        var ds = getDataSet(query, "%" + search + "%");

        List<ResourceItem> resourceItems = new List<ResourceItem>();
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            resourceItems.Add(getItemFromRow(row));
        }

        return resourceItems;
    }

    public ResourceItem? getResourceItem(int id)
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
