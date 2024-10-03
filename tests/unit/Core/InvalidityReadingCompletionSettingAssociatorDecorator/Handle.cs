namespace Paraminter.Associating.Processing.Invalidation;

using Moq;

using Paraminter.Associating.Commands;
using Paraminter.Associating.Models;
using Paraminter.Processing.Commands;
using Paraminter.Processing.Invalidation.Queries;

using System;

using Xunit;

public sealed class Handle
{
    [Fact]
    public void NullCommand_ThrowsArgumentNullException()
    {
        var fixture = FixtureFactory.Create<IAssociateArgumentsData>();

        var result = Record.Exception(() => Target(fixture, null!));

        Assert.IsType<ArgumentNullException>(result);
    }

    [Fact]
    public void ValidCommand_Invalidated_DoesNotSetCompletion()
    {
        var fixture = FixtureFactory.Create<IAssociateArgumentsData>();

        var command = Mock.Of<IAssociateArgumentsCommand<IAssociateArgumentsData>>();

        fixture.DecorateeMock.Setup((handler) => handler.Handle(command));

        fixture.InvalidityReaderMock.Setup(static (handler) => handler.Handle(It.IsAny<IIsProcessInvalidatedQuery>())).Returns(true);

        Target(fixture, command);

        fixture.CompletionSetterMock.Verify(static (handler) => handler.Handle(It.IsAny<ISetProcessCompletionCommand>()), Times.Never());
        fixture.DecorateeMock.Verify((handler) => handler.Handle(command), Times.Once());
    }

    [Fact]
    public void ValidCommand_NotInvalidated_SetsCompletionAfter()
    {
        var fixture = FixtureFactory.Create<IAssociateArgumentsData>();

        var command = Mock.Of<IAssociateArgumentsCommand<IAssociateArgumentsData>>();
        var sequence = new MockSequence();

        fixture.DecorateeMock.InSequence(sequence).Setup((handler) => handler.Handle(command));
        fixture.CompletionSetterMock.InSequence(sequence).Setup(static (handler) => handler.Handle(It.IsAny<ISetProcessCompletionCommand>()));

        fixture.InvalidityReaderMock.Setup(static (handler) => handler.Handle(It.IsAny<IIsProcessInvalidatedQuery>())).Returns(false);

        Target(fixture, command);

        fixture.DecorateeMock.Verify((handler) => handler.Handle(command), Times.Once());
        fixture.CompletionSetterMock.Verify(static (handler) => handler.Handle(It.IsAny<ISetProcessCompletionCommand>()), Times.Once());
    }

    private static void Target<TData>(
        IFixture<TData> fixture,
        IAssociateArgumentsCommand<TData> command)
        where TData : IAssociateArgumentsData
    {
        fixture.Sut.Handle(command);
    }
}
