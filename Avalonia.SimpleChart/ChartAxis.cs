//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using System;
using Avalonia.Media;

namespace Avalonia.SimpleChart;

public abstract class ChartAxis : AvaloniaObject
{
	#region Constants
	private const double MinimumValueStep = 0.0001;
	private const double MinimumDateTimeStep = 1;
	#endregion

	#region Fields
	public static readonly StyledProperty<bool> IsInvertedProperty =
		AvaloniaProperty.Register<ChartAxis, bool>(nameof(IsInverted));

	public static readonly StyledProperty<bool> IsPrimaryProperty =
		AvaloniaProperty.Register<ChartAxis, bool>(nameof(IsPrimary), true);

	public static readonly StyledProperty<bool> IsSecondaryProperty =
		AvaloniaProperty.Register<ChartAxis, bool>(nameof(IsSecondary));

	public static readonly StyledProperty<AxisType> AxisTypeProperty =
		AvaloniaProperty.Register<ChartAxis, AxisType>(nameof(AxisType));

	public static readonly StyledProperty<string?> LabelTextProperty =
		AvaloniaProperty.Register<ChartAxis, string?>(nameof(LabelText));

	public static readonly StyledProperty<int> LabelOffsetProperty =
		AvaloniaProperty.Register<ChartAxis, int>(nameof(LabelOffset), 20);

	public static readonly StyledProperty<IBrush?> LabelForegroundProperty =
		AvaloniaProperty.Register<ChartAxis, IBrush?>(nameof(LabelForeground), Brushes.Black);

	public static readonly StyledProperty<double> LabelFontSizeProperty =
		AvaloniaProperty.Register<ChartAxis, double>(nameof(LabelFontSize), 10);

	public static readonly StyledProperty<FontFamily> LabelFontFamilyProperty =
		AvaloniaProperty.Register<ChartAxis, FontFamily>(nameof(LabelFontFamily), FontFamily.Default);

	public static readonly StyledProperty<FontStyle> LabelFontStyleProperty =
		AvaloniaProperty.Register<ChartAxis, FontStyle>(nameof(LabelFontStyle));

	public static readonly StyledProperty<FontWeight> LabelFontWeightProperty =
		AvaloniaProperty.Register<ChartAxis, FontWeight>(nameof(LabelFontWeight), FontWeight.Normal);

	public static readonly StyledProperty<int> ValueOffsetProperty =
		AvaloniaProperty.Register<ChartAxis, int>(nameof(ValueOffset), 10);

	public static readonly StyledProperty<IBrush?> ValueForegroundProperty =
		AvaloniaProperty.Register<ChartAxis, IBrush?>(nameof(ValueForeground), Brushes.Black);

	public static readonly StyledProperty<double> ValueFontSizeProperty =
		AvaloniaProperty.Register<ChartAxis, double>(nameof(ValueFontSize), 10);

	public static readonly StyledProperty<FontFamily> ValueFontFamilyProperty =
		AvaloniaProperty.Register<ChartAxis, FontFamily>(nameof(ValueFontFamily), FontFamily.Default);

	public static readonly StyledProperty<FontStyle> ValueFontStyleProperty =
		AvaloniaProperty.Register<ChartAxis, FontStyle>(nameof(ValueFontStyle));

	public static readonly StyledProperty<FontWeight> ValueFontWeightProperty =
		AvaloniaProperty.Register<ChartAxis, FontWeight>(nameof(ValueFontWeight), FontWeight.Normal);

	public static readonly StyledProperty<IPen?> LinePenProperty =
		AvaloniaProperty.Register<ChartAxis, IPen?>(nameof(LinePen), new Pen(Brushes.Black));

	public static readonly StyledProperty<TickType> MajorTickProperty =
		AvaloniaProperty.Register<ChartAxis, TickType>(nameof(MajorTick), TickType.TwoSide);

	public static readonly StyledProperty<TickType> MinorTickProperty =
		AvaloniaProperty.Register<ChartAxis, TickType>(nameof(MinorTick), TickType.Inner);

	public static readonly StyledProperty<int> MajorTickSizeProperty =
		AvaloniaProperty.Register<ChartAxis, int>(nameof(MajorTickSize), 8);

