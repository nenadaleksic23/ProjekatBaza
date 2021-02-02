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
            string query = $"SELECT * FROM dbo.{_entityName}";
            return ReadFromDatabase(query);
        }
        public virtual T ReadById(int entityId)
        {
            string query = $"SELECT * FROM dbo.{_entityName} where {_primaryKeyName} = {entityId}";
            return ReadFromDatabase(query)?.FirstOrDefault();
        }

        public virtual bool Update(T entity)
        {
            var query = GenerateUpdateQuery(entity);
            return ExecuteNonParametrizedQuery(query);
        }

        public virtual bool Insert(T entity)
        {
            var query = GenerateInsertQuery(entity);
            return ExecuteNonParametrizedQuery(query);

        }
        public virtual bool Delete(int primaryKeyId)
        {
            string query = $"DELETE FROM dbo.{_entityName} WHERE {_primaryKeyName} = {primaryKeyId}";
            return ExecuteNonParametrizedQuery(query);
        }

        public  List<T> ReadFromDatabase(string query)
        {
            var json = ConvertDataTabletoJsonString(query);
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        protected bool ExecuteNonParametrizedQuery(string query)
        {
            try
            {
                using (var sc = new SqlConnection(_connectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception e)
            {
            }
            return false;
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
        private string ConvertDataTabletoJsonString(string query)
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
        private string GenerateInsertQuery(T entity)
        {
            string result = $"Insert into {_entityName}(";
            var nonPrimaryProps = _properties.Where(m => m.Name != _primaryKeyName);

            foreach (var item in nonPrimaryProps)
            {
                result += $"[{item.Name}],";
            }

            result = result.Remove(result.Length - 1, 1);
            result += ") values (";

            foreach (var item in nonPrimaryProps)
            {
                var value = typeof(T).GetProperty(item.Name).GetValue(entity as T).ToString();
                result += $"'{value}',";
            }
            result = result.Remove(result.Length - 1, 1);
            result += ")";

            return result;
        }
        private string GenerateUpdateQuery(T entity)
        {
            string query = $"UPDATE dbo.{_entityName} SET ";
            var nonPrimaryProps = _properties.Where(m => m.Name != _primaryKeyName);

            foreach (var item in nonPrimaryProps)
            {
                var value = typeof(T).GetProperty(item.Name).GetValue(entity);
                if (value != null)
                {
                    query += $"{item.Name} = '{value}'";
                }
                else
                {
                    query += $"{item.Name} = null";
                }
                query += ",";
            }
            query = query.Remove(query.Length - 1, 1);

            var primaryKeyValue = typeof(T).GetProperty(_primaryKeyName).GetValue(entity);
            query += $"WHERE {_primaryKeyName} = '{primaryKeyValue}'";
            return query;
        }
    }
}