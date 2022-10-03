//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using System;
using System.Linq;
using Avalonia.Media;

namespace Avalonia.SimpleChart;

public class LineSeries : ChartSeries
{
	#region Fields
	public static readonly StyledProperty<IPen?> LinePenProperty =
		AvaloniaProperty.Register<LineSeries, IPen?>(nameof(LinePen));
	#endregion

	#region Properties
	public override SeriesType SeriesType => SeriesType.Line;

	public IPen? LinePen
	{
		get => GetValue(LinePenProperty);
		set => SetValue(LinePenProperty, value);
	}
	#endregion

	#region Methods
	public override void RenderSeries(ChartRenderer chartRenderer, ChartViewport chartViewport, ChartAxis axisX, ChartAxis axisY)
	{
		RenderLines(chartRenderer, chartViewport, axisX, axisY);
		RenderPoints(chartRenderer, chartViewport, axisX, axisY);
	}

	private void RenderLines(ChartRenderer chartRenderer, ChartViewport chartViewport, ChartAxis axisX, ChartAxis axisY)
	{
		var points = PointsCollection.ToArray();
		if (points.Length < 2)
			return;

		var linePen = LinePen;
		if (linePen is null)
			return;

		var displayBounds = chartViewport.InnerBounds;
		var offsetX = displayBounds.X;
		var offsetY = displayBounds.Y;
		var sizeX = displayBounds.Width;
		var sizeY = displayBounds.Height;

		var minimumX = chartViewport.MinimumX;
		var maximumX = chartViewport.MaximumX;
		var minimumY = chartViewport.MinimumY;
		var maximumY = chartViewport.MaximumY;

		var overlapX = false;
		var overlapY = false;
		var startX = double.NaN;
		var endX = double.NaN;
		var startY = double.NaN;
		var endY = double.NaN;
		var stepX = (maximumX - minimumX) / sizeX;
		var stepY = (maximumY - minimumY) / sizeY;
		var prevPoint = points.First();
		var startPoint = prevPoint;
		var lastX = double.NaN; 
		var lastY = double.NaN;
		var lastValid = false;

		using var clip = chartRenderer.ClipBounds(displayBounds);
		foreach (var currPoint in points.Skip(1))
		{
			// No start point.
			if (!startPoint.IsValid)
			{
				// Setup start point and reset overlap.
				startPoint = currPoint;
				lastValid = false;
				overlapX = false;
				overlapY = false;
			}
			// No current point.
			else if (!currPoint.IsValid)
			{
				// Is overlap.
				if (overlapX || overlapY)
				{
					double x1, x2, y1, y2;
					if (overlapX)
					{
						var x = startPoint.X;
						x1 = x2 = lastValid ? lastX : axisX.SolveCoordinate(x, minimumX, maximumX, offsetX, sizeX);
						y1 = axisY.SolveCoordinate(startY, minimumY, maximumY, offsetY, sizeY);
						y2 = axisY.SolveCoordinate(endY, minimumY, maximumY, offsetY, sizeY);
					}
					else
					{
						x1 = axisX.SolveCoordinate(startX, minimumX, maximumX, offsetX, sizeX);
						x2 = axisX.SolveCoordinate(endX, minimumX, maximumX, offsetX, sizeX);
						y1 = y2 = lastValid ? lastY : axisY.SolveCoordinate(startPoint.Y, minimumY, maximumY, offsetY, sizeY);
					}
					if (!double.IsNaN(x1) && !double.IsNaN(x2) && !double.IsNaN(y1) && !double.IsNaN(y2))
						chartRenderer.RenderLine(x1, y1, x2, y2, linePen);
				}

				// Reset overlap.
				startPoint = currPoint;
				lastValid = false;
				overlapX = false;
				overlapY = false;
			}
			// Points beyond the range.
			else if ((startPoint.X < minimumX && currPoint.X < minimumX) ||
			         (startPoint.X > maximumX && currPoint.X > maximumX) ||
			         (startPoint.Y < minimumY && currPoint.Y < minimumY) ||
			         (startPoint.Y > maximumY && currPoint.Y > maximumY))
			{
				// Reset overlap.
				startPoint = currPoint;
				lastValid = false;
				overlapX = false;
				overlapY = false;
			}
			else
			{
				// Coordinate difference.
				var diffX = Math.Abs(startPoint.X - currPoint.X) >= stepX;
				var diffY = Math.Abs(startPoint.Y - currPoint.Y) >= stepY;
				if (diffX || diffY)
				{
					// Overlap X.
					if (!diffX)
					{
						if (!overlapX)
						{
							if (startPoint.Y < currPoint.Y)
							{
								startY = startPoint.Y;
								endY = currPoint.Y;
							}
							else
							{
								startY = currPoint.Y;
								endY = startPoint.Y;
							}

							overlapX = true;
						}
						else
						{
							if (currPoint.Y < startY)
								startY = currPoint.Y;
							else if (currPoint.Y > endY)
								endY = currPoint.Y;
						}
					}
					// Overlap Y.
					else if (!diffY)
					{
						if (!overlapY)
						{
							if (startPoint.X < currPoint.X)
							{
								startX = startPoint.X;
								endX = currPoint.X;
							}
							else
							{
								startX = currPoint.X;
								endX = startPoint.X;
							}

							overlapY = true;
						}
						else
						{
							if (currPoint.X < startX)
								startX = currPoint.X;
							else if (currPoint.X > endX)
								endX = currPoint.X;
						}
					}
					// Different coordinates.
					else
					{
						if (!lastValid)
						{
							lastX = axisX.SolveCoordinate(startPoint.X, minimumX, maximumX, offsetX, sizeX);
							lastY = axisY.SolveCoordinate(startPoint.Y, minimumY, maximumY, offsetY, sizeY);
						}

						if (overlapX)
						{
							var y1 = axisY.SolveCoordinate(startY, minimumY, maximumY, offsetY, sizeY);
							var y2 = axisY.SolveCoordinate(endY, minimumY, maximumY, offsetY, sizeY);
							if (!double.IsNaN(y1) && !double.IsNaN(y2) && !double.IsNaN(lastX))
								chartRenderer.RenderLine(lastX, y1, lastX, y2, linePen);
						}
						else if (overlapY)
						{
							var x1 = axisX.SolveCoordinate(startX, minimumX, maximumX, offsetX, sizeX);
							var x2 = axisX.SolveCoordinate(endX, minimumX, maximumX, offsetX, sizeX);
							if (!double.IsNaN(x1) && !double.IsNaN(x2) && !double.IsNaN(lastY))
								chartRenderer.RenderLine(x1, lastY, x2, lastY, linePen);
						}

						var currX = axisX.SolveCoordinate(currPoint.X, minimumX, maximumX, offsetX, sizeX);
						var currY = axisY.SolveCoordinate(currPoint.Y, minimumY, maximumY, offsetY, sizeY);

						if (prevPoint.IsValid)
						{
							if (!prevPoint.X.Equals(startPoint.X) || !prevPoint.Y.Equals(startPoint.Y))
							{
								lastX = axisX.SolveCoordinate(prevPoint.X, minimumX, maximumX, offsetX, sizeX);
								lastY = axisY.SolveCoordinate(prevPoint.Y, minimumY, maximumY, offsetY, sizeY);
							}

							if (!double.IsNaN(lastX) && !double.IsNaN(lastY) && !double.IsNaN(currX) && !double.IsNaN(currY))
								chartRenderer.RenderLine(lastX, lastY, currX, currY, linePen);
						}

						startPoint = currPoint;
						lastX = currX;
						lastY = currY;
						lastValid = true;
						overlapX = false;
						overlapY = false;
					}
				}
			}

			prevPoint = currPoint;
		}

		if (overlapX || overlapY)
		{
			double x1, x2, y1, y2;
			if (overlapX)
			{
				var x = startPoint.X;
				x1 = x2 = lastValid ? lastX : axisX.SolveCoordinate(x, minimumX, maximumX, offsetX, sizeX);
				y1 = axisY.SolveCoordinate(startY, minimumY, maximumY, offsetY, sizeY);
				y2 = axisY.SolveCoordinate(endY, minimumY, maximumY, offsetY, sizeY);
			}
			else
			{
				x1 = axisX.SolveCoordinate(startX, minimumX, maximumX, offsetX, sizeX);
				x2 = axisX.SolveCoordinate(endX, minimumX, maximumX, offsetX, sizeX);
				y1 = y2 = lastValid ? lastY : axisY.SolveCoordinate(startPoint.Y, minimumY, maximumY, offsetY, sizeY);
			}
			if (!double.IsNaN(x1) && !double.IsNaN(x2) && !double.IsNaN(y1) && !double.IsNaN(y2))
				chartRenderer.RenderLine(x1, y1, x2, y2, linePen);
		}
	}
	#endregion
}