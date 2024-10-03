namespace Paraminter.Associating.Processing.Invalidation.Queries;

using Paraminter.Processing.Invalidation.Queries;

internal sealed class IsProcessInvalidatedQuery
    : IIsProcessInvalidatedQuery
{
    public static IIsProcessInvalidatedQuery Instance { get; } = new IsProcessInvalidatedQuery();

    private IsProcessInvalidatedQuery() { }
}
