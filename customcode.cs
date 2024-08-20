// used on FolderBlueprintLaunchAsync, GetFolder, GetTasks, ModifyTasks, TaskBlueprintLaunchAsync

public class Script : ScriptBase
{

    public override async Task<HttpResponseMessage> ExecuteAsync()
    {
        var strRequestUri = this.Context.Request.RequestUri.AbsoluteUri;
        var strRequestUriDecode = HttpUtility.UrlDecode(strRequestUri);
        strRequestUriDecode = RemoveNullParameters(strRequestUriDecode);
        strRequestUriDecode = ConvertToJSONFields(strRequestUriDecode,"fields", "status","addParents");
        
        this.Context.Request.RequestUri = new Uri(strRequestUriDecode);
        HttpResponseMessage response = await this.Context.SendAsync(this.Context.Request, this.CancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        
        //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        //response.Content = new StringContent(GetCall(strRequestUri) + this.Context.Request.Method);
        
        return response;
    }

    private string ConvertToJSONFields(string originalUrl, params string[] parametersToConvert)
    {
        Uri uri = new Uri(originalUrl);
        string queryString = uri.Query.TrimStart('?');
        string[] queryParams = queryString.Split('&');

        List<string> modifiedParams = new List<string>();

        foreach (string param in queryParams)
        {
            string[] keyValue = param.Split('=');
            string paramName = keyValue[0];
            string paramValue = keyValue.Length > 1 ? keyValue[1] : "";

            if (parametersToConvert.Contains(paramName))
            {
                string[] values = paramValue.Split(',');
                string jsonValues = "[" + string.Join(",", values.Select(v => "\"" + v + "\"")) + "]";
                modifiedParams.Add(paramName + "=" + Uri.EscapeDataString(jsonValues));
            }
            else
            {
                modifiedParams.Add(param);
            }
        }

        string modifiedQueryString = string.Join("&", modifiedParams);
        string modifiedUrl = uri.GetLeftPart(UriPartial.Path) + "?" + modifiedQueryString;
        return modifiedUrl;
    }

    private string RemoveNullParameters(string originalUrl)
    {
        Uri uri = new Uri(originalUrl);
        string queryString = uri.Query.TrimStart('?');
        string[] queryParams = queryString.Split('&');

        List<string> modifiedParams = new List<string>();

        foreach (string param in queryParams)
        {
            string[] keyValue = param.Split('=');
            string paramName = keyValue[0];
            string paramValue = keyValue.Length > 1 ? keyValue[1] : "";

            if (!string.IsNullOrEmpty(paramValue))
            {
                modifiedParams.Add(param);
            }
        }

        string modifiedQueryString = string.Join("&", modifiedParams);
        string modifiedUrl = uri.GetLeftPart(UriPartial.Path) + "?" + modifiedQueryString;
        return modifiedUrl;
    }
}
