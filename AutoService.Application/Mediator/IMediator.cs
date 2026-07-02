namespace AutoService.Application.Mediator
{
    public interface IMediator
    {
        Task Send<TRequest>(TRequest request)
            where TRequest : IRequest;

        Task<TResponse> Send<TResponse>(IRequest<TResponse> request);
    }
}

