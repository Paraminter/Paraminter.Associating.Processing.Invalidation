namespace Paraminter.Associating.Processing.Invalidation;

using Moq;

using Paraminter.Associating.Commands;
using Paraminter.Associating.Models;
using Paraminter.Cqs;
using Paraminter.Processing.Commands;
using Paraminter.Processing.Invalidation.Queries;

internal static class FixtureFactory
{
    public static IFixture<TData> Create<TData>()
        where TData : IAssociateArgumentsData
    {
        Mock<ICommandHandler<IAssociateArgumentsCommand<TData>>> decorateeMock = new(MockBehavior.Strict);

        Mock<ICommandHandler<ISetProcessCompletionCommand>> completionSetterMock = new(MockBehavior.Strict);
        Mock<IQueryHandler<IIsProcessInvalidatedQuery, bool>> invalidityReaderMock = new(MockBehavior.Strict);

        var sut = new InvalidityReadingCompletionSettingAssociatorDecorator<TData>(decorateeMock.Object, completionSetterMock.Object, invalidityReaderMock.Object);

        return new Fixture<TData>(sut, decorateeMock, completionSetterMock, invalidityReaderMock);
    }

    private sealed class Fixture<TData>
        : IFixture<TData>
        where TData : IAssociateArgumentsData
    {
        private readonly ICommandHandler<IAssociateArgumentsCommand<TData>> Sut;

        private readonly Mock<ICommandHandler<IAssociateArgumentsCommand<TData>>> DecorateeMock;

        private readonly Mock<ICommandHandler<ISetProcessCompletionCommand>> CompletionSetterMock;
        private readonly Mock<IQueryHandler<IIsProcessInvalidatedQuery, bool>> InvalidityReaderMock;

        public Fixture(
            ICommandHandler<IAssociateArgumentsCommand<TData>> sut,
            Mock<ICommandHandler<IAssociateArgumentsCommand<TData>>> decorateeMock,
            Mock<ICommandHandler<ISetProcessCompletionCommand>> completionSetterMock,
            Mock<IQueryHandler<IIsProcessInvalidatedQuery, bool>> invalidityReaderMock)
        {
            Sut = sut;

            DecorateeMock = decorateeMock;

            CompletionSetterMock = completionSetterMock;
            InvalidityReaderMock = invalidityReaderMock;
        }

        ICommandHandler<IAssociateArgumentsCommand<TData>> IFixture<TData>.Sut => Sut;

        Mock<ICommandHandler<IAssociateArgumentsCommand<TData>>> IFixture<TData>.DecorateeMock => DecorateeMock;

        Mock<ICommandHandler<ISetProcessCompletionCommand>> IFixture<TData>.CompletionSetterMock => CompletionSetterMock;
        Mock<IQueryHandler<IIsProcessInvalidatedQuery, bool>> IFixture<TData>.InvalidityReaderMock => InvalidityReaderMock;
    }
}
