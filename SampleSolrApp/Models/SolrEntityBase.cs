using System;
using System.Collections.Generic;
using SolrNet.Attributes;

namespace SampleSolrApp.Models
{
    public abstract class SolrEntityBase
    {
        protected SolrEntityBase(string type)
        {
            Type = type;
            Id = Guid.NewGuid();
        }


        [SolrField("Name")]
        public string Name { get; set; }

        [SolrUniqueKey("Id")]       
        public Guid Id { get; private set; }

        [SolrField("ObjectId")]
        public int ObjectId { get; set; }

        [SolrField("Type")]
        public string Type { get; set; }

        [SolrField("TitleIds")]
        public ICollection<int> TitleIds { get; set; }

    }
}