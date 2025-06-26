// used on FolderBlueprintLaunchAsync, GetFolder, GetTasks, ModifyTasks, TaskBlueprintLaunchAsync

/// <summary>
/// This script is designed to be used as custom code for a Wrike custom connector in Power Automate.
/// It intercepts the outgoing request and modifies the URI to handle specific Wrike API requirements,
/// such as converting comma-separated parameter values into JSON arrays.
/// </summary>
public class Script : ScriptBase
{
    /// <summary>
    /// The main entry point for the script execution.
    /// It modifies the request URI and then sends the request to the Wrike API.
    /// </summary>
    /// <returns>The HttpResponseMessage from the Wrike API.</returns>
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

    /// <summary>
    /// Converts the values of specified query parameters from comma-separated strings to JSON arrays.
    /// For example, a parameter like "status=Active,Completed" will be converted to "status=["Active","Completed"]".
    /// </summary>
    /// <param name="originalUrl">The original request URL.</param>
    /// <param name="parametersToConvert">An array of query parameter names to convert.</param>
    /// <returns>The modified URL with converted query parameters.</returns>
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

    /// <summary>
    /// Removes query parameters that have null or empty values from the URL.
    /// </summary>
    /// <param name="originalUrl">The original request URL.</param>
    /// <returns>The modified URL with null parameters removed.</returns>
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
