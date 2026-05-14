using System.Collections.ObjectModel;

namespace RPGConverter.Engine;

public class StatementNodeCollection : Collection<StatementNode>
{
    protected override void InsertItem(int index, StatementNode item)
    {
        if (index > 0)
        {
            item.Previous = Items[index - 1];
            Items[index - 1].Next = item;
        }

        if (index < Count)
        {
            item.Next = Items[index];
            Items[index].Previous = item;
        }

        base.InsertItem(index, item);
    }

    protected override void RemoveItem(int index)
    {
        var itemToRemove = Items[index];
        var previousNode = index > 0 ? Items[index - 1] : null;
        var nextNode = index < Count - 1 ? Items[index + 1] : null;

        if (previousNode != null)
        {
            previousNode.Next = nextNode;
        }

        if (nextNode != null)
        {
            nextNode.Previous = previousNode;
        }

        itemToRemove.Next = null;
        itemToRemove.Previous = null;

        base.RemoveItem(index);
    }

    protected override void SetItem(int index, StatementNode item)
    {
        Items[index].Next = null;
        Items[index].Previous = null;

        if (index > 0)
        {
            item.Previous = Items[index - 1];
            Items[index - 1].Next = item;
        }

        if (index < Count - 1)
        {
            item.Next = Items[index + 1];
            Items[index + 1].Previous = item;
        }

        base.SetItem(index, item);
    }

    protected override void ClearItems()
    {
        foreach (var item in Items)
        {
            item.Next = null;
            item.Previous = null;
        }

        base.ClearItems();
    }
}
