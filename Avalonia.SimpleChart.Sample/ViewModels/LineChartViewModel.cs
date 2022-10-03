//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using System;
using System.Collections.ObjectModel;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.SimpleChart.Sample.ViewModels;

[ObservableObject]
public partial class LineChartViewModel
{
	#region Fields
	[ObservableProperty]
	private ObservableCollection<ChartPoint> _linePoints;

	[ObservableProperty]
	private double _minimumX;

	[ObservableProperty]
	private double _maximumX;

	[ObservableProperty]
	private double _stepX;

	[ObservableProperty]
	private double _minimumY;

	[ObservableProperty]
	private double _maximumY;

	[ObservableProperty]
	private double _stepY;

	[ObservableProperty]
	private bool _addLabels;

	[ObservableProperty]
	private bool _addMarkers;

	private readonly Random _randomGenerator;

	private static readonly IBrush[] _brushes =
	{
		Brushes.Blue, Brushes.Red, Brushes.Green, Brushes.Black, Brushes.Orange, Brushes.DarkGray
	};
	#endregion

	#region Constructors
	public LineChartViewModel()
	{
		_randomGenerator = new Random();
		_minimumX = 0;
		_maximumX = 10;
		_stepX = 1;
		_minimumY = 0;
		_maximumY = 10;
		_stepY = 1;
		_linePoints = new ObservableCollection<ChartPoint>();
	}
	#endregion

	#region Methods
	[RelayCommand]
	private void AddPoints(object? parameter)
	{
		var count = Convert.ToInt32(parameter);
		const double step = 1;
		for (var i = 0; i < count; ++i)
		{
			var x = _linePoints.Count * step;
			var y = 100 * _randomGenerator.NextDouble();
			var brush = _brushes[_randomGenerator.Next(0, _brushes.Length)];
			var pen = new Pen(_brushes[_randomGenerator.Next(0, _brushes.Length)]);
			var label = _addLabels ? new PointLabel($"X: {x:##.###}\nY: {y:##.###}", 8, new LabelStyle(brush, 10, FontFamily.Default, FontStyle.Normal, FontWeight.Bold), LabelLocation.TopCenter) : null;
			var style = _addMarkers ? new PointStyle(MarkerType.Circle, 5, brush, pen) : null;
			var point = new ChartPoint
			(
				x,
				y,
				label,
				style
			);
			_linePoints.Add(point);

			if (x < _minimumX)
				MinimumX = x;
			if (x > _maximumX)
				MaximumX = x;
			StepX = SolveAxisStep(_minimumX, _maximumX);

			if (y < _minimumY)
				MinimumY = y;
			if (y > _maximumY)
				MaximumY = y;
			StepY = SolveAxisStep(_minimumY, _maximumY);
		}
	}

	[RelayCommand]
	private void ClearPoints()
	{
		_linePoints.Clear();
		MinimumX = 0;
		MaximumX = 10;
		StepX = 1;
		MinimumY = 0;
		MaximumY = 10;
		StepY = 1;
	}

	private static double SolveAxisStep(double minimum, double maximum)
	{
		return (maximum - minimum) switch
		{
			<= 10 => 1,
			<= 50 => 5,
			<= 100 => 10,
			<= 500 => 50,
			<= 1000 => 100,
			_ => 1000
		};
	}
	#endregion
}