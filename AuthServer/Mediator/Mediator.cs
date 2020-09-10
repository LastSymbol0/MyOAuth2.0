using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AuthServer.Mediator
{
    public class Mediator : IMediator
    {
        private IServiceProvider ServiceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public async Task<TResponce> Execute<TCommand, TResponce>(TCommand command)
            where TCommand : ICommand<TResponce>
        {
            var handler = ServiceProvider.GetService<ICommandHandler<TCommand, TResponce>>();

            return await handler.Handle(command);
        }

        public async Task<TResponce> Get<TQuery, TResponce>(TQuery query)
            where TQuery : IQuery<TResponce>
        {
            var handler = ServiceProvider.GetService<IQueryHandler<TQuery, TResponce>>();

            return await handler.Handle(query);
        }
    }

    static class Extensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddTransient<IMediator, Mediator>();

            return services;
        }

        public static IServiceCollection AddCommand<TCommand, THandler>(this IServiceCollection services)
            where TCommand : ICommand<bool>
            where THandler : class, ICommandHandler<TCommand, bool>
        {
            services.AddTransient<ICommandHandler<TCommand, bool>, THandler>();
            return services;
        }

        public static IServiceCollection AddCommand<TCommand, THandler, TResponce>(this IServiceCollection services)
            where TCommand : ICommand<TResponce>
            where THandler : class, ICommandHandler<TCommand, TResponce>
        {
            services.AddTransient<ICommandHandler<TCommand, TResponce>, THandler>();
            return services;
        }

        public static IServiceCollection AddQuery<TQuery, THandler, TResponce>(this IServiceCollection services)
            where TQuery : IQuery<TResponce>
            where THandler : class, IQueryHandler<TQuery, TResponce>
        {
            services.AddTransient<IQueryHandler<TQuery, TResponce>, THandler>();
            return services;
        }
    }
}
