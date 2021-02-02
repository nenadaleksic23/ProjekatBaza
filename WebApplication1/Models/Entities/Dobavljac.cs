using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebApplication1.Models.Base;

namespace WebApplication1.Models.Entities
{
    public class Dobavljac : Entity
    {
        [PrimaryKey]
        public int IDDobaljac { get; set; }
        public string Naziv { get; set; }
        public string Adresa { get; set; }
        public int BankaId { get; set; }
        public string Telefon { get; set; }
        public List<Banka> Banke { get; set; }
    }

    public class Response 
    {
        object ResposneData { get; set; }
        string ErrorMessage { get; set; }
        public bool IsSucess 
        {
            get => string.IsNullOrEmpty(ErrorMessage);
        }
    }

}