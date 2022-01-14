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
		[PrimaryKey, AutoIncrement] // DEBUG - what is it incrementing? not iD, which is what I want
		private int iD { get; set; }
		public int ID { get => iD; set => iD = value; }
		public List<int> parents = new List<int> { 0 };
		public List<int> children = new List<int> { 0 };
		public string content = "";
		private Shape shape = new Shape();
		public Shape Shape { get => shape; set => shape = value; }
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
		public int id;
		private double height;
		private double width;
		public double scale;
		private double[] location = new double[2];
		public double[] margin = { 0, 0, 0, 0 };
		public double[] padding = { 5, 5, 5, 5 };
		private int facesLeft;
		private int facesRight;
		private double radiusLeft;
		private double radiusRight;
		public string svgPath;
		public string fill;
		public string stroke;
		public List<byte> pattern = new List<byte>() { 0 };

		public double[] Location { get => location; set => location = value; }
		public double Height { get => height; set => height = value; }
		public double Width { get => width; set => width = value; }
		public int FacesLeft { get => facesLeft; set => facesLeft = value; }
		public int FacesRight { get => facesRight; set => facesRight = value; }
		public double RadiusLeft { get => radiusLeft; set => radiusLeft = value; }
		public double RadiusRight { get => radiusRight; set => radiusRight = value; }
		


		public void Clear()
		{
			Height = 50;
			width = 100;
			location[0] = 0; location[1] = 0;
			margin[0] = 2; margin[1] = 2; margin[2] = 2; margin[3] = 2;
			padding[0] = 2; padding[1] = 2; padding[2] = 2; padding[3] = 2;
			facesLeft = 2;
			facesRight = 2;
			radiusLeft = (Height - (padding[1] + padding[3])) / 4;
			radiusRight = (Height - (padding[1] + padding[3])) / 4;
			svgPath = "";
			fill = "ff999999";
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
			if (shape.Height == 0)
			{
				//SKRect bounds;
				//canvas.GetLocalClipBounds(out bounds);
				//shape.height = bounds.Height;
			}

			double vectorCurrent = Math.PI; // starting at top, direction = pi
			FullArc arc = ArcPlot.ArcDivider(Math.PI, Math.PI / 2, shape.facesLeft + 1);
			double halfArcCos = Math.Abs(Math.Cos(arc.Angle / 2));
			//how far away from the laft and right sides is the start of the end pieces
			float offsetLeft = Convert.ToSingle((((shape.Height - (shape.padding[1] + shape.padding[3])) / 2) / halfArcCos) - (shape.radiusLeft / halfArcCos) + shape.radiusLeft);
			float offsetRight = Convert.ToSingle((((shape.Height - (shape.padding[1] + shape.padding[3])) / 2) / halfArcCos) - (shape.radiusRight / halfArcCos) + shape.radiusRight);
			string svgPath = " m " + (offsetLeft + shape.padding[0] + shape.location[0]) + " " + (shape.padding[1] + shape.location[1]);

			float[,] pathPoint = new float[2, 4];
			for (int j = 0; j <= 1; j++) // creates left and bottom then right and top
			{
				double radius = (j == 0) ? radiusLeft : radiusRight;
				if (radius < 0) radius = 0;
				double gap = (Math.Tan(arc.Angle / 2) * ((shape.Height - (shape.padding[1] + shape.padding[3])) / 2)) - (Math.Tan(arc.Angle / 2) * radius); // gap = half of line between arcs
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
				arc = ArcPlot.ArcDivider(Math.PI, Math.PI / 2, shape.facesRight + 1); // prepare for right side
			}
			svgPath += " z ";
			shape.svgPath = svgPath;
		}
	}
}

