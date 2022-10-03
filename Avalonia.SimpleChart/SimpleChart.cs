//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace Avalonia.SimpleChart;

public class SimpleChart : Control
{
	#region Constants
	private const int UpdateTimerInterval = 10;
	private const int ZoomTicksCount = 10;
	private const double ZoomMinimumInterval = 0.001;
	#endregion

	#region Fields
	private Rect _innerBounds;
	private bool _needInvalidate;

	private ChartAxisX _axisX;
	private ChartAxisY _axisY;
	private ChartPan? _chartPan;
	private ChartZoom? _chartZoom;
	private ChartCursor? _chartCursor;
	private IEnumerable<ChartSeries> _seriesCollection;
	private readonly DispatcherTimer _updateTimer;

	public static readonly StyledProperty<IBrush?> BackgroundProperty =
		AvaloniaProperty.Register<SimpleChart, IBrush?>(nameof(Background), Brushes.White);

	public static readonly StyledProperty<Thickness> PaddingProperty =
		AvaloniaProperty.Register<SimpleChart, Thickness>(nameof(Padding));

	public static readonly StyledProperty<bool> InSquareProperty =
		AvaloniaProperty.Register<SimpleChart, bool>(nameof(InSquare));

	public static readonly DirectProperty<SimpleChart, ChartAxisX> AxisXProperty =
		AvaloniaProperty.RegisterDirect<SimpleChart, ChartAxisX>(nameof(AxisX), o => o.AxisX, (o, v) => o.AxisX = v);

	public static readonly DirectProperty<SimpleChart, ChartAxisY> AxisYProperty =
		AvaloniaProperty.RegisterDirect<SimpleChart, ChartAxisY>(nameof(AxisY), o => o.AxisY, (o, v) => o.AxisY = v);

	public static readonly DirectProperty<SimpleChart, ChartZoom?> ChartZoomProperty =
		AvaloniaProperty.RegisterDirect<SimpleChart, ChartZoom?>(nameof(ChartZoom), o => o.ChartZoom, (o, v) => o.ChartZoom = v);

	public static readonly DirectProperty<SimpleChart, ChartPan?> ChartPanProperty =
		AvaloniaProperty.RegisterDirect<SimpleChart, ChartPan?>(nameof(ChartPan), o => o.ChartPan, (o, v) => o.ChartPan = v);

	public static readonly DirectProperty<SimpleChart, ChartCursor?> ChartCursorProperty =
		AvaloniaProperty.RegisterDirect<SimpleChart, ChartCursor?>(nameof(ChartCursor), o => o.ChartCursor, (o, v) => o.ChartCursor = v);

	public static readonly DirectProperty<SimpleChart, IEnumerable<ChartSeries>> SeriesCollectionProperty =
		AvaloniaProperty.RegisterDirect<SimpleChart, IEnumerable<ChartSeries>>(nameof(SeriesCollection), o => o.SeriesCollection, (o, v) => o.SeriesCollection = v);
	#endregion

	#region Properties
	public IBrush? Background
	{
		get => GetValue(BackgroundProperty);
		set => SetValue(BackgroundProperty, value);
	}

	public Thickness Padding
	{
		get => GetValue(PaddingProperty);
		set => SetValue(PaddingProperty, value);
	}

	public bool InSquare
	{
		get => GetValue(InSquareProperty);
		set => SetValue(InSquareProperty, value);
	}

	public ChartAxisX AxisX
	{
		get => _axisX;
		set
		{
			_axisX.PropertyChanged -= OnInvalidateChart;
			SetAndRaise(AxisXProperty, ref _axisX, value);
			_axisX.PropertyChanged += OnInvalidateChart;
		}
	}

	public ChartAxisY AxisY
	{
		get => _axisY;
		set
		{
			_axisY.PropertyChanged -= OnInvalidateChart;
			SetAndRaise(AxisYProperty, ref _axisY, value);
			_axisY.PropertyChanged += OnInvalidateChart;
		}
	}

	public ChartPan? ChartPan
	{
		get => _chartPan;
		set
		{
			if (_chartPan is not null)
				_chartPan.PropertyChanged -= OnInvalidateChart;

			SetAndRaise(ChartPanProperty, ref _chartPan, value);

			if (_chartPan is not null)
				_chartPan.PropertyChanged += OnInvalidateChart;
		}
	}

