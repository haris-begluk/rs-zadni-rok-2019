using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    }
}