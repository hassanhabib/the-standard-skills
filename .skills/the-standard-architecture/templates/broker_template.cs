// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Standard-compliant Storage Broker
// Replace [Entity] with the entity name (e.g., Student)
// Replace [Resource] with the resource name (e.g., Storage, Api, Queue)

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace [Namespace].Brokers.Storages
{
    public partial class StorageBroker : DbContext, IStorageBroker
    {
        public DbSet<[Entity]> [Entities] { get; set; }
    }
}

namespace [Namespace].Brokers.Storages
{
    public partial class StorageBroker
    {
        public async ValueTask<[Entity]> Insert[Entity]Async([Entity] [entity]) =>
            await this.InsertAsync([entity]);

        public IQueryable<[Entity]> SelectAll[Entities]() =>
            this.SelectAll<[Entity]>();

        public async ValueTask<[Entity]> Select[Entity]ByIdAsync(Guid [entity]Id) =>
            await this.SelectAsync<[Entity]>([entity]Id);

        public async ValueTask<[Entity]> Update[Entity]Async([Entity] [entity]) =>
            await this.UpdateAsync([entity]);

        public async ValueTask<[Entity]> Delete[Entity]Async([Entity] [entity]) =>
            await this.DeleteAsync([entity]);
    }
}

namespace [Namespace].Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<[Entity]> Insert[Entity]Async([Entity] [entity]);
        IQueryable<[Entity]> SelectAll[Entities]();
        ValueTask<[Entity]> Select[Entity]ByIdAsync(Guid [entity]Id);
        ValueTask<[Entity]> Update[Entity]Async([Entity] [entity]);
        ValueTask<[Entity]> Delete[Entity]Async([Entity] [entity]);
    }
}
