using System;
using System.Collections.Generic;
using SolrNet.Attributes;

namespace SampleSolrApp.Models
{
    public abstract class SolrEntityBase
    {
        [SolrField("Name")]
        public string Name { get; set; }

        [SolrUniqueKey("Id")]       
        public string Id { get; set; }
    }
}