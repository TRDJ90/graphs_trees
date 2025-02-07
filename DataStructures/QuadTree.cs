using Primitives;
using Raylib_cs;
using System.Numerics;

namespace DataStructures;

public interface IQTElement 
{
    Vector2 GetPosition();
}

public class QuadTree<T> where T : IQTElement 
{
    private const uint MAX_LEVEL = 8;

    private readonly List<T> elements;
    private readonly uint maxPoints;
    private readonly uint level;
    
    private QuadTree<T>? northwest, northeast, southwest, southeast = null;

    public readonly AABB boundary;
    public Color DebugColor {get; set;}

    public QuadTree(AABB boundary, uint maxPoints, uint level)
    {
        this.boundary = boundary;
        this.elements = [];
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

            this.northwest = new QuadTree<T>(new AABB(new Vector2(centerX - halfDeltaX, centerY - halfDeltaY), new Vector2(centerX, centerY)), this.maxPoints, nextLevel);
            this.northeast = new QuadTree<T>(new AABB(new Vector2(centerX, centerY - halfDeltaY), new Vector2(centerX + halfDeltaX, centerY)), this.maxPoints, nextLevel);
            this.southeast = new QuadTree<T>(new AABB(new Vector2(centerX, centerY), new Vector2(centerX + halfDeltaX, centerY + halfDeltaY)), this.maxPoints, nextLevel);
            this.southwest = new QuadTree<T>(new AABB(new Vector2(centerX - halfDeltaX, centerY), new Vector2(centerX, centerY + halfDeltaY)), this.maxPoints, nextLevel);

            this.northwest.DebugColor = debugColorForLevel;
            this.northeast.DebugColor = debugColorForLevel;
            this.southeast.DebugColor = debugColorForLevel;
            this.southwest.DebugColor = debugColorForLevel;
        }
        
        // Dont split reached MAX_LEVEL
        return;
        
    }

    public List<T> QueryNeighbors(T point) 
    {
        // Base case
        if(this.IsLeaf() && this.boundary.Intersect(point.GetPosition())) {
            return this.elements;
        }

        // Recursively ask children
        List<T> neighbors = [];

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

    public List<T> RadialQueryNeighbors(T point, float radius)
    {
        // Base case
        if (this.IsLeaf() && this.boundary.IntersectWithCircle(point.GetPosition(), radius))
        {   
            var list = new List<T>();

            foreach(var n in this.elements)
            {
                Vector2 delta = point.GetPosition() - n.GetPosition();
                if(delta.Length() < radius)
                {
                    list.Add(n);
                }
            }

            return list;
        }

        // Recursively ask children
        List<T> neighbors = [];

        if (this.northwest != null)
        {
            neighbors.AddRange(this.northwest.RadialQueryNeighbors(point, radius));
        }

        if (this.northeast != null)
        {
            neighbors.AddRange(this.northeast.RadialQueryNeighbors(point, radius));
        }

        if (this.southeast != null)
        {
            neighbors.AddRange(this.southeast.RadialQueryNeighbors(point, radius));
        }

        if (this.southwest != null)
        {
            neighbors.AddRange(this.southwest.RadialQueryNeighbors(point, radius));
        }

        return neighbors;
    }

    public void RemoveAllElements()
    {
        // base condition
        if(this.IsLeaf()) {
            this.elements.Clear();
            return;
        }

        // Recursively go through children
        this.northwest?.RemoveAllElements();
        this.northeast?.RemoveAllElements();
        this.southeast?.RemoveAllElements();
        this.southwest?.RemoveAllElements();

        this.northwest = null;
        this.northeast = null;
        this.southeast = null;
        this.southwest = null;
    }

    public void Insert(T point)
    {
        // Recursive stop conditions
        if (!this.boundary.Intersect(point.GetPosition()))
        {
            return;
        }

        if (this.IsLeaf() && this.elements.Count < this.maxPoints)
        {
            this.elements.Add(point);
            return;
        }

        // If we got are a leave and there was not any space available, we split.
        if (this.IsLeaf())
        {
            this.Split();
            // After split insert our own points to newly created children
            foreach (T p in this.elements)
            {
                this.northwest?.Insert(p);
                this.northeast?.Insert(p);
                this.southeast?.Insert(p);
                this.southwest?.Insert(p);
            }

            // Clear our points list because we are not leave anymore.
            this.elements.Clear();
        }

        // Recursively call child nodes.
        this.northwest?.Insert(point);
        this.northeast?.Insert(point);
        this.southeast?.Insert(point);
        this.southwest?.Insert(point);
    }

    public void InsertAll(List<T> pointList)
    {
        foreach(var point in pointList)
        {
            this.Insert(point);
        }
    }

    public void DrawDebug()
    {
        // base case
        if (this.IsLeaf())
        {
            this.boundary.Draw(this.DebugColor);
            return;
        }

        // Recursively call children.
        this.northwest?.DrawDebug();
        this.northeast?.DrawDebug();
        this.southeast?.DrawDebug();
        this.southwest?.DrawDebug();
    }
}