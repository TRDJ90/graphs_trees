using System.Numerics;
using DataStructures;

namespace Primitives;

public class Boids: IQTElement
{
    public Vector2 Position {get; set; } = Vector2.Zero;
    public Vector2 Direction {get; set; } = Vector2.Zero;
    public Vector2 Acceleration {get; set; } = Vector2.Zero;

    public Boids()
    {}

    public Boids(Vector2 position)
    {
        this.Position = position;
    }

    public Vector2 GetPosition()
    {
        return this.Position;
    }
}