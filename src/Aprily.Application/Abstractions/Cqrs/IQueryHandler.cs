using Aprily.SharedKernel;

namespace Aprily.Application.Abstractions.Cqrs;

public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken ct);
}
