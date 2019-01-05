using MvcPalautettavaSKe.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPalautettavaSKe.Controllers
{
    public class TunnitController : Controller
    {
        // GET: Tunnit
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList()
        {
            //luodaan uusi entiteettiolio
            Projektit2Entities entities = new Projektit2Entities();

            //haetaan Tunnit-taulusta kaikki data
            var tunnit = (from t in entities.Tunnit
                          select t).ToList();

            // muutetaan data json-muotoon toimitettavaksi selaimelle ja suljetaan tietokantayhteys
            string json = JsonConvert.SerializeObject(tunnit);
            entities.Dispose();

            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten)
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSingleHours(string id)
        {
            //Haetaan klikatun tuntikirjauksen tiedot tietokannasta

            //luodaan uusi entiteettiolio
            Projektit2Entities entities = new Projektit2Entities();

            //muutetaan modaali-ikkunasta tullut string-tyyppinen tunti-id int-tyyppiseksi
            int intID = int.Parse(id);

            //haetaan tietokannasta Tunnit-taulusta tuntikirjauksen tiedot tunti-id:n perusteella
            var tunti = (from t in entities.Tunnit
                         where t.TuntiId == intID
                         select t).FirstOrDefault();

            //muutetaan olio json-muotoon selaimelle toimittamista varten ja suljetaan tietokantayhteys
            string json = JsonConvert.SerializeObject(tunti);
            entities.Dispose();

            //palautetaan tiedot selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(Tunnit hour)
        {
            //PÄIVITETÄÄN TIETOJA TAI LISÄTÄÄN UUSIA TIETOJA

            //onnistuuko tallennus?
            bool ok = false;

            //Tietokantaan tallennetaan tietoja vain, mikäli
            //Projekti-Id ja Henkilö-Id ovat int-tyyppisiä
            //Päivämäärä ja Tunnit -kentät eivät ole tyhjiä
            if (hour.ProjektiId > 0 && 
                hour.HenkiloId > 0 && 
                hour.Päivämäärä != null && 
                hour.Tunnit1 > 0)
            {
                //Luodaan uusi entiteettiolio
                Projektit2Entities entities = new Projektit2Entities();

                int hourId = hour.TuntiId;

                if (hourId == 0)
                {
                    //uusien tietojan lisääminen

                    int projId = hour.ProjektiId;
                    int personId = hour.HenkiloId;

                    try
                    {
                        //tarkistetaan, että käyttäjän syöttämä projekti-id ja henkilö-id ovat olemassa ao. tietokantatauluista
                        var projekti = (from p in entities.Projektit
                                        where p.ProjektiId == projId
                                        select p).First();

                        var henkilo = (from h in entities.Projektihenkilot
                                       where h.HenkiloId == personId
                                       select h).First();

                        //uuden tiedon lisääminen tietokantaan dbItem-olion avulla
                        Tunnit dbItem = new Tunnit()
                        {
                            ProjektiId = hour.ProjektiId,
                            HenkiloId = hour.HenkiloId,
                            Päivämäärä = hour.Päivämäärä,
                            Tunnit1 = hour.Tunnit1
                        };

                        //lisätään dbItem-olion tiedot ja tallennetaan muutokset tietokantaan
                        entities.Tunnit.Add(dbItem);
                        entities.SaveChanges();
                        ok = true;
                    }

                    catch
                    {
                        //Poikkeusta ei käsitellä, mutta ohjelma ei kaadu.
                        //ok jää arvoon false eikä tietoja tallenneta tietokantaan.
                        //Käyttäjä saa virheilmoituksen, ettei tietoja tallennettu.
                    }
                }
                else
                {
                    //päivitetään olemassa olevia tietoja

                    //haetaan olemassaolevat tiedot tietokannasta
                    Tunnit dbItem = (from t in entities.Tunnit
                                     where t.TuntiId == hourId
                                     select t).FirstOrDefault();

                    //tallennetaan modaali-ikkunasta tulevat tiedot dbItem-olioon
                    if (dbItem != null)
                    {
                        dbItem.ProjektiId = hour.ProjektiId;
                        dbItem.HenkiloId = hour.HenkiloId;
                        dbItem.Päivämäärä = hour.Päivämäärä;
                        dbItem.Tunnit1 = hour.Tunnit1;

                        //tallennetaan uudet tiedot tietokantaan
                        entities.SaveChanges();
                        ok = true;
                    }
                }

                //suljetaan tietokantayhteys
                entities.Dispose();
            }

            //palautetaan tallennuskuittaus selaimelle
            return Json(ok, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Delete(string id)
        {
            //luodaan uusi entiteettiolio
            Projektit2Entities entities = new Projektit2Entities();

            //tallennuksen onnistuminen
            bool ok = false;

            //muutetaan selaimelta tullut string-tyyppinen tuntikirjaus-id int-tyyppiseksi
            int intID = int.Parse(id);

            //haetaan poistettavan kirjauksen tiedot dbItem-olioon id:n perusteella
            Tunnit dbItem = (from t in entities.Tunnit
                             where t.TuntiId == intID
                             select t).FirstOrDefault();

            //jos tiedot löytyy
            if (dbItem != null)
            {
                //poistetaan tiedot entiteettimallista
                entities.Tunnit.Remove(dbItem);

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