<%@ Import Namespace="SampleSolrApp.Models"%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PaginationInfo>" %>

<div>
    Results <%= Model.FirstItemIndex %> - <%= Model.LastItemIndex%> of <b><%= Model.TotalItemCount %></b>
</div>

<div class="pagination">

  <ul>
     <li class="prev <%= (Model.HasPrevPage) ? string.Empty : "disabled" %>"><a href="<%= Model.PrevPageUrl ?? "#" %>">&larr; Previous</a></li>
     <% foreach (var page in Model.Pages) { %>
     
     <li <%= (page == Model.CurrentPage) ? "class='active'" : "" %>><a href="<%= Model.PageUrlFor(page) %>"><%= page %></a></li>

     <% } %>

     <li class="next <%= (Model.HasNextPage) ? string.Empty : "disabled" %>"><a href="<%= Model.NextPageUrl ?? "#" %>">Next &rarr;</a></li>
  </ul>

</div>