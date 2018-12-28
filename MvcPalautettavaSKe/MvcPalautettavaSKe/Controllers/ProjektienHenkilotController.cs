using MvcPalautettavaSKe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MvcPalautettavaSKe.Controllers
{
    public class ProjektienHenkilotController : Controller
    {
        // GET: ProjektienHenkilot
        public ActionResult Index()
        {
            Projektit2Entities entities = new Projektit2Entities();
            try
            {
                //listaus projekteista
                List<Projektit> projektit = entities.Projektit.ToList();
                return View(projektit);
            }
            finally
            {
                //suljetaan tietokantayhteys
                entities.Dispose();
            }

        }

        public ActionResult GetPersonHoursData(string id)
        {
            Projektit2Entities entities = new Projektit2Entities();
            try
            {
                int ID = int.Parse(id);

                // haetaan projekti-ID:n perusteella tiedot henkilöistä ja heidän tunneistaan
                List<int> henkiloIDt = (from t in entities.Tunnit
                                        where t.ProjektiId == ID
                                        select t.HenkiloId).Distinct().ToList();


                //PersonHour-lista
                List<PersonHour> personHours = new List<PersonHour>();

                //lista-alkiomuuttuja
                PersonHour personHour;

                decimal summa; //tuntisumma
                //var nimi = "";  //henkilön nimi

                foreach (int hloID in henkiloIDt)
                {
                    personHour = new PersonHour();

                    summa = (from t in entities.Tunnit
                             where t.ProjektiId == ID
                             && t.HenkiloId == hloID
                             select t.Tunnit1).Sum();

                    var nimi = (from h in entities.Projektihenkilot
                            where h.HenkiloId == hloID
                            select new
                            {
                                etunimi = h.Etunimi,
                                sukunimi = h.Sukunimi
                            }).FirstOrDefault();

                    personHour.PersonID = hloID;
                    personHour.Name = nimi.etunimi + " " + nimi.sukunimi;
                    personHour.Hours = summa;

                    personHours.Add(personHour);
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<td colspan = \"2\">" +
                    "<table class = \"table table-striped\">");

                foreach (PersonHour prsh in personHours)
                {
                    sb.AppendLine("<tr><td>" +
                        prsh.PersonID + "</td>" +
                        "<td>" + prsh.Name + "</td>" +
                        "<td>" + prsh.Hours + "</td></tr>");
                }
                sb.AppendLine("</table></td>");

                var jsonData = new { html = sb.ToString()};
                return Json(jsonData,JsonRequestBehavior.AllowGet);
            }
            finally
            {
                //suljetaan tietokantayhteys
                entities.Dispose();
            }

            
        }
    }

    public class PersonHour
    {
        public int PersonID { get; set; }
        public string Name { get; set; }
        public decimal Hours { get; set; }
    }
}