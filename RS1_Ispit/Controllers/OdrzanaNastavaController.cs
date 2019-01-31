using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RS1_Ispit_asp.net_core.EF;
using RS1_Ispit_asp.net_core.EntityModels;
using RS1_Ispit_asp.net_core.ViewModels;

namespace RS1_Ispit_asp.net_core.Controllers
{
    public class OdrzanaNastavaController : Controller
    {
        MojContext context;
        public OdrzanaNastavaController(MojContext mojContext)
        {
            context = mojContext;
        }
        public IActionResult Index()
        {
            List<SkolaNastavniciVM> model = context.Nastavnik.Select(x => new SkolaNastavniciVM
            {
                NastavnikID = x.Id,
                Nastavnik = x.Ime + " " + x.Prezime,
                Skola = context.Odjeljenje.Include(b => b.Skola).FirstOrDefault(g => g.RazrednikID == x.Id).Skola.Naziv

            }).ToList();
            return View(model);
        }
        public IActionResult Odaberi(int id)
        {

            MaturskiIspitVM model = new MaturskiIspitVM
            {
                NastavnikId=id,
                Rows=context.MaturskiIspit.Include(h=>h.Skola).Include(j=>j.Predmet).Where(x=>x.NastavnikId==id).Select(b=>new MaturskiIspitVM.Row
                {
                    MaturskiIspitId=b.Id,
                    Datum=b.Datum.ToString("dd.MM.yyyy"),
                    Skola=b.Skola.Naziv,
                    Predmet=b.Predmet.Naziv,
                    NisuPristupili=context.MaturskiIspitStavka.Include(n=>n.Ucenik)
                    .Where(g=>g.MaturskiIspitId==b.Id && g.PristupioIspitu==false).Select(f=>f.Ucenik.ImePrezime).ToList()

                }).ToList()
                
            };

            return View(model);
        }
        public IActionResult Dodaj(int id)
        {

            MaturskiIspitDodajVM model = new MaturskiIspitDodajVM
            {
                Skole = context.Skola.Select(g => new SelectListItem
                {
                    Value=g.Id.ToString(),
                    Text=g.Naziv

                }).ToList(),
                NastavnikId=id,
                Nastavnik=context.Nastavnik.Where(x=>x.Id==id).FirstOrDefault().Ime+" "+
                context.Nastavnik.Where(x => x.Id == id).FirstOrDefault().Prezime,
                SkolskaGodina=context.Odjeljenje.Include(b=>b.SkolskaGodina).Where(x=>x.RazrednikID==id)
                .FirstOrDefault().SkolskaGodina.Aktuelna? context.Odjeljenje.Where(x => x.RazrednikID == id)
                .FirstOrDefault().SkolskaGodina.Naziv:(DateTime.Now.Year +" / " +(DateTime.Now.Year+1)).ToString(),
                Predmet=context.PredajePredmet.Include(j=>j.Predmet).Where(n=>n.NastavnikID==id).Select(d=>new SelectListItem
                {
                    Value=d.Predmet.Id.ToString(),
                    Text=d.Predmet.Naziv
                }).ToList(),

            };
            return View(model);
        }

