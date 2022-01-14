using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Threading;

namespace ShapeTest
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

			canvasView.InvalidateSurface();

			//Device.StartTimer(TimeSpan.FromSeconds(1f / 60), () =>
			//{
			//	canvasView.InvalidateSurface();
			//	return true;
			//});
		}

		public static List<ShapeItem> shapeList = new List<ShapeItem>();

		public const double pi = Math.PI;

		private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			// Prepare canvas
			SKSurface surface = e.Surface;
			SKCanvas canvas = surface.Canvas;
			// canvas transforms
			//int width = e.Info.Width;
			//int height = e.Info.Height;
			//canvas.Translate(width / 2, height / 2);
			//canvas.Scale(Math.Min(width / 210f, height / 520f));
			canvas.Scale(3);
			// create and draw the screen
			DrawScreen(canvas);
		}
		private static void DrawScreen(SKCanvas canvas)
		{

			// test items - DEBUG - App can loop in debug creating additional ShapeItems.
			// Using break points to pause the app can recreate this issue.
			// Minimum 2 instances are created without while or if statements
			int testQuantity = 5;
			while (shapeList.Count < testQuantity)
			{
				shapeList.Add(new ShapeItem());
			}

			double[] location = new double[2];
			location[0] = 0; location[1] = 0;

			// draw everything on the list once using ID to determine saturation

			foreach (ShapeItem shapeItem in shapeList)
			{
				shapeItem.Clear();
				shapeItem.Shape.Location[0] = location[0];
				shapeItem.Shape.Location[1] = location[1];
				shapeItem.Shape.CreateSvgPath(shapeItem.Shape);
				SKPath bezierPath = SKPath.ParseSvgPathData(shapeItem.Shape.svgPath);
				//SKColor fillColor = SKColor.Parse(shapeItem.Shape.fill);
				SKColor fillColor = SKColor.Parse("ff" + Convert.ToString(222222 * (shapeItem.ID + 1))); // DEBUG - ShapeItem iD not working making this not work
				SKPaint paintFill = new SKPaint() { Style = SKPaintStyle.Fill, Color = fillColor, TextSize = Convert.ToSingle(2 * shapeItem.Shape.Height / 3) };
				SKColor strokeColor = SKColor.Parse(shapeItem.Shape.stroke);
				SKPaint paintStroke = new SKPaint() { Style = SKPaintStyle.Stroke, Color = strokeColor, TextSize = Convert.ToSingle(2 * shapeItem.Shape.Height / 3) };

				canvas.DrawPath(bezierPath, paintFill);
				canvas.DrawPath(bezierPath, paintStroke);
				//SKPoint textPoint = new SKPoint(-150, 100);
				SKPoint circlePoint = new SKPoint(0, 0);

				SKRect rect = new SKRect(0, 0, Convert.ToSingle(shapeItem.Shape.Width), Convert.ToSingle(shapeItem.Shape.Height));
				canvas.DrawRect(rect, paintStroke);
				canvas.DrawCircle(circlePoint, 2, paintFill);
				//canvas.DrawText(listShape.Shape.svgPath, textPoint, paintFill);
				location[1] += shapeItem.Shape.Height;
			}
		}
	}
}
