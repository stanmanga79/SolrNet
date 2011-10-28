using System.Web.Mvc;
using SampleSolrApp.Models;
using SolrNet;

namespace SampleSolrApp.Controllers
{
    public class IndexMaintenanceController : Controller
    {
        private readonly ISolrOperations<SolrTitle> _titleIndexer;

        public IndexMaintenanceController(ISolrOperations<SolrTitle> titleIndexer)
        {
            _titleIndexer = titleIndexer;
        }

        //
        // POST: /Optimize/
        [HttpPost]
        public ActionResult Optimize()
        {
            var optimizeResults = _titleIndexer.Optimize();
            var message = string.Format("Optimization of solr Index complete.  Optimization Time {0} ms.", optimizeResults.QTime);

            return Json(new { IsValid = true, Message = message });
        }

    }
}