	public static readonly StyledProperty<int> MinorTickSizeProperty =
		AvaloniaProperty.Register<ChartAxis, int>(nameof(MinorTickSize), 4);

	public static readonly StyledProperty<IPen?> MajorTickPenProperty =
		AvaloniaProperty.Register<ChartAxis, IPen?>(nameof(MajorTickPen));

	public static readonly StyledProperty<IPen?> MinorTickPenProperty =
		AvaloniaProperty.Register<ChartAxis, IPen?>(nameof(MinorTickPen));

	public static readonly StyledProperty<IPen?> MajorGridPenProperty =
		AvaloniaProperty.Register<ChartAxis, IPen?>(nameof(MajorGridPen));

	public static readonly StyledProperty<IPen?> MinorGridPenProperty =
		AvaloniaProperty.Register<ChartAxis, IPen?>(nameof(MinorGridPen));

	public static readonly StyledProperty<double> MinimumValueProperty =
		AvaloniaProperty.Register<ChartAxis, double>(nameof(MinimumValue));

	public static readonly StyledProperty<double> MaximumValueProperty =
		AvaloniaProperty.Register<ChartAxis, double>(nameof(MaximumValue), 100);

	public static readonly StyledProperty<double> ValueStepProperty =
		AvaloniaProperty.Register<ChartAxis, double>(nameof(ValueStep), 10);

	public static readonly StyledProperty<int> StepDivisionProperty =
		AvaloniaProperty.Register<ChartAxis, int>(nameof(StepDivision), 4);

	public static readonly StyledProperty<string?> ValueFormatProperty =
		AvaloniaProperty.Register<ChartAxis, string?>(nameof(ValueFormat));
	#endregion

	#region Properties
	public bool IsInverted
	{
		get => GetValue(IsInvertedProperty);
		set => SetValue(IsInvertedProperty, value);
	}

	public bool IsPrimary
	{
		get => GetValue(IsPrimaryProperty);
		set => SetValue(IsPrimaryProperty, value);
	}

	public bool IsSecondary
	{
		get => GetValue(IsSecondaryProperty);
		set => SetValue(IsSecondaryProperty, value);
	}

	public AxisType AxisType
	{
		get => GetValue(AxisTypeProperty);
		set => SetValue(AxisTypeProperty, value);
	}

	public string? LabelText
	{
		get => GetValue(LabelTextProperty);
		set => SetValue(LabelTextProperty, value);
	}

	public int LabelOffset
	{
		get => GetValue(LabelOffsetProperty);
		set => SetValue(LabelOffsetProperty, value);
	}

	public IBrush? LabelForeground
	{
		get => GetValue(LabelForegroundProperty);
		set => SetValue(LabelForegroundProperty, value);
	}

	public double LabelFontSize
	{
		get => GetValue(LabelFontSizeProperty);
		set => SetValue(LabelFontSizeProperty, value);
	}

	public FontFamily LabelFontFamily
	{
		get => GetValue(LabelFontFamilyProperty);
		set => SetValue(LabelFontFamilyProperty, value);
	}

	public FontStyle LabelFontStyle
	{
		get => GetValue(LabelFontStyleProperty);
		set => SetValue(LabelFontStyleProperty, value);
	}

	public FontWeight LabelFontWeight
	{
		get => GetValue(LabelFontWeightProperty);
		set => SetValue(LabelFontWeightProperty, value);
	}

	public int ValueOffset
	{
		get => GetValue(ValueOffsetProperty);
		set => SetValue(ValueOffsetProperty, value);
	}

	public IBrush? ValueForeground
	{
		get => GetValue(ValueForegroundProperty);
		set => SetValue(ValueForegroundProperty, value);
	}

	public double ValueFontSize
	{
		get => GetValue(ValueFontSizeProperty);
		set => SetValue(ValueFontSizeProperty, value);
	}

	public FontFamily ValueFontFamily
	{
		get => GetValue(ValueFontFamilyProperty);
		set => SetValue(ValueFontFamilyProperty, value);
	}

	public FontStyle ValueFontStyle
	{
		get => GetValue(ValueFontStyleProperty);
		set => SetValue(ValueFontStyleProperty, value);
	}

	public FontWeight ValueFontWeight
	{
		get => GetValue(ValueFontWeightProperty);
		set => SetValue(ValueFontWeightProperty, value);
	}

