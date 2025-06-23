using System.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        item.Id = Int32.Parse(row["id"].ToString()?? "");
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

    public ResourceItem getAvailableResource(string beginDate, string endDate, int? resourceTypeId)
    {
        beginDate += " 00:00:00";
        string query = @$"SELECT * FROM (
            SELECT r.* FROM resources as r
            LEFT JOIN booking as b ON r.id = b.hw_resourse_id AND b.begin_date >= '{endDate}' AND '{beginDate}' >= b.end_date
            WHERE b.id is null AND r.available = 1 AND r.resource_type_id = '{resourceTypeId}'
            LIMIT 1) as t";
        return getItemFromRow(getDataSet(query).Tables[0].Rows[0]);
    }

    public bool isAvailableSpecific(string beginDate, string endDate, int? bookingId, int? resourceId)
    {
        string query = @$"SELECT * FROM (
            SELECT b.* FROM resources as r
            LEFT JOIN booking as b ON r.id = b.hw_resourse_id AND b.begin_date >= '{endDate}' AND '{beginDate}' >= b.end_date AND b.id != '{bookingId}'
            WHERE b.id is null AND r.available = 1 AND r.id = '{resourceId}'
            LIMIT 1) as t";
        Console.WriteLine(query);
        return getDataSet(query).Tables[0].Rows.Count > 0;
    }

    public List<ResourceItem> getResources(string? search, bool isAdmin = false, int? resourceTypeId = null)
    {
        string query = "SELECT * FROM " + tableName;
        Console.WriteLine(search);
        List<string> conditions = [];
        if (search != null)
        {
            conditions.Add($"id = '{search}' OR serial_no LIKE @search");
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

    public ResourceItem? getResourceByType(int? resourceTypeId)
    {
        if (resourceTypeId == null) return null;
        string query = @$"SELECT r.*
            FROM resources as r 
            LEFT JOIN booking as b on b.hw_resourse_id = r.id AND end_date >= current_date() AND b.canceled = 0 AND b.returned = 0
            WHERE r.resource_type_id = {resourceTypeId} AND r.available = 1 AND b.hw_resourse_id is null
            LIMIT 1";
        Console.WriteLine(query);

        var ds = getDataSet(query);

        var dsr = ds.Tables[0].Rows;
        if (dsr.Count == 0)
        {
            return null;
        }

        return getItemFromRow(dsr[0]);
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
