using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using be_scrapping_service.Context;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using be_scrapping_service.Entity;
namespace be_scrapping_service.Controllers;


[ApiController]
[Route("[controller]")]
public class ScraperController : ControllerBase
{
  private readonly ScrapingContext _context;
  private readonly UserManager<User> _userManager;

  public ScraperController(
    ScrapingContext context,
    UserManager<User> userManager
  )
  {
    _context = context;
    _userManager = userManager;
  }

 
  [HttpPost, Authorize]
   async public Task<object> PostHistory(string companyName)
  {
    var results = await new ScrappingService("https://www.occ.com.mx")
      .ParseHtml(companyName);

    if(results == 0)
      return BadRequest("There is no data")

    _context.Histories.Add(new Entity.History
    {
      JobsCount = results,
      UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
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
  public object GetHistory(int page) => new {
    Info = new {
      total = _context.Histories
        .Where(history => history.UserId.Equals(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        .Count()
    },
    results = _context.Histories
      .OrderByDescending(history => history.HistoryId)
      .Where(history => history.UserId.Equals(User.FindFirst(ClaimTypes.NameIdentifier).Value))
      .Skip(page * 10)
      .Take(10)
      .ToList()
  };
    
}
