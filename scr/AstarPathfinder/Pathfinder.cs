using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if USING_UNITY_ENGINE_SHIMS
using UnityEngine;
#endif

namespace AstarPathfinder;

public class Pathfinder(Node[,] grid, ICalculableHeuristicCost? calculable = null)
{
    private readonly ICalculableHeuristicCost calculable = calculable ?? new Euclidean();
    private readonly int width = grid.GetLength(0);
    private readonly int height = grid.GetLength(1);

    // 隣接ノードを取る用
    private static readonly Vector2Int[] deltas = [new (-1, -1), new (0, -1), new (1, -1), new (-1, 0), new (1, 0), new (-1, 1), new (0, 1), new (1, 1)];
    
    public int FindPath(Node from, Node to, ref Vector2Int[] buffer)
    {
        using var openList = new TempList<Node>(buffer.Length);
        ref var currentRef = ref grid[from.Index.x, from.Index.y];
        openList.Add(currentRef);
        currentRef.State = NodeState.Closed;
        currentRef.ParentIndex = null;
        var current = currentRef;
        // 隣接ノードバッファー
        var adjacentBuffer = (stackalloc Vector2Int[8]);

        var currentCost = 0;

        while(true)
        {
            currentCost++;
            var adjacentCount = GetAdjacentNodes(width, height, current.Index, adjacentBuffer);

            // 隣接ノードをOpen状態にする
            foreach(var index in adjacentBuffer[..adjacentCount])
            {
                ref var node = ref grid[index.x, index.y];
                if(node.State is NodeState.None && !node.IsBan)
                {
                    node.State = NodeState.Open;
                    node.ParentIndex = current.Index;
                    // 開始距離(currentCost) + ヒューリスティック関数での距離 + Weight を足してスコアとする
                    node.Score = currentCost + calculable.Calculate(current.Index, to.Index) + node.Wight;
                    openList.Add(node);
                }
            }

            // 次の最短ルートとなるノードを取得
            ref var c = ref GetMinCostNode(openList.Span, out var minIndex);
            if(minIndex < 0) break;

            // 現在のノードを閉じる
            c.State = NodeState.Closed;
            current = c;

            openList.RemoveAtSwapBack(minIndex);
            // ゴールに到達したかオープンリストが空か
            if(current.Equals(to) || openList.Count == 0)
            {
                break;
            }
        }

        current = grid[current.Index.x, current.Index.y];
        var count = 0;

        while(current.ParentIndex is not null)
        {
            if(buffer.Length <= count)
            {
                Array.Resize(ref buffer, buffer.Length * 2);
            }

            // 受け取った座標とxyが逆転しているので反転して返す
            buffer[count] = new(current.Index.y, current.Index.x);
            var index = current.ParentIndex.Value;
            current = grid[index.x, index.y];
            count++;
        }

        return count;
    }

    private ref Node GetMinCostNode(Span<Node> openNodeList, out int minIndex)
    {
        var minScore = float.MaxValue;
        minIndex = -1;
        ref var shortestNode = ref openNodeList[0];
        
        for(var i = 0; i < openNodeList.Length; i++)
        {
            ref var node = ref openNodeList[i];
            if(node.State is not NodeState.Open) continue;
            // コストが少ないものを記録していく
            if(minScore > node.Score)
            {
                minScore = node.Score;
                shortestNode = ref node;
                minIndex = i;
            }
            // スコアが同じ場合はWeightを見る
            else if(node.Score < minScore + float.Epsilon * 8)
            {
                if(shortestNode.Wight <= node.Wight) continue;
                shortestNode = ref node;
                minIndex = i;
            }
        }

        return ref shortestNode;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetAdjacentNodes(int width, int height, Vector2Int index, Span<Vector2Int> adjacentIndexes)
    {
        var count = 0;

        var x = index.x;
        var y = index.y;
        foreach(var delta in deltas)
        {
            var nx = x + delta.x;
            var ny = y + delta.y;

            // 境界値チェック
            if((uint)nx >= (uint)width || (uint)ny >= (uint)height) continue;

            adjacentIndexes[count] = new(nx, ny);
            count++;
        }

        return count;
    }

    public void Reset()
    {
        var span = MemoryMarshal.CreateSpan(ref grid[0, 0], grid.Length);

        for(var i = 0; i < span.Length; i++)
        {
            ref var n = ref span[i];
            n.ParentIndex = null;
            n.State = default;
            n.Score = 0;
        }

        // for(var x = 0; x < width; x++)
        // {
        //     for(var y = 0; y < height; y++)
        //     {
        //         ref var n = ref grid[x, y];
        //         n.ParentIndex = null;
        //         n.State = NodeState.None;
        //         n.Score = 0;
        //     }
        // }
    }
}