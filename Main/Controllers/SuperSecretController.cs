using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MomentaryMessages.Data.DataTransferObjects;
using MomentaryMessages.Data.Services;
using MomentaryMessages.Helper;
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
      var secretMessage = await SuperSecretHelper.GetSecretMessage(m_service, User, userName);
      return View(nameof(Index), secretMessage);
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
