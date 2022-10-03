//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;

namespace Avalonia.SimpleChart;

public abstract class ChartSeries : ChartItem
{
	#region Fields
	private bool _isVisible;
	private bool _isChangeable;
	private bool _isChanging;
	private bool _cursorEnable;
	private ChartPoint? _changeablePoint;
	private IEnumerable<ChartPoint> _pointsCollection;

	public static readonly DirectProperty<ChartSeries, bool> CursorEnableProperty =
		AvaloniaProperty.RegisterDirect<ChartSeries, bool>(nameof(CursorEnable), o => o.CursorEnable, (o, v) => o.CursorEnable = v);

	public static readonly DirectProperty<ChartSeries, bool> IsVisibleProperty =
		AvaloniaProperty.RegisterDirect<ChartSeries, bool>(nameof(IsVisible), o => o.IsVisible, (o, v) => o.IsVisible = v);

	public static readonly DirectProperty<ChartSeries, bool> IsChangeableProperty =
		AvaloniaProperty.RegisterDirect<ChartSeries, bool>(nameof(IsChangeable), o => o.IsChangeable, (o, v) => o.IsChangeable = v);

	public static readonly DirectProperty<ChartSeries, IEnumerable<ChartPoint>> PointsCollectionProperty =
		AvaloniaProperty.RegisterDirect<ChartSeries, IEnumerable<ChartPoint>>(nameof(PointsCollection), o => o.PointsCollection, (o, v) => o.PointsCollection = v);
	#endregion

	#region Properties
	public abstract SeriesType SeriesType { get; }

	public bool IsChanging => _isChanging;

	public ChartPoint? ChangeablePoint => _changeablePoint;

	public bool IsVisible
	{
		get => _isVisible;
		set => SetAndRaise(IsVisibleProperty, ref _isVisible, value);
	}

	public bool IsChangeable
	{
		get => _isChangeable;
		set => SetAndRaise(IsChangeableProperty, ref _isChangeable, value);
	}

	public bool CursorEnable
	{
		get => _cursorEnable;
		set => SetAndRaise(CursorEnableProperty, ref _cursorEnable, value);
	}

	public IEnumerable<ChartPoint> PointsCollection
	{
		get => _pointsCollection;
		set => SetAndRaise(PointsCollectionProperty, ref _pointsCollection, value);
	}
	#endregion

	#region Constructors
	protected ChartSeries()
	{
		_isVisible = true;
		_isChangeable = false;
		_pointsCollection = Enumerable.Empty<ChartPoint>();
	}
	#endregion

	#region Methods
	public void StartChanging(ChartPoint point)
	{
		_changeablePoint = point;
		_isChanging = true;
	}

	public void StopChanging()
	{
		_changeablePoint = null;
		_isChanging = false;
	}

	public abstract void RenderSeries(ChartRenderer chartRenderer, ChartViewport chartViewport, ChartAxis axisX, ChartAxis axisY);

	protected void RenderPoints(ChartRenderer chartRenderer, ChartViewport chartViewport, ChartAxis axisX, ChartAxis axisY)
	{
		var minimumX = chartViewport.MinimumX;
		var maximumX = chartViewport.MaximumX;
		var minimumY = chartViewport.MinimumY;
		var maximumY = chartViewport.MaximumY;
		var points = _pointsCollection
			.Where(p => p.X >= minimumX && p.X <= maximumX && p.Y >= minimumY && p.Y <= maximumY)
			.ToArray();

		var displayBounds = chartViewport.InnerBounds;
		var offsetX = displayBounds.X;
		var offsetY = displayBounds.Y;
		var sizeX = displayBounds.Width;
		var sizeY = displayBounds.Height;

		using var clip = chartRenderer.ClipBounds(displayBounds);
		foreach (var point in points)
		{
			var pointX = point.X;
			var pointY = point.Y;
			pointX = axisX.SolveCoordinate(pointX, minimumX, maximumX, offsetX, sizeX);
			pointY = axisY.SolveCoordinate(pointY, minimumY, maximumY, offsetY, sizeY);
			if (double.IsNaN(pointX) || double.IsNaN(pointY))
				continue;

			var pointStyle = point.Style;
			if (pointStyle is not null && pointStyle.Size > 0)
			{
				var pointSize = pointStyle.Size;
				var pointFill = pointStyle.Fill;
				var pointBorder = pointStyle.Border;
				switch (pointStyle.Marker)
				{
					case MarkerType.None:
						break;

					case MarkerType.Circle:
						chartRenderer.RenderCircle(pointX, pointY, pointSize, pointBorder, pointFill);
						break;

					case MarkerType.Square:
						chartRenderer.RenderSquare(pointX, pointY, pointSize, pointBorder, pointFill);
						break;
				}
			}

			var pointLabel = point.Label;
			if (pointLabel is not null && !string.IsNullOrEmpty(pointLabel.Text))
			{
				var labelText = pointLabel.Text;
				var labelStyle = pointLabel.Style;
				var labelOffset = pointLabel.Offset;
				var labelLocation = pointLabel.Location;
				var fontSize = labelStyle.FontSize;
				var fontFamily = labelStyle.FontFamily;
				var fontStyle = labelStyle.FontStyle;
				var fontWeight = labelStyle.FontWeight;
				var labelForeground = labelStyle.Foreground;
				var labelTypeface = new Typeface(fontFamily, fontStyle, fontWeight);
				var formattedText = new FormattedText(labelText, labelTypeface, fontSize, TextAlignment.Left, TextWrapping.NoWrap, Size.Empty);
				var labelHeight = formattedText.Bounds.Height;
				var labelWidth = formattedText.Bounds.Width;
				var labelPosition = labelLocation switch
				{
					LabelLocation.LeftCenter => new Point(pointX - labelWidth - labelOffset, pointY - labelHeight / 2),
					LabelLocation.RightCenter => new Point(pointX + labelOffset, pointY - labelHeight / 2),
					LabelLocation.TopCenter => new Point(pointX - labelWidth / 2, pointY - labelHeight - labelOffset),
					LabelLocation.BottomCenter => new Point(pointX - labelWidth / 2, pointY + labelOffset),
					LabelLocation.LeftTop => new Point(pointX - labelWidth - labelOffset, pointY - labelHeight - labelOffset),
					LabelLocation.LeftBottom => new Point(pointX - labelWidth - labelOffset, pointY + labelOffset),
					LabelLocation.RightTop => new Point(pointX + labelOffset, pointY - labelHeight - labelOffset),
					LabelLocation.RightBottom => new Point(pointX + labelOffset, pointY + labelOffset),
					_ => new Point(pointX, pointY)
				};
				chartRenderer.RenderText(formattedText, labelPosition, 0, labelForeground);
			}
		}
	}
	#endregion
}