using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models.Base;

namespace WebApplication1.Models.Entities
{
    public class Banka : Entity
    {
        [PrimaryKey]
        public int BankaId { get; set; }
        public string Naziv { get; set; }
    }
}