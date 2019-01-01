using MvcPalautettavaSKe.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPalautettavaSKe.Controllers
{
    public class ProjektitController : Controller
    {
        // GET: Projektit
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList()
        {
            //luodaan uusi entiteettiolio
            Projektit2Entities entities = new Projektit2Entities();

            //haetaan Projektit-taulusta kaikki data
            var projektit = (from p in entities.Projektit
                             select p).ToList();

            //muutetaan data json-muotoon toimitettavaksi selaimelle
            //ja suljetaan tietokantayhteys.
            string json = JsonConvert.SerializeObject(projektit);
            entities.Dispose();

            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten)
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSingleProject(string id)
        {
            // haetaan tietokannasta klikatun projektin tiedot

            //luodaan uusi entiteettiolio
            Projektit2Entities entities = new Projektit2Entities();

            //muutetaan modaali-ikkunasta tullut string-tyyppinen projekti-id int-tyyppiseksi
            int ID = int.Parse(id);

            //haetaan Projektit-taulusta tiedot Projekti-Id:n perusteella
            var projekti = (from p in entities.Projektit
                            where p.ProjektiId == ID
                            select p).FirstOrDefault();

            //muutetaan olio json-muotoon toimitettavaksi selaimelle ja suljetaan tietokantayhteys
            string json = JsonConvert.SerializeObject(projekti);
            entities.Dispose();

            //lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(Projektit project)
        {
            //onnistuuko tallennus tietokantaa vai ei?
            bool OK = false;

            //Uuden projektin lisääminen
            //Uusia tietoja lisätään vain, mikäli Nimi-kenttä ei ole tyhjä
            if (project.Nimi != null)
            {
                //luodaan uusi entiteettiolio
                Projektit2Entities entities = new Projektit2Entities();

                int id = project.ProjektiId;

                if (id == 0)
                {
                    // uuden projektin lisääminen dbItem-olion avulla
                    Projektit dbItem = new Projektit()
                    {
                        Nimi = project.Nimi
                    };

                    //lisätään dbItem-olion sisältämät tiedot ja tallennetaan muutokset tietokantaan
                    entities.Projektit.Add(dbItem);
                    entities.SaveChanges();
                    OK = true;
                }
                else
                {
                    //muokataan olemassaolevia tietoa
                    //haetaan tiedot tietokannasta
                    Projektit dbItem = (from p in entities.Projektit
                                        where p.ProjektiId == id
                                        select p).FirstOrDefault();

                    //tallennetaan modaali-ikkunasta tulevat tiedot dbItem-olioon
                    if (dbItem != null)
                    {
                        dbItem.Nimi = project.Nimi;

                        //tallennetaan uudet tiedot tietokantaa
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
            bool ok = false;

            //muutetaan selaimelta tullut string-tyyppinen projekti-id int-tyyppiseksi
            int intID = int.Parse(id);

            //haetaan poistettavan projektin tiedot id:n perusteella dbItem-olioon
            Projektit dbItem = (from p in entities.Projektit
                                where p.ProjektiId == intID
                                select p).FirstOrDefault();

            //jos tiedot löytyy
            if (dbItem != null)
            {
                //poistetaan tiedot entiteettimallista
                entities.Projektit.Remove(dbItem);

                //tallennetaan muutokset tietokantaan
                entities.SaveChanges();
                ok = true;
            }

            //suljetaan tietokantayhteys
            entities.Dispose();

            //palautetaan tallennuskuittaus selaimelle
            return Json(ok, JsonRequestBehavior.AllowGet);
        }
    }
}