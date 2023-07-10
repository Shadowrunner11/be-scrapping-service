using System.ComponentModel.DataAnnotations;

namespace be_scrapping_service;

  public class AuthRequest
  {
      [Required]
      public string Email { get; set; } = null!;

      [Required]
      public string Password { get; set; } = null!;
  }
