using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//add tiles to point
public class MazeGenerator : MonoBehaviour
{

    public enum WallType
    {
        Top,
        Left,
        Bottom,
        Right
    }

    //move to separate file
    public class Point
    {
        public Vector2 Possition;
        public int X;
        public int Y;
        //change naming
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
    //change naming
    public Point[,] _point;
    private int _width;
    private int _height;
    public Stack<Point> _path = new Stack<Point>();
    public List<Point> _visited = new List<Point>();
    //try to move to point 
    public List<Point> _neighbours = new List<Point>();
    public Point start;
    public Point finish;

    public Point GetNeighbour(Point point, string NeighbourLocation)
    {
        switch(NeighbourLocation)
        {
            case "top":
                return _point[point.X, point.Y - 1];
            case "bot":
                return _point[point.X, point.Y + 1];
            case "left":
                return _point[point.X - 1, point.Y];
            case "right":
                return _point[point.X + 1, point.Y];
            default:
                return null;
        }
    }

    public GameObject PickGameObject(Point point)
    {
        if (!GetNeighbour(point, "top")._isCell && !GetNeighbour(point, "bot")._isCell && GetNeighbour(point, "left")._isCell && GetNeighbour(point, "right")._isCell)
        {
            return PathH;
        }

        if (GetNeighbour(point, "top")._isCell && GetNeighbour(point, "bot")._isCell && !GetNeighbour(point, "left")._isCell && !GetNeighbour(point, "right")._isCell)
        {
            return PathV;
        }

        if (GetNeighbour(point, "top")._isCell && !GetNeighbour(point, "bot")._isCell && !GetNeighbour(point, "left")._isCell && GetNeighbour(point, "right")._isCell)
        {
            return Turn2;
        }

        if (!GetNeighbour(point, "top")._isCell && GetNeighbour(point, "bot")._isCell && !GetNeighbour(point, "left")._isCell && GetNeighbour(point, "right")._isCell)
        {
            return Turn1;
        }

        if (GetNeighbour(point, "top")._isCell && !GetNeighbour(point, "bot")._isCell && GetNeighbour(point, "left")._isCell && !GetNeighbour(point, "right")._isCell)
        {
            return Turn4;
        }

        if (!GetNeighbour(point, "top")._isCell && GetNeighbour(point, "bot")._isCell && GetNeighbour(point, "left")._isCell && !GetNeighbour(point, "right")._isCell)
        {
            return Turn3;
        }

        if (!GetNeighbour(point, "top")._isCell && GetNeighbour(point, "bot")._isCell && !GetNeighbour(point, "left")._isCell && !GetNeighbour(point, "right")._isCell)
        {
            return EndBot;
        }

        if (GetNeighbour(point, "top")._isCell && !GetNeighbour(point, "bot")._isCell && !GetNeighbour(point, "left")._isCell && !GetNeighbour(point, "right")._isCell)
        {
            return EndTop;
        }

        if (!GetNeighbour(point, "top")._isCell && !GetNeighbour(point, "bot")._isCell && !GetNeighbour(point, "left")._isCell && GetNeighbour(point, "right")._isCell)
        {
            return EndLeft;
        }

        if (!GetNeighbour(point, "top")._isCell && !GetNeighbour(point, "bot")._isCell && GetNeighbour(point, "left")._isCell && !GetNeighbour(point, "right")._isCell)
        {
            return EndRight;
        }

        if (GetNeighbour(point, "top")._isCell && !GetNeighbour(point, "bot")._isCell && GetNeighbour(point, "left")._isCell && GetNeighbour(point, "right")._isCell)
        {
            return TBot;
        }

        if (!GetNeighbour(point, "top")._isCell && GetNeighbour(point, "bot")._isCell && GetNeighbour(point, "left")._isCell && GetNeighbour(point, "right")._isCell)
        {
            return TTop;
        }

        if (GetNeighbour(point, "top")._isCell && GetNeighbour(point, "bot")._isCell && GetNeighbour(point, "left")._isCell && !GetNeighbour(point, "right")._isCell)
        {
            return TLeft;
        }

        if (GetNeighbour(point, "top")._isCell && GetNeighbour(point, "bot")._isCell && !GetNeighbour(point, "left")._isCell && GetNeighbour(point, "right")._isCell)
        {
            return TRight;
        }

        else return Floor;
    }

    public void MazeGeneratorr()
    {

        //move to global
        _width = 15;
        _height = 15;
        start = new Point(1, 1, true, true);
        //what is the magic number?
        finish = new Point(_width - 3, _height - 3, true, true);
        _point = new Point[_width, _height];
        for (var i = 0; i < _width; i++)
            for (var j = 0; j < _height; j++)
                //weird bool - rename
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
                // path
                if (_point[i, j]._isCell)
                {

                    Vector2 myPos = new Vector2(_point[i, j].X, _point[i, j].Y);
                    Instantiate(PickGameObject(_point[i,j]), myPos, Quaternion.identity);

                }
                else
                {
                    Vector2 myPos = new Vector2(_point[i, j].X, _point[i, j].Y);
                    Instantiate(Floor, myPos, Quaternion.identity);
                }

            }
    }

    //move to point
    private void GetNeighbours(Point curretnCell)
    {
        //move cal to point
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
            //move calc to point
            if (curNeighbour.X > 0 && curNeighbour.X < _width && curNeighbour.Y > 0 && curNeighbour.Y < _height)
            {
                if (_point[curNeighbour.X, curNeighbour.Y]._isCell && !_point[curNeighbour.X, curNeighbour.Y]._isVisited)
                {
                    _neighbours.Add(curNeighbour);
                }
            }
        }

    }

    //possible move to point
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

    // Start is called before the first frame update
    void Start()
    {
        MazeGeneratorr();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
