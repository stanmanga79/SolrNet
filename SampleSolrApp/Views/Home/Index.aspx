<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ProductView>" %>
<%@ Import Namespace="SampleSolrApp.Helpers"%>
<%@ Import Namespace="SampleSolrApp.Models"%>

<asp:Content ID="indexHead" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/facet.css" rel="stylesheet" type="text/css" />
    <title>Welcome to Solr Title search!</title>
</asp:Content>

<asp:Content ContentPlaceHolderID="SideBar" runat="server">
<% foreach (var f in Model.Search.Facets) { %>        
            <ul class="removeBullet">
                <li>
                    <%= Html.SolrFieldPropName<SolrTitle>(f.Key) %>
                    <ul>
                        <li><a class="removeFacet" href="<%= Url.RemoveFacet(f.Key) %>"><%= f.Value %></a></li>
                    </ul>
                </li>
            </ul>
            <% } %>
        
            <ul>
                <% foreach (var f in Model.Facets) { %> 
                <li>
                    <%= Html.SolrFieldPropName<SolrTitle>(f.Key) %>
                    <ul>
                        <% foreach (var fv in f.Value) { %>
                        <li><a href="<%= Url.SetFacet(f.Key, fv.Key) %>"><%= fv.Key %></a> <span>(<%= fv.Value %>)</span></li>
                        <%} %>
                    </ul>
                </li>
                <% } %>
            </ul>        
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <form method="get" action="<%= Url.Action("Index") %>">
        <%= Html.TextBox("q", Model.Search.FreeSearch) %>
        <input type="submit" value="Search" class="btn primary" />
        <% if (!string.IsNullOrEmpty(Model.DidYouMean)) { %>
        Did you mean <strong><em><a href="<%= Url.ForQuery(Model.DidYouMean) %>"><%= Model.DidYouMean%></a></em></strong>
        <% } %>
        <% if (Model.QueryError) { %> 
        <span class="error">Invalid query</span>
        <% } %>
    </form>    

    <div>                    
        <div>
            <%= Html.Grid(Model.Products)
                    .Attributes(@class => "zebra-striped")
                    .Columns(col =>
                     {
                         col.For(prop => prop.Name);
                         col.For(prop => prop.ObjectId);
                         col.For(prop => prop.ReleaseYear);
                     })%>
            
        </div>
        
        <% Html.RenderPartial("Pagination", new PaginationInfo {
            PageUrl = Url.SetParameter("page", "!0"),
            CurrentPage = Model.Search.PageIndex, 
            PageSize = Model.Search.PageSize,
            TotalItemCount = Model.TotalCount,
        }); %>
        
        <div class="pagesize">
            <% Html.Repeat(new[] { 10, 25, 50, 100 }, ps => { %>
                <% if (ps == Model.Search.PageSize) { %>
                <span><%= ps%></span>
                <% } else { %>
                <a href="<%= Url.SetParameters(new {pagesize = ps, page = 1}) %>"><%= ps%></a>
                <% } %>
            <% }, () => { %> | <% }); %>
            items per page
        </div>
    </div>
</asp:Content>
