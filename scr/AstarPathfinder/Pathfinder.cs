using System.Buffers;
#if USING_UNITY_ENGINE_SHIMS
using UnityEngine;
#endif

namespace AstarPathfinder;

public class Pathfinder
{
    private readonly ICalculableHeuristicCost calculable;
    
    private Node[,] grid;
    private readonly int width;
    private readonly int height;
    
    // 隣接ノードを取る用
    private static readonly　int[] dx = [-1, 0, 1, -1, 1, -1, 0, 1];
    private static readonly　int[] dy = [-1, -1, -1, 0, 0, 1, 1, 1];
    
    public Pathfinder(Node[,] grid, ICalculableHeuristicCost? calculable = null)
    {
        this.grid = grid;
        width = grid.GetLength(0);
        height = grid.GetLength(1);
        this.calculable ??= new Euclidean();
    }
    
    public int FindPath(Node from, Node to, ref Vector2Int[] buffer)
    {
        using var openList = new TempList<Node>(buffer.Length);
        ref var current = ref grid[from.Index.x, from.Index.y];
        openList.Add(current);
        current.State = NodeState.Closed;
        current.ParentIndex = null;
        
        // 隣接ノードバッファー
        var adjacentBuffer =  ArrayPool<Vector2Int>.Shared.Rent(8);

        var currentCost = 0;

        while(true)
        {
            currentCost++;
            var adjacentCount = GetAdjacentNodes(ref current, adjacentBuffer);
            
            // 隣接ノードをOpen状態にする
            for(var i = 0; i < adjacentCount; i++)
            {
                var index = adjacentBuffer[i];
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
            current = ref GetMinCostNode(openList.Span);

            // 現在のノードを閉じる
            current.State = NodeState.Closed;

            // ゴールに到達したかオープンリストが空か
            if(current.Equals(to) || IsAllClosed(openList))
            {
                break;
            }
        }

        var count = 0;
        while(current.ParentIndex is not null)
        {
            if(buffer.Length <= count)
            {
                // ??
                var newBuffer = new Vector2Int[currentCost];
                Array.Copy(buffer, newBuffer, buffer.Length);
                buffer = newBuffer;
            }
            buffer[count] = current.Index;
            var index = current.ParentIndex.Value;
            current = grid[index.x, index.y];
            count++;
        }

        ArrayPool<Vector2Int>.Shared.Return(adjacentBuffer);
        return count;
    }

    private ref Node GetMinCostNode(Span<Node> openNodeList)
    {
        var minScore = float.MaxValue;
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
            }
            // スコアが同じ場合はWeightを見る
            else if(Math.Abs(node.Score - minScore) < float.Epsilon * 8)
            {
                if(shortestNode.Wight <= node.Wight) continue;
                shortestNode = ref node;
            }
        }

        return ref shortestNode;
    }

    bool IsAllClosed(TempList<Node> list)
    {
        // うーんSIMDにできないか そもそもTempListをRemoveできるように改良すべきか
        for(var i = 0; i < list.Count; i++)
        {
            if(list[i].State is not NodeState.Closed) return false;
        }

        return true;
    }

    private int GetAdjacentNodes(ref Node node, Vector2Int[] adjacentIndexes)
    {
        var count = 0;
        
        var x = node.Index.x;
        var y = node.Index.y;
        
        for(var i = 0; i < dx.Length; i++)
        {
            var nx = x + dx[i];
            var ny = y + dy[i];

            // 境界値チェック
            if(nx < 0 || nx >= width || ny < 0 || ny >= height) continue;
            
            adjacentIndexes[count] = new(nx, ny);
            count++;
        }

        return count;
    }

    public void Reset()
    {
        for(var x = 0; x < width; x++)
        {
            for(var y = 0; y < height; y++)
            {
                ref var n = ref grid[x, y];
                n.ParentIndex = null;
                n.State = NodeState.None;
                n.Score = 0;
            }
        }
    }
}