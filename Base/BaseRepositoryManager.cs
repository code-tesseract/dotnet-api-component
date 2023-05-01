using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Component.Base;

public interface IBaseRepositoryManager<TContext> where TContext : DbContext
{
	void                  Save();
	IDbContextTransaction Transaction();
	void                  Commit(IDbContextTransaction   tr);
	void                  Rollback(IDbContextTransaction tr);

	Task                        SaveAsync(CancellationToken         ct);
	Task<IDbContextTransaction> TransactionAsync(CancellationToken  ct);
	Task                        CommitAsync(IDbContextTransaction   tr, CancellationToken ct);
	Task                        RollbackAsync(IDbContextTransaction tr, CancellationToken ct);
}

public class BaseRepositoryManager<TContext> : IBaseRepositoryManager<TContext> where TContext : DbContext
{
	private readonly TContext _context;
	protected BaseRepositoryManager(TContext context) => _context = context;

	/* Synchronous method implementation */
	public void                  Save()                             => _context.SaveChanges();
	public IDbContextTransaction Transaction()                      => _context.Database.BeginTransaction();
	public void                  Commit(IDbContextTransaction   tr) => tr.Commit();
	public void                  Rollback(IDbContextTransaction tr) => tr.Rollback();

	/* Asynchronous method implementation */
	public async Task SaveAsync(CancellationToken ct) => await _context.SaveChangesAsync(ct);

	public async Task<IDbContextTransaction> TransactionAsync(CancellationToken ct) =>
		await _context.Database.BeginTransactionAsync(ct);

	public async Task CommitAsync(IDbContextTransaction   tr, CancellationToken ct) => await tr.CommitAsync(ct);
	public async Task RollbackAsync(IDbContextTransaction tr, CancellationToken ct) => await tr.RollbackAsync(ct);
}