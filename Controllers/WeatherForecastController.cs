using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using be_scrapping_service.Context;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using be_scrapping_service.Entity;
namespace be_scrapping_service.Controllers;


[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
  private static readonly string[] Summaries = new[]
  {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

  private readonly ILogger<WeatherForecastController> _logger;
  private readonly ScrapingContext _context;

  private readonly UserManager<User> _userManager;

  public WeatherForecastController(
      ILogger<WeatherForecastController> logger,
      ScrapingContext context,
      UserManager<User> userManager
  )
  {
    _logger = logger;
    _context = context;
    _userManager = userManager;
  }

  [HttpGet(Name = "GetWeatherForecast")]
  public IEnumerable<WeatherForecast> Get()
  {
    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
      Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
      TemperatureC = Random.Shared.Next(-20, 55),
      Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    })
    .ToArray();
  }

  [HttpPost, Authorize]
  [Route("scrap")]
  async public Task<object> PostHistory(string companyName)
  {
    var results = await new ScrappingService("https://www.occ.com.mx")
      .ParseHtml(companyName);

    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    _context.Histories.Add(new Entity.History
    {
      JobsCount = results,
      UserId = userId,
      CompanyName = companyName
    });

    _context.SaveChanges();

    return new
    {
      JobsCount = results,
      CompanyName = companyName
    };
  }

  [HttpGet, Authorize]
  [Route("history")]
  public List<Entity.History> GetHistory(int page)  =>
     _context.Histories
      .Where(history => history.UserId.Equals(User.FindFirst(ClaimTypes.NameIdentifier).Value))
      .Skip(page * 10)
      .Take(10)
      .ToList();
}
