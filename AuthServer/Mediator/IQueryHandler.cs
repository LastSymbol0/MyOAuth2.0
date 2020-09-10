using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Mediator
{
    public interface IQueryHandler<TQuery, TResponce> where TQuery : IQuery<TResponce>
    {
        Task<TResponce> Handle(TQuery query);
    }
}
