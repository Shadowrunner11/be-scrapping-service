using be_scrapping_service;
using HtmlAgilityPack;
using System.Net;


class ScrappingService
{
  private readonly string _baseUrl;

  public ScrappingService(string baseUrl){
    this._baseUrl = baseUrl;
  }

  private async Task<string> callUrl(string url)
  {
        HttpClient client = new HttpClient();
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
        client.DefaultRequestHeaders.Accept.Clear();

        // TODO: use Uri API
        var response = client.GetStringAsync(this._baseUrl + url);
        return await response;
  }

  private string getInnerText(HtmlNode node, string xpath){
    return node.SelectSingleNode(xpath).InnerText.ToString();
  }

  private int parseJobsCount(string rawJobCounts){
    // TODO: check btter way to do this
    return int.Parse(
      rawJobCounts
        .Replace("resultados", "")
        .Replace(",", "")
        .Trim()
    );

  }

  async public Task<List<OCCItem>> ParseHtml()
  {
    HtmlDocument htmlDoc = new HtmlDocument();
   
    htmlDoc.LoadHtml(await this.callUrl("/empleos"));

    var programmerLinks = htmlDoc.DocumentNode.SelectNodes(".//div[contains(@id,\"jobcard\")]");

    List<OCCItem> items = new List<OCCItem>();
    
    foreach (HtmlNode element in programmerLinks)
    {    
      OCCItem item = new OCCItem();

      item.CompanyName = this.getInnerText(element, "//span[contains(@class,\"locContainer\")]");

      item.JobsCount = this.parseJobsCount(
        this.getInnerText(element, "//div[contains(@class,\"leftSide\")]/p")
      );

      item.CreatedAt = DateTime.UtcNow;

      items.Add(item);
    }

    return items;
  }
}




