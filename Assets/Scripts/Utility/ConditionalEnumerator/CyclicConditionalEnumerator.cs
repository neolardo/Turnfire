using System.Collections.Generic;
using System.Linq;

public class CyclicConditionalEnumerator<T> where T: class, IConditionalEnumerable
{
    private readonly List<T> _items;
    private int _index = InvalidIndex;

    private const int InvalidIndex = -1;

    public CyclicConditionalEnumerator(IEnumerable<T> teams)
    {
        _items = teams.ToList();
    }

    public T Current => _index == InvalidIndex? null : _items[_index];

    public T MoveNext(out bool wrapped)
    {
        wrapped = false;

        if (!_items.Any(t => t.EnumeratorCondition))
            return null;

        int startIndex = _index;

        do
        {
            _index = (_index + 1) % _items.Count;
        }
        while (!_items[_index].EnumeratorCondition);

        wrapped = _index <= startIndex;
        return _items[_index];
    }

    public T PeekNext(out bool wrapped)
    {
        wrapped = false;

        if (!_items.Any(t => t.EnumeratorCondition))
            return null;

        int startIndex = _index;
        int tempInd = _index;
        do
        {
            tempInd = (tempInd + 1) % _items.Count;
        }
        while (!_items[tempInd].EnumeratorCondition);

        wrapped = tempInd <= startIndex;
        return _items[tempInd];
    }

    public void Reset()
    {
        _index = -1;
    }
}
