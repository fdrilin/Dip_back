using Microsoft.EntityFrameworkCore;
using System.Data;
using MySql.Data.MySqlClient;

namespace TodoApi.Repositories;

public class BaseRepository
{
    private MySqlConnection connect() 
    {
        return new MySqlConnection("server=localhost;database=booking;user id=root;password=0000");
    }

    protected DataSet getDataSet(string query)
    {
        MySqlConnection db = connect();
        MySqlCommand cmd = new MySqlCommand(query, db);

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        var ds = new DataSet();
        adapter.Fill(ds);
        return ds;
    }
}