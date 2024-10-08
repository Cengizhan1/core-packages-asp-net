﻿using System.Transactions;
using MediatR;

namespace Core.Application.Pipelines.Transaction;

public class TransactionScopeBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>,ITransactionalRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);
        TResponse response;
        try
        {
            response = await next();
            scope.Complete();
        }
        catch (Exception)
        {
            scope.Dispose();
            throw;
        }
        return response;
    }
}
