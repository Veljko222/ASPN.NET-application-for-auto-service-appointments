using AutoService.Application.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace AutoService.Web.Mediator
{
    public class SimpleMediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public SimpleMediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Send<TRequest>(TRequest request)
            where TRequest : IRequest
        {
            var handler = _serviceProvider
                .GetRequiredService<IRequestHandler<TRequest>>();

            await handler.Handle(request);
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            var handlerType = typeof(IRequestHandler<,>)
                .MakeGenericType(request.GetType(), typeof(TResponse));

            dynamic handler = _serviceProvider.GetRequiredService(handlerType);

            return await handler.Handle((dynamic)request);
        }
    }
}

