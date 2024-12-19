using Xunit.Abstractions;

namespace AstarPathfinder.Test;

public class PathFindTest
{
    private readonly ITestOutputHelper testOutputHelper;

    public PathFindTest(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test()
    {
        var nodeSource = new [,]
        {
            {0,1,0,0,0,0,0,0,0,0},
            {0,1,0,0,0,0,0,0,0,0},
            {0,1,0,0,0,0,0,0,0,0},
            {0,1,0,0,0,0,0,0,0,0},
            {0,1,0,0,0,0,0,0,0,0},
            {0,1,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
        };
        
        var rows = nodeSource.GetLength(0);
        var cols = nodeSource.GetLength(1);
        
        var nodeArray = new Node[rows, cols];

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
        
        var start = new Vector2Int(0, 0);
        var goal = new Vector2Int(rows - 1, cols - 1);

        var finder = new Pathfinder(nodeArray);
        var destination = new Vector2Int[100];
        
        var count = finder.FindPath(nodeArray[0, 0], nodeArray[rows - 1, cols - 1], ref destination);

        foreach(var pos in destination.Take(count))
        {
            testOutputHelper.WriteLine($"({pos.x}, {pos.y})");
        }
    }
}