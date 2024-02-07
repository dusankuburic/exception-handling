namespace Tegla.Persistence.Brokers.Storages;

public partial class StorageBroker : IStorageBroker
{
    private readonly ApplicationDbContext _ctx;
    public StorageBroker(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }
}
