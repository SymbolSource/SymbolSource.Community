<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SymbolSource.Server.Basic.Host.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <dl>
        <dt>Visuak Studio Debugging URL</dt>
        <dd><a href="<%=GetAbsoluteUri("/WinDbg/pdb")%>"><%=GetAbsoluteUri("/WinDbg/pdb")%></a></dd>
        <dt>NuGet Push URL</dt>
        <dd><a href="<%=GetAbsoluteUri("/NuGet")%>"><%=GetAbsoluteUri("/NuGet")%></a></dd>
        <dt>NuGet Feed URL</dt>
        <dd><a href="<%=GetAbsoluteUri("/NuGet/FeedService.mvc")%>"><%=GetAbsoluteUri("/NuGet/FeedService.mvc")%></a></dd>
        <dt>OpenWrap Repository URL</dt>
        <dd><a href="<%=GetAbsoluteUri("/OpenWrap")%>"><%=GetAbsoluteUri("/OpenWrap")%></a></dd>
    </dl>
</body>
</html>