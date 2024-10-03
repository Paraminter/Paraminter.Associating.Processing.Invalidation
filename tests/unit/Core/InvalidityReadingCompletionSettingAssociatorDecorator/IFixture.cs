namespace Paraminter.Associating.Processing.Invalidation;

using Moq;

using Paraminter.Associating.Commands;
using Paraminter.Associating.Models;
using Paraminter.Cqs;
using Paraminter.Processing.Commands;
using Paraminter.Processing.Invalidation.Queries;

internal interface IFixture<TData>
    where TData : IAssociateArgumentsData
{
    public abstract ICommandHandler<IAssociateArgumentsCommand<TData>> Sut { get; }

    public abstract Mock<ICommandHandler<IAssociateArgumentsCommand<TData>>> DecorateeMock { get; }

    public abstract Mock<ICommandHandler<ISetProcessCompletionCommand>> CompletionSetterMock { get; }
    public abstract Mock<IQueryHandler<IIsProcessInvalidatedQuery, bool>> InvalidityReaderMock { get; }
}
