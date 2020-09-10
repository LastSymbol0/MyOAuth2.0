using AuthServer.Domain.AggregatesModel.SessionAggregate;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthServer.Infrastructure.Repository
{
    public class SessionsContext : DbContext
    {
        public SessionsContext(DbContextOptions<SessionsContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var splitStringConverter = new ValueConverter<IList<ScopeAccess>, string>(
                v => string.Join(";", v),
                v => v.Split(new[] { ';' })
                      .Select(i => new ScopeAccess(i)).ToList());

            var valueComparer = new ValueComparer<IList<ScopeAccess>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => (IList<ScopeAccess>)c.ToList());

            modelBuilder.Entity<Session>()
                .OwnsOne(i => i.AccessParameters)
                .Property(nameof(AccessParameters.Scopes))
                    .HasConversion(splitStringConverter)
                    .Metadata.SetValueComparer(valueComparer);

            modelBuilder.Entity<Session>()
                .Property(i => i.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Client>()
                .Property(i => i.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<ResourceOwner>()
                .Property(i => i.Id)
                .ValueGeneratedNever();
        }

        public DbSet<Session> Sessions { get; set; }
    }
}
