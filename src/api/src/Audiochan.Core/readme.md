# Audiochan.Core

This project contains code for the application and database logic, as well as cross-cutting concerns like helpers and shared models.

### Why is the database logic inside here instead of the Infrastructure layer?

Good question! Unfortunately the answer might be weak but it is good for my situation for now. Before, the database logic was actually inside the Infrastructure layer, and it would make sense to do so. You never know when you need to change the implementation of your database logic, and you want to create an abstraction for it so the logic that depends on it does not get affected. Also, since the project uses Entity Framework Core for its database access, I needed a way to get data from the application layer, so you would solve this by using the repository pattern. The repository pattern is a strategy for abstracting the database layer. 

```c#
public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    
    private readonly ISampleRepository _sampleRepository;
    public ISampleRepository Samples => _sampleRepository;
    
    public UnitOfWork (DbContext dbContext)
    {
        _sampleRepository = new SampleRepository(dbContext);
    }
}

public class SampleRepository : ISampleRepository
{
    private readonly DbContext _dbContext;
    private readonly DbSet<Sample> _dbSet;
    
    public SampleRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<Sample>();
    }
    
    public async Task<Sample> GetAll(CancellationToken ct = default)
    {
        return await _dbSet.ToListAsync(ct);
    }
    
    public async Task Add(Sample sample, Cancellation ct = default)
    {
        await _dbSet.AddAsync(sample, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
    
    public async Task Update(Sample sample, Cancellation ct = default)
    {
        _dbSet.Update(sample);
        await _dbContext.SaveChangesAsync(ct);
    }
    
    public async Task Remove(Sample sample, Cancellation ct = default)
    {
        _dbSet.Remove(sample);
        await _dbContext.SaveChangesAsync(ct);
    }
}
```

The issue with the repository pattern is that commands and queries may need to get or load differently. For instance, one query may need to get all of the fields for a resource, but another query may need a couple fields from a resource of the same type. Or certain commands need to load a resource with all the included relationships, while another command need to load with no included relationships. Basically, you don't want to fetch or load unnecessary data, unless you extend your repository pattern, but that's the thing, you need to keep on extending your repository. 

Another solution is to do what is demonstrated in the [Clean Architecture template](https://github.com/jasontaylordev/CleanArchitecture). You create an abstraction of the Entity Framework Core's DbContext, and add this abstraction in the application layer.

```c#
// Application layer
public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; set; }

    DbSet<TodoItem> TodoItems { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

// Infrastructure Layer
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; }

    public DbSet<TodoList> TodoLists { get; set; }
}
```

Now we abstracted Entity Framework Core from our application layer.... or did we?

If you look at the abstraction for the DBContext, you'll notice that we added DbSet properties. DbSet belongs to Entity Framework Core. So the application layer needs to depend on Entity Framework Core. So what is the point of having abstracting DbContext when the abstraction is leaking the implementation. This is known as a leaky abstraction.

This is when I started to question my sanity and why I chose to implement clean architecture. I like the concept of dependency inversion, and decoupling the application and infrastructure logic, but as you can tell, there are some disadvantages for my situation. So since I want to use Entity Framework Core, and want to use access Entity Framework Core from the application layer, why not just add the implementation in the application layer. This is fine for me, as long I am **100% COMMITTED** to using Entity Framework Core. 