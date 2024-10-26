namespace Paraminter.Associating.Processing.Invalidation;

using Paraminter.Associating.Commands;
using Paraminter.Associating.Models;
using Paraminter.Associating.Processing.Invalidation.Commands;
using Paraminter.Cqs;
using Paraminter.Processing.Invalidation.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>Decorates an associator by resetting the invalidity status before invoking the decoratee.</summary>
/// <typeparam name="TData">The type representing the data used to associate arguments with parameters.</typeparam>
public sealed class InvalidityResettingAssociatorDecorator<TData>
    : ICommandHandler<IAssociateArgumentsCommand<TData>>
    where TData : IAssociateArgumentsData
{
    private readonly ICommandHandler<IAssociateArgumentsCommand<TData>> Decoratee;

    private readonly ICommandHandler<IResetProcessInvalidityCommand> InvalidityResetter;

    /// <summary>Instantiates a decorator of an associator, which resets the invalidity status before invoking the decoratee.</summary>
    /// <param name="decoratee">The decorated associator.</param>
    /// <param name="invalidityResetter">Resets the invalidity status.</param>
    public InvalidityResettingAssociatorDecorator(
        ICommandHandler<IAssociateArgumentsCommand<TData>> decoratee,
        ICommandHandler<IResetProcessInvalidityCommand> invalidityResetter)
    {
        Decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));

        InvalidityResetter = invalidityResetter ?? throw new ArgumentNullException(nameof(invalidityResetter));
    }

    async Task ICommandHandler<IAssociateArgumentsCommand<TData>>.Handle(
        IAssociateArgumentsCommand<TData> command,
        CancellationToken cancellationToken)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        await InvalidityResetter.Handle(ResetProcessInvalidityCommand.Instance, cancellationToken).ConfigureAwait(false);

        await Decoratee.Handle(command, cancellationToken).ConfigureAwait(false);
    }
}
