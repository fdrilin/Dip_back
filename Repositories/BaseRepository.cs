using Microsoft.EntityFrameworkCore;
using System.Data;
using MySql.Data.MySqlClient;

namespace TodoApi.Repositories;

public class BaseRepository
{
    MySqlConnection? connection = null;
    protected string tableName = null;

    private MySqlConnection connect() 
    {
        if (connection == null) {
            connection = new MySqlConnection("server=localhost;database=booking;user id=root;password=0000");
            connection.Open();
        }

        return connection;
    }

    protected DataSet getDataSet(string query, string? search = null, int? excludeId = null)
    {
        if (excludeId != null) 
        {
            query += " AND id != @id";
        }

        connect();
        MySqlCommand cmd = new MySqlCommand(query, connection);
        if (search != null) {
            cmd.Parameters.AddWithValue("@search", search);
        }

        if (excludeId != null) {
            cmd.Parameters.AddWithValue("@id", excludeId);
        }

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        var ds = new DataSet();
        adapter.Fill(ds);

        return ds;
    }

    protected DataRow? getRowById(int id) 
    {
        connect();
        string query = $"SELECT * FROM {tableName} WHERE id = @id"; 
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", id);

        MySqlDataAdapter adapter = new(cmd);
        var ds = new DataSet();
        adapter.Fill(ds);

        return ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0] : null;
    }

    protected int addRow(Dictionary<string, string> data) 
    {
        connect();
        string fields = "";
        string values = "";

        foreach(KeyValuePair<string, string> item in data) {
            fields += (fields == ""?"": ", ") + item.Key ;
            values += (values == ""?"": ", ") + "@" + item.Key;
        }

        string query = "INSERT INTO " + tableName + "(" + fields + ") VALUES(" + values + ")";

        Console.WriteLine(query);

        MySqlCommand cmd = new MySqlCommand(query, connection);
        foreach(KeyValuePair<string, string> item in data) {
            cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
        }
        foreach (MySqlParameter parameter in cmd.Parameters) {
            Console.WriteLine(parameter.Value);
        }
        cmd.ExecuteNonQuery();

        MySqlCommand lastId = new MySqlCommand("SELECT LAST_INSERT_ID();", connection);

        return Int32.Parse(lastId.ExecuteScalar().ToString());
    }

    protected bool updateRow(Dictionary<string, string> data, int id) 
    {
        connect();
        string sets = "";
        
        foreach(KeyValuePair<string, string> item in data) {
            sets += (sets == ""?"": ", ") + item.Key + " = @" + item.Key;
        }

        string query = "UPDATE " + tableName + " SET " + sets + " WHERE id = @id";

        Console.WriteLine(query);

        MySqlCommand cmd = new MySqlCommand(query, connection);
        foreach(KeyValuePair<string, string> item in data) {
            cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
        }
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();

        return true;
    }

    protected bool deleteRow(int id) 
    {
        connect();
        var query = "DELETE FROM " + tableName + " WHERE id = @id";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();

        return true;
    }
}