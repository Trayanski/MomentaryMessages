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
      string validatedUserName = User.IsInRole("Admin")
        && userName != null
        && !userName.IsNullOrEmpty()
          ? userName
          : User?.Identity?.Name ?? string.Empty;
      var dtos = await m_service.GetAllAsync();
      if (dtos.Any(x => x.ViewerName == userName))
        return View(nameof(Index), "There are no secrets here");

      await m_service.AddAsync(new SecretViewLogDto() { ViewerName = userName });
      return View(nameof(Index), $"You have found the secret {userName}!");
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
  }
}
