using SkiaSharp;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ShapeTest
{
	
	public class ShapeItem
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }
		public List<int> parents = new List<int> { 0 };
		public List<int> children = new List<int> { 0 };
		public string content = "";
		public Shape shape = new Shape();
		public void Clear()
		{
			parents.Clear();
			children.Clear();
			content = "";
			shape.Clear();
		}
	}

public class Shape
	{
		public double height;
		public double width;
		public double scale;
		public double[] location = new double[2];
		public double[] margin = { 0, 0, 0, 0 };
		public double[] padding = { 5, 5, 5, 5 };
		public int facesLeft;
		public int facesRight;
		public double radiusLeft;
		public double radiusRight;
		public string svgPath;
		public string fill;
		public string stroke;
		public List<byte> pattern = new List<byte>() { 0 };

		public void Clear()
		{
			height = 50;
			width = 100;
			location[0] = 0; location[1] = 0;
			margin[0] = 2; margin[1] = 22; margin[2] = 2; margin[3] = 2;
			padding[0] = 2; padding[1] = 2; padding[2] = 2; padding[3] = 2;
			facesLeft = 8;
			facesRight = 4;
			radiusLeft = (height - (padding[1] + padding[3])) / 3;
			radiusRight = (height - (padding[1] + padding[3])) / 3;
			svgPath = "";
			//fill = "ffffffff";
			stroke = "ff000000";
			pattern.Clear();
			CreateSvgPath(this);
		}

		//public void Clone(out Shape clone)
		//{
		//	string serialized = JsonConvert.SerializeObject(this);
		//	clone = JsonConvert.DeserializeObject<Shape>(serialized);
		//}

		public void CreateSvgPath(Shape shape)
		{

			if (shape.width == 0)
			{
				//SKRect bounds;
				//canvas.GetLocalClipBounds(out bounds);
				//shape.width = bounds.Width;
			}
			if (shape.height == 0)
			{
				//SKRect bounds;
				//canvas.GetLocalClipBounds(out bounds);
				//shape.height = bounds.Height;
			}

			double vectorCurrent = Math.PI; // starting at top, direction = pi
			FullArc arc = ArcPlot.ArcDivider(Math.PI, Math.PI / 2, shape.facesLeft + 1);
			double halfArcCos = Math.Abs(Math.Cos(arc.Angle / 2));
			//how far away from the laft and right sides is the start of the end pieces
			float offsetLeft = Convert.ToSingle((((shape.height - (shape.padding[1] + shape.padding[3])) / 2) / halfArcCos) - (shape.radiusLeft / halfArcCos) + shape.radiusLeft);
			float offsetRight = Convert.ToSingle((((shape.height - (shape.padding[1] + shape.padding[3])) / 2) / halfArcCos) - (shape.radiusRight / halfArcCos) + shape.radiusRight);
			string svgPath = " m " + (offsetLeft + shape.padding[0] + shape.location[0]) + " " + (shape.padding[1] + shape.location[1]);

			float[,] pathPoint = new float[2, 4];
			for (int j = 0; j <= 1; j++) // creates left and bottom then right and top
			{
				double radius = (j == 0) ? radiusLeft : radiusRight;
				if (radius < 0) radius = 0;
				double gap = (Math.Tan(arc.Angle / 2) * ((shape.height - (shape.padding[1] + shape.padding[3])) / 2)) - (Math.Tan(arc.Angle / 2) * radius); // gap = half of line between arcs
				for (int i = 0; i < arc.Count; i++)
				{
					pathPoint = ArcPlot.ArcPointsArray(vectorCurrent, arc.Angle, radius);
					if (i == 0) // single gap before first curve
					{
						pathPoint[0, 0] = Convert.ToSingle(Math.Cos(vectorCurrent) * gap);
						pathPoint[1, 0] = Convert.ToSingle(Math.Sin(vectorCurrent) * gap);
					}
					else // *2 = 1 gap after curve and 1 before next curve
					{
						pathPoint[0, 0] = Convert.ToSingle(Math.Cos(vectorCurrent) * gap * 2);
						pathPoint[1, 0] = Convert.ToSingle(Math.Sin(vectorCurrent) * gap * 2);
					}
					svgPath += " l " + pathPoint[0, 0] + " " + pathPoint[1, 0];
					svgPath += " c " + pathPoint[0, 1] + " " + pathPoint[1, 1] + " " + pathPoint[0, 2] + " " + pathPoint[1, 2] + " " + pathPoint[0, 3] + " " + pathPoint[1, 3];
					vectorCurrent -= arc.Angle;
				}
				pathPoint[0, 0] = Convert.ToSingle(Math.Cos(vectorCurrent) * (shape.width - (offsetLeft + offsetRight) + gap - (shape.padding[0] + shape.padding[2])));
				//pathPoint[0, 0] = Convert.ToSingle((Math.Cos(vectorCurrent) * gap) + (Math.Cos(vectorCurrent) * (shape.width - shape.height))); // gap after last curve
				pathPoint[1, 0] = Convert.ToSingle(Math.Sin(vectorCurrent) * gap);
				svgPath += " l " + pathPoint[0, 0] + " " + pathPoint[1, 0];
				arc = ArcPlot.ArcDivider(Math.PI, Math.PI / 2, shape.facesRight + 1);
			}
			svgPath += " z ";
			shape.svgPath = svgPath;
		}
	}
}

