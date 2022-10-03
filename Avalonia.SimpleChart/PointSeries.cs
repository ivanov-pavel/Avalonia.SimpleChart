//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

namespace Avalonia.SimpleChart;

public class PointSeries : ChartSeries
{
	#region Properties
	public override SeriesType SeriesType => SeriesType.Points;
	#endregion

	#region Methods
	public override void RenderSeries(ChartRenderer chartRenderer, ChartViewport chartViewport, ChartAxis axisX, ChartAxis axisY)
	{
		RenderPoints(chartRenderer, chartViewport, axisX, axisY);
	}
	#endregion
}