	public ChartZoom? ChartZoom
	{
		get => _chartZoom;
		set
		{
			if (_chartZoom is not null)
				_chartZoom.PropertyChanged -= OnInvalidateChart;

			SetAndRaise(ChartZoomProperty, ref _chartZoom, value);

			if (_chartZoom is not null)
				_chartZoom.PropertyChanged += OnInvalidateChart;
		}
	}

	public ChartCursor? ChartCursor
	{
		get => _chartCursor;
		set
		{
			if (_chartCursor is not null)
				_chartCursor.PropertyChanged -= OnInvalidateChart;

			SetAndRaise(ChartCursorProperty, ref _chartCursor, value);

			if (_chartCursor is not null)
				_chartCursor.PropertyChanged += OnInvalidateChart;
		}
	}

	public IEnumerable<ChartSeries> SeriesCollection
	{
		get => _seriesCollection;
		set
		{
			RemoveSeriesHandlers();

			SetAndRaise(SeriesCollectionProperty, ref _seriesCollection, value);

			SetupSeriesHandlers();
		}
	}

	private double MinimumX => _chartZoom?.IsZoomed == true ? _chartZoom.MinimumX : _axisX.MinimumValue;
	private double MaximumX => _chartZoom?.IsZoomed == true ? _chartZoom.MaximumX : _axisX.MaximumValue;
	private double StepX => _chartZoom?.IsZoomed == true ? _chartZoom.StepX : _axisX.ValueStep;

	private double MinimumY => _chartZoom?.IsZoomed == true ? _chartZoom.MinimumY : _axisY.MinimumValue;
	private double MaximumY => _chartZoom?.IsZoomed == true ? _chartZoom.MaximumY : _axisY.MaximumValue;
	private double StepY => _chartZoom?.IsZoomed == true ? _chartZoom.StepY : _axisY.ValueStep;
	#endregion

	#region Events
	public event EventHandler<ChartPoint>? PointClicked;
	public event EventHandler<ChartPoint>? PointDoubleClicked;
	#endregion

	#region Constructors
	public SimpleChart()
	{
		_axisX = new ChartAxisX();
		_axisY = new ChartAxisY();
		_seriesCollection = new ObservableCollection<ChartSeries>();
		_updateTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(UpdateTimerInterval), DispatcherPriority.Render, OnControlUpdate);

