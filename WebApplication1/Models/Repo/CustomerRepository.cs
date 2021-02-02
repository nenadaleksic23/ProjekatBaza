using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models.Base;
using WebApplication1.Models.Entities;

namespace WebApplication1.Models.Repo
{
    public class CustomerRepository : Repository<Dobavljac>
    {
        public override bool Insert(Dobavljac entity)
        {
            bool result;
            string query1 = "insert into datum (vreme,minuti,sekunde)";
            result = ExecuteNonParametrizedQuery(query1);
            string query2 = "insert into stagod...";
            result = ExecuteNonParametrizedQuery(query2);

            if (result)
            {
                //jeeeeeeee
            }
            return result;
        }
    }
}