//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using Avalonia.Media;

namespace Avalonia.SimpleChart;

public class PointStyle
{
	#region Fields
	private readonly MarkerType _marker;
	private readonly IBrush _fill;
	private readonly IPen _border;
	private readonly double _size;
	#endregion

	#region Properties
	public MarkerType Marker => _marker;
	public double Size => _size;
	public IBrush Fill => _fill;
	public IPen Border => _border;
	#endregion

	#region Constructors
	public PointStyle(MarkerType marker, double size, IBrush fill, IPen border)
	{
		_marker = marker;
		_size = size;
		_fill = fill;
		_border = border;
	}
	#endregion
}