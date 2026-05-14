using System.Diagnostics;

namespace RPGConverter.Engine;

public abstract record StatementNode
{
    private string Id { get; } = Guid.NewGuid().ToString()[..4];

    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public StatementNode? Next { get; set; }

    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public StatementNode? Previous { get; set; }

    public virtual string GetDebuggerDisplay() => $"Id: {Id} | Prev: {Previous?.Id ?? "<null>"} | Next: {Next?.Id ?? "<null>"}";
    public sealed override string ToString() => GetDebuggerDisplay();
}

public record EmptyLineNode : StatementNode;

public record CommentNode : StatementNode
{
    public string Comment { get; init; } = null!;
    public override string GetDebuggerDisplay() => Comment + " | " + base.GetDebuggerDisplay();
}

public record ControlNode : StatementNode
{
    public string KeyWord { get; init; } = null!;
    public string? Parameter { get; init; }

    public override string GetDebuggerDisplay() => KeyWord + (Parameter != null ? $"({Parameter})" : "") + " | " + base.GetDebuggerDisplay();
}
