using Microsoft.AspNetCore.Mvc;
using MomentaryMessages.Data.Services;
using MomentaryMessages.Helper;

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
      var secretMessage = await SuperSecretHelper.GetSecretMessage(m_service, User, userName);
      return new JsonResult(secretMessage);
    }
  }
}
