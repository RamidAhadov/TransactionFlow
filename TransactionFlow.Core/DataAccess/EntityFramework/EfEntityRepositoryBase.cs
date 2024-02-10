using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TransactionFlow.Entities;

namespace TransactionFlow.Core.DataAccess.EntityFramework;

public class EfEntityRepositoryBase<TEntity,TContext>:IEntityRepository<TEntity>
where TEntity: class,IEntity,new()
where TContext: DbContext, new()
{
    public List<TEntity> GetList(Expression<Func<TEntity, bool>>? filter = null)
    {
        using var context = new TContext();
        return filter == null ? context.Set<TEntity>().ToList() : context.Set<TEntity>().Where(filter).ToList();
    }

    public TEntity? Get(Expression<Func<TEntity, bool>> filter)
    {
        using var context = new TContext();
        return context.Set<TEntity>().SingleOrDefault(filter);
    }

    public void Add(TEntity entity)
    {
        using var context = new TContext();
        var addedEntity = context.Entry(entity);
        addedEntity.State = EntityState.Added;
        context.SaveChanges();
    }

    public void Update(TEntity entity)
    {
        using var context = new TContext();
        var modifiedEntity = context.Entry(entity);
        modifiedEntity.State = EntityState.Modified;
        context.SaveChanges();
    }

    public void Delete(TEntity entity)
    {
        using var context = new TContext();
        var deletedEntity = context.Entry(entity);
        deletedEntity.State = EntityState.Deleted;
        context.SaveChanges();
    }

    public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? filter = null)
    {
        await using var context = new TContext();

        var query = context.Set<TEntity>().AsQueryable();

        if (filter != null) query = query.Where(filter);

        return await query.ToListAsync().ConfigureAwait(false);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter)
    {
        await using var context = new TContext();
        return await context.Set<TEntity>().SingleOrDefaultAsync(filter).ConfigureAwait(false);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await using var context = new TContext();
        var addedEntity = context.Entry(entity);
        addedEntity.State = EntityState.Added;
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        await using var context = new TContext();
        var modifiedEntity = context.Entry(entity);
        modifiedEntity.State = EntityState.Modified;
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        await using var context = new TContext();
        var deletedEntity = context.Entry(entity);
        deletedEntity.State = EntityState.Deleted;
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> DeleteAsync(Expression<Func<TEntity,bool>> filter)
    {
        await using var context = new TContext();
        var deletedEntity = context.Entry( await context.Set<TEntity>().SingleOrDefaultAsync(filter));
        deletedEntity.State = EntityState.Deleted;
        await context.SaveChangesAsync();
        return deletedEntity.Entity;
    }
}