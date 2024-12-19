#if USING_UNITY_ENGINE_SHIMS
using UnityEngine;
#endif
namespace AstarPathfinder;

public class NodeRef(Node Node)
{
    public Node Node{ get; } = Node;
}