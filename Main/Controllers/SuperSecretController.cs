using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MomentaryMessages.Data.DataTransferObjects;
using MomentaryMessages.Data.Services;
using MomentaryMessages.ViewModels;

namespace MomentaryMessages.Controllers
{
  [Authorize]
  public class SuperSecretController : Controller
  {
    private readonly SecretViewLogsService m_service;

    public SuperSecretController(SecretViewLogsService service)
    {
      m_service = service;
    }

    // GET: SuperSecret
    public async Task<IActionResult> Index(string? userName)
    {
      string validatedUserName = User?.Identity?.Name ?? string.Empty;
      var hasProvidedUserName = userName != null && !userName.IsNullOrEmpty();
      if (User!.IsInRole("Admin"))
      {
        if (hasProvidedUserName)
          validatedUserName = userName;
      }
      else
      {
        if (hasProvidedUserName)
          return RedirectToAction("Index", "Home", null);
      }

      var dtos = await m_service.GetAllAsync();
      var dto = dtos.FirstOrDefault(x => x.ViewerName == validatedUserName);
      if (dto == null)
        dto = new SecretViewLogDto(validatedUserName);
      else if ((dto.ExpiryDate != null && dto.ExpiryDate < DateTime.Now)
        || dto.RemainingViewsCount == 0)
        return View(nameof(Index), "There are no secrets here");

      try
      {
        await m_service.AddOrUpdateAsync(dto);
      }
      catch (Exception ex)
      {
        return View(nameof(Index), $"{ex.Message}; {ex.InnerException?.Message}");
      }

      return View(nameof(Index), $"You have found the secret {validatedUserName}!");
    }

    // GET: SuperSecret/GenerateLink
    [Authorize(Roles = "Admin")]
    public IActionResult GenerateLink()
      => View(
        nameof(GenerateLink),
        new GenerateLinkViewModel()
        {
          URLPrefix = $"{Request.Scheme}://{Request.Host}/{RouteData.Values.Single(x => x.Key.Equals("controller")).Value}?userName="
        });

    // GET: SuperSecret/StoreSecretLinkContract
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<JsonResult> StoreSecretLinkContract(string userName, DateTime? expiryDate = null, int? canBeClickedXNumberOfTimes = null)
    {
      const int c_remainingViewsCountForContractEntity = 1;
      try
      {
        await m_service.AddOrUpdateAsync(new SecretViewLogDto(
          userName,
          expiryDate,
          canBeClickedXNumberOfTimes ?? c_remainingViewsCountForContractEntity));
      }
      catch (Exception ex)
      {
        return new JsonResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
      }

      return new JsonResult("Success") { StatusCode = StatusCodes.Status200OK };
    }
  }
}
