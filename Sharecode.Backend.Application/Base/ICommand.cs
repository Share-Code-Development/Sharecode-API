using MediatR;

namespace Sharecode.Backend.Application.Base;

public interface ICommand : IRequest, ICommandBase
{
    
}

public interface ICommand<TResponse> : IRequest<TResponse>, ICommandBase
{
    
}