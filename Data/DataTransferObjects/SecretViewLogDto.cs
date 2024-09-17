using System.ComponentModel.DataAnnotations;

namespace MomentaryMessages.Data.DataTransferObjects
{
  public class SecretViewLogDto
  {
    public SecretViewLogDto(string viewerName, DateTime? expiryDate = null, int? remainingViewsCount = null)
    {
      ViewerName = viewerName;
      ExpiryDate = expiryDate;
      RemainingViewsCount = remainingViewsCount ?? 0;
    }

    [Key]
    public string ViewerName { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int RemainingViewsCount { get; set; }
  }
}
