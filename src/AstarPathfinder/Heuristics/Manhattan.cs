#if USING_UNITY_ENGINE_SHIMS
using UnityEngine;
#endif
namespace AstarPathfinder;

public class Manhattan : ICalculableHeuristicCost
{
    public static readonly Manhattan Default = new();
    
    public float Calculate(Vector2Int from, Vector2Int to)
    {
        return Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
    }
}