	public IPen? LinePen
	{
		get => GetValue(LinePenProperty);
		set => SetValue(LinePenProperty, value);
	}

	public TickType MajorTick
	{
		get => GetValue(MajorTickProperty);
		set => SetValue(MajorTickProperty, value);
	}

	public TickType MinorTick
	{
		get => GetValue(MinorTickProperty);
		set => SetValue(MinorTickProperty, value);
	}

	public int MajorTickSize
	{
		get => GetValue(MajorTickSizeProperty);
		set => SetValue(MajorTickSizeProperty, value);
	}

	public int MinorTickSize
	{
		get => GetValue(MinorTickSizeProperty);
		set => SetValue(MinorTickSizeProperty, value);
	}

	public IPen? MajorTickPen
	{
		get => GetValue(MajorTickPenProperty);
		set => SetValue(MajorTickPenProperty, value);
	}

	public IPen? MinorTickPen
	{
		get => GetValue(MinorTickPenProperty);
		set => SetValue(MinorTickPenProperty, value);
	}

	public IPen? MajorGridPen
	{
		get => GetValue(MajorGridPenProperty);
		set => SetValue(MajorGridPenProperty, value);
	}

	public IPen? MinorGridPen
	{
		get => GetValue(MinorGridPenProperty);
		set => SetValue(MinorGridPenProperty, value);
	}

	public double MinimumValue
	{
		get => GetValue(MinimumValueProperty);
		set => SetValue(MinimumValueProperty, value);
	}

	public double MaximumValue
	{
		get => GetValue(MaximumValueProperty);
		set => SetValue(MaximumValueProperty, value);
	}

	public double ValueStep
	{
		get => GetValue(ValueStepProperty);
		set => SetValue(ValueStepProperty, value);
	}

	public int StepDivision
	{
		get => GetValue(StepDivisionProperty);
		set => SetValue(StepDivisionProperty, value);
	}

	public string? ValueFormat
	{
		get => GetValue(ValueFormatProperty);
		set => SetValue(ValueFormatProperty, value);
	}
	#endregion

	#region Methods
	public double SolveStep(double minimum, double maximum, int count)
	{
		if (double.IsNaN(minimum) || double.IsNaN(maximum) || minimum >= maximum)
			return double.NaN;

		double range;
		var type = AxisType;
		if (type == AxisType.Value)
			range = maximum - minimum;
		else
		{
			var minimumDateTime = ToDateTime(minimum);
			var maximumDateTime = ToDateTime(maximum);
			range = type switch
			{
				AxisType.Year => maximumDateTime.Year - minimumDateTime.Year,
				AxisType.Month => (maximumDateTime.Year - minimumDateTime.Year) * 12 + (maximumDateTime.Month - minimumDateTime.Month),
				AxisType.Day => (maximumDateTime.Year - minimumDateTime.Year) * 365 + (maximumDateTime.DayOfYear - minimumDateTime.DayOfYear),
				AxisType.Hour => (maximumDateTime - minimumDateTime).Hours,
				AxisType.Minute => (maximumDateTime - minimumDateTime).Minutes,
				AxisType.Second => (maximumDateTime - minimumDateTime).Seconds,
				_ => 0
			};
		}

		if (double.IsNaN(range) || range <= 0 || count <= 0)
			return double.NaN;

		var step = range / count;
		var mag = Math.Floor(Math.Log10(step));
		var magPow = Math.Pow(10.0, mag);
		var magMsd = Math.Ceiling(step / magPow);
		if (magMsd > 5.0)
			magMsd = 10.0;
		else if (magMsd > 2.0)
			magMsd = 5.0;
		else if (magMsd > 1.0)
			magMsd = 2.0;
		step = magMsd * magPow;

		if (type == AxisType.Value)
		{
			if (step < MinimumValueStep)
				step = MinimumValueStep;
		}
		else
		{
			if (step < MinimumDateTimeStep)
				step = MinimumDateTimeStep;
		}

		return step;
	}

	public abstract void RenderAxis(ChartRenderer chartRenderer, ChartViewport chartViewport);

