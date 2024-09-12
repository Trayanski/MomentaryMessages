using System.ComponentModel.DataAnnotations;

namespace MomentaryMessages.Data.Models
{
  public class SecretViewLog
  {
    [Key]
    public required string ViewerMachineName { get; set; }
    public DateTime InitialViewDate { get; set; } = DateTime.Now;
    public int ViewsCount { get; set; } = 0;
  }
}
