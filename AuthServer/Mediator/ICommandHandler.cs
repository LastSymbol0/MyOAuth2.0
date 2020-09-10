using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Mediator
{
    public interface ICommandHandler<TCommand, TResponce> where TCommand : ICommand<TResponce>
    {
        Task<TResponce> Handle(TCommand command);
    }
}