	public double SolveCoordinate(double axisValue, double axisMinimum, double axisMaximum, double displayOffset, double displaySize)
	{
		if (double.IsNaN(axisMinimum) || double.IsNaN(axisMaximum) || axisMinimum >= axisMaximum)
			return double.NaN;

		return Math.Round
		(
			IsInverted
				? displayOffset + displaySize * (1 - (axisValue - axisMinimum) / (axisMaximum - axisMinimum))
				: displayOffset + displaySize * ((axisValue - axisMinimum) / (axisMaximum - axisMinimum))
		);
	}

	public double SolveValue(double displayCoordinate, double axisMinimum, double axisMaximum, double displayOffset, double displaySize)
	{
		if (double.IsNaN(axisMinimum) ||
		    double.IsNaN(axisMaximum) ||
		    axisMinimum >= axisMaximum ||
		    displaySize <= 0 ||
		    displayCoordinate < displayOffset ||
		    displayCoordinate > displayOffset + displaySize)
			return double.NaN;

		return IsInverted
			? axisMinimum + (axisMaximum - axisMinimum) * (1 - (displayCoordinate - displayOffset) / displaySize)
			: axisMinimum + (axisMaximum - axisMinimum) * ((displayCoordinate - displayOffset) / displaySize);
	}

	public string FormatValue(double value)
	{
		return AxisType == AxisType.Value
			? value.ToString(ValueFormat)
			: ToDateTime(value).ToString(ValueFormat);
	}

	protected double AlignValue(double value, double step)
	{
		if (AxisType == AxisType.Value)
			return step * Math.Floor(value / step);

		var dateTime = ToDateTime(value);
		dateTime = AxisType switch
		{
			AxisType.Year => dateTime.DayOfYear == 1 && dateTime.TimeOfDay.Ticks == 0
				? new DateTime(dateTime.Year, 1, 1)
				: new DateTime(dateTime.Year, 1, 1).AddYears(1),

			AxisType.Month => dateTime.Day == 1 && dateTime.TimeOfDay.Ticks == 0
				? new DateTime(dateTime.Year, dateTime.Month, 1)
				: new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1),

			AxisType.Day => dateTime.TimeOfDay.Hours == 0 && dateTime.TimeOfDay.Minutes == 0 && dateTime.TimeOfDay.Seconds == 0 && dateTime.TimeOfDay.Milliseconds == 0
				? new DateTime(dateTime.Year, dateTime.Month, dateTime.Day)
				: new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(1),

			AxisType.Hour => dateTime.TimeOfDay.Minutes == 0 && dateTime.TimeOfDay.Seconds == 0 && dateTime.TimeOfDay.Milliseconds == 0
				? new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0)
				: new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0).AddHours(1),

			AxisType.Minute => dateTime.TimeOfDay.Seconds == 0 && dateTime.TimeOfDay.Milliseconds == 0
				? new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0)
				: new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0).AddMinutes(1),

			AxisType.Second => dateTime.TimeOfDay.Milliseconds == 0
				? new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second)
				: new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second).AddSeconds(1),
			_ => throw new InvalidOperationException("Incorrect axis type.")
		};
		return FromDateTime(dateTime);
	}

	protected double IncrementValue(double value, double delta)
	{
		return AxisType switch
		{
			AxisType.Value => value + delta,
			AxisType.Year => FromDateTime(ToDateTime(value).AddYears((int)delta)),
			AxisType.Month => FromDateTime(ToDateTime(value).AddMonths((int)delta)),
			AxisType.Day => FromDateTime(ToDateTime(value).AddDays(delta)),
			AxisType.Hour => FromDateTime(ToDateTime(value).AddHours(delta)),
			AxisType.Minute => FromDateTime(ToDateTime(value).AddMinutes(delta)),
			AxisType.Second => FromDateTime(ToDateTime(value).AddSeconds(delta)),
			_ => throw new InvalidOperationException("Incorrect axis type.")
		};
	}

	protected static bool IsValidBounds(Rect bounds)
	{
		return !double.IsNaN(bounds.X) && !double.IsNaN(bounds.Y) && bounds.Width > 0 && bounds.Height > 0;
	}

	protected static bool IsValidInterval(double minimum, double maximum, double step)
	{
		return !double.IsNaN(minimum) && !double.IsNaN(maximum) && step > 0 && minimum < maximum;
	}

	private static DateTime ToDateTime(double value)
	{
		return new DateTime((long)value);
	}

	private static double FromDateTime(DateTime dateTime)
	{
		return dateTime.Ticks;
	}
	#endregion
}

