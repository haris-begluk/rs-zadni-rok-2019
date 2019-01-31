using Microsoft.VisualStudio.TestTools.UnitTesting;
using RS1_Ispit_asp.net_core.Controllers;
using RS1_Ispit_asp.net_core.EF;

namespace Testovi
{
    [TestClass]
    public class TestoviUcenika
    {
        [TestMethod]
        public void ProvjeraUcenika()
        {
            MojContext context = GetContext.get();

            OdrzanaNastavaController odrzanaNastavaController = new OdrzanaNastavaController(context);

            bool rezultat = odrzanaNastavaController.ProvjeriUcenika(1);

            Assert.AreEqual(true, rezultat);
        }
    }
}
