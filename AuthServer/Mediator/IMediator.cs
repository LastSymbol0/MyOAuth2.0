using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Mediator
{
    public interface IMediator
    {
        public Task<TResponce> Exe<TResponce>(ICommand<TResponce> command);

        public Task<TResponce> Execute<TCommand, TResponce>(TCommand command)
            where TCommand : ICommand<TResponce>;

        public Task<TResponce> Get<TQuery, TResponce>(TQuery query)
            where TQuery : IQuery<TResponce>;
    }
}
