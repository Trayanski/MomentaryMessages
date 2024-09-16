using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MomentaryMessages.Data.DataTransferObjects;
using MomentaryMessages.Data.Models;

namespace MomentaryMessages.Data.Services
{

  public class SecretViewLogsService
  {
    private const string c_cacheKey = $"{nameof(SecretViewLogsService)}/{nameof(GetAllAsync)}";
    private IMemoryCache m_cache;
    private ApplicationDbContext m_context;

    public SecretViewLogsService(ApplicationDbContext context, IMemoryCache cache)
    {
      m_context = context;
      m_cache = cache;
    }

    /// <summary>Gets all entities asynchronously as data transfer objects.</summary>
    /// <returns>All entities as data transfer objects.</returns>
    public async Task<IEnumerable<SecretViewLogDto>> GetAllAsync()
    {
      var dtos = await GetAllAsyncHelper();
      return dtos;
    }

    /// <summary>Gets an entity asynchronously as data transfer object.</summary>
    /// <param name="viewerName">The viewer name of a potentially stored entity.</param>
    /// <returns>An entity as data transfer object or null if entity with provided id is not found.</returns>
    public async Task<SecretViewLogDto> GetByViewerNameAsync(string viewerName)
    {
      var entities = await GetAllAsyncHelper();
      return entities.FirstOrDefault(x => x.ViewerName == viewerName);
    }

    /// <summary>Adds an entity asynchronously to the database.</summary>
    /// <param name="dto">The data transfer object from which an entity can be created.</param>
    /// <returns>The id of the newly created entity.</returns>
    public virtual async Task<string> AddAsync(SecretViewLogDto dto)
    {
      var model = MapToModel(dto);
      await m_context.SecretViewLogs.AddAsync(model);
      await m_context.SaveChangesAsync();
      ClearCache();
      return model.ViewerName;
    }

    /// <summary>
    /// Updates an entity with the provided data transfer object data.
    /// Note: Normally this method contains an id as the first argument, 
    ///   but i didn't want to mess with the predefined logic, so i left it like that.
    /// </summary>
    /// <param name="updatedEntityDto">The updated entity as data transfer object.</param>
    public virtual async Task UpdateAsync(string viewerName, SecretViewLogDto updatedEntityDto)
    {
      var entityDto = await GetByViewerNameAsync(viewerName);
      if (entityDto == null)
        return;

      var entity = await m_context.SecretViewLogs.FindAsync(viewerName);
      if (entity == null)
      {
        var newEntity = m_context.SecretViewLogs.Entry(MapToModel(updatedEntityDto));
        newEntity.State = EntityState.Modified;
      }
      else
      {
        var existingEntity = m_context.Entry(entity);
        existingEntity.CurrentValues.SetValues(MapToModel(updatedEntityDto));
        existingEntity.State = EntityState.Modified;
      }

      await m_context.SaveChangesAsync();
      ClearCache();
    }

    /// <summary>Deletes an entity with provided id.</summary>
    /// <param name="viewerName">The viewer name of the entity that is going to be deleted.</param>
    public virtual async Task DeleteAsync(string viewerName)
    {
      var entityDto = await GetByViewerNameAsync(viewerName);
      if (entityDto == null)
        return;

      var entityEntry = m_context.SecretViewLogs.Entry(new SecretViewLog { ViewerName = viewerName });
      entityEntry.State = EntityState.Deleted;
      await m_context.SaveChangesAsync();
      ClearCache();
    }

    /// <summary>Generates a data transfer object from a model object.</summary>
    /// <param name="model">The model object.</param>
    /// <returns>The data transfer object object.</returns>
    public SecretViewLogDto MapToDto(SecretViewLog model)
    {
      return new SecretViewLogDto
      {
        ViewerName = model.ViewerName,
        InitialViewDate = model.InitialViewDate,
        ViewsCount = model.ViewsCount
      };
    }

    /// <summary>Generates a model object from a data transfer object.</summary>
    /// <param name="dto">The data transfer object.</param>
    /// <returns>The model object.</returns>
    public SecretViewLog MapToModel(SecretViewLogDto dto)
    {
      return new SecretViewLog
      {
        ViewerName = dto.ViewerName,
        InitialViewDate = dto.InitialViewDate,
        ViewsCount = dto.ViewsCount
      };
    }

    private async Task<List<SecretViewLogDto>> GetAllAsyncHelper()
    {
      if (!m_cache.TryGetValue(c_cacheKey, out List<SecretViewLogDto> dtos))
      {
        var entities = await m_context.SecretViewLogs.ToListAsync();
        dtos = entities.Select(MapToDto).ToList();
        m_cache.Set(c_cacheKey, dtos);
      }

      return dtos;
    }

    private void ClearCache() => m_cache.Remove(c_cacheKey);
  }
}
