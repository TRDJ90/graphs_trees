using System.Numerics;
using Raylib_cs;

namespace Primitives;

public class AABB
{
    // Represent Top Left and Bottom Right points of a Area.
    public readonly Vector2 ULXY;
    public readonly Vector2 BRXY;

    public AABB(Vector2 topLeft, Vector2 bottomRight)
    {
        this.ULXY = topLeft;
        this.BRXY = bottomRight;
    }

    public bool Intersect(Vector2 point)
    {
        bool withinMinX = point.X > this.ULXY.X;
        bool withinMaxY = point.Y < this.ULXY.Y;

        bool withinMaxX = point.X < this.BRXY.X;
        bool withinMinY = point.Y > this.BRXY.Y;

        if ((withinMinX && withinMinY && withinMaxX && withinMaxY) == false)
        {
            return false;
        }

        return withinMinX && withinMinY && withinMaxX && withinMaxY;
    }

    public void Draw(Color color)
    {
        var x1 = this.ULXY.X;
        var y1 = this.ULXY.Y;

        var x2 = BRXY.X;
        var y2 = BRXY.Y;

        Raylib.DrawLine3D(new Vector3(x1, 0.01f, y1), new Vector3(x2, 0.01f, y1), color);
        Raylib.DrawLine3D(new Vector3(x2, 0.01f, y1), new Vector3(x2, 0.01f, y2), color);
        Raylib.DrawLine3D(new Vector3(x2, 0.01f, y2), new Vector3(x1, 0.01f, y2), color);
        Raylib.DrawLine3D(new Vector3(x1, 0.01f, y2), new Vector3(x1, 0.01f, y1), color);
    }
}