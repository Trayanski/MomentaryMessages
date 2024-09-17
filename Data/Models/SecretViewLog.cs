using System.ComponentModel.DataAnnotations;

namespace MomentaryMessages.Data.Models
{
  public class SecretViewLog
  {
    [Key]
    public required string ViewerName { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int RemainingViewsCount { get; set; }
  }
}
