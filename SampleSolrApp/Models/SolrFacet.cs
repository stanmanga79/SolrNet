using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleSolrApp.Models
{
    public class SolrFacet
    {
        public string Name { get; set; }
        public KeyValuePair<string, int> Facet { get; set; }
        public int Result { get; set; }

    }

}