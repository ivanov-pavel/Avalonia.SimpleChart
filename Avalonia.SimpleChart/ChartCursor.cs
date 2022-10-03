//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;

namespace Avalonia.SimpleChart;

public class ChartCursor : ChartItem
{
	#region Fields
	private IReadOnlyList<Point> _cursorPositions;
	private IReadOnlyList<ChartPoint> _cursorPoints;

	public static readonly StyledProperty<CursorType> CursorTypeProperty =
		AvaloniaProperty.Register<ChartCursor, CursorType>(nameof(CursorType));

	public static readonly StyledProperty<double> PointSizeProperty =
		AvaloniaProperty.Register<ChartCursor, double>(nameof(PointSize), 4);

	public static readonly StyledProperty<double> TextOffsetProperty =
		AvaloniaProperty.Register<ChartCursor, double>(nameof(TextOffset), 8);

	public static readonly StyledProperty<double> TextMarginProperty =
		AvaloniaProperty.Register<ChartCursor, double>(nameof(TextMargin), 4);

	public static readonly StyledProperty<string?> TextFormatProperty =
		AvaloniaProperty.Register<ChartCursor, string?>(nameof(TextFormat));

	public static readonly StyledProperty<IBrush?> TextForegroundProperty =
		AvaloniaProperty.Register<ChartCursor, IBrush?>(nameof(TextForeground), Brushes.Black);

	public static readonly StyledProperty<IBrush?> BackgroundBrushProperty =
		AvaloniaProperty.Register<ChartCursor, IBrush?>(nameof(BackgroundBrush));

	public static readonly StyledProperty<IPen?> BorderPenProperty =
		AvaloniaProperty.Register<ChartCursor, IPen?>(nameof(BorderPen));

	public static readonly StyledProperty<double> BorderRadiusProperty =
		AvaloniaProperty.Register<ChartCursor, double>(nameof(BorderRadius));

	public static readonly StyledProperty<IPen?> LinePenProperty =
		AvaloniaProperty.Register<ChartCursor, IPen?>(nameof(LinePen));

	public static readonly StyledProperty<double> FontSizeProperty =
		AvaloniaProperty.Register<ChartCursor, double>(nameof(FontSize), 10);

	public static readonly StyledProperty<FontFamily> FontFamilyProperty =
		AvaloniaProperty.Register<ChartCursor, FontFamily>(nameof(FontFamily), FontFamily.Default);

	public static readonly StyledProperty<FontStyle> FontStyleProperty =
		AvaloniaProperty.Register<ChartCursor, FontStyle>(nameof(FontStyle));

	public static readonly StyledProperty<FontWeight> FontWeightProperty =
		AvaloniaProperty.Register<ChartCursor, FontWeight>(nameof(FontWeight), FontWeight.Normal);
	#endregion

	#region Properties
	public CursorType CursorType
	{
		get => GetValue(CursorTypeProperty);
		set => SetValue(CursorTypeProperty, value);
	}

	public double PointSize
	{
		get => GetValue(PointSizeProperty);
		set => SetValue(PointSizeProperty, value);
	}

	public double TextOffset
	{
		get => GetValue(TextOffsetProperty);
		set => SetValue(TextOffsetProperty, value);
	}

	public double TextMargin
	{
		get => GetValue(TextMarginProperty);
		set => SetValue(TextMarginProperty, value);
	}

	public string? TextFormat
	{
		get => GetValue(TextFormatProperty);
		set => SetValue(TextFormatProperty, value);
	}

	public IBrush? TextForeground
	{
		get => GetValue(TextForegroundProperty);
		set => SetValue(TextForegroundProperty, value);
	}

	public IPen? BorderPen
	{
		get => GetValue(BorderPenProperty);
		set => SetValue(BorderPenProperty, value);
	}

	public double BorderRadius
	{
		get => GetValue(BorderRadiusProperty);
		set => SetValue(BorderRadiusProperty, value);
	}

	public IPen? LinePen
	{
		get => GetValue(LinePenProperty);
		set => SetValue(LinePenProperty, value);
	}

