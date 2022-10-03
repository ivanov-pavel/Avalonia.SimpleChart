//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using Avalonia.Media;

namespace Avalonia.SimpleChart;

public class LabelStyle
{
	#region Fields
	private readonly IBrush _foreground;
	private readonly double _fontSize;
	private readonly FontFamily _fontFamily;
	private readonly FontStyle _fontStyle;
	private readonly FontWeight _fontWeight;

	public static readonly LabelStyle Default = new(Brushes.Black, 10, FontFamily.Default, FontStyle.Normal, FontWeight.Normal);
	#endregion

	#region Properties
	public IBrush Foreground => _foreground;
	public double FontSize => _fontSize;
	public FontFamily FontFamily => _fontFamily;
	public FontStyle FontStyle => _fontStyle;
	public FontWeight FontWeight => _fontWeight;
	#endregion

	#region Constructor
	public LabelStyle(IBrush foreground, double fontSize, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight)
	{
		_foreground = foreground;
		_fontSize = fontSize;
		_fontFamily = fontFamily;
		_fontStyle = fontStyle;
		_fontWeight = fontWeight;
	}
	#endregion
}