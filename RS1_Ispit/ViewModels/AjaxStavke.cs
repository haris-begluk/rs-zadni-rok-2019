using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class AjaxStavke
    {
        public int MaturskiIspitStavkaId { get;  set; }
        public string Ucenik { get;  set; }
        public double ProsjekOcjena { get;  set; }
        public string PristupioIspitu { get;  set; }
        public float Rezulat { get;  set; }
    }
}
