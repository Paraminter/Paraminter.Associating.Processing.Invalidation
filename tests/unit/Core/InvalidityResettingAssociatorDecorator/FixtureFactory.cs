namespace Paraminter.Associating.Processing.Invalidation;

using Moq;

using Paraminter.Associating.Commands;
using Paraminter.Associating.Models;
using Paraminter.Cqs;
using Paraminter.Processing.Invalidation.Commands;

internal static class FixtureFactory
{
    public static IFixture<TData> Create<TData>()
        where TData : IAssociateArgumentsData
    {
        Mock<ICommandHandler<IAssociateArgumentsCommand<TData>>> decorateeMock = new(MockBehavior.Strict);

        Mock<ICommandHandler<IResetProcessInvalidityCommand>> invalidityResetterMock = new(MockBehavior.Strict);

        var sut = new InvalidityResettingAssociatorDecorator<TData>(decorateeMock.Object, invalidityResetterMock.Object);

        return new Fixture<TData>(sut, decorateeMock, invalidityResetterMock);
    }

    private sealed class Fixture<TData>
        : IFixture<TData>
        where TData : IAssociateArgumentsData
    {
        private readonly ICommandHandler<IAssociateArgumentsCommand<TData>> Sut;

        private readonly Mock<ICommandHandler<IAssociateArgumentsCommand<TData>>> DecorateeMock;

        private readonly Mock<ICommandHandler<IResetProcessInvalidityCommand>> InvalidityResetterMock;

        public Fixture(
            ICommandHandler<IAssociateArgumentsCommand<TData>> sut,
            Mock<ICommandHandler<IAssociateArgumentsCommand<TData>>> decorateeMock,
            Mock<ICommandHandler<IResetProcessInvalidityCommand>> invalidityResetterMock)
        {
            Sut = sut;

            DecorateeMock = decorateeMock;

            InvalidityResetterMock = invalidityResetterMock;
        }

        ICommandHandler<IAssociateArgumentsCommand<TData>> IFixture<TData>.Sut => Sut;

        Mock<ICommandHandler<IAssociateArgumentsCommand<TData>>> IFixture<TData>.DecorateeMock => DecorateeMock;

        Mock<ICommandHandler<IResetProcessInvalidityCommand>> IFixture<TData>.InvalidityResetterMock => InvalidityResetterMock;
    }
}
