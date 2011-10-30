#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SampleSolrApp.Models;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.DSL;
using SolrNet.Exceptions;

namespace SampleSolrApp.Controllers {
    [HandleError]
    public class HomeController : Controller {
        private readonly ISolrReadOnlyOperations<SolrTitle> solr;

        public HomeController(ISolrReadOnlyOperations<SolrTitle> solr) {
            this.solr = solr;
        }

        /// <summary>
        /// Builds the Solr query from the search parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ISolrQuery BuildQuery(SearchParameters parameters) {
            if (!string.IsNullOrEmpty(parameters.FreeSearch))
                return new SolrQuery(parameters.FreeSearch);

            return SolrQuery.All;
        }

        public ICollection<ISolrQuery> BuildFilterQueries(SearchParameters parameters)
        {
            var filteredQueries = new List<ISolrQuery>();

            var queriesFromFacets = from p in parameters.Facets
                                    select (ISolrQuery)Query.Field(p.Key).Is(p.Value);

            if (parameters.QueryFacets.Count > 0) filteredQueries.Add(getCustomDecadeQueries().Single(q => q.From.ToString() == parameters.QueryFacets.First().Value));

            filteredQueries.AddRange(queriesFromFacets.ToList());
            
            return filteredQueries;
        }


        /// <summary>
        /// All selectable facet fields
        /// </summary>
        //private static readonly string[] AllFacetFields = new[] {"cat", "manu_exact"};
        private static readonly string[] AllFacetFields = new[] {"TitleType" };

        /// <summary>
        /// Gets the selected facet fields
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<string> SelectedFacetFields(SearchParameters parameters) {
            return parameters.Facets.Select(f => f.Key);
        }

        public SortOrder[] GetSelectedSort(SearchParameters parameters) {
            return new[] {SortOrder.Parse(parameters.Sort)}.Where(o => o != null).ToArray();
        }

        public ActionResult Index(SearchParameters parameters) {
            try {
                var start = (parameters.PageIndex - 1)*parameters.PageSize;


                var facetCustomQueries = getSolrCustomQueries();

                var facetQueries = new List<ISolrFacetQuery>();

                facetQueries.AddRange(facetCustomQueries);

                facetQueries.AddRange(AllFacetFields.Except(SelectedFacetFields(parameters))
                    .Select(f => new SolrFacetFieldQuery(f) {MinCount = 1})
                    .Cast<ISolrFacetQuery>()
                    .ToList());

                
                var matchingProducts = solr.Query(BuildQuery(parameters), new QueryOptions {
                    FilterQueries = BuildFilterQueries(parameters),
                    Rows = parameters.PageSize,
                    Start = start,
                    OrderBy = GetSelectedSort(parameters),
                    SpellCheck = new SpellCheckingParameters(),
                    Facet = new FacetParameters {
                        Queries = facetQueries,
                    },
                });
                var view = new ProductView {
                    Products = matchingProducts,
                    Search = parameters,
                    TotalCount = matchingProducts.NumFound,
                    Facets = matchingProducts.FacetFields,
                    CustomFacets = matchingProducts.FacetQueries,
                    SolrFacets = FormatSolrFacets(matchingProducts.FacetQueries),
                    DidYouMean = GetSpellCheckingResult(matchingProducts),
                };
                return View(view);
            } catch (InvalidFieldException) {
                return View(new ProductView {
                    QueryError = true,
                });
            }
        }

        private List<SolrFacet> FormatSolrFacets(IEnumerable<KeyValuePair<string, int>> facetQueries)
        {
            var solrFacets = new List<SolrFacet>();

            //Add 2000s
            KeyValuePair<string, int> facet = facetQueries.SingleOrDefault(f => f.Key.Contains("2000"));
            solrFacets.Add(new SolrFacet {Id = 2000, Field = "ReleaseYear" ,Name = "2000 plus", Facet = facet, Result = facet.Value });
            //90's
            facet = facetQueries.SingleOrDefault(f => f.Key.Contains("1990"));
            solrFacets.Add(new SolrFacet { Id = 1990, Field = "ReleaseYear", Name = "1990s", Facet = facet, Result = facet.Value });

            //80's
            facet = facetQueries.SingleOrDefault(f => f.Key.Contains("1980"));
            solrFacets.Add(new SolrFacet { Id = 1980, Field = "ReleaseYear", Name = "1980s", Facet = facet, Result = facet.Value });
            
            //70's
            facet = facetQueries.SingleOrDefault(f => f.Key.Contains("1970"));
            solrFacets.Add(new SolrFacet { Id = 1970, Field = "ReleaseYear", Name = "1970s", Facet = facet, Result = facet.Value });
            
            //60's
            facet = facetQueries.SingleOrDefault(f => f.Key.Contains("1960"));
            solrFacets.Add(new SolrFacet { Id = 1960, Field = "ReleaseYear", Name = "1960s", Facet = facet, Result = facet.Value });
            
            //less than 60's
            facet = facetQueries.SingleOrDefault(f => f.Key.Contains("1959"));
            solrFacets.Add(new SolrFacet { Id = int.MinValue, Field = "ReleaseYear", Name = "1959 below", Facet = facet, Result = facet.Value });
            


            return solrFacets;
        }
      
        private static List<ISolrFacetQuery> getSolrCustomQueries()
        {
            var facetQueries = new List<ISolrFacetQuery>();
          
            getCustomDecadeQueries().ForEach(facetQuery => facetQueries.Add(new SolrFacetQuery(facetQuery)));

            return facetQueries;
        }


        private static List<SolrQueryByRange<int>> getCustomDecadeQueries()
        {
            var queries = new List<SolrQueryByRange<int>>
                              {
                                  new SolrQueryByRange<int>("ReleaseYear", 2000, int.MaxValue),
                                  new SolrQueryByRange<int>("ReleaseYear", 1990, 1999),
                                  new SolrQueryByRange<int>("ReleaseYear", 1980, 1989),
                                  new SolrQueryByRange<int>("ReleaseYear", 1970, 1979),
                                  new SolrQueryByRange<int>("ReleaseYear", 1960, 1969),
                                  new SolrQueryByRange<int>("ReleaseYear", int.MinValue, 1959)
                              };
          
            return queries;
        }

      
        private string GetSpellCheckingResult(ISolrQueryResults<SolrTitle> products) {
            return string.Join(" ", products.SpellChecking
                                        .Select(c => c.Suggestions.FirstOrDefault())
                                        .Where(c => !string.IsNullOrEmpty(c))
                                        .ToArray());
        }
    }
}