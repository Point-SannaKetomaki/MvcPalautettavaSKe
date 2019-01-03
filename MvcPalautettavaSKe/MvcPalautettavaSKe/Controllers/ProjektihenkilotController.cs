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
            //Luodaan uusi entiteettiolio 
            Projektit2Entities entities = new Projektit2Entities();

            //Haetaan Projektihenkilot -taulusta kaikki data
            var henkilot = (from h in entities.Projektihenkilot
                            select h).ToList();

            //Muutetaan data json -muotoon toimitettavaksi selaimelle. Suljetaan tietokantayhteys.
            string json = JsonConvert.SerializeObject(henkilot);
            entities.Dispose();

            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten) 
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //Lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSinglePerson(string id)
        {
            //Haetaan tietokannasta "klikatun" henkilön tiedot

            //Luodaan uusi entiteettiolio
            Projektit2Entities entities = new Projektit2Entities();

            //Muutetaan modaali-ikkunasta tullut string-tyyppinen henkilö-id int-tyyppiseksi
            int ID = int.Parse(id);

            //Haetaan Projektihenkilot -taulusta tiedot henkilö-Id:n perusteella
            var henkilo = (from h in entities.Projektihenkilot
                            where h.HenkiloId == ID
                            select h).FirstOrDefault();

            //Muutetaan olio json -muotoon toimitettavaksi selaimelle. Suljetaan tietokantayhteys.
            string json = JsonConvert.SerializeObject(henkilo);
            entities.Dispose();

            //Lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(Projektihenkilot person)
        {
            //TIETOJEN PÄIVITYS JA UUDEN HENKILÖN LISÄYS

            //onnistuuko tallennus tietokantaan vai ei?
            bool OK = false;

            //Tietokantaan tallennetaan uusia tietoja vain mikäli, 
            //Etunimi- , Sukunimi- ja Osoite -kentät eivät ole tyhjiä
            //ja Esimies -kenttä sisältää int-tyyppisen arvon
            if (!string.IsNullOrWhiteSpace(person.Etunimi) && 
                !string.IsNullOrWhiteSpace(person.Sukunimi) && 
                !string.IsNullOrWhiteSpace(person.Osoite) && 
                person.Esimies > 0)
            {
                //luodaan uusi entiteettiolio
                Projektit2Entities entities = new Projektit2Entities();

                int id = person.HenkiloId;
            
                if (id == 0)
                {
                    // uuden henkilön lisääminen dbItem-nimisen olion avulla
                    Projektihenkilot dbItem = new Projektihenkilot()
                    {
                        Etunimi = person.Etunimi,
                        Sukunimi = person.Sukunimi,
                        Osoite = person.Osoite,
                        Esimies = person.Esimies
                    };

                    //lisätään dbItem-olion tiedot ja tallennetaan muutokset tietokantaan
                    entities.Projektihenkilot.Add(dbItem);
                    entities.SaveChanges();
                    OK = true;
                }
                else
                {
                    //muokataan olemassaolevia tietoja 
                    //haetaan olemassaolevat tiedot tietokannasta
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

                        //tallennetaan uudet tiedot tietokantaan
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
            //luodaan uusi entiteettiolio
            Projektit2Entities entities = new Projektit2Entities();
            
            //tallennuksen onnistuminen
            bool OK = false;

            //Muutetaan selaimelta tullut string-tyyppinen henkilö-id (id) int-tyyppiseksi
            int intID = int.Parse(id);

            //haetaan poistettavan henkilön tiedot dbItem-olioon ID:n perusteella
            Projektihenkilot dbItem = (from h in entities.Projektihenkilot
                                       where h.HenkiloId == intID
                                       select h).FirstOrDefault();

            //jos tiedot löytyy
            if (dbItem != null)
            {
                //poistetaan tiedot entiteettimallista
                entities.Projektihenkilot.Remove(dbItem);

                //tallennetaan muutokset tietokantaan
                entities.SaveChanges();
                OK = true;
            }

            //suljetaan tietokantayhteys
            entities.Dispose();

            //palautetaan tallennuskuittaus selaimelle
            return Json(OK, JsonRequestBehavior.AllowGet);
        }
    }
}