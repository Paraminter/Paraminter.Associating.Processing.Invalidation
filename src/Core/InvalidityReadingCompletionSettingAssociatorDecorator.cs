namespace Paraminter.Associating.Processing.Invalidation;

using Paraminter.Associating.Commands;
using Paraminter.Associating.Models;
using Paraminter.Associating.Processing.Invalidation.Commands;
using Paraminter.Associating.Processing.Invalidation.Queries;
using Paraminter.Cqs;
using Paraminter.Processing.Commands;
using Paraminter.Processing.Invalidation.Queries;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>Decorates an associator by setting the completion status after invoking the decoratee, if the invalidity status is not set.</summary>
/// <typeparam name="TData">The type representing the data used to associate arguments with parameters.</typeparam>
public sealed class InvalidityReadingCompletionSettingAssociatorDecorator<TData>
    : ICommandHandler<IAssociateArgumentsCommand<TData>>
    where TData : IAssociateArgumentsData
{
    private readonly ICommandHandler<IAssociateArgumentsCommand<TData>> Decoratee;

    private readonly ICommandHandler<ISetProcessCompletionCommand> CompletionSetter;
    private readonly IQueryHandler<IIsProcessInvalidatedQuery, bool> InvalidityReader;

    /// <summary>Instantiates a decorator of an associator, which, if the invalidity status is not set, sets the completion status after invoking the decoratee.</summary>
    /// <param name="decoratee">The decorated associator.</param>
    /// <param name="completionSetter">Sets the completion status.</param>
    /// <param name="invalidityReader">Reads the invalidity status.</param>
    public InvalidityReadingCompletionSettingAssociatorDecorator(
        ICommandHandler<IAssociateArgumentsCommand<TData>> decoratee,
        ICommandHandler<ISetProcessCompletionCommand> completionSetter,
        IQueryHandler<IIsProcessInvalidatedQuery, bool> invalidityReader)
    {
        Decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));

        CompletionSetter = completionSetter ?? throw new ArgumentNullException(nameof(completionSetter));
        InvalidityReader = invalidityReader ?? throw new ArgumentNullException(nameof(invalidityReader));
    }

    async Task ICommandHandler<IAssociateArgumentsCommand<TData>>.Handle(
        IAssociateArgumentsCommand<TData> command,
        CancellationToken cancellationToken)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        await Decoratee.Handle(command, cancellationToken).ConfigureAwait(false);

        if (await InvalidityReader.Handle(IsProcessInvalidatedQuery.Instance, cancellationToken).ConfigureAwait(false) is false)
        {
            await CompletionSetter.Handle(SetProcessCompletionCommand.Instance, cancellationToken).ConfigureAwait(false);
        }
    }
}
