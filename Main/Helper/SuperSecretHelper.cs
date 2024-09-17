using Microsoft.IdentityModel.Tokens;
using MomentaryMessages.Data.DataTransferObjects;
using MomentaryMessages.Data.Services;
using System.Security.Claims;

namespace MomentaryMessages.Helper
{
  public class SuperSecretHelper
  {
    public static async Task<string> GetSecretMessage(SecretViewLogsService service, ClaimsPrincipal user, string? userName)
    {
      const string c_noSecretsMessage = "There are no secrets here";
      var validatedUserName = user?.Identity?.Name ?? string.Empty;
      var hasProvidedUserName = userName != null && !userName.IsNullOrEmpty();
      if (user!.IsInRole("Admin"))
      {
        if (hasProvidedUserName)
        {
          validatedUserName = userName;
        }
      }
      else if (hasProvidedUserName)
      {
        return c_noSecretsMessage;
      }

      var dtos = await service.GetAllAsync();
      // Check if a link exists in the db
      var dto = dtos.FirstOrDefault(x => x.ViewerName == validatedUserName);
      if (dto == null)
        dto = new SecretViewLogDto(validatedUserName);
      // If there is a link in the db, check if it is expired and if it has any available viewings
      else if ((dto.ExpiryDate != null && dto.ExpiryDate < DateTime.Now)
        || dto.RemainingViewsCount == 0)
        return c_noSecretsMessage;

      try
      {
        await service.AddOrUpdateAsync(dto);
      }
      catch (Exception ex)
      {
        return $"{ex.Message}; {ex.InnerException?.Message}";
      }

      return $"You have found the secret {validatedUserName}!";
    }
  }
}
