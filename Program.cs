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
			Position = new Vector3(2, 5, 5),
			Target = new Vector3(2, 0, -1),
			Up = new Vector3(0, 1, 0),
			FovY = 45.0f,
			Projection = CameraProjection.Perspective
		};

		Raylib.DisableCursor();
		Raylib.SetTargetFPS(60);

		var qtBoundary = new AABB(new Vector2(0, 5), new Vector2(5, 0));
		var quadTree = new QuadTree<Boids>(qtBoundary, 4, 0);

	    var radius = 0.8f;
		var points = SpawnRandomPoints(256, qtBoundary);
		//var points = SpawnTestPoints(quadTree);

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
					UpdatePoints(points, quadTree.boundary);
					quadTree.RemoveAllElements();
					quadTree.InsertAll(points);

					var testBoid = new Boids(new Vector2(1.55f, 1.72f));
					var queryNeighbors = quadTree.RadialQueryNeighbors(testBoid, radius);

					foreach (var point in points)
					{
						if (!queryNeighbors.Contains(point))
						{
							Raylib.DrawSphere(new Vector3(point.Position.X, 0, -point.Position.Y),
								0.05f,
								Color.SkyBlue);
						}
					}

					foreach (var point in queryNeighbors)
					{
						Raylib.DrawSphere(new Vector3(point.Position.X, 0, -point.Position.Y),
							0.05f,
							Color.Green);
					}

					Raylib.DrawCylinder(new Vector3(testBoid.Position.X, 0, -testBoid.Position.Y), radius, radius, 0.0001f, 32, new Color(255, 0, 0, 50));

					Raylib.DrawSphere(new Vector3(testBoid.Position.X, 0, -testBoid.Position.Y),
							0.05f,
							Color.Red);

					quadTree.DrawDebug();
				}
				Raylib.EndMode3D();
			}
			Raylib.EndDrawing();
		}

		Raylib.CloseWindow();
	}

	// static List<IQTElement> SpawnTestPoints(QuadTree<IQTElement> qt)
	// {
	// 	var list = new List<IQTElement>
	// 	{
	// 		new(0.8f, 0.6f),
    //         new(0.4f, 0.7f),

	// 		new(1.8f, 0.6f),
	// 		new(1.6f, 1.7f),
	// 		new(1.51f, 1.65f),
	// 		new(1.6f, 1.8f),
	// 	};

	// 	foreach (var p in list)
	// 	{
	// 		qt.Insert(p);
	// 	}


	// 	return list;
	// }

	static void UpdatePoints(List<Boids> points, AABB boundary)
	{
		var rnd = new Random();
		var velocity = 0.01f;

		for (int i = 0; i < points.Count; i++)
		{
			var point = points[i];
			var updatedX = point.Position.X + velocity;
			var updatedY = point.Position.Y + velocity;

			// Loop around
			if (updatedX < boundary.ULXY.X)
			{
				updatedX = boundary.BRXY.X;
			}
			if (updatedX > boundary.BRXY.X)
			{
				updatedX = boundary.ULXY.X;
			}

			if (updatedY < boundary.BRXY.Y)
			{
				updatedY = boundary.ULXY.Y;
			}
			if (updatedY > boundary.ULXY.Y)
			{
				updatedY = boundary.BRXY.Y;
			}

			var updatedPoint = new Vector2(updatedX, updatedY);
			points[i].Position = updatedPoint;
		}
	}

	public static List<Boids> SpawnRandomPoints(int count, AABB boundary)
	{
		var rnd = new Random();
		float x;
		float y;

		var length = Math.Abs(boundary.BRXY.X - boundary.ULXY.X);
		var width = Math.Abs(boundary.BRXY.Y - boundary.ULXY.Y);

		var boids = new List<Boids>();

		for (int i = 0; i < count; i++)
		{
			// Generates a double between 0.0 ~ 1.0
			x = (float)(rnd.NextDouble() * length);
			y = (float)(rnd.NextDouble() * width);

			var boid = new Boids(new Vector2(x, y));
			boids.Add(boid);
		}
		return boids;
	}
}

