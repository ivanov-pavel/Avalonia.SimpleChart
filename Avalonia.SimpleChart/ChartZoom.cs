//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using System;
using Avalonia.Media;

namespace Avalonia.SimpleChart;

public class ChartZoom : ChartItem
{
	#region Fields
	private bool _isZoomed;
	private bool _isZooming;
	private double _minimumX;
	private double _maximumX;
	private double _stepX;
	private double _minimumY;
	private double _maximumY;
	private double _stepY;
	private Point _startPosition;
	private Point _endPosition;
	
	public static readonly StyledProperty<IBrush?> BackgroundBrushProperty =
		AvaloniaProperty.Register<ChartZoom, IBrush?>(nameof(BackgroundBrush));

	public static readonly StyledProperty<IPen?> BorderPenProperty =
		AvaloniaProperty.Register<ChartZoom, IPen?>(nameof(BorderPen));
	#endregion

	#region Properties
	public bool IsZoomed => _isZoomed;
	public bool IsZooming => _isZooming;
	public double MinimumX => _minimumX;
	public double MaximumX => _maximumX;
	public double StepX => _stepX;
	public double MinimumY => _minimumY;
	public double MaximumY => _maximumY;
	public double StepY => _stepY;
	public Point StartPosition => _startPosition;
	public Point EndPosition => _endPosition;
	
	public IPen? BorderPen
	{
		get => GetValue(BorderPenProperty);
		set => SetValue(BorderPenProperty, value);
	}

	public IBrush? BackgroundBrush
	{
		get => GetValue(BackgroundBrushProperty);
		set => SetValue(BackgroundBrushProperty, value);
	}
	#endregion

	#region Constructors
	public ChartZoom()
	{
		_minimumX = double.NaN;
		_maximumX = double.NaN;
		_stepX = double.NaN;
		_minimumY = double.NaN;
		_maximumY = double.NaN;
		_stepY = double.NaN;
	}
	#endregion

	#region Methods
	public void SetZoom(double minimumX, double maximumX, double stepX, double minimumY, double maximumY, double stepY)
	{
		_minimumX = minimumX;
		_maximumX = maximumX;
		_stepX = stepX;
		_minimumY = minimumY;
		_maximumY = maximumY;
		_stepY = stepY;
		_isZoomed = true;
	}

	public void ResetZoom()
	{
		_minimumX = double.NaN;
		_maximumX = double.NaN;
		_stepX = double.NaN;
		_minimumY = double.NaN;
		_maximumY = double.NaN;
		_stepY = double.NaN;
		_isZoomed = false;
	}

	public void MoveZoom(double dx, double dy)
	{
		_minimumX += dx;
		_maximumX += dx;
		_minimumY += dy;
		_maximumY += dy;
	}

	public void StartZooming(Point position)
	{
		_startPosition = position;
		_isZooming = true;
	}

	public void StopZooming()
	{
		_isZooming = false;
	}

	public void UpdateZooming(Point position)
	{
		_endPosition = position;
	}

	public void RenderZoom(ChartRenderer chartRenderer)
	{
		if (!_isZooming)
			return;

		var x1 = Math.Min(_startPosition.X, _endPosition.X);
		var x2 = Math.Max(_startPosition.X, _endPosition.X);
		var y1 = Math.Min(_startPosition.Y, _endPosition.Y);
		var y2 = Math.Max(_startPosition.Y, _endPosition.Y);
		var rect = new Rect(x1, y1, x2 - x1, y2 - y1);
		chartRenderer.RenderRectangle(rect, 0, BorderPen, BackgroundBrush);
	}
	#endregion
}