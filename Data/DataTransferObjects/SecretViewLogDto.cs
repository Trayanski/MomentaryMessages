using System.ComponentModel.DataAnnotations;

namespace MomentaryMessages.Data.DataTransferObjects
{
  public class SecretViewLogDto
  {
    [Key]
    public required string ViewerName { get; set; }
    public DateTime InitialViewDate { get; set; } = DateTime.Now;
    public int ViewsCount { get; set; } = 0;
  }
}