public class ChartAxisX : ChartAxis
{
	#region Methods
	public override void RenderAxis(ChartRenderer chartRenderer, ChartViewport chartViewport)
	{
		var isPrimary = IsPrimary;
		var isSecondary = IsSecondary;
		if (!isPrimary && !isSecondary)
			return;

		var displayBounds = chartViewport.InnerBounds;
		var minimumValue = chartViewport.MinimumX;
		var maximumValue = chartViewport.MaximumX;
		var valueStep = chartViewport.StepX;
		if (!IsValidBounds(displayBounds))
			return;

		if (!IsValidInterval(minimumValue, maximumValue, valueStep))
			return;

		var axisType = AxisType;
		var currentValue = AlignValue(minimumValue, valueStep);
		while (currentValue <= maximumValue)
		{
			if (axisType == AxisType.Value)
				RenderMinor(chartRenderer, displayBounds, currentValue, minimumValue, maximumValue, valueStep);
			if (currentValue >= minimumValue)
				RenderMajor(chartRenderer, displayBounds, currentValue, minimumValue, maximumValue);
			currentValue = IncrementValue(currentValue, valueStep);
		}

		RenderAxis(chartRenderer, displayBounds);
		RenderLabel(chartRenderer, displayBounds);
	}

	private void RenderMajor(ChartRenderer renderer, Rect bounds, double value, double minimum, double maximum)
	{
		var majorTick = MajorTick;
		var tickSize = MajorTickSize;
		var tickPen = MajorTickPen;
		var gridPen = MajorGridPen;

		var displayTop = bounds.Top;
		var displayBottom = bounds.Bottom;
		var displayOffset = bounds.X;
		var displaySize = bounds.Width;

		var coordinate = SolveCoordinate(value, minimum, maximum, displayOffset, displaySize);
		if (double.IsNaN(coordinate))
			return;

		if (value >= minimum && gridPen is not null)
			RenderGrid(renderer, coordinate, gridPen, displayTop, displayBottom);

		if (majorTick != TickType.None && tickPen is not null)
			RenderTick(renderer, coordinate, majorTick, tickSize, tickPen, displayTop, displayBottom);

		RenderValue(renderer, value, coordinate, displayTop, displayBottom);
	}

	private void RenderMinor(ChartRenderer renderer, Rect bounds, double value, double minimum, double maximum, double step)
	{
		var stepDivision = StepDivision;
		if (stepDivision <= 1)
			return;

		var minorTick = MinorTick;
		var tickSize = MinorTickSize;
		var tickPen = MinorTickPen;
		var gridPen = MinorGridPen;

		var displayTop = bounds.Top;
		var displayBottom = bounds.Bottom;
		var displayOffset = bounds.X;
		var displaySize = bounds.Width;

		var increment = step / stepDivision;
		for (var division = value + increment; division < value + step; division += increment)
		{
			if (division < minimum || division > maximum)
				continue;

			var coordinate = SolveCoordinate(division, minimum, maximum, displayOffset, displaySize);
			if (double.IsNaN(coordinate))
				continue;

			if (gridPen is not null)
				RenderGrid(renderer, coordinate, gridPen, displayTop, displayBottom);

			if (minorTick != TickType.None && tickPen is not null)
				RenderTick(renderer, coordinate, minorTick, tickSize, tickPen, displayTop, displayBottom);
		}
	}

	private void RenderAxis(ChartRenderer renderer, Rect bounds)
	{
		var isPrimary = IsPrimary;
		var isSecondary = IsSecondary;
		if (!isPrimary && !isSecondary)
			return;

		var linePen = LinePen;
		if (linePen is null)
			return;

		var x1 = bounds.Left;
		var y1 = bounds.Bottom;
		var x2 = bounds.Right;
		var y2 = bounds.Top;

		if (isPrimary)
			renderer.RenderLine(x1, y1, x2, y1, linePen);

		if (isSecondary)
			renderer.RenderLine(x1, y2, x2, y2, linePen);
	}

