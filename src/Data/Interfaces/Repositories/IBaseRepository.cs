namespace Data.Interfaces.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class, IEntity
{
    public Task<TEntity?> GetById(int id, CancellationToken cancellationToken = default);
    public Task<IEnumerable<TEntity>> GetAll(CancellationToken cancellationToken = default);
    public void Add(TEntity entity);
    public void Update(TEntity entity);
    public void Delete(TEntity entity);
    public Task<int> SaveChanges(CancellationToken cancellationToken = default);
}