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
		public double margin;
		public double padding;
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
			height = 100;
			width = 200;
			margin = 0;
			padding = 1;
			facesLeft = 2;
			facesRight = 4;
			radiusLeft = (height - (2 * padding)) / 10;
			radiusRight = (height - (2 * padding)) / 4;
			svgPath = "";
			fill = "ffffffff";
			stroke = "ff000000";
			pattern.Clear();
			CreateSvgPath(this);

		}

		public void Clone(out Shape clone)
		{
			string serialized = JsonConvert.SerializeObject(this);
			clone = JsonConvert.DeserializeObject<Shape>(serialized);
		}

		public void CreateSvgPath(Shape shape)
		{
			string svgPath = " m " + Convert.ToString((shape.height / 2)) + " " + Convert.ToString(shape.padding);
			float[,] pathPoint = new float[2, 4];
			float offset = 0f;
			double vectorStart = Math.PI;
			// create left face path - counterclockwise, starting at top, direction = pi
			double vectorCurrent = vectorStart; // direction of travel
			FullArc arc = ArcPlot.ArcDivider(Math.PI, Math.PI / 2, shape.facesLeft + 1);

			for (int j = 0; j < 2; j++) // creates left and bottom then right and top
			{
				double radius = (j == 0) ? radiusLeft : radiusRight;
				if (radius < 0) radius = 0;
				double gap = (Math.Tan(arc.Angle/2) * (shape.height/2 - shape.padding)) - (Math.Tan(arc.Angle / 2) * radius); // gap = half of line between arcs
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
				pathPoint[0, 0] = Convert.ToSingle((Math.Cos(vectorCurrent) * gap) + (Math.Cos(vectorCurrent) * (shape.width - shape.height))); // gap after last curve
				pathPoint[1, 0] = Convert.ToSingle(Math.Sin(vectorCurrent) * gap);
				svgPath += " l " + pathPoint[0, 0] + " " + pathPoint[1, 0];
				arc = ArcPlot.ArcDivider(Math.PI, Math.PI / 2, shape.facesRight + 1);
			}
			svgPath += " z ";
			shape.svgPath = svgPath;
		}

	}
}