	private void RenderLabel(ChartRenderer renderer, Rect bounds)
	{
		var isPrimary = IsPrimary;
		var isSecondary = IsSecondary;
		if (!isPrimary && !isSecondary)
			return;

		var labelText = LabelText;
		var labelForeground = LabelForeground;
		if (string.IsNullOrEmpty(labelText) || labelForeground is null)
			return;

		var labelOffset = LabelOffset;
		var fontFamily = LabelFontFamily;
		var fontSize = LabelFontSize;
		var fontStyle = LabelFontStyle;
		var fontWeight = LabelFontWeight;

		var boundsLeft = bounds.Left;
		var boundsWidth = bounds.Width;
		var boundsTop = bounds.Top;
		var boundsBottom = bounds.Bottom;

		var labelTypeface = new Typeface(fontFamily, fontStyle, fontWeight);
		var formattedText = new FormattedText(labelText, labelTypeface, fontSize, TextAlignment.Left, TextWrapping.NoWrap, Size.Empty);
		var labelBounds = formattedText.Bounds;
		var labelWidth = labelBounds.Width;
		var labelHeight = labelBounds.Height;
		var labelX = boundsLeft + (boundsWidth - labelWidth) / 2;

		if (isPrimary)
		{
			var labelY = boundsBottom + labelOffset;
			var labelPosition = new Point(labelX, labelY);
			renderer.RenderText(formattedText, labelPosition, 0, labelForeground);
		}

		if (isSecondary)
		{
			var labelY = boundsTop - labelOffset - labelHeight;
			var labelPosition = new Point(labelX, labelY);
			renderer.RenderText(formattedText, labelPosition, 0, labelForeground);
		}
	}

	private void RenderValue(ChartRenderer renderer, double value, double coordinate, double displayTop, double displayBottom)
	{
		var isPrimary = IsPrimary;
		var isSecondary = IsSecondary;
		if (!isPrimary && !isSecondary)
			return;

		var valueForeground = ValueForeground;
		if (valueForeground is null)
			return;

		var valueFormat = ValueFormat;
		var valueOffset = ValueOffset;
		var fontFamily = ValueFontFamily;
		var fontSize = ValueFontSize;
		var fontStyle = ValueFontStyle;
		var fontWeight = ValueFontWeight;

		var valueText = value.ToString(valueFormat);
		var labelTypeface = new Typeface(fontFamily, fontStyle, fontWeight);
		var formattedText = new FormattedText(valueText, labelTypeface, fontSize, TextAlignment.Left, TextWrapping.NoWrap, Size.Empty);
		var labelBounds = formattedText.Bounds;
		var labelWidth = labelBounds.Width;
		var labelHeight = labelBounds.Height;
		var labelX = coordinate - labelWidth / 2;

		if (isPrimary)
		{
			var labelY = displayBottom + valueOffset;
			var labelPosition = new Point(labelX, labelY);
			renderer.RenderText(formattedText, labelPosition, 0, valueForeground);
		}

		if (isSecondary)
		{
			var labelY = displayTop - valueOffset - labelHeight;
			var labelPosition = new Point(labelX, labelY);
			renderer.RenderText(formattedText, labelPosition, 0, valueForeground);
		}
	}

	private void RenderTick(ChartRenderer renderer, double coordinate, TickType tick, int size, IPen pen, double top, double bottom)
	{
		var isPrimary = IsPrimary;
		var isSecondary = IsSecondary;
		if (!isPrimary && !isSecondary)
			return;

		if (isPrimary)
		{
			renderer.RenderLine
			(
				coordinate,
				tick is TickType.Inner or TickType.TwoSide ? bottom - size : bottom,
				coordinate,
				tick is TickType.Outer or TickType.TwoSide ? bottom + size : bottom,
				pen
			);
		}

		if (isSecondary)
		{
			renderer.RenderLine
			(
				coordinate,
				tick is TickType.Inner or TickType.TwoSide ? top + size : top,
				coordinate,
				tick is TickType.Outer or TickType.TwoSide ? top - size : top,
				pen
			);
		}
	}

	private static void RenderGrid(ChartRenderer renderer, double coordinate, IPen pen, double top, double bottom)
	{
		renderer.RenderLine(coordinate, top, coordinate, bottom, pen);
	}
	#endregion
}

