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
			//Prepare canvas
			SKSurface surface = e.Surface;
			SKCanvas canvas = surface.Canvas;

			// canvas transforms
			int width = e.Info.Width;
			int height = e.Info.Height;
			//canvas.Translate(width / 2, height / 2);
			//canvas.Scale(Math.Min(width / 210f, height / 520f));
			canvas.Scale(3);

			// create and draw the screen
			DrawScreen(canvas);
		}
		private static void DrawScreen(SKCanvas canvas)
		{
			
			shapeList.Add(new ShapeItem());
			shapeList.Add(new ShapeItem());
			shapeList.Add(new ShapeItem());
			shapeList.Add(new ShapeItem());

			double[] location = new double[2];
			location[0] = 0; location[1] = 0;
			foreach (ShapeItem shapeItem in shapeList)
			{
				
				shapeItem.Clear();
				shapeItem.shape.location[0] = location[0];
				shapeItem.shape.location[1] = location[1];
				SKPath bezierPath = SKPath.ParseSvgPathData(shapeItem.shape.svgPath);
				//SKColor fillColor = SKColor.Parse(shapeItem.shape.fill);
				SKColor fillColor = SKColor.Parse("ff" + Convert.ToString(111111 * (shapeItem.ID + 1)));
				SKPaint paintFill = new SKPaint() { Style = SKPaintStyle.Fill, Color = fillColor, TextSize = Convert.ToSingle(2 * shapeItem.shape.height / 3) };
				SKColor strokeColor = SKColor.Parse(shapeItem.shape.stroke);
				SKPaint paintStroke = new SKPaint() { Style = SKPaintStyle.Stroke, Color = strokeColor, TextSize = Convert.ToSingle(2 * shapeItem.shape.height / 3) };


				canvas.DrawPath(bezierPath, paintFill);
				canvas.DrawPath(bezierPath, paintStroke);
				//SKPoint textPoint = new SKPoint(-150, 100);
				SKPoint circlePoint = new SKPoint(0, 0);

				SKRect rect = new SKRect(0, 0, Convert.ToSingle(shapeItem.shape.width), Convert.ToSingle(shapeItem.shape.height));
				canvas.DrawRect(rect, paintStroke);
				canvas.DrawCircle(circlePoint, 2, paintFill);
				//canvas.DrawText(listShape.shape.svgPath, textPoint, paintFill);
				//Console.WriteLine(listShape.shape.svgPath);
				location[1] += shapeItem.shape.height;
			}
		}
	}
}
