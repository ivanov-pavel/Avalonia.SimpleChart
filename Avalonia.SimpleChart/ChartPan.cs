//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

namespace Avalonia.SimpleChart;

public class ChartPan : ChartItem
{
	#region Fields
	private bool _isPanning;
	private Point _panPosition;
	#endregion

	#region Properties
	public bool IsPanning => _isPanning;

	public Point PanPosition => _panPosition;
	#endregion

	#region Methods
	public void StartPanning(Point position)
	{
		_isPanning = true;
		_panPosition = position;
	}

	public void StopPanning()
	{
		_isPanning = false;
	}
	#endregion
}