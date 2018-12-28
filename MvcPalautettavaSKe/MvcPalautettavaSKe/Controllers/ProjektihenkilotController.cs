using MvcPalautettavaSKe.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPalautettavaSKe.Controllers
{
    public class ProjektihenkilotController : Controller
    {
        // GET: Projektihenkilot
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList()
        {
            //Luo uuden entiteettiolion 
            ProjektitEntities entities = new ProjektitEntities();

            //Haetaan Projektihenkilot -taulusta kaikki data
            var henkilot = (from h in entities.Projektihenkilot
                            select h).ToList();

            //Muutetaan olio json -muotoon toimitettavaksi selaimelle. Suljetaan tietokantayhteys.
            string json = JsonConvert.SerializeObject(henkilot);
            entities.Dispose();

            //IE-selainta varten ohitetaan välimuisti, jotta näyttö päivittyy
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //Lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSinglePerson(string id)
        {
            //Haetaan tietokannasta "klikatun" henkilön tiedot

            //Luo uuden entiteettiolion 
            ProjektitEntities entities = new ProjektitEntities();

            int ID = int.Parse(id);

            //Haetaan Projektihenkilot -taulusta tiedot henkilö-Id:n perusteella
            var henkilo = (from h in entities.Projektihenkilot
                            where h.HenkiloId == ID
                            select h).FirstOrDefault();

            //Muutetaan olio json -muotoon toimitettavaksi selaimelle. Suljetaan tietokantayhteys.
            string json = JsonConvert.SerializeObject(henkilo);
            entities.Dispose();

            //IE - selainta varten ohitetaan välimuisti, jotta näyttö päivittyy
            //Response.Expires = -1;
            //Response.CacheControl = "no-cache";

            //Lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(Projektihenkilot person)
        {
            //onnistuuko tallennus tietokantaan vai ei?
            bool OK = false;

            //Tietokantaan tallennetaan uusia tietoja vain mikäli, 
            //Etunimi- , Sukunimi- ja Osoite -kentät eivät ole tyhjiä ja 
            //Esimies -kenttä sisältää int-tyyppisen arvon
            if (person.Etunimi != null && person.Sukunimi != null && person.Osoite != null && person.Esimies != null)
            {
                //luodaan uusi entiteettiolio
                ProjektitEntities entities = new ProjektitEntities();

                int id = person.HenkiloId;

                if (id == 0)
                {
                    // uuden henkilön lisääminen
                    Projektihenkilot dbItem = new Projektihenkilot()
                    {
                        Etunimi = person.Etunimi,
                        Sukunimi = person.Sukunimi,
                        Osoite = person.Osoite,
                        Esimies = person.Esimies
                    };

                    //lisätään dbItem-olioon ja tallennetaan muutokset tietokantaan
                    entities.Projektihenkilot.Add(dbItem);
                    entities.SaveChanges();
                    OK = true;
                }
                else
                {
                    //muokataan tietoja, haetaan olemassaolevat tiedot tietokannasta
                    Projektihenkilot dbItem = (from h in entities.Projektihenkilot
                                               where h.HenkiloId == id
                                               select h).FirstOrDefault();

                    //tallennetaan modaali-ikkunasta tulevat tiedot dbItem-olioon
                    if (dbItem != null)
                    {
                        dbItem.Etunimi = person.Etunimi;
                        dbItem.Sukunimi = person.Sukunimi;
                        dbItem.Osoite = person.Osoite;
                        dbItem.Esimies = person.Esimies;



                        //tallennetaan tiedot tietokantaan
                        entities.SaveChanges();
                        OK = true;
                    }
                }

                //suljetaan tietokantayhteys
                entities.Dispose();
            }

            //palautetaan tallennuskuittaus selaimelle (muuttuja OK)
            return Json(OK, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Delete(string id)
        {
            ProjektitEntities entities = new ProjektitEntities();
            
            //tallennuksen onnistuminen
            bool OK = false;

            int intID = int.Parse(id);

            //etsitään poistettavan henkilön tiedot dbItem-olioon ID:n perusteella
            Projektihenkilot dbItem = (from h in entities.Projektihenkilot
                                       where h.HenkiloId == intID
                                       select h).FirstOrDefault();

            if (dbItem != null)
            {
                //poistetaan tiedot entiteettimallista
                entities.Projektihenkilot.Remove(dbItem);

                //poistetaan tietokannasta ja tallennetaan tiedot
                entities.SaveChanges();
                OK = true;
            }
            entities.Dispose();

            return Json(OK, JsonRequestBehavior.AllowGet);
        }

    }
}