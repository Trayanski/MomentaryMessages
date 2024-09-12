using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MomentaryMessages.Data.DataTransferObjects;
using MomentaryMessages.Data.Services;

namespace MomentaryMessages.Controllers
{
  public class SuperSecretController : Controller
  {
    private readonly SecretViewLogsService m_service;

    public SuperSecretController(SecretViewLogsService service)
    {
      m_service = service;
    }

    // GET: SuperSecret
    public async Task<IActionResult> Index()
    {
      var dtos = await m_service.GetAllAsync();
      return View(dtos);
    }

    // GET: SuperSecret/Details/5
    public async Task<IActionResult> Details(string id)
    {
      if (id == null)
        return NotFound();

      var dtos = await m_service.GetAllAsync();
      var secretViewLogModel = dtos.FirstOrDefault(m => m.ViewerMachineName == id);
      if (secretViewLogModel == null)
        return NotFound();

      return View(secretViewLogModel);
    }

    // GET: SuperSecret/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: SuperSecret/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ViewerMachineName,InitialViewDate,ViewsCount")] SecretViewLogDto secretViewLogModel)
    {
      if (ModelState.IsValid)
      {
        await m_service.AddAsync(secretViewLogModel);
        return RedirectToAction(nameof(Index));
      }

      return View(secretViewLogModel);
    }

    // GET: SuperSecret/Edit/5
    public async Task<IActionResult> Edit(string viewerMachineName)
    {
      if (viewerMachineName == null)
        return NotFound();

      var secretViewLogModel = await m_service.GetByViewerMachineNameAsync(viewerMachineName);
      if (secretViewLogModel == null)
        return NotFound();

      return View(secretViewLogModel);
    }

    // POST: SuperSecret/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string viewerMachineName, [Bind("ViewerMachineName,InitialViewDate,ViewsCount")] SecretViewLogDto secretViewLogModel)
    {
      if (viewerMachineName != secretViewLogModel.ViewerMachineName)
        return NotFound();

      if (ModelState.IsValid)
      {
        try
        {
          await m_service.UpdateAsync(viewerMachineName, secretViewLogModel);
        }
        catch (DbUpdateConcurrencyException)
        {
          if (await SecretViewLogModelExists(secretViewLogModel.ViewerMachineName))
            return NotFound();
          else
            throw;
        }

        return RedirectToAction(nameof(Index));
      }

      return View(secretViewLogModel);
    }

    // GET: SuperSecret/Delete/5
    public async Task<IActionResult> Delete(string viewerMachineName)
    {
      if (viewerMachineName == null)
        return NotFound();

      var dtos = await m_service.GetAllAsync();
      var secretViewLogModel = dtos.FirstOrDefault(m => m.ViewerMachineName == viewerMachineName);
      if (secretViewLogModel == null)
        return NotFound();

      return View(secretViewLogModel);
    }

    // POST: SuperSecret/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string viewerMachineName)
    {
      var dtos = await m_service.GetAllAsync();
      var secretViewLogModel = dtos.FirstOrDefault(m => m.ViewerMachineName == viewerMachineName);
      if (secretViewLogModel != null)
        await m_service.DeleteAsync(viewerMachineName);

      return RedirectToAction(nameof(Index));
    }

    private async Task<bool> SecretViewLogModelExists(string viewerMachineName)
    {
      var dtos = await m_service.GetAllAsync();
      return dtos.Any(e => e.ViewerMachineName == viewerMachineName);
    }
  }
}
