using FluentAssertions;
using Xunit.Abstractions;

namespace AstarPathfinder.Test;

public class PathFindTest
{
    private ITestOutputHelper testOutputHelper;
    private Pathfinder finder;
    private Node[,] nodeArray;
    private int rows, cols;
    private Vector2Int[] destination = new Vector2Int[100];

    public PathFindTest(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
        
        var nodeSource = new [,]
        {
            {0,0,0,0,0,0,0,0,0,0},
            {0,1,0,0,0,0,0,0,1,0},
            {0,0,0,0,0,0,0,0,1,0},
            {0,1,0,0,0,0,0,0,1,0},
            {0,1,0,0,0,0,0,0,1,0},
            {0,1,0,0,0,0,0,0,1,0},
            {0,0,0,0,0,0,0,0,1,0},
        };
        
        rows = nodeSource.GetLength(0);
        cols = nodeSource.GetLength(1);
        
        nodeArray = new Node[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // 現在のインデックスを設定
                var index = new Vector2Int(row, col);

                // 重み (例として 1.0f に設定)
                var wight = nodeSource[row, col] == 1?10000:0;

                // 禁止フラグ (例として nodeSource の値が 1 の場合に true)
                var isBan = nodeSource[row, col] == 1;

                // Node 構造体を作成し、配列に代入
                nodeArray[row, col] = new Node(index, wight, isBan);
            }
        }

        finder = new Pathfinder(nodeArray);
    }
    
    [Fact]
    public void Test()
    {
        var first = TestCore();
        var second = TestCore();

        first.Should().Equal(second);
    }

    Vector2Int[] TestCore()
    {
        testOutputHelper.WriteLine("====================Start====================");
        var count = finder.FindPath(nodeArray[0, 0], nodeArray[rows - 1, cols - 1], ref destination);

        var dest = destination.Take(count).ToArray();
        foreach(var pos in destination.Take(count))
        {
            testOutputHelper.WriteLine($"({pos.x}, {pos.y})");
        }
        
        finder.Reset();
        return dest;
    }
}