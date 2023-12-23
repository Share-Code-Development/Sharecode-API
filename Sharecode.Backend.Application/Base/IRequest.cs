using MediatR;

namespace Sharecode.Backend.Application.Base;

public interface IAppRequest : IRequest, IRequestBase { }
public interface IAppRequest<out TResponse> : IRequest<TResponse>, IRequestBase { }
