namespace AstarPathfinder;

using System;
using System.Buffers;

internal ref struct TempList<T>
{
    private int index;
    private T[] array;
    
    public Span<T> Span => array.AsSpan(0, index);
    public int Count => index;

    public TempList(int defaultCapacity)
    {
        this.array = ArrayPool<T>.Shared.Rent(defaultCapacity);
        this.index = 0;
    }

    public TempList() : this(16) {}

    public void Add(T item)
    {
        if(array.Length <= index)
        {
            var newArray = ArrayPool<T>.Shared.Rent(index * 2);
            Array.Copy(array, newArray, index);
            ArrayPool<T>.Shared.Return(array, true);
            array = newArray;
        }

        array[index++] = item;
    }

    public void RemoveAt(int i)
    {
        if ((uint)i >= (uint)this.index)
        {
            ThrowArgumentOutOfRangeException(index, i);
        }
        if (i < index)
        {
            Array.Copy(array, i + 1, array, i, index - i);
        }
        index--;
    }

    public ref T this[int i]
    {
        get
        {
            if (i < 0 || i >= index)
                ThrowArgumentOutOfRangeException(index, i);
            return ref array[i];
        }
    }

    public void Dispose()
    {
        if(array is null) return;
        ArrayPool<T>.Shared.Return(array, true);
    }

    public Span<T>.Enumerator GetEnumerator() => Span.GetEnumerator();

    private static void ThrowArgumentOutOfRangeException(int size, int i) => throw new ArgumentOutOfRangeException(nameof(i), $"Index {i} is out of range. Valid range: 0-{size - 1}");
}