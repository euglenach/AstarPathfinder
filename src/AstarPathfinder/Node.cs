#if USING_UNITY_ENGINE_SHIMS
using UnityEngine;
#endif

namespace AstarPathfinder;

public struct Node : IEquatable<Node>
{
    public Vector2Int Index;
    public int Weight;
    public bool IsBan;
    
    public Vector2Int? ParentIndex;
    public NodeState State;
    public float Score;

    public Node(Vector2Int index, int weight, bool isBan)
    {
        Index = index;
        Weight = weight;
        IsBan = isBan;
    }

    public bool Equals(Node other)
    {
        return Index.x == other.Index.x && Index.y == other.Index.y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Node other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Index.x, Index.y);
    }
}