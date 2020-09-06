using MediatR;

namespace Brutus.Core
{
    public interface ICommandHandler<in T>:IRequestHandler<T> where T:ICommand {}
}