public class ChartAxisY : ChartAxis
{
	#region Methods
	public override void RenderAxis(ChartRenderer chartRenderer, ChartViewport chartViewport)
	{
		var isPrimary = IsPrimary;
		var isSecondary = IsSecondary;
		if (!isPrimary && !isSecondary)
			return;

		var displayBounds = chartViewport.InnerBounds;
		var minimumValue = chartViewport.MinimumY;
		var maximumValue = chartViewport.MaximumY;
		var valueStep = chartViewport.StepY;
		if (!IsValidBounds(displayBounds))
			return;

		if (!IsValidInterval(minimumValue, maximumValue, valueStep))
			return;

		var axisType = AxisType;
		var currentValue = AlignValue(minimumValue, valueStep);
		while (currentValue <= maximumValue)
		{
			if (axisType == AxisType.Value)
				RenderMinor(chartRenderer, displayBounds, currentValue, minimumValue, maximumValue, valueStep);
			if (currentValue >= minimumValue)
				RenderMajor(chartRenderer, displayBounds, currentValue, minimumValue, maximumValue);
			currentValue = IncrementValue(currentValue, valueStep);
		}

		RenderAxis(chartRenderer, displayBounds);
		RenderLabel(chartRenderer, displayBounds);
	}

	private void RenderMajor(ChartRenderer renderer, Rect bounds, double value, double minimum, double maximum)
	{
		var majorTick = MajorTick;
		var tickSize = MajorTickSize;
		var tickPen = MajorTickPen;
		var gridPen = MajorGridPen;

		var displayLeft = bounds.Left;
		var displayRight = bounds.Right;
		var displayOffset = bounds.Y;
		var displaySize = bounds.Height;

		var coordinate = SolveCoordinate(value, minimum, maximum, displayOffset, displaySize);
		if (double.IsNaN(coordinate))
			return;

		if (value >= minimum && gridPen is not null)
			RenderGrid(renderer, coordinate, gridPen, displayLeft, displayRight);

		if (majorTick != TickType.None && tickPen is not null)
			RenderTick(renderer, coordinate, majorTick, tickSize, tickPen, displayLeft, displayRight);

		RenderValue(renderer, value, coordinate, displayLeft, displayRight);
	}

	private void RenderMinor(ChartRenderer renderer, Rect bounds, double value, double minimum, double maximum, double step)
	{
		var stepDivision = StepDivision;
		var minorTick = MinorTick;
		var tickSize = MinorTickSize;
		var tickPen = MinorTickPen;
		var gridPen = MinorGridPen;

		if (stepDivision <= 1)
			return;

		var displayLeft = bounds.Left;
		var displayRight = bounds.Right;
		var displayOffset = bounds.Y;
		var displaySize = bounds.Height;

		var increment = step / stepDivision;
		for (var division = value + increment; division < value + step; division += increment)
		{
			if (division < minimum || division > maximum)
				continue;

			var coordinate = SolveCoordinate(division, minimum, maximum, displayOffset, displaySize);
			if (double.IsNaN(coordinate))
				continue;

			if (gridPen is not null)
				RenderGrid(renderer, coordinate, gridPen, displayLeft, displayRight);

			if (minorTick != TickType.None && tickPen is not null)
				RenderTick(renderer, coordinate, minorTick, tickSize, tickPen, displayLeft, displayRight);
		}
	}

	private void RenderAxis(ChartRenderer renderer, Rect bounds)
	{
		var isPrimary = IsPrimary;
		var isSecondary = IsSecondary;
		if (!isPrimary && !isSecondary)
			return;

		var linePen = LinePen;
		if (linePen is null)
			return;

		var x1 = bounds.Left;
		var y1 = bounds.Bottom;
		var x2 = bounds.Right;
		var y2 = bounds.Top;

		if (isPrimary)
			renderer.RenderLine(x1, y1, x1, y2, linePen);

		if (isSecondary)
			renderer.RenderLine(x2, y1, x2, y2, linePen);
	}

