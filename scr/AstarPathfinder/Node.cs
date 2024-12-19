#if USING_UNITY_ENGINE_SHIMS
using UnityEngine;
#endif

namespace AstarPathfinder;

public struct Node : IEquatable<Node>
{
    public Vector2Int Index;
    public float Wight;
    public bool IsBan;
    
    public NodeRef? Parent;
    public NodeState State;
    public float Score;

    public Node(Vector2Int index, float wight, bool isBan)
    {
        Index = index;
        Wight = wight;
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