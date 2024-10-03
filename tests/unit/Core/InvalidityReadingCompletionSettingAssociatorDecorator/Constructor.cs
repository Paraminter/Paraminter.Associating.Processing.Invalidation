namespace Paraminter.Associating.Processing.Invalidation;

using Moq;

using Paraminter.Associating.Commands;
using Paraminter.Associating.Models;
using Paraminter.Cqs;
using Paraminter.Processing.Commands;
using Paraminter.Processing.Invalidation.Queries;

using System;

using Xunit;

public sealed class Constructor
{
    [Fact]
    public void NullDecoratee_ThrowsArgumentNullException()
    {
        var result = Record.Exception(() => Target<IAssociateArgumentsData>(
            null!,
            Mock.Of<ICommandHandler<ISetProcessCompletionCommand>>(),
            Mock.Of<IQueryHandler<IIsProcessInvalidatedQuery, bool>>()));

        Assert.IsType<ArgumentNullException>(result);
    }

    [Fact]
    public void NullCompletionSetter_ThrowsArgumentNullException()
    {
        var result = Record.Exception(() => Target(
            Mock.Of<ICommandHandler<IAssociateArgumentsCommand<IAssociateArgumentsData>>>(),
            null!,
            Mock.Of<IQueryHandler<IIsProcessInvalidatedQuery, bool>>()));

        Assert.IsType<ArgumentNullException>(result);
    }

    [Fact]
    public void NullInvalidityReader_ThrowsArgumentNullException()
    {
        var result = Record.Exception(() => Target(
            Mock.Of<ICommandHandler<IAssociateArgumentsCommand<IAssociateArgumentsData>>>(),
            Mock.Of<ICommandHandler<ISetProcessCompletionCommand>>(),
            null!));

        Assert.IsType<ArgumentNullException>(result);
    }

    [Fact]
    public void ValidArguments_ReturnsHandler()
    {
        var result = Target(
            Mock.Of<ICommandHandler<IAssociateArgumentsCommand<IAssociateArgumentsData>>>(),
            Mock.Of<ICommandHandler<ISetProcessCompletionCommand>>(),
            Mock.Of<IQueryHandler<IIsProcessInvalidatedQuery, bool>>());

        Assert.NotNull(result);
    }

    private static InvalidityReadingCompletionSettingAssociatorDecorator<TData> Target<TData>(
        ICommandHandler<IAssociateArgumentsCommand<TData>> decoratee,
        ICommandHandler<ISetProcessCompletionCommand> completionSetter,
        IQueryHandler<IIsProcessInvalidatedQuery, bool> invalidityReader)
        where TData : IAssociateArgumentsData
    {
        return new InvalidityReadingCompletionSettingAssociatorDecorator<TData>(decoratee, completionSetter, invalidityReader);
    }
}
