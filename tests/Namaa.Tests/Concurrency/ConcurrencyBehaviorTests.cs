using FluentValidation;
using MediatR;
using Namaa.Application.Abstractions.Behaviors;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Common.Concurrency;

namespace Namaa.Tests.Concurrency;

public sealed class ConcurrencyBehaviorTests
{
    [Fact]
    public async Task HandleStoresEightByteTokenForHandlerAndClearsItAfterward()
    {
        byte[] expected = [1, 2, 3, 4, 5, 6, 7, 8];
        var command = new TestCommand(Convert.ToBase64String(expected));
        var context = new RecordingConcurrencyTokenContext();
        var behavior = new ConcurrencyBehavior<TestCommand, bool>(context);
        var handlerReached = false;

        var response = await behavior.Handle(
            command,
            () =>
            {
                handlerReached = true;
                Assert.Equal(expected, context.CurrentToken);
                return Task.FromResult(true);
            },
            CancellationToken.None);

        Assert.True(response);
        Assert.True(handlerReached);
        Assert.Equal(1, context.UseCount);
        Assert.Equal(1, context.ClearCount);
        Assert.Null(context.CurrentToken);
    }

    [Fact]
    public async Task HandleRejectsMalformedTokenBeforeHandler()
    {
        var command = new TestCommand(Convert.ToBase64String(new byte[7]));
        var context = new RecordingConcurrencyTokenContext();
        var behavior = new ConcurrencyBehavior<TestCommand, bool>(context);
        var handlerReached = false;

        await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(
            command,
            () =>
            {
                handlerReached = true;
                return Task.FromResult(true);
            },
            CancellationToken.None));

        Assert.False(handlerReached);
        Assert.Equal(0, context.UseCount);
        Assert.Equal(0, context.ClearCount);
    }

    [Fact]
    public async Task HandleClearsTokenWhenHandlerThrows()
    {
        var command = new TestCommand(Convert.ToBase64String(new byte[8]));
        var context = new RecordingConcurrencyTokenContext();
        var behavior = new ConcurrencyBehavior<TestCommand, bool>(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => behavior.Handle(
            command,
            () => throw new InvalidOperationException("Handler failed."),
            CancellationToken.None));

        Assert.Equal(1, context.UseCount);
        Assert.Equal(1, context.ClearCount);
        Assert.Null(context.CurrentToken);
    }

    private sealed record TestCommand(string RowVersion) : IRequest<bool>, IConcurrencyCommand;

    private sealed class RecordingConcurrencyTokenContext : IConcurrencyTokenContext
    {
        public byte[]? CurrentToken { get; private set; }
        public int UseCount { get; private set; }
        public int ClearCount { get; private set; }

        public void UseExpectedRowVersion(byte[] rowVersion)
        {
            UseCount++;
            CurrentToken = rowVersion.ToArray();
        }

        public void ClearExpectedRowVersion()
        {
            ClearCount++;
            CurrentToken = null;
        }
    }
}
