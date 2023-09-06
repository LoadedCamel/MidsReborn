namespace Mids_Reborn.Core.Utils
{
    internal static class Consts
    {
        internal const string InitHtml = @"<!DOCTYPE html>
<html>
<head>
<style type=""text/css"">
body, html {
    width: 100%;
    height: 100%;
    overflow: hidden;
    background-color: #000;
}

body {
    background-image: url(""http://appassets.mrb/images/MRBLoading.gif"");
    background-repeat: no-repeat;
    background-size: auto 100%;
    background-position: center;
}

div#message {
    margin: 12px auto auto auto;
    text-align: center;
    color: Gainsboro;
    font-family: Sans-Serif;
    font-size: 11pt;
    font-weight: bold;
    text-shadow: 0 0 3px #000, 0 0 1px #000, 0 0 1px #000;
    transition: all 0.5s ease-out;
}
</style>
</head>
<body>
    <div id=""message""></div>
</body>
</html>";
    }
}
