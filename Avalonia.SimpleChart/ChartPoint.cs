//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using System.ComponentModel;

namespace Avalonia.SimpleChart;

public class ChartPoint : INotifyPropertyChanged
{
	#region Fields
	private double _x;
	private double _y;

	private PointLabel? _label;
	private PointStyle? _style;

	private static readonly PropertyChangedEventArgs _xChangedEventArgs = new(nameof(X));
	private static readonly PropertyChangedEventArgs _yChangedEventArgs = new(nameof(Y));
	private static readonly PropertyChangedEventArgs _labelChangedEventArgs = new(nameof(Label));
	private static readonly PropertyChangedEventArgs _styleChangedEventArgs = new(nameof(Style));
	#endregion

	#region Properties
	public double X
	{
		get => _x;
		set
		{
			if (_x.Equals(value))
				return;

			_x = value;
			PropertyChanged?.Invoke(this, _xChangedEventArgs);
		}
	}

	public double Y
	{
		get => _y;
		set
		{
			if (_y.Equals(value))
				return;

			_y = value;
			PropertyChanged?.Invoke(this, _yChangedEventArgs);
		}
	}

	public PointLabel? Label
	{
		get => _label;
		set
		{
			if (_label == value)
				return;

			_label = value;
			PropertyChanged?.Invoke(this, _labelChangedEventArgs);
		}
	}

	public PointStyle? Style
	{
		get => _style;
		set
		{
			if (_style == value)
				return;

			_style = value;
			PropertyChanged?.Invoke(this, _styleChangedEventArgs);
		}
	}

	public bool IsValid => !double.IsNaN(_x) && !double.IsNaN(_y);
	#endregion

	#region Events
	public event PropertyChangedEventHandler? PropertyChanged;
	#endregion

	#region Constructors
	public ChartPoint(double x, double y)
	{
		_x = x;
		_y = y;
	}

	public ChartPoint(double x, double y, PointLabel label)
	{
		_x = x;
		_y = y;
		_label = label;
	}

	public ChartPoint(double x, double y, PointStyle style)
	{
		_x = x;
		_y = y;
		_style = style;
	}

	public ChartPoint(double x, double y, PointLabel? label, PointStyle? style)
	{
		_x = x;
		_y = y;
		_label = label;
		_style = style;
	}
	#endregion

	public const int PointNeighborhoodSize = 4;
}