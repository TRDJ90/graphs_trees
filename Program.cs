using System.Numerics;
using Raylib_cs;

class Program
{
	public static void Main()
	{
		Raylib.InitWindow(1280, 720, "Graphs & Trees");

		// Setup 3d camera.
		Camera3D camera = new Camera3D();
		camera.Position = new Vector3(0, 10, 10);
		camera.Target = Vector3.Zero;
		camera.Up = new Vector3(0, 1, 0);
		camera.FovY = 45.0f;
		camera.Projection = CameraProjection.Perspective;

		Raylib.DisableCursor();
		Raylib.SetTargetFPS(60);

		var quadTree = new QTNode(new AABB(new Vector2(-3, 3), new Vector2(3, -3)), Color.Black);
		quadTree.split();

		// NOTE: test spit quad tree segment.	
		if(quadTree.northwest != null)
		{
			quadTree.northwest.split();

			if(quadTree.northwest?.northwest != null)
			{
				quadTree.northwest?.northwest.split();
			}
		}

		if (quadTree.southeast != null)
		{
			quadTree.southeast.split();
			quadTree.southeast.southwest?.split();
			quadTree.southeast?.southwest?.northeast?.split();
			quadTree.southeast?.southwest?.northeast?.southeast?.split();
		}

		while (!Raylib.WindowShouldClose())
		{
			// Update
			Raylib.UpdateCamera(ref camera, CameraMode.Free);

			// Draw
			Raylib.BeginDrawing();
			{
				Raylib.ClearBackground(Color.LightGray);

				Raylib.BeginMode3D(camera);
				{
					//Raylib.DrawGrid(30, 1);
					Raylib.DrawGrid(60, 0.5f);

					quadTree.DrawDebug();
				}
				Raylib.EndMode3D();
			}
			Raylib.EndDrawing();
		}

		Raylib.CloseWindow();
	}
}

public class AABB
{
	public readonly Vector2 minXY;
	public readonly Vector2 maxXY;

	public AABB(Vector2 minXY, Vector2 maxXY)
	{
		this.minXY = minXY;
		this.maxXY = maxXY;
	}

	public void Draw(Color color)
	{
		var x1 = this.minXY.X;
		var y1 = this.minXY.Y;

		var x2 = maxXY.X;
		var y2 = maxXY.Y;
		
		Raylib.DrawLine3D(new Vector3(x1, 0.01f, -y1), new Vector3(x2, 0.01f, -y1), color);
		Raylib.DrawLine3D(new Vector3(x2, 0.01f, -y1), new Vector3(x2, 0.01f, -y2), color);
		Raylib.DrawLine3D(new Vector3(x2, 0.01f, -y2), new Vector3(x1, 0.01f, -y2), color);
		Raylib.DrawLine3D(new Vector3(x1, 0.01f, -y2), new Vector3(x1, 0.01f, -y1), color);
	}
}

public class QTNode
{
	private const uint MAX_CAP = 8;
	private const uint MAX_LEVEL = 10;
	
	private Vector2[] points = new Vector2[MAX_CAP];
	private Color debugColor;
	private readonly AABB boundary;
	private readonly uint currentLvl = 0;

	public QTNode? northwest, northeast, southwest, southeast = null;

	public QTNode(AABB boundary, Color color)
	{
		this.points = new Vector2[MAX_CAP];
		this.boundary = boundary;

		Random rnd = new Random();
		this.debugColor = color;
	}

	public void split()
	{
		var boundary = this.boundary;
		var halfDeltaX = (boundary.maxXY.X - boundary.minXY.X) / 2;
		var halfDeltaY = (boundary.maxXY.Y - boundary.minXY.Y) / 2;
	
		// This lvl center
		var centerX = boundary.maxXY.X - halfDeltaX;
		var centerY = boundary.maxXY.Y - halfDeltaY;

		this.northwest = new QTNode(new AABB(new Vector2(centerX - halfDeltaX, centerY - halfDeltaY), new Vector2(centerX, centerY)), Color.Black);
		this.northeast = new QTNode(new AABB(new Vector2(centerX, centerY), new Vector2(centerX + halfDeltaX, centerY - halfDeltaY)), Color.Black);
		this.southeast = new QTNode(new AABB(new Vector2(centerX, centerY), new Vector2(centerX + halfDeltaX, centerY + halfDeltaY)), Color.Black);
		this.southwest = new QTNode(new AABB(new Vector2(centerX - halfDeltaX, centerY + halfDeltaY), new Vector2(centerX, centerY)), Color.Black);
	}

	public void DrawDebug()
	{
		this.boundary.Draw(this.debugColor);

		if (this.northwest != null)
		{
			this.northwest.DrawDebug();
		}

		if (this.northeast != null)
		{
			this.northeast.DrawDebug();
		}

		if (this.southeast != null)
		{
			this.southeast.DrawDebug();
		}

		if (this.southwest != null)
		{
			this.southwest.DrawDebug();
		}
	}
}
