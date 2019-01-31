using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_Ispit_asp.net_core.EF;
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
    }
}