using MediatR;

namespace Sharecode.Backend.Application.Base;

public interface IAppRequest : IRequest, IRequestBase { }
public interface IAppRequest<TResponse> : IRequest<TResponse>, IRequestBase { }
