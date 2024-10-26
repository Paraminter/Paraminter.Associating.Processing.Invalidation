namespace Paraminter.Associating.Processing.Invalidation;

using Moq;

using Paraminter.Associating.Commands;
using Paraminter.Associating.Models;
using Paraminter.Processing.Commands;
using Paraminter.Processing.Invalidation.Queries;

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
    public async Task ValidCommand_Invalidated_DoesNotSetCompletion()
    {
        var fixture = FixtureFactory.Create<IAssociateArgumentsData>();

        var command = Mock.Of<IAssociateArgumentsCommand<IAssociateArgumentsData>>();

        fixture.DecorateeMock.Setup((handler) => handler.Handle(command, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        fixture.InvalidityReaderMock.Setup(static (handler) => handler.Handle(It.IsAny<IIsProcessInvalidatedQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

        await Target(fixture, command, CancellationToken.None);

        fixture.CompletionSetterMock.Verify(static (handler) => handler.Handle(It.IsAny<ISetProcessCompletionCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        fixture.DecorateeMock.Verify((handler) => handler.Handle(command, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task ValidCommand_NotInvalidated_SetsCompletionAfter()
    {
        var fixture = FixtureFactory.Create<IAssociateArgumentsData>();

        var command = Mock.Of<IAssociateArgumentsCommand<IAssociateArgumentsData>>();
        var sequence = new MockSequence();

        fixture.DecorateeMock.InSequence(sequence).Setup((handler) => handler.Handle(command, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        fixture.CompletionSetterMock.InSequence(sequence).Setup(static (handler) => handler.Handle(It.IsAny<ISetProcessCompletionCommand>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        fixture.InvalidityReaderMock.Setup(static (handler) => handler.Handle(It.IsAny<IIsProcessInvalidatedQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(false));

        await Target(fixture, command, CancellationToken.None);

        fixture.DecorateeMock.Verify((handler) => handler.Handle(command, It.IsAny<CancellationToken>()), Times.Once());
        fixture.CompletionSetterMock.Verify(static (handler) => handler.Handle(It.IsAny<ISetProcessCompletionCommand>(), It.IsAny<CancellationToken>()), Times.Once());
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
