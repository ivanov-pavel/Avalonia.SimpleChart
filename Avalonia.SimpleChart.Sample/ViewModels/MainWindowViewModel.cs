//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.SimpleChart.Sample.ViewModels;

[ObservableObject]
public partial class MainWindowViewModel
{
	#region Fields
	private LineChartViewModel _lineChartViewModel;
	#endregion

	#region Properties
	public LineChartViewModel LineChartViewModel => _lineChartViewModel;
	#endregion

	#region Constructors
	public MainWindowViewModel()
	{
		_lineChartViewModel = new LineChartViewModel();
	}
	#endregion
}