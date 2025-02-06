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

        return point.X > this.ULXY.X && point.Y > this.BRXY.Y && point.X < this.BRXY.X && point.Y < this.ULXY.Y;
    }

    public bool IntersectWithCircle(Vector2 point, float radius)
    {
        float distMinX = Math.Abs(this.ULXY.X - point.X);
        float distMaxY = Math.Abs(this.ULXY.Y - point.Y);

        float distMaxX = Math.Abs(this.BRXY.X - point.X);
        float distMinY = Math.Abs(this.BRXY.X - point.X);

        return distMinX < radius || distMinY < radius || distMaxX < radius || distMaxY < radius;
    }

    public void Draw(Color color)
    {
        var x1 = this.ULXY.X;
        var y1 = this.ULXY.Y;

        var x2 = BRXY.X;
        var y2 = BRXY.Y;

        Raylib.DrawLine3D(new Vector3(x1, 0.01f, -y1), new Vector3(x2, 0.01f, -y1), color);
        Raylib.DrawLine3D(new Vector3(x2, 0.01f, -y1), new Vector3(x2, 0.01f, -y2), color);
        Raylib.DrawLine3D(new Vector3(x2, 0.01f, -y2), new Vector3(x1, 0.01f, -y2), color);
        Raylib.DrawLine3D(new Vector3(x1, 0.01f, -y2), new Vector3(x1, 0.01f, -y1), color);
    }
}