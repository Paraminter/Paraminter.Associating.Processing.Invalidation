namespace Paraminter.Associating.Processing.Invalidation;

using Moq;

using Paraminter.Associating.Commands;
using Paraminter.Associating.Models;
using Paraminter.Processing.Invalidation.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

public sealed class Handle
{
    [Fact]
    public async Task NullCommand_ThrowsArgumentNullException()
    {
        var fixture = FixtureFactory.Create<IAssociateArgumentsData>();

        var result = await Record.ExceptionAsync(() => Target(fixture, null!, CancellationToken.None));

        Assert.IsType<ArgumentNullException>(result);
    }

    [Fact]
    public async Task ValidCommand_ResetsInvalidityBefore()
    {
        var fixture = FixtureFactory.Create<IAssociateArgumentsData>();

        var command = Mock.Of<IAssociateArgumentsCommand<IAssociateArgumentsData>>();

        var sequence = new MockSequence();

        fixture.InvalidityResetterMock.InSequence(sequence).Setup(static (handler) => handler.Handle(It.IsAny<IResetProcessInvalidityCommand>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        fixture.DecorateeMock.InSequence(sequence).Setup((handler) => handler.Handle(command, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await Target(fixture, command, CancellationToken.None);

        fixture.InvalidityResetterMock.Verify(static (handler) => handler.Handle(It.IsAny<IResetProcessInvalidityCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        fixture.DecorateeMock.Verify((handler) => handler.Handle(command, It.IsAny<CancellationToken>()), Times.Once());
    }

    private static async Task Target<TData>(
        IFixture<TData> fixture,
        IAssociateArgumentsCommand<TData> command,
        CancellationToken cancellationToken)
        where TData : IAssociateArgumentsData
    {
        await fixture.Sut.Handle(command, cancellationToken);
    }
}