		SetupAxesHandlers();
		SetupSeriesHandlers();
	}
	#endregion

	#region Methods
	public override void Render(DrawingContext context)
	{
		var axisX = _axisX;
		var axisY = _axisY;
		var chartZoom = _chartZoom;
		var chartCursor = _chartCursor;
		var innerBounds = _innerBounds;
		var seriesCollection = _seriesCollection;

		var background = Background;

		var minimumX = MinimumX;
		var maximumX = MaximumX;
		var stepX = StepX;

		var minimumY = MinimumY;
		var maximumY = MaximumY;
		var stepY = StepY;

		var chartRenderer = new ChartRenderer(context);
		var chartViewport = new ChartViewport(innerBounds, minimumX, maximumX, stepX, minimumY, maximumY, stepY);
		chartRenderer.RenderRectangle(innerBounds, 0, null, background);
		axisX.RenderAxis(chartRenderer, chartViewport);
		axisY.RenderAxis(chartRenderer, chartViewport);
		foreach (var series in seriesCollection)
			series.RenderSeries(chartRenderer, chartViewport, axisX, axisY);
		chartZoom?.RenderZoom(chartRenderer);
		chartCursor?.RenderCursor(chartRenderer, chartViewport, axisX, axisY);
	}

	private void SetupAxesHandlers()
	{
		_axisX.PropertyChanged += OnInvalidateChart;
		_axisY.PropertyChanged += OnInvalidateChart;
	}

	private void SetupSeriesHandlers()
	{
		foreach (var series in _seriesCollection)
			SetupSeriesHandlers(series);

		if (_seriesCollection is ObservableCollection<ChartSeries> newCollection)
			newCollection.CollectionChanged += OnSeriesCollectionChanged;
	}

	private void RemoveSeriesHandlers()
	{
		if (_seriesCollection is ObservableCollection<ChartSeries> oldCollection)
			oldCollection.CollectionChanged -= OnSeriesCollectionChanged;

		foreach (var series in _seriesCollection)
			RemoveSeriesHandlers(series);
	}

	private void SetupSeriesHandlers(ChartSeries series)
	{
		series.PropertyChanged += OnSeriesPropertyChanged;
		SetupPointsHandlers(series.PointsCollection);
	}

	private void RemoveSeriesHandlers(ChartSeries series)
	{
		series.PropertyChanged -= OnSeriesPropertyChanged;
		RemovePointsHandlers(series.PointsCollection);
	}

	private void SetupPointsHandlers(IEnumerable<ChartPoint>? points)
	{
		if (points is null)
			return;

		foreach (var point in points)
			point.PropertyChanged += OnPointPropertyChanged;

		if (points is ObservableCollection<ChartPoint> observable)
			observable.CollectionChanged += OnPointsCollectionChanged;
	}

	private void RemovePointsHandlers(IEnumerable<ChartPoint>? points)
	{
		if (points is null)
			return;

		foreach (var point in points)
			point.PropertyChanged -= OnPointPropertyChanged;

		if (points is ObservableCollection<ChartPoint> observable)
			observable.CollectionChanged -= OnPointsCollectionChanged;
	}

	private void OnSeriesPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property.Name == nameof(ChartSeries.PointsCollection))
		{
			if (e.OldValue is IEnumerable<ChartPoint> oldPoints)
				RemovePointsHandlers(oldPoints);
			if (e.NewValue is IEnumerable<ChartPoint> newPoints)
				SetupPointsHandlers(newPoints);
		}

		_needInvalidate = true;
	}

	private void OnSeriesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.OldItems is not null)
		{
			foreach (ChartSeries series in e.OldItems)
				RemoveSeriesHandlers(series);
		}

		if (e.NewItems is not null)
		{
			foreach (ChartSeries series in e.NewItems)
				SetupSeriesHandlers(series);
		}

		_needInvalidate = true;
	}

	private void OnPointPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		_needInvalidate = true;
	}

	private void OnPointsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.OldItems is not null)
		{
			foreach (ChartPoint point in e.OldItems)
				point.PropertyChanged -= OnPointPropertyChanged;
		}

		if (e.NewItems is not null)
		{
			foreach (ChartPoint point in e.NewItems)
				point.PropertyChanged += OnPointPropertyChanged;
		}

		_needInvalidate = true;
	}

	protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
	{
		base.OnPropertyChanged(change);

		switch (change.Property.Name)
		{
			case nameof(Bounds):
			case nameof(Padding):
			case nameof(InSquare):
				_innerBounds = SolveInnerRectangle(Padding, InSquare, Bounds.Width, Bounds.Height);
				break;

			default:
				if (change.OldValue.Value is ChartItem oldItem)
					oldItem.PropertyChanged -= OnInvalidateChart;
				if (change.NewValue.Value is ChartItem newItem)
					newItem.PropertyChanged += OnInvalidateChart;
				break;
		}

		_needInvalidate = true;
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		_updateTimer.Start();
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		_updateTimer.Stop();
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		var axisX = _axisX;
		var axisY = _axisY;
		var chartPan = _chartPan;
		var chartZoom = _chartZoom;
		var innerBounds = _innerBounds;
		var clickedHandler = PointClicked;
		var doubleClickedHandler = PointDoubleClicked;

		var minimumX = MinimumX;
		var maximumX = MaximumX;
		var minimumY = MinimumY;
		var maximumY = MaximumY;

		var handleClicked = clickedHandler is not null && e.ClickCount == 1;
		var handleDoubleClicked = doubleClickedHandler is not null && e.ClickCount == 2;
		if (handleClicked || handleDoubleClicked)
		{
			var offsetX = innerBounds.X;
			var offsetY = innerBounds.Y;
			var sizeX = innerBounds.Width;
			var sizeY = innerBounds.Height;
			var (coordinateX, coordinateY) = e.GetPosition(this);
			var point = new ChartPoint
			(
				axisX.SolveValue(coordinateX, minimumX, maximumX, offsetX, sizeX),
				axisY.SolveValue(coordinateY, minimumY, maximumY, offsetY, sizeY)
			);

			if (handleClicked)
				clickedHandler?.Invoke(this, point);
			else
				doubleClickedHandler?.Invoke(this, point);
		}

		if (chartZoom is not null && (e.KeyModifiers & KeyModifiers.Control) != 0)
		{
			var position = e.GetPosition(this);
			chartZoom.StartZooming(position);
		}
		else if (chartPan is not null && (e.KeyModifiers & KeyModifiers.Shift) != 0)
		{
			var position = e.GetPosition(this);
			if (chartZoom?.IsZoomed == true)
				chartPan.StartPanning(position);
		}
		else
		{
			var seriesCollection = _seriesCollection;
			var (positionX, positionY) = e.GetPosition(this);
			foreach (var series in seriesCollection)
			{
				if (!series.IsVisible || !series.IsChangeable)
					continue;

				foreach (var point in series.PointsCollection)
				{
					var offsetX = innerBounds.X;
					var offsetY = innerBounds.Y;
					var sizeX = innerBounds.Width;
					var sizeY = innerBounds.Height;
					var pointX = point.X;
					var pointY = point.Y;
					pointX = axisX.SolveCoordinate(pointX, minimumX, maximumX, offsetX, sizeX);
					pointY = axisY.SolveCoordinate(pointY, minimumY, maximumY, offsetY, sizeY);
					var size = point.Style?.Size ?? ChartPoint.PointNeighborhoodSize;
					if (Math.Abs(pointX - positionX) > size / 2 || Math.Abs(pointY - positionY) > size / 2)
						continue;

					series.StartChanging(point);
					return;
				}
			}
		}
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		var axisX = _axisX;
		var axisY = _axisY;
		var chartPan = _chartPan;
		var chartZoom = _chartZoom;
		var innerBounds = _innerBounds;
		var seriesCollection = _seriesCollection;

		var minimumX = MinimumX;
		var maximumX = MaximumX;
		var minimumY = MinimumY;
		var maximumY = MaximumY;

		if (chartZoom?.IsZooming == true)
		{
			var zoomStartX = chartZoom.StartPosition.X;
			var zoomStartY = chartZoom.StartPosition.Y;
			var zoomStopX = chartZoom.EndPosition.X;
			var zoomStopY = chartZoom.EndPosition.Y;

			// Zooming.
			if (zoomStopX > zoomStartX && zoomStopY > zoomStartY)
			{
				var offsetX = innerBounds.X;
				var offsetY = innerBounds.Y;
				var sizeX = innerBounds.Width;
				var sizeY = innerBounds.Height;

				zoomStartX = axisX.SolveValue(zoomStartX, minimumX, maximumX, offsetX, sizeX);
				zoomStopX = axisX.SolveValue(zoomStopX, minimumX, maximumX, offsetX, sizeX);
				zoomStartY = axisY.SolveValue(zoomStartY, minimumY, maximumY, offsetY, sizeY);
				zoomStopY = axisY.SolveValue(zoomStopY, minimumY, maximumY, offsetY, sizeY);
				if (!double.IsNaN(zoomStartX) &&
				    !double.IsNaN(zoomStopX) &&
				    !double.IsNaN(zoomStartY) &&
				    !double.IsNaN(zoomStopY))
				{
					var maximumZoom =
						Math.Abs(zoomStartX - zoomStopX) < ZoomMinimumInterval ||
						Math.Abs(zoomStartY - zoomStopY) < ZoomMinimumInterval;
					if (!maximumZoom)
					{
						if (zoomStartX > zoomStopX)
							(zoomStartX, zoomStopX) = (zoomStopX, zoomStartX);
						var zoomStepX = axisX.SolveStep(zoomStartX, zoomStopX, ZoomTicksCount);

						if (zoomStartY > zoomStopY)
							(zoomStartY, zoomStopY) = (zoomStopY, zoomStartY);
						var zoomStepY = axisY.SolveStep(zoomStartY, zoomStopY, ZoomTicksCount);

						chartZoom.SetZoom(zoomStartX, zoomStopX, zoomStepX, zoomStartY, zoomStopY, zoomStepY);
					}
				}
			}
			// Reset zoom.
			else if (zoomStopX < zoomStartX && zoomStopY < zoomStartY)
				chartZoom.ResetZoom();

			chartZoom.StopZooming();
			_needInvalidate = true;
		}

		if (chartPan?.IsPanning == true)
			chartPan.StopPanning();

		foreach (var series in seriesCollection)
		{
			if (series.IsChanging)
				series.StopChanging();
		}
	}

	protected override void OnPointerMoved(PointerEventArgs e)
	{
		base.OnPointerMoved(e);

		var pointerPosition = e.GetPosition(this);
		var innerBounds = _innerBounds;
		if (innerBounds.IsEmpty || !innerBounds.Contains(pointerPosition))
			return;

		var offsetX = innerBounds.X;
		var offsetY = innerBounds.Y;
		var sizeX = innerBounds.Width;
		var sizeY = innerBounds.Height;

		var axisX = _axisX;
		var axisY = _axisY;
		var chartPan = _chartPan;
		var chartZoom = _chartZoom;
		var chartCursor = _chartCursor;
		var seriesCollection = _seriesCollection;

		var minimumX = MinimumX;
		var maximumX = MaximumX;
		var stepX = StepX;

		var minimumY = MinimumY;
		var maximumY = MaximumY;
		var stepY = StepY;

		if (chartCursor is not null)
		{
			var chartViewport = new ChartViewport(innerBounds, minimumX, maximumX, stepX, minimumY, maximumY, stepY);
			chartCursor.UpdateCursor(pointerPosition, chartViewport, axisX, axisY, seriesCollection);
		}

		if (chartZoom?.IsZooming == true)
			chartZoom.UpdateZooming(pointerPosition);

		if (chartPan?.IsPanning == true && chartZoom?.IsZoomed == true)
		{
			var startPanX = chartPan.PanPosition.X;
			var startPanY = chartPan.PanPosition.Y;
			var stopPanX = pointerPosition.X;
			var stopPanY = pointerPosition.Y;
			startPanX = axisX.SolveValue(startPanX, minimumX, maximumX, offsetX, sizeX);
			startPanY = axisY.SolveValue(startPanY, minimumY, maximumY, offsetY, sizeY);
			stopPanX = axisX.SolveValue(stopPanX, minimumX, maximumX, offsetX, sizeX);
			stopPanY = axisY.SolveValue(stopPanY, minimumY, maximumY, offsetY, sizeY);

			if (!double.IsNaN(startPanX) &&
			    !double.IsNaN(stopPanX) &&
			    !double.IsNaN(startPanY) &&
			    !double.IsNaN(stopPanY))
			{
				var dx = startPanX - stopPanX;
				var dy = startPanY - stopPanY;

				if (chartZoom.MinimumX + dx < axisX.MinimumValue)
					dx = axisX.MinimumValue - chartZoom.MinimumX;

				if (chartZoom.MaximumX + dx > axisX.MaximumValue)
					dx = axisX.MaximumValue - chartZoom.MaximumX;

				if (chartZoom.MinimumY + dy < axisY.MinimumValue)
					dy = axisY.MinimumValue - chartZoom.MinimumY;

				if (chartZoom.MaximumY + dy > axisY.MaximumValue)
					dy = axisY.MaximumValue - chartZoom.MaximumY;

				chartZoom.MoveZoom(dx, dy);
			}

			chartPan.StartPanning(pointerPosition);
		}

		foreach (var series in seriesCollection)
		{
			if (!series.IsChanging)
				continue;

			var point = series.ChangeablePoint;
			if (point is null)
				continue;

			var x = pointerPosition.X;
			var y = pointerPosition.Y;
			x = axisX.SolveValue(x, minimumX, maximumX, offsetX, sizeX);
			y = axisY.SolveValue(y, minimumY, maximumY, offsetY, sizeY);
			if (!double.IsNaN(x) && !double.IsNaN(y))
			{
				point.X = x;
				point.Y = y;
			}

			break;
		}

		_needInvalidate = true;
	}

	protected override void OnPointerLeave(PointerEventArgs e)
	{
		base.OnPointerLeave(e);

		_chartCursor?.ResetCursor();
		_needInvalidate = true;
	}

	private static Rect SolveInnerRectangle(Thickness padding, bool squared, double width, double height)
	{
		var x = padding.Left;
		var y = padding.Top;
		var w = width - (padding.Left + padding.Right);
		var h = height - (padding.Top + padding.Bottom);
		if (w <= 0 || h <= 0)
			return Rect.Empty;

		if (squared)
		{
			var s = Math.Min(w, h);
			x += (w - s) / 2;
			y += (h - s) / 2;
			w = s;
			h = s;
		}

		return new Rect(x, y, w, h);
	}

	private void OnInvalidateChart(object? sender, EventArgs e)
	{
		_needInvalidate = true;
	}

	private void OnControlUpdate(object? sender, EventArgs e)
	{
		if (!_needInvalidate)
			return;

		_needInvalidate = false;
		InvalidateVisual();
	}
	#endregion
}