	private void RenderLabel(ChartRenderer renderer, Rect bounds)
	{
		var isPrimary = IsPrimary;
		var isSecondary = IsSecondary;
		if (!isPrimary && !isSecondary)
			return;

		var labelText = LabelText;
		var labelForeground = LabelForeground;
		if (string.IsNullOrEmpty(labelText) || labelForeground is null)
			return;

		var labelOffset = LabelOffset;
		var fontFamily = LabelFontFamily;
		var fontSize = LabelFontSize;
		var fontStyle = LabelFontStyle;
		var fontWeight = LabelFontWeight;

		var boundsLeft = bounds.Left;
		var boundsRight = bounds.Right;
		var boundsTop = bounds.Top;
		var boundsBottom = bounds.Bottom;
		var boundsHeight = bounds.Height;

		var labelTypeface = new Typeface(fontFamily, fontStyle, fontWeight);
		var formattedText = new FormattedText(labelText, labelTypeface, fontSize, TextAlignment.Left, TextWrapping.NoWrap, Size.Empty);
		var labelBounds = formattedText.Bounds;
		var labelWidth = labelBounds.Width;
		var labelHeight = labelBounds.Height;

		if (isPrimary)
		{
			var labelX = boundsLeft - labelOffset - labelHeight;
			var labelY = boundsBottom - (boundsHeight - labelWidth) / 2;
			var labelPosition = new Point(labelX, labelY);
			renderer.RenderText(formattedText, labelPosition, 90, labelForeground);
		}

		if (isSecondary)
		{
			var labelX = boundsRight + labelOffset + labelHeight;
			var labelY = boundsTop + (boundsHeight - labelWidth) / 2;
			var labelPosition = new Point(labelX, labelY);
			renderer.RenderText(formattedText, labelPosition, -90, labelForeground);
		}
	}

	private void RenderValue(ChartRenderer renderer, double value, double coordinate, double displayLeft, double displayRight)
	{
		var isPrimary = IsPrimary;
		var isSecondary = IsSecondary;
		if (!isPrimary && !isSecondary)
			return;

		var valueForeground = ValueForeground;
		if (valueForeground is null)
			return;

		var valueFormat = ValueFormat;
		var valueOffset = ValueOffset;
		var fontFamily = ValueFontFamily;
		var fontSize = ValueFontSize;
		var fontStyle = ValueFontStyle;
		var fontWeight = ValueFontWeight;

		var valueText = value.ToString(valueFormat);
		var labelTypeface = new Typeface(fontFamily, fontStyle, fontWeight);
		var formattedText = new FormattedText(valueText, labelTypeface, fontSize, TextAlignment.Left, TextWrapping.NoWrap, Size.Empty);
		var labelBounds = formattedText.Bounds;
		var labelWidth = labelBounds.Width;
		var labelHeight = labelBounds.Height;

		if (isPrimary)
		{
			var labelX = displayLeft - valueOffset - labelHeight;
			var labelY = coordinate + labelWidth / 2;
			var labelPosition = new Point(labelX, labelY);
			renderer.RenderText(formattedText, labelPosition, 90, valueForeground);
		}

		if (isSecondary)
		{
			var labelX = displayRight + valueOffset + labelHeight;
			var labelY = coordinate - labelWidth / 2;
			var labelPosition = new Point(labelX, labelY);
			renderer.RenderText(formattedText, labelPosition, -90, valueForeground);
		}
	}

	private void RenderTick(ChartRenderer renderer, double coordinate, TickType tick, int size, IPen pen, double left, double right)
	{
		var isPrimary = IsPrimary;
		var isSecondary = IsSecondary;
		if (!isPrimary && !isSecondary)
			return;

		if (isPrimary)
		{
			renderer.RenderLine
			(
				tick is TickType.Outer or TickType.TwoSide ? left - size : left,
				coordinate,
				tick is TickType.Inner or TickType.TwoSide ? left + size : left,
				coordinate,
				pen
			);
		}

		if (isSecondary)
		{
			renderer.RenderLine
			(
				tick is TickType.Inner or TickType.TwoSide ? right - size : right,
				coordinate,
				tick is TickType.Outer or TickType.TwoSide ? right + size : right,
				coordinate,
				pen
			);
		}
	}

	private static void RenderGrid(ChartRenderer renderer, double coordinate, IPen pen, double left, double right)
	{
		renderer.RenderLine(left, coordinate, right, coordinate, pen);
	}
	#endregion
}