	public IBrush? BackgroundBrush
	{
		get => GetValue(BackgroundBrushProperty);
		set => SetValue(BackgroundBrushProperty, value);
	}

	public double FontSize
	{
		get => GetValue(FontSizeProperty);
		set => SetValue(FontSizeProperty, value);
	}

	public FontFamily FontFamily
	{
		get => GetValue(FontFamilyProperty);
		set => SetValue(FontFamilyProperty, value);
	}

	public FontStyle FontStyle
	{
		get => GetValue(FontStyleProperty);
		set => SetValue(FontStyleProperty, value);
	}

	public FontWeight FontWeight
	{
		get => GetValue(FontWeightProperty);
		set => SetValue(FontWeightProperty, value);
	}
	#endregion

	#region Methods
	public ChartCursor()
	{
		_cursorPoints = Array.Empty<ChartPoint>();
		_cursorPositions = Array.Empty<Point>();
	}

	public void ResetCursor()
	{
		_cursorPoints = Array.Empty<ChartPoint>();
		_cursorPositions = Array.Empty<Point>();
	}

	public void UpdateCursor(Point cursorPosition, ChartViewport chartViewport, ChartAxis axisX, ChartAxis axisY, IEnumerable<ChartSeries> seriesCollection)
	{
		var cursorType = CursorType;
		var cursorX = cursorPosition.X;
		var cursorY = cursorPosition.Y;
		var minimumX = chartViewport.MinimumX;
		var maximumX = chartViewport.MaximumX;
		var minimumY = chartViewport.MinimumY;
		var maximumY = chartViewport.MaximumY;
		var innerBounds = chartViewport.InnerBounds;
		var offsetX = innerBounds.X;
		var offsetY = innerBounds.Y;
		var sizeX = innerBounds.Width;
		var sizeY = innerBounds.Height;

		var positions = new List<Point>();
		var points = new List<ChartPoint>();
		if (cursorType == CursorType.Free)
		{
			positions.Add(cursorPosition);
			var x = axisX.SolveValue(cursorX, minimumX, maximumX, offsetX, sizeX);
			var y = axisY.SolveValue(cursorY, minimumY, maximumY, offsetY, sizeY);
			if (!double.IsNaN(x) && !double.IsNaN(y))
				points.Add(new ChartPoint(x, y));
		}
		else
		{
			foreach (var series in seriesCollection)
			{
				if (!series.IsVisible || !series.CursorEnable)
					continue;

				foreach (var point in series.PointsCollection)
				{
					var pointX = point.X;
					var pointY = point.Y;
					if (pointX < minimumX || pointX > maximumX || pointY < minimumY || pointY > maximumY)
						continue;

					var x = axisX.SolveCoordinate(pointX, minimumX, maximumX, offsetX, sizeX);
					var y = axisY.SolveCoordinate(pointY, minimumY, maximumY, offsetY, sizeY);
					if (double.IsNaN(x) || double.IsNaN(y))
						continue;

					var size = point.Style?.Size ?? ChartPoint.PointNeighborhoodSize;
					size = Math.Max(size / 2, 2);

					if ((cursorType is CursorType.ByX or CursorType.ByXY && Math.Abs(x - cursorX) > size) ||
					    (cursorType is CursorType.ByY or CursorType.ByXY && Math.Abs(y - cursorY) > size))
						continue;

					positions.Add(new Point(x, y));
					points.Add(new ChartPoint(pointX, pointY));
					break;
				}
			}
		}

		_cursorPoints = points;
		_cursorPositions = positions;
	}

