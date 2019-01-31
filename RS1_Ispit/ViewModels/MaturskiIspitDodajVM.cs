using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class MaturskiIspitDodajVM
    {
        public List<SelectListItem> Skole { get; set; }
        public int OdabranaSkola { get; set; }
        public List<SelectListItem> Predmet { get; set; }
        public int OdabraniPredmet { get; set; }
        public string Nastavnik { get;  set; }
        public string SkolskaGodina { get;  set; }
        public DateTime DatumIspita { get; set; }
        public int NastavnikId { get;  set; }
    }
}
