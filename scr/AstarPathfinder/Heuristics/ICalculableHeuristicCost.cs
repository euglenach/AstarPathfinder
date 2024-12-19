#if USING_UNITY_ENGINE_SHIMS
using UnityEngine;
#endif

namespace AstarPathfinder;

public interface ICalculableHeuristicCost
{
    float Calculate(Vector2Int from, Vector2Int to);
}