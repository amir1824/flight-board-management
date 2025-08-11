using System.Linq.Expressions;
using System.Net;
using FlightBoard.Api.Dal.Interfaces;
using FlightBoard.Api.Dal.DbModels.BaseModels;
using FlightBoard.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;                
using FluentValidation.Results;        

[ApiController]
[Route("api/[controller]")]
public abstract class CrudControllerBase<T>(
    ICrudDal<T> crud,
    IUnitOfWork uow,
    ILogger<CrudControllerBase<T>> logger
) : ControllerBase where T : BaseModel
{
    protected readonly ICrudDal<T> _crud = crud;
    protected readonly IUnitOfWork _uow = uow;
    protected readonly ILogger _log = logger;

    protected virtual Expression<Func<T, bool>> BuildFilter() => _ => true;
    protected virtual Task BeforeCreateAsync(T model, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task BeforeUpdateAsync(T model, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task BeforeDeleteAsync(T entity, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task AfterCreateAsync(T entity, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task AfterUpdateAsync(T entity, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task AfterDeleteAsync(int id, CancellationToken ct) => Task.CompletedTask;

    protected virtual async Task ValidateAsync(T model, CancellationToken ct)
    {
        var validator = HttpContext?.RequestServices.GetService(typeof(IValidator<T>)) as IValidator<T>;
        if (validator is null) return;

        ValidationResult result = await validator.ValidateAsync(model, ct);
        if (result.IsValid) return;

        var hasConflict = result.Errors.Any(e =>
            string.Equals(e.ErrorCode, "Conflict", StringComparison.OrdinalIgnoreCase));

        var message = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
        throw new HttpException(hasConflict ? HttpStatusCode.Conflict : HttpStatusCode.BadRequest, message);
    }

    [NonAction]
    [HttpGet]
    public virtual async Task<IEnumerable<T>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);

        var skip = (page - 1) * pageSize;
        return await _crud.ListAsync(BuildFilter(), skip, pageSize, ct);
    }

    [HttpGet("{id:int}")]
    public virtual async Task<T> GetById(int id, CancellationToken ct = default)
    {
        var entity = await _crud.GetByIdAsync(id, ct);
        if (entity is null)
            throw new HttpException(HttpStatusCode.NotFound, $"{typeof(T).Name} with id {id} not found");

        return entity;
    }

    [HttpPost]
    public virtual async Task<T> Create([FromBody] T model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            throw new HttpException(HttpStatusCode.BadRequest, "Invalid model");

        await ValidateAsync(model, ct);            

        await BeforeCreateAsync(model, ct);
        await _crud.AddAsync(model, ct);
        await _uow.SaveChangesAsync(ct);
        await AfterCreateAsync(model, ct);

        return model;
    }

    [HttpPut("{id:int}")]
    public virtual async Task Update(int id, [FromBody] T model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            throw new HttpException(HttpStatusCode.BadRequest, "Invalid model");

        await ValidateAsync(model, ct);            

        if (model.Id != id)
            throw new HttpException(HttpStatusCode.BadRequest, "Id mismatch");

        var current = await _crud.GetByIdAsync(id, ct);
        if (current is null)
            throw new HttpException(HttpStatusCode.NotFound, $"{typeof(T).Name} with id {id} not found");

        await BeforeUpdateAsync(model, ct);
        await _crud.UpdateAsync(model, ct);
        await _uow.SaveChangesAsync(ct);
        await AfterUpdateAsync(model, ct);
    }

    [HttpDelete("{id:int}")]
    public virtual async Task Delete(int id, CancellationToken ct = default)
    {
        var entity = await _crud.GetByIdAsync(id, ct);
        if (entity is null)
            throw new HttpException(HttpStatusCode.NotFound, $"{typeof(T).Name} with id {id} not found");

        await BeforeDeleteAsync(entity, ct);
        await _crud.RemoveAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        await AfterDeleteAsync(id, ct);
    }

    [NonAction]
    [HttpGet("search")]
    public virtual Task Search(CancellationToken ct = default)
        => throw new HttpException(HttpStatusCode.NotFound, "Search is not implemented for this resource.");
}