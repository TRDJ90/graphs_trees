using System.Data;
using System.Numerics;
using System.Reflection;
using DataStructures;

namespace Primitives;

public class Boid : IQTElement
{
    private static readonly float MAX_FORCE = 0.8f;
    private static readonly float SPEED = 0.15f;

    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 Direction { get; set; } = Vector2.Zero;
    public Vector2 Acceleration { get; set; } = Vector2.Zero;

    public float PerceptionRadius { get; set; } = 0.4f;

    public Boid()
    { }

    public Boid(Vector2 position)
    {
        this.Position = position;
    }

    public Vector2 GetPosition()
    {
        return this.Position;
    }

    public void Update(List<Boid> neighbours, AABB boundary)
    {
        var alignment = this.Align(neighbours);
        var seperation = this.Separation(neighbours);

        this.Acceleration = Vector2.Zero;

        this.Acceleration += alignment;
        this.Acceleration += seperation;

        this.Position += this.Direction;
        this.Direction += this.Acceleration;
        this.Direction = LimitMagnitude(this.Direction, SPEED);
    }

    private Vector2 LimitMagnitude(Vector2 vec, float limit)
    {
        if (vec.Length() > limit)
        {
            return Vector2.Normalize(vec) * limit;
        }

        return new Vector2(vec.X, vec.Y);
    }

    private Vector2 SetMagnitude(Vector2 vec, float magnitude)
    {
        return Vector2.Normalize(vec) * magnitude;
    }

    private Vector2 Align(List<Boid> neighbours)
    {
        var steering = Vector2.Zero;
        var count = 0;
        foreach (var n in neighbours)
        {
            var diff = this.Position - n.Position;
            var dist = diff.Length();
            if (this != n && dist < this.PerceptionRadius)
            {
                steering += n.Direction;
                count += 1;
            }
        }

        if (count > 0)
        {
            steering /= count;
            steering = SetMagnitude(steering, SPEED);
            steering -= this.Direction;
            steering = LimitMagnitude(steering, MAX_FORCE);
        }

        return steering;
    }

    private Vector2 Separation(List<Boid> neighbours)
    {
        var steering = Vector2.Zero;
        var count = 0;
        foreach (var n in neighbours)
        {
            var diff = this.Position - n.Position;
            var dist = diff.Length();
            if (this != n && dist < 0.08f)
            {
                diff /= dist * dist;
                steering += diff;
                count += 1;
            }
        }

        if (count > 0 && steering != Vector2.Zero)
        {
            steering /= count;
            steering = SetMagnitude(steering, SPEED);
            steering -= this.Direction;
            steering = LimitMagnitude(steering, MAX_FORCE);
        }

        return steering;
    }
}