using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MomentaryMessages.Data.DataTransferObjects;
using MomentaryMessages.Data.Services;

namespace MomentaryMessages.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SuperSecretApiController : ControllerBase
  {
    private readonly SecretViewLogsService m_service;

    public SuperSecretApiController(SecretViewLogsService service)
    {
      m_service = service;
    }

    // GET: SuperSecret
    [HttpGet]
    public async Task<JsonResult> Index(string? userName)
    {
      string validatedUserName = User.IsInRole("Admin")
        && userName != null
        && !userName.IsNullOrEmpty()
          ? userName
          : User?.Identity?.Name ?? string.Empty;
      var dtos = await m_service.GetAllAsync();
      if (dtos
        .Any(x =>
          x.ViewerName == validatedUserName
          && (x.ExpiryDate == null || (x.ExpiryDate != null && x.ExpiryDate < DateTime.Now))
          && x.RemainingViewsCount == 0))
        return new JsonResult("There are no secrets here");

      try
      {
        await m_service.AddOrUpdateAsync(new SecretViewLogDto(validatedUserName));
      }
      catch (Exception ex)
      {
        return new JsonResult(ex.Message);
      }

      return new JsonResult($"You have found the secret {userName}!");
    }
  }
}
