#if USING_UNITY_ENGINE_SHIMS
using UnityEngine;
#endif

namespace AstarPathfinder;

public class Euclidean : ICalculableHeuristicCost
{
    public static readonly Euclidean Default = new();
    
    public float Calculate(Vector2Int from, Vector2Int to)
    {
        var dx = from.x - to.x;
        var dy = from.y - to.y;
        return (float)Math.Sqrt(dx * dx + dy * dy);
    }
}