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
using SolrNet.Attributes;

namespace SampleSolrApp.Models {
    public class SolrTitle : SolrEntityBase 
    {
        [SolrField("Synopsis")]
        public string Synopsis { get; set; }

        [SolrField("ReleaseYear")]
        public int ReleaseYear { get; set; }

        [SolrField("TitleType")]
        public string TitleType { get; set; }
    }
}