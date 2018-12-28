using MvcPalautettavaSKe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MvcPalautettavaSKe.Controllers
{
    public class HenkiloidenProjektitController : Controller
    {
        // GET: HenkiloidenProjektit
        public ActionResult Index()
        {
            Projektit2Entities entities = new Projektit2Entities();
            try
            {
                //haetaan tietokannasta kaikki henkilötiedot listaan
                List<Projektihenkilot> model = entities.Projektihenkilot.ToList();
                return View(model);
            }
            finally
            {
                //suljetaan tietokantayhteys
                entities.Dispose();
            }
            
        }
        public ActionResult GetProjectData(string id)
        {
            Projektit2Entities entities = new Projektit2Entities();
            try
            {
                int ID = int.Parse(id);

                //haetaan tietokannasta projekti, joille henkilö tehnyt tunteja
                List<int> projektiIDt = (from t in entities.Tunnit
                                         where t.HenkiloId == ID
                                         select t.ProjektiId).Distinct().ToList();


                //lopullinen lista, johon kerätään tiedot
                List<ProjectHour> projectHours = new List<ProjectHour>();

                //tähän muuttujaan kerätään foreach-silmukassa tarvittavat kolme tietoa (alkio-muuttuja)
                ProjectHour projectHour;

                decimal summa;
                string nimi;

                foreach (int proID in projektiIDt)
                {
                    projectHour = new ProjectHour();
                    summa = (from t in entities.Tunnit
                             where t.HenkiloId == ID
                             && t.ProjektiId == proID
                             select t.Tunnit1).Sum();

                    nimi = (from p in entities.Projektit
                            where p.ProjektiId == proID
                            select p.Nimi).FirstOrDefault();

                    projectHour.ProjectID = proID;
                    projectHour.ProjectName = nimi;
                    projectHour.Hours = summa;
                    
                    //edellä rakennettu lista-alkio lisätään listaan projectHours
                    projectHours.Add(projectHour);
                }

                //rakennetaan taulukko
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<td colspan = \"3\">" +
                    "<table class=\"table table-striped\">");

                foreach (ProjectHour ph in projectHours)
                {
                    sb.AppendLine("<tr><td>" +
                        ph.ProjectID + "</td>" +
                        "<td>" + ph.ProjectName + "</td>" +
                        "<td>" + ph.Hours + "</td></tr>");
                }
                sb.AppendLine("</table></td>");

                //siirretään taulukko json-muotoisena selaimelle
                var jsonData = new { html = sb.ToString() };
                return Json(jsonData,JsonRequestBehavior.AllowGet);
            }
            finally
            {
                //suljetaan tietokantayhteys
                entities.Dispose();
            }
        }
    }

    //tämä luokka on lista-alkion tyyppi
    public class ProjectHour
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public decimal Hours { get; set; }
    }
}