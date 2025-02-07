using System.Numerics;
using DataStructures;
using Primitives;
using Raylib_cs;

class Program
{
	public static void Main()
	{
		Raylib.InitWindow(1280, 720, "Graphs & Trees");

		// Setup 3d camera.
		// NOTE: Raylib -Z is the direction going into the monitor!!!
		Camera3D camera = new()
		{
			Position = new Vector3(5, 8, 6),
			Target = new Vector3(5, 0, -3),
			Up = new Vector3(0, 1, 0),
			FovY = 45.0f,
			Projection = CameraProjection.Perspective
		};

		Raylib.DisableCursor();
		Raylib.SetTargetFPS(60);

		var qtBoundary = new AABB(new Vector2(0, 15), new Vector2(15, 0));
		var quadTree = new QuadTree<Boid>(qtBoundary, 8, 0);

		var amount = 512;
		var boids = SpawnRandomPoints(amount, qtBoundary);
		quadTree.InsertAll(boids);

		int count = 0;
		while (!Raylib.WindowShouldClose())
		{
			// Update
			Raylib.UpdateCamera(ref camera, CameraMode.Free);

			// Draw
			Raylib.BeginDrawing();
			{
				Raylib.ClearBackground(Color.White);


				Raylib.BeginMode3D(camera);
				{
					UpdatePoints(boids, quadTree);
					quadTree.RemoveAllElements();
					quadTree.InsertAll(boids);

					foreach (var boid in boids)
					{
						Raylib.DrawSphere(new Vector3(boid.Position.X, 0, -boid.Position.Y),
							0.05f,
							Color.SkyBlue);

					}
					quadTree.DrawDebug();
				}
				Raylib.EndMode3D();
			}
			Raylib.EndDrawing();
			count += 1;
		}

		Raylib.CloseWindow();
	}

	static void UpdatePoints(List<Boid> boids, QuadTree<Boid> qt)
	{
		for (int i = 0; i < boids.Count; i++)
		{
			var boid = boids[i];
			var neighbors = qt.RadialQueryNeighbors(boid, boid.PerceptionRadius * 2);
			boid.Update(neighbors, qt.boundary);

			if(!qt.boundary.Intersect(boid.Position))
			{
				boid.Position = Clamp(boid.Position, qt.boundary);
			}
		}
	}

	static Vector2 Clamp(Vector2 position, AABB boundary)
	{
		var minX = boundary.ULXY.X;
		var maxX = boundary.BRXY.X;
		var minY = boundary.BRXY.Y;
		var maxY = boundary.ULXY.Y;
		
		var updatedX = position.X;
		var updatedY = position.Y;

		// Loop around
		if (updatedX < minX)
		{
			updatedX = maxX - 0.01f;
		}
		if (updatedX > maxX)
		{
			updatedX = minX + 0.01f;
		}

		if (updatedY < minY)
		{
			updatedY = maxY - 0.01f;
		}
		if (updatedY > maxY)
		{
			updatedY = minY + 0.01f;
		}

		return new Vector2(updatedX, updatedY);
	}

	static List<Boid> SpawnRandomPoints(int count, AABB boundary)
	{
		var rnd = new Random();
		float x, y, dirX, dirY;

		var length = Math.Abs(boundary.BRXY.X - boundary.ULXY.X);
		var width = Math.Abs(boundary.BRXY.Y - boundary.ULXY.Y);

		var boids = new List<Boid>();

		for (int i = 0; i < count; i++)
		{
			// Generates a double between 0.0 ~ 1.0
			x = (float)(rnd.NextDouble() * length);
			y = (float)(rnd.NextDouble() * width);

			dirX = (float)rnd.NextDouble() - (float)rnd.NextDouble();
			dirY = (float)rnd.NextDouble() - (float)rnd.NextDouble();

			var boid = new Boid()
			{
				Position = new Vector2(x, y),
				Direction = Vector2.Normalize(new Vector2(dirX, dirY)),
				Acceleration = Vector2.Zero,
			};

			boids.Add(boid);
		}

		return boids;
	}
}

