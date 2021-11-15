using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

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

		SKPaint blackFill = new SKPaint
		{
			Style = SKPaintStyle.Fill,
			Color = SKColors.Black
		};
		SKPaint redFill = new SKPaint
		{
			Style = SKPaintStyle.Fill,
			Color = SKColors.DarkRed,
		};


		public const double pi = Math.PI;

		private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			//Prepare canvas
			SKSurface surface = e.Surface;
			SKCanvas canvas = surface.Canvas;
			canvas.DrawPaint(redFill);

			// canvas transforms
			int width = e.Info.Width;
			int height = e.Info.Height;
			canvas.Translate(width / 2, height / 2);
			canvas.Scale(Math.Min(width / 210f, height / 520f));

			// initialize
			ShapeItem listShape = new ShapeItem();
			listShape.Clear();

			// create and draw shape
			DrawScreen(canvas, listShape);

		}

		private static void DrawScreen(SKCanvas canvas, ShapeItem listShape)
		{
			SKPath bezierPath = SKPath.ParseSvgPathData(listShape.shape.svgPath);
			SKColor fillColor = SKColor.Parse(listShape.shape.fill);
			SKPaint paintFill = new SKPaint() { Style = SKPaintStyle.Fill, Color = fillColor, TextSize = 6 };
			SKColor strokeColor = SKColor.Parse(listShape.shape.stroke);
			SKPaint paintStroke = new SKPaint() { Style = SKPaintStyle.Stroke, Color = strokeColor };

			canvas.DrawPath(bezierPath, paintFill);
			canvas.DrawPath(bezierPath, paintStroke);
			SKPoint textPoint = new SKPoint(-150, 100);
			SKPoint circlePoint = new SKPoint(0, 0);

			SKRect rect = new SKRect(0, 0, 200, 100);
			canvas.DrawRect(rect, paintStroke);
			canvas.DrawCircle(circlePoint, 2, paintFill);
			//canvas.DrawText(listShape.shape.svgPath, textPoint, paintFill);
			//Console.WriteLine(listShape.shape.svgPath);
		}
	}
}
