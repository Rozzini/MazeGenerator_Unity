using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public class Point
    {
        public Vector2 Possition;
        public int X;
        public int Y;
        public int TileNum;
        public bool _isCell { get; set; }
        public bool _isVisited { get; set; }

        public Point(int x, int y, bool isVisited = false, bool isCell = true)
        {
            Possition = new Vector2(x,y);
            X = x;
            Y = y;
            _isCell = isCell;
            _isVisited = isVisited;
        }
    }

    public GameObject Floor;
    public GameObject PathH;
    public GameObject PathV;
    public GameObject Turn1;
    public GameObject Turn2;
    public GameObject Turn3;
    public GameObject Turn4;
    public GameObject EndTop;
    public GameObject EndBot;
    public GameObject EndRight;
    public GameObject EndLeft;
    public GameObject TTop;
    public GameObject TBot;
    public GameObject TLeft;
    public GameObject TRight;
    public GameObject XTurn;
    public GameObject Finish;
    public GameObject Temp;
    public Point[,] _point;
    private int _width;
    private int _height;
    public Stack<Point> _path = new Stack<Point>();
    public List<Point> _visited = new List<Point>();
    public List<Point> _solve = new List<Point>();
    public List<Point> _neighbours = new List<Point>();
    public Point start;
    public Point finish;


    public void DeclareTile(Point point)
    {
        int TileDeclare = 0;

        /*
         *       * - is current cell, Literals are neighbours. This code calculates sum depending on neighbour Point _isCell or not, values for each neighbour are given bellow
         *       T - 2;
         *       B - 8;
         *       L - 4;
         *       R - 1;
         *         T
         *       L * R
         *         B
         */        
        if(_point[point.X, point.Y - 1]._isCell)
        {
            TileDeclare += 2;
        }
        if(_point[point.X, point.Y + 1]._isCell)
        {
            TileDeclare += 8;
        }
        if(_point[point.X + 1, point.Y ]._isCell)
        {
            TileDeclare += 1;
        }
        if(_point[point.X - 1, point.Y]._isCell)
        {
            TileDeclare += 4;
        }

        point.TileNum = TileDeclare;
    }


    public GameObject PickGameObject(Point point)
    {
        DeclareTile(point);

        if (point.TileNum == 1)
        {
            return EndLeft;
        }

        if (point.TileNum == 2)
        {
            return EndTop;
        }

        if (point.TileNum == 3)
        {
            return Turn2;
        }

        if (point.TileNum == 4)
        {
            return EndRight;
        }

        if (point.TileNum == 5)
        {
            return PathH;
        }

        if (point.TileNum == 6)
        {
            return Turn4;
        }

        if (point.TileNum == 7)
        {
            return TBot;
        }

        if (point.TileNum == 8)
        {
            return EndBot;
        }

        if (point.TileNum == 9)
        {
            return Turn1;
        }

        if (point.TileNum == 10)
        {
            return PathV;
        }

        if (point.TileNum == 11)
        {
            return TRight;
        }

        if (point.TileNum == 12)
        {
            return Turn3;
        }

        if (point.TileNum == 13)
        {
            return TTop;
        }

        if (point.TileNum == 14)
        {
            return TLeft;
        }

        if (point.TileNum == 15)
        {
            return XTurn;
        }

        else return Floor;
    }

    public void MazeGeneratorr()
    {
        _width = 35;
        _height = 35;
        start = new Point(1, 1, true, true);
        finish = new Point(_width - 2, _height - 2, true, true);
        _point = new Point[_width, _height];
        for (var i = 0; i < _width; i++)
            for (var j = 0; j < _height; j++)
                if ((i % 2 != 0 && j % 2 != 0) && (i < _width - 1 && j < _height - 1))
                {
                    _point[i, j] = new Point(i, j);
                }
                else
                {
                    _point[i, j] = new Point(i, j, false, false);
                }
        _path.Push(start);
        _point[start.X, start.Y] = start;

        CreateMaze();

        SpawnMaze();

        Vector3 myPos = new Vector3(start.X, start.Y, -1);
        Instantiate(Finish, myPos, Quaternion.identity);

        SolveMaze();
    }

    private void CreateMaze()
    {
        _point[start.X, start.Y] = start;
        while (_path.Count != 0)
        {
            _neighbours.Clear();
            GetNeighbours(_path.Peek());
            if (_neighbours.Count != 0)
            {
                Point nextCell = ChooseNeighbour(_neighbours);
                RemoveWall(_path.Peek(), nextCell);
                nextCell._isVisited = true;
                _point[nextCell.X, nextCell.Y]._isVisited = true;
                _path.Push(nextCell);
            }
            else
            {
                _path.Pop();
            }

        }
    }

    public void SpawnMaze()
    {
        for (int i = 0; i < _width; i++)
            for (int j = 0; j < _height; j++)
            {
                if (_point[i, j]._isCell)
                {

                    Vector2 myPos = new Vector2(_point[i, j].X, _point[i, j].Y);
                    Instantiate(PickGameObject(_point[i,j]), myPos, Quaternion.identity);

                }
            }
    }

    private void GetNeighbours(Point curretnCell)
    {
        int x = curretnCell.X;
        int y = curretnCell.Y;
        const int distance = 2;
        Point[] possibleNeighbours = new[]
        {
                new Point(x, y - distance),
                new Point(x + distance, y),
                new Point(x, y + distance),
                new Point(x - distance, y)
            };
        for (int i = 0; i < 4; i++)
        {
            Point curNeighbour = possibleNeighbours[i];
            if (curNeighbour.X > 0 && curNeighbour.X < _width && curNeighbour.Y > 0 && curNeighbour.Y < _height)
            {
                if (_point[curNeighbour.X, curNeighbour.Y]._isCell && !_point[curNeighbour.X, curNeighbour.Y]._isVisited)
                {
                    _neighbours.Add(curNeighbour);
                }
            }
        }

    }

    private Point ChooseNeighbour(List<Point> neighbours)
    {
        int r = Random.Range(0, neighbours.Count);
        return neighbours[r];
    }

    private void RemoveWall(Point first, Point second)
    {
        int XDifference = second.X - first.X;
        int YDifference = second.Y - first.Y;
        int XAddCoords = 0;
        int YAddCoords = 0;

        if (XDifference == 0)
        {
            XAddCoords = 0;
        }
        else
        {
            XAddCoords = XDifference / Mathf.Abs(XDifference);
        }

        if (YDifference == 0)
        {
            YAddCoords = 0;
        }
        else
        {
            YAddCoords = YDifference / Mathf.Abs(YDifference);
        }

        _point[first.X + XAddCoords, first.Y + YAddCoords]._isCell = true;
        _point[first.X + XAddCoords, first.Y + YAddCoords]._isVisited = true;
        second._isVisited = true;
        _point[second.X, second.Y] = second;
    }

    public void SolveMaze()
    {
        bool flag = false;
        foreach (Point p in _point)
        {
            if (_point[p.X, p.Y]._isCell == true)
            {
                _point[p.X, p.Y]._isVisited = false;
            }
        }
        _path.Clear();
        _path.Push(start);
        while (_path.Count != 0)
        {
            if (_path.Peek().X == finish.X && _path.Peek().Y == finish.Y)
            {
                flag = true;
            }
            if (!flag)
            {
                _neighbours.Clear();
                GetNeighboursSolve(_path.Peek());
                if (_neighbours.Count != 0)
                {
                    Point nextCell = ChooseNeighbour(_neighbours);
                    nextCell._isVisited = true;
                    _point[nextCell.X, nextCell.Y]._isVisited = true;
                    _path.Push(nextCell);
                    _visited.Add(_path.Peek());
                }
                else
                {
                    _path.Pop();
                }
            }
            else
            {
                _solve.Add(_path.Peek());
                _path.Pop();
            }
        }
    }

    private void GetNeighboursSolve(Point localcell)
    {
        int x = localcell.X;
        int y = localcell.Y;
        const int distance = 1;
        Point[] possibleNeighbours = new[]
        {
                new Point(x, y - distance),
                new Point(x + distance, y),
                new Point(x, y + distance),
                new Point(x - distance, y)
            };
        for (int i = 0; i < 4; i++)
        {
            Point curNeighbour = possibleNeighbours[i];
            if (curNeighbour.X > 0 && curNeighbour.X < _width && curNeighbour.Y > 0 && curNeighbour.Y < _height)
            {
                if (_point[curNeighbour.X, curNeighbour.Y]._isCell && !_point[curNeighbour.X, curNeighbour.Y]._isVisited)
                {
                    _neighbours.Add(curNeighbour);
                }
            }
        }
    }

    int iter = 0;
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 5;
        MazeGeneratorr();
    }

    // Update is called once per frame
    void Update()
    {
        if (iter < _solve.Count)
        {
            Destroy(GameObject.FindWithTag("Player"));
            Vector3 myPos = new Vector3(_solve[iter].X, _solve[iter].Y, -1);
            Instantiate(Temp, myPos, Quaternion.identity);
            iter++;
        }
    }
}
