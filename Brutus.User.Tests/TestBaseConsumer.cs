using System;
using System.Threading.Tasks;
using MassTransit.Testing;

namespace Brutus.User.Tests
{
    public abstract class TestBaseConsumer
    {
        protected readonly InMemoryTestHarness Harness;

        protected TestBaseConsumer()
        {
            Harness = new InMemoryTestHarness();
        }
        
        protected async Task RunTest(Func<Task> testBodyFunc)
        {
            await Harness.Start();
            try
            {
                await testBodyFunc();
            }
            finally
            {
                await Harness.Stop();
            }
        }
    }
}
