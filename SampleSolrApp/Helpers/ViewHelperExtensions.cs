using System;
using System.Collections.Generic;
using System.Linq;
using SampleSolrApp.Models;

namespace SampleSolrApp.Helpers
{
    public class ViewHelperExtensions
    {
        public static string GetFacetDisplayName(List<SolrFacet> sourceFacts, string facetId)
        {
            return sourceFacts.Single(f => f.Id == Int32.Parse(facetId)).Name;
        }
    }
}