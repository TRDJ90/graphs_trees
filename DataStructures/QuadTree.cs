using Primitives;
using Raylib_cs;
using System.Numerics;

namespace DataStructures;

public class QuadTree
{
    private const uint MAX_LEVEL = 8;

    private readonly List<Vector2> points;
    private readonly AABB boundary;
    private readonly uint maxPoints;
    private readonly uint level;
    
    private QuadTree? northwest, northeast, southwest, southeast = null;

    public Color DebugColor {get; set;}

    public QuadTree(AABB boundary, uint maxPoints, uint level)
    {
        this.boundary = boundary;
        this.points = [];
        this.maxPoints = maxPoints;
        this.level = level;

        this.DebugColor = Color.Black;
    }

    private bool IsLeaf()
    {
        return this.northwest == null &&
            this.northeast == null &&
            this.southeast == null &&
            this.southwest == null;
    }

    private void Split()
    {
        var boundary = this.boundary;
        var halfDeltaX = (boundary.BRXY.X - boundary.ULXY.X) / 2;
        var halfDeltaY = (boundary.BRXY.Y - boundary.ULXY.Y) / 2;

        // This node's center point
        var centerX = boundary.BRXY.X - halfDeltaX;
        var centerY = boundary.BRXY.Y - halfDeltaY;

        if(this.level < MAX_LEVEL) 
        {   
            uint nextLevel = this.level + 1;

            Color debugColorForLevel = nextLevel switch
            {
               
                1 => Color.Red,
                2 => Color.Green,
                3 => Color.Blue,
                4 => Color.Orange,
                5 => Color.Violet,
                6 => Color.Brown,
                7 => Color.Yellow,
                _ => Color.Blank,
            };

            this.northwest = new QuadTree(new AABB(new Vector2(centerX - halfDeltaX, centerY - halfDeltaY), new Vector2(centerX, centerY)), this.maxPoints, nextLevel);
            this.northeast = new QuadTree(new AABB(new Vector2(centerX, centerY - halfDeltaY), new Vector2(centerX + halfDeltaX, centerY)), this.maxPoints, nextLevel);
            this.southeast = new QuadTree(new AABB(new Vector2(centerX, centerY), new Vector2(centerX + halfDeltaX, centerY + halfDeltaY)), this.maxPoints, nextLevel);
            this.southwest = new QuadTree(new AABB(new Vector2(centerX - halfDeltaX, centerY), new Vector2(centerX, centerY + halfDeltaY)), this.maxPoints, nextLevel);

            this.northwest.DebugColor = debugColorForLevel;
            this.northeast.DebugColor = debugColorForLevel;
            this.southeast.DebugColor = debugColorForLevel;
            this.southwest.DebugColor = debugColorForLevel;
        }
        
        // Dont split reached MAX_LEVEL
        return;
        
    }

    public List<Vector2> QueryNeighbors(Vector2 point) 
    {
        // Base case
        if(this.IsLeaf() && this.boundary.Intersect(point)) {
            return this.points;
        }

        // Recursively ask children
        List<Vector2> neighbors = [];

        if(this.northwest != null) 
        {
            neighbors.AddRange(this.northwest.QueryNeighbors(point));
        }

        if (this.northeast != null)
        {
            neighbors.AddRange(this.northeast.QueryNeighbors(point));
        }

        if (this.southeast != null)
        {
            neighbors.AddRange(this.southeast.QueryNeighbors(point));
        }

        if (this.southwest != null)
        {
            neighbors.AddRange(this.southwest.QueryNeighbors(point));
        }

        return neighbors;
    }

    public void RemoveAllPoints()
    {
        // base condition
        if(this.IsLeaf()) {
            this.points.Clear();
            return;
        }

        // Recursively go through children
        this.northwest?.RemoveAllPoints();
        this.northeast?.RemoveAllPoints();
        this.southeast?.RemoveAllPoints();
        this.southwest?.RemoveAllPoints();

        this.northwest = null;
        this.northeast = null;
        this.southeast = null;
        this.southwest = null;
    }

    public void Insert(Vector2 point)
    {
        // Recursive stop conditions
        if (!this.boundary.Intersect(point))
        {
            return;
        }

        if (this.IsLeaf() && this.points.Count < this.maxPoints)
        {
            this.points.Add(point);
            return;
        }

        // If we got are a leave and there was not any space available, we split.
        if (this.IsLeaf())
        {
            this.Split();
            // After split insert our own points to newly created children
            foreach (Vector2 p in this.points)
            {
                this.northwest?.Insert(p);
                this.northeast?.Insert(p);
                this.southeast?.Insert(p);
                this.southwest?.Insert(p);
            }

            // Clear our points list because we are not leave anymore.
            this.points.Clear();
        }

        // Recursively call child nodes.
        this.northwest?.Insert(point);
        this.northeast?.Insert(point);
        this.southeast?.Insert(point);
        this.southwest?.Insert(point);
    }

    public void InsertAll(List<Vector2> pointList)
    {
        foreach(var point in pointList)
        {
            this.Insert(point);
        }
    }

    public List<Vector2> SpawnRandomPoints(int count)
    {
        var rnd = new Random();
        float x;
        float y;

        var length = Math.Abs(this.boundary.BRXY.X - this.boundary.ULXY.X) / 2;
        var width  = Math.Abs(this.boundary.BRXY.Y - this.boundary.ULXY.Y) / 2;

        var pointList = new List<Vector2>();

        for (int i = 0; i < count; i++)
        {
            // Generates a double between 0.0 ~ 1.0
            x = (float)(rnd.NextDouble() * length);
            y = (float)(rnd.NextDouble() * width);

            Quadrant direction = (Quadrant)(rnd.Next() % 4 + 1);
            
            // 1 = northwest, 2 = northeast, 3 = southeast, 4 = southwest
            // TODO: Probably need to change these directions into enums but yolo for now
            switch (direction)
            {
                case Quadrant.NorthWest:
                    x *= -1f;
                    break;
                case Quadrant.NorthEast:
                    break;
                case Quadrant.SouthEast:
                    y *= -1;
                    break;
                case Quadrant.SouthWest:
                    x *= -1; 
                    y *= -1;
                    break;
            }

            var point = new Vector2(x, y);
            this.Insert(point);
            pointList.Add(point);
        }
        return pointList;
    }

    public void DrawDebug()
    {
        // base case
        if (this.IsLeaf())
        {
            foreach (var point in this.points)
            {
                Raylib.DrawCube(new Vector3(point.X, 0, point.Y),
                    0.05f,
                    0.05f,
                    0.05f,
                    this.DebugColor);
            }
            this.boundary.Draw(this.DebugColor);
            return;
        }

        // Recursively call children.
        this.northwest?.DrawDebug();
        this.northeast?.DrawDebug();
        this.southeast?.DrawDebug();
        this.southwest?.DrawDebug();
    }

    private enum Quadrant: int
    {
        NorthWest = 1,
        NorthEast = 2,
        SouthEast = 3,
        SouthWest = 4,
    }
}