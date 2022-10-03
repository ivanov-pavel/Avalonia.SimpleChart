//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

namespace Avalonia.SimpleChart;

public enum LabelLocation
{
	LeftCenter,
	RightCenter,
	TopCenter,
	BottomCenter,
	LeftTop,
	LeftBottom,
	RightTop,
	RightBottom,
	TopLeft = LeftTop,
	TopRight = RightTop,
	BottomLeft = LeftBottom,
	BottomRight = RightBottom
}