        public IActionResult Snimi(MaturskiIspitDodajVM maturskiIspitDodajVM)
        {
            MaturskiIspit maturskiIspit = new MaturskiIspit
            {
                NastavnikId=maturskiIspitDodajVM.NastavnikId,
                SkolaId=maturskiIspitDodajVM.OdabranaSkola,
                PredmetId=maturskiIspitDodajVM.OdabraniPredmet,
                Napomena="",
                Datum=maturskiIspitDodajVM.DatumIspita

            };
            context.MaturskiIspit.Add(maturskiIspit);

            int OdjeljenjeId = context.Odjeljenje.
                Where(x => x.Razred == 4 && x.SkolaID == maturskiIspitDodajVM.OdabranaSkola)
                .FirstOrDefault().Id;

            List<Ucenik> ucenici = context.OdjeljenjeStavka.Include(f=>f.Ucenik).
                Where(g => g.OdjeljenjeId == OdjeljenjeId).Select(x => x.Ucenik).ToList();

            foreach(Ucenik ucenik in ucenici)
            {
                bool status = ProvjeriUcenika(ucenik.Id);
                if(status)
                {
                    MaturskiIspitStavka maturskiIspitStavka = new MaturskiIspitStavka
                    {
                        MaturskiIspitId = maturskiIspit.Id,
                        PristupioIspitu = false,
                        ProsjekOcjena = context.DodjeljenPredmet
                        .Where(x => x.OdjeljenjeStavkaId == context.OdjeljenjeStavka
                        .Where(g => g.UcenikId == ucenik.Id).FirstOrDefault().Id).Average(b => b.ZakljucnoKrajGodine),
                        Rezultat=0,
                        UcenikId=ucenik.Id

                    };
                    context.MaturskiIspitStavka.Add(maturskiIspitStavka);
                    
                }
            }
            context.SaveChanges();

            return Redirect("/OdrzanaNastava/Odaberi?Id=" + maturskiIspitDodajVM.NastavnikId);
        }
        public bool ProvjeriUcenika(int id)
        {
            bool status = true;
            if(context.DodjeljenPredmet
                .Where(g=>g.OdjeljenjeStavkaId==context
                .OdjeljenjeStavka.Where(h=>h.UcenikId==id).FirstOrDefault().Id).Count(x=>x.ZakljucnoKrajGodine==1)>0)
            {
                status = false;
            }

            if(context.MaturskiIspitStavka.Where(x=>x.UcenikId==id).Count(g=>g.Rezultat>55)>0)
            {
                status = false;
            }

            return status;
        }
        public IActionResult Uredi(int id)
        {

            MaturskiIspitUrediVM model = context.MaturskiIspit.Include(h=>h.Predmet).Where(x=>x.Id==id).Select(c => new MaturskiIspitUrediVM
            {
                MaturskiIspitID=c.Id,
                Predmet=c.Predmet.Naziv,
                Datum=c.Datum.ToString("dd.MM.yyyy"),
                Napomena=c.Napomena


            }).FirstOrDefault();

            return View(model);
        }
        public IActionResult UrediSnimi(MaturskiIspitUrediVM maturskiIspitUrediVM)
        {
            MaturskiIspit maturskiIspit = context.MaturskiIspit.Find(maturskiIspitUrediVM.MaturskiIspitID);
            maturskiIspit.Napomena = maturskiIspitUrediVM.Napomena;
            context.MaturskiIspit.Update(maturskiIspit);
            context.SaveChanges();

            return Redirect("/OdrzanaNastava/Uredi?id=" + maturskiIspitUrediVM.MaturskiIspitID);
        }
        public IActionResult AjaxStavke(int id)
        {
            List<AjaxStavke> model = context.MaturskiIspitStavka.Include(h=>h.Ucenik).Where(x => x.MaturskiIspitId == id).Select(b => new ViewModels.AjaxStavke
            {
                MaturskiIspitStavkaId=b.Id,
                Ucenik=b.Ucenik.ImePrezime,
                ProsjekOcjena = context.DodjeljenPredmet
                        .Where(x => x.OdjeljenjeStavkaId == context.OdjeljenjeStavka
                        .Where(g => g.UcenikId == b.UcenikId).FirstOrDefault().Id).Average(c => c.ZakljucnoKrajGodine),
                PristupioIspitu=b.PristupioIspitu?"Da":"Ne",
                Rezulat=b.Rezultat

            }).ToList();

            return PartialView(model);
        }
        public IActionResult Pristupio(int id)
        {

            MaturskiIspitStavka stavka = context.MaturskiIspitStavka.Find(id);
            stavka.PristupioIspitu = !stavka.PristupioIspitu;

            context.MaturskiIspitStavka.Update(stavka);
            context.SaveChanges();

            return Redirect("/OdrzanaNastava/Uredi?id="
                + context.MaturskiIspitStavka.FirstOrDefault(x => x.Id == id).MaturskiIspitId);
        }
        public IActionResult UrediUcenika(int id)
        {
            MaturskiIspitStavka stavka = context.MaturskiIspitStavka.Include(g=>g.Ucenik).FirstOrDefault(x => x.Id == id);

            UrediUcenikaVM model = new UrediUcenikaVM
            {
                MaturskiIspitStavkaID=stavka.Id,
                Ucenik=stavka.Ucenik.ImePrezime,
                Bodovi=stavka.Rezultat
            };

            return PartialView(model);
        }
        public IActionResult UrediUcenikaSnimi(UrediUcenikaVM urediUcenikaVM)
        {
            MaturskiIspitStavka maturskiIspit = context.MaturskiIspitStavka.Find(urediUcenikaVM.MaturskiIspitStavkaID);
            maturskiIspit.Rezultat = urediUcenikaVM.Bodovi;
            context.MaturskiIspitStavka.Update(maturskiIspit);
            context.SaveChanges();

            return Redirect("/OdrzanaNastava/Uredi?id="
                + context.MaturskiIspitStavka
                .FirstOrDefault(x => x.Id == urediUcenikaVM.MaturskiIspitStavkaID)
                .MaturskiIspitId);
        }
    }


}

//Prilikom Snimanja novog maturskog ispita potrebno je dodati
//zapise iz međutabele:
//- koji omogućavaju evidentiranje bodova na maturskom ispitu
//za sve učenike IV razreda iz odbrane škole,
//- dodati međuzapise samo za maturante koji imaju uslove za
//pristup maturskom ispitu(validnost rada ove metode
//provjeriti integracijskim testom u Zadatku 3.a.)
//o pozitivan uspjeh u IV razredu(tj.da nema
//zaključenih ocjena 1 na kraju godine)
//o da nema položen maturski ispit(broj bodova na
//prethodno evidentiranom maturskom ispitu mora
//biti veći od 55)