	public void RenderCursor(ChartRenderer chartRenderer, ChartViewport chartViewport, ChartAxis axisX, ChartAxis axisY)
	{
		var textFormat = TextFormat;
		var textForeground = TextForeground;
		var backgroundBrush = BackgroundBrush;
		var borderPen = BorderPen;
		var borderRadius = BorderRadius;
		var linePen = LinePen;
		var textMargin = TextMargin;
		var textOffset = TextOffset;
		var pointSize = PointSize;
		var fontSize = FontSize;
		var fontFamily = FontFamily;
		var fontStyle = FontStyle;
		var fontWeight = FontWeight;
		var textTypeface = new Typeface(fontFamily, fontStyle, fontWeight);

		var innerBounds = chartViewport.InnerBounds;
		var displayLeft = innerBounds.Left;
		var displayRight = innerBounds.Right;
		var displayTop = innerBounds.Top;
		var displayBottom = innerBounds.Bottom;

		var cursorRects = new List<Rect>();
		var cursorPoints = _cursorPoints;
		var cursorPositions = _cursorPositions;
		using var clip = chartRenderer.ClipBounds(innerBounds);
		for (var i = 0; i < cursorPositions.Count; ++i)
		{
			if (i >= cursorPoints.Count)
				continue;

			var pointX = cursorPoints[i].X;
			var pointY = cursorPoints[i].Y;
			var positionX = cursorPositions[i].X;
			var positionY = cursorPositions[i].Y;
			chartRenderer.RenderLine(positionX, displayTop, positionX, displayBottom, linePen);
			chartRenderer.RenderLine(displayLeft, positionY, displayRight, positionY, linePen);
			chartRenderer.RenderCircle(positionX, positionY, pointSize, linePen, null);
			var cursorLabel = string.IsNullOrEmpty(textFormat)
				? $"X: {axisX.FormatValue(pointX)}\r\nY: {axisY.FormatValue(pointY)}"
				: string.Format(textFormat, pointX, pointY);

			var labelText = new FormattedText(cursorLabel, textTypeface, fontSize, TextAlignment.Left, TextWrapping.NoWrap, Size.Empty);
			var labelWidth = labelText.Bounds.Width;
			var labelHeight = labelText.Bounds.Height;

			// 1 2 3
			// 4   5
			// 6 7 8
			var textRects = new Rect[]
			{
				// 2.
				new(positionX - 0.5 * labelWidth - textMargin, positionY - labelHeight - textOffset - 2 * textMargin, labelWidth + 2 * textMargin, labelHeight + 2 * textMargin),
				// 7.
				new(positionX - 0.5 * labelWidth - textMargin, positionY + textOffset, labelWidth + 2 * textMargin, labelHeight + 2 * textMargin),
				// 4.
				new(positionX - labelWidth - textOffset - 2 * textMargin, positionY - 0.5 * labelHeight - textMargin, labelWidth + 2 * textMargin, labelHeight + 2 * textMargin),
				// 5.
				new(positionX + textOffset, positionY - 0.5 * labelHeight - textMargin, labelWidth + 2 * textMargin, labelHeight + 2 * textMargin),
				// 1.
				new(positionX - labelWidth - textOffset - 2 * textMargin, positionY - labelHeight - textOffset - 2 * textMargin, labelWidth + 2 * textMargin, labelHeight + 2 * textMargin),
				// 3.
				new(positionX + textOffset, positionY - labelHeight - textOffset - 2 * textMargin, labelWidth + 2 * textMargin, labelHeight + 2 * textMargin),
				// 6.
				new(positionX - labelWidth - textOffset - 2 * textMargin, positionY + 8, labelWidth + 2 * textMargin, labelHeight + 2 * textMargin),
				// 8.
				new(positionX + 8, positionY + 8, labelWidth + 2 * textMargin, labelHeight + 2 * textMargin)
			};

			var textRect = Rect.Empty;
			foreach (var rect1 in textRects)
			{
				if (!innerBounds.Contains(rect1) || cursorRects.Any(rect2 => rect2.Intersects(rect1)))
					continue;

				textRect = rect1;
				break;
			}

			if (textRect.IsEmpty)
				continue;

			var textLocation = new Point(textRect.Left + textRect.Width / 2 - labelWidth / 2, textRect.Top + textRect.Height / 2 - labelHeight / 2);
			cursorRects.Add(textRect);
			chartRenderer.RenderRectangle(textRect, borderRadius, borderPen, backgroundBrush);
			chartRenderer.RenderText(labelText, textLocation, 0, textForeground);
		}
	}
	#endregion
}