namespace be_scrapping_service.Entity
{
  public class History{
    public int HistoryId { get; set; }
    public int JobsCount { get; set; }
    public required string UserId {get; set;}
    public virtual User? User {get; set;}
    public required string CompanyName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
  }
}
