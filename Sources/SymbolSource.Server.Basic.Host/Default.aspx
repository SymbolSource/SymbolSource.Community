<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    protected string GetAbsoluteUri(string relativeUri)
    {
        return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + relativeUri;
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SymbolSource - Server Basic</title>
    <link rel="shortcut icon" href="/favicon.png" />
</head>
<body>
    <h1>SymbolSource - Server Basic</h1>
    
    <h2>Visual Studio Debugging URL</h2>
    <p>
        <strong><code><%=GetAbsoluteUri("WinDbg/pdb")%></code></strong> <a href="http://www.symbolsource.org/Public/Home/VisualStudio">More info about configuration</a>
    </p>
    <h2>NuGet - Pushing packages</h2>
    <p>
        <code>nuget push *.symbols.nupkg 123 -Source </code><strong><code><%=GetAbsoluteUri("NuGet")%></code></strong>
    </p>
    <h2>OpenWrap - Repository URL</h2>
    <p>
        <strong><code><%=GetAbsoluteUri("OpenWrap")%></code></strong>
    </p>
<%--    
        <dt>NuGet Feed URL</dt>
        <dd><a href="<%=GetAbsoluteUri("NuGet/FeedService.mvc")%>"><%=GetAbsoluteUri("NuGet/FeedService.mvc")%></a></dd>
--%>    
</body>
</html>