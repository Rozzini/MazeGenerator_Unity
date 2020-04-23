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
        public bool _isGoodNeighbour;

        public Point(int x, int y, bool isVisited = false, bool isCell = true)
        {
            Possition = new Vector2(x,y);
            X = x;
            Y = y;
            _isCell = isCell;
            _isVisited = isVisited;
        }

        public bool GetInfo()
        {
            if (this._isCell && !this._isVisited)
                return true;
            else
                return false;
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
    private int Width;
    private int Height;
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
        switch(point.TileNum)
        {
            case 1:
                return EndLeft; 
            case 2:
                return EndTop; 
            case 3:
                return Turn2;
            case 4:
                return EndRight; 
            case 5:
                return PathH; 
            case 6:
                return Turn4; 
            case 7:
                return TBot; 
            case 8:
                return EndBot; 
            case 9:
                return Turn1; 
            case 10:
                return PathV; 
            case 11:
                return TRight;
            case 12:
                return Turn3;
            case 13:
                return TTop;
            case 14:
                return TLeft;
            case 15:
                return XTurn;
            default:
                return Floor; 
        }
        
    }

    public void MazeGeneratorr()
    {
        Width = 35;
        Height = 35;
        start = new Point(1, 1, true, true);
        finish = new Point(Width - 2, Height - 2, true, true);
        _point = new Point[Width, Height];
        for (var i = 0; i < Width; i++)
            for (var j = 0; j < Height; j++)
            {
                bool _isOdd = (bool)(i % 2 != 0 && j % 2 != 0);
                bool _isInBounds = (bool)(i < Width - 1 && j < Height - 1);
                if (_isOdd && _isInBounds)
                {
                    _point[i, j] = new Point(i, j);
                }
                else
                {
                    _point[i, j] = new Point(i, j, false, false);
                }
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
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
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
            if (curNeighbour.X > 0 && curNeighbour.X < Width && curNeighbour.Y > 0 && curNeighbour.Y < Height)
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

        XAddCoords = (XDifference == 0) ? 0 : ( XDifference / Mathf.Abs(XDifference));

        YAddCoords = (YDifference == 0) ? 0 : ( YDifference / Mathf.Abs(YDifference));
        

        _point[first.X + XAddCoords, first.Y + YAddCoords]._isCell = true;
        _point[first.X + XAddCoords, first.Y + YAddCoords]._isVisited = true;
        second._isVisited = true;
        _point[second.X, second.Y] = second;
    }


    public bool Compare(Point point1, Point point2)
    {
        if (point1.X == point2.X && point1.Y == point2.Y)
            return true;
        else
            return false;
    }

    public void SolvationMechanism()
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

    public void SolveMaze()
    {
        bool FinishFlag = false;
        foreach (Point p in _point)
        {
            if (p._isCell == true)
            {
               p._isVisited = false;
            }
        }

        _path.Clear();
        _path.Push(start);


        while (_path.Count != 0)
        {
            if (Compare(_path.Peek(), finish))
            {
                FinishFlag = true;
            }
            if (!FinishFlag)
            {
                SolvationMechanism();
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
            if (curNeighbour.X > 0 && curNeighbour.X < Width && curNeighbour.Y > 0 && curNeighbour.Y < Height)
            {
                if (_point[curNeighbour.X, curNeighbour.Y].GetInfo())
                {
                    _neighbours.Add(curNeighbour);
                }
            }
        }
    }

    int iter = 0;
    
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 5;
        MazeGeneratorr();
    }

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
