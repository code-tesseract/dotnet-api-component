using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Component.Base;

public interface IBaseRepository<T>
{
    IQueryable<T> Find(Expression<Func<T, bool>>? exp = null);
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task CreateAsync(T entity, CancellationToken ct);
}

public class BaseRepository<TContext, T> : IBaseRepository<T> where T : class where TContext : DbContext
{
    private readonly TContext _context;
    protected BaseRepository(TContext context) => _context = context;
    
    public IQueryable<T> Find(Expression<Func<T, bool>>? exp = null)
        => exp == null ? _context.Set<T>() : _context.Set<T>().Where(exp);

    public void Create(T entity) => _context.Set<T>().Add(entity);

    public void Update(T entity) => _context.Set<T>().Update(entity);

    public void Delete(T entity) => _context.Set<T>().Remove(entity);

    public async Task CreateAsync(T entity, CancellationToken ct) => await _context.Set<T>().AddAsync(entity, ct);
}