using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models.Base;
using WebApplication1.Models.Entities;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Repository<Dobavljac> repo = new Repository<Dobavljac>();
            Repository<Banka> bankRepo = new Repository<Banka>();

            var list = repo.Read();
            bool isCompleted = repo.Insert(new Dobavljac()
            {
                Adresa = "Ljubicicina",
                Naziv = "Babasera",
                Telefon = "011-123123123"
            });

            bool isCompleted2 = repo.Update(new Dobavljac()
            {
                IDDobaljac = 1,
                Adresa = "Mirijevska",
                Naziv = "Babasera jos veca",
                Telefon = "011-123123123"
            });

            bool isCompleted3 = repo.Delete(2);

            Dobavljac obj = repo.ReadById(3);
            obj.Banke = bankRepo.Read();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}