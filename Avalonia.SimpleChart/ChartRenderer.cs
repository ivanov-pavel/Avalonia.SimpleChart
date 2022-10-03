//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using System;
using Avalonia.Media;

namespace Avalonia.SimpleChart;

public class ChartRenderer
{
	#region Fields
	private readonly DrawingContext _context;
	#endregion

	#region Constructors
	public ChartRenderer(DrawingContext context)
	{
		_context = context;
	}
	#endregion

	#region Methods
	public IDisposable ClipBounds(Rect bounds)
	{
		return _context.PushClip(bounds);
	}

	public void RenderLine(double x1, double y1, double x2, double y2, IPen? pen)
	{
		_context.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
	}

	public void RenderText(FormattedText text, Point location, double angle, IBrush? foreground)
	{
		var origin = new Point();
		var rotation = Matrix.CreateRotation(-angle * Math.PI / 180);
		var translation = Matrix.CreateTranslation(location.X, location.Y);
		using (_context.PushPreTransform(translation))
		using (_context.PushPreTransform(rotation))
			_context.DrawText(foreground, origin, text);
	}

	public void RenderCircle(double x, double y, double size, IPen? border, IBrush? fill)
	{
		var center = new Point(x, y);
		var radius = size / 2;
		_context.DrawEllipse(fill, border, center, radius, radius);
	}

	public void RenderSquare(double x, double y, double size, IPen? border, IBrush? fill)
	{
		var rect = new Rect(new Point(x - size / 2, y - size / 2), new Size(size, size));
		_context.DrawRectangle(fill, border, rect);
	}

	public void RenderRectangle(Rect rect, double radius, IPen? border, IBrush? fill)
	{
		_context.DrawRectangle(fill, border, rect, radius, radius);
	}
	#endregion
}