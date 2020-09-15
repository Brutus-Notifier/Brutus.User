using MassTransit;

namespace Brutus.Core
{
    public interface ICommandHandler<in T>:IConsumer<T> where T: class, ICommand {}
}