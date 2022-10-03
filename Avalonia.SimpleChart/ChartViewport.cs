//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

namespace Avalonia.SimpleChart;

public class ChartViewport
{
	#region Properties
	public Rect InnerBounds { get; }
	public double MinimumX { get; }
	public double MaximumX { get; }
	public double StepX { get; }
	public double MinimumY { get; }
	public double MaximumY { get; }
	public double StepY { get; }
	#endregion

	#region Constructors
	public ChartViewport(Rect innerBounds, double minimumX, double maximumX, double stepX, double minimumY, double maximumY, double stepY)
	{
		InnerBounds = innerBounds;
		MinimumX = minimumX;
		MaximumX = maximumX;
		StepX = stepX;
		MinimumY = minimumY;
		MaximumY = maximumY;
		StepY = stepY;
	}
	#endregion
}