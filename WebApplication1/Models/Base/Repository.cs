using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc.Ajax;
using System.Web.Script.Serialization;
using WebApplication1.Models.Entities;

namespace WebApplication1.Models.Base
{
    public class Repository<T>
        where T : Entity
    {
        private string _entityName;
        private string _primaryKeyName;
        private IEnumerable<PropertyInfo> _properties;
        private const string _connectionString = "Persist Security Info=False;Integrated Security = true; Initial Catalog = Baza; server=(local)";

        public Repository()
        {
            LoadEntityEssentials();
        }
        public virtual List<T> Read()
        {
            try
            {
                string query = $"SELECT * FROM dbo.{_entityName}";
                var json = ConvertDataTabletoString(query);
                
                var val = JsonConvert.DeserializeObject<List<T>>(json);
                return val;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public virtual void Update(T entity)
        {

        }
        public virtual void Insert(T entity)
        {

        }
        public virtual void Delete(T entity)
        {

        }

        private void LoadEntityEssentials()
        {
            _entityName = typeof(T).Name;

            _properties = typeof(T).GetProperties();

            foreach (var prop in _properties)
            {
                var attr = prop.GetCustomAttributes(typeof(PrimaryKey), false);
                if (attr?.Length > 0)
                {
                    //ovo znaci da je ovo primarni kljuc jer svakom primarnom kljucu dodeljujemo primary key
                    _primaryKeyName = prop.Name;
                    return;
                }
            }

        }
        private string ConvertDataTabletoString(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    return serializer.Serialize(rows);
                }
            }
        }
    }
}