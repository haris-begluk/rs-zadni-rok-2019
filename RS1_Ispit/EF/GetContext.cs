using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.EF
{
    public class GetContext
    {
        public static MojContext get()
        {
            DbContextOptionsBuilder<MojContext> builder =
                new DbContextOptionsBuilder<MojContext>().UseSqlServer("Server=xps15;Database=Januar2019;Trusted_Connection=true;MultipleActiveResultSets=true;User ID=sa;Password=test");

            return new MojContext(builder.Options);
        }
    }
}
