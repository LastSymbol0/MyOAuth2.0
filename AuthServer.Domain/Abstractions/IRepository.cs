using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace AuthServer.Domain.Abstractions
{
    public interface IRepository<T> where T : class, IAggregateRoot
    {
        public DbContext Context { get; }
    }
}
