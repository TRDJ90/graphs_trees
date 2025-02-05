using System.Diagnostics;
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
        Camera3D camera = new()
        {
            Position = new Vector3(0, 10, -15),
            Target = Vector3.Zero,
            Up = new Vector3(0, 1, 0),
            FovY = 45.0f,
            Projection = CameraProjection.Perspective
        };

        Raylib.DisableCursor();
		Raylib.SetTargetFPS(60);

		var quadTree = new QuadTree(new AABB(new Vector2(-10, 10), new Vector2(10, -10)), 16, 0);
		var points = quadTree.SpawnRandomPoints(4096);

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
					//Raylib.DrawGrid(200, 0.5f);
					UpdatePoints(points, quadTree);

					quadTree.RemoveAllPoints();
					quadTree.InsertAll(points);
					quadTree.DrawDebug();
				}
				Raylib.EndMode3D();
			}
			Raylib.EndDrawing();
		}

		Raylib.CloseWindow();
	}

	static void UpdatePoints(List<Vector2> points, QuadTree qt) {
		var rnd = new Random();
		var velocity = 0.01f;

		for (int i = 0; i < points.Count; i++) {
			var point = points[i];
			var updatedX = point.X + velocity;
			var updatedY = point.Y + velocity;

			// Loop around
			if (updatedX < -10)
			{
				updatedX = 10;
			}
			if(updatedX > 10)
			{
				updatedX = -10;
			}

			if (updatedY < -10)
			{
				updatedY = 10;
			}
			if (updatedY > 10)
			{
				updatedY = -10;
			}

			var updatedPoint = new Vector2(updatedX, updatedY);
			points[i] = updatedPoint;
		}
	}
}


