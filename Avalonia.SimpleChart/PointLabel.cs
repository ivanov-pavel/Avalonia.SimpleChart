//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

namespace Avalonia.SimpleChart;

public class PointLabel
{
	#region Fields
	private readonly string _text;
	private readonly int _offset;
	private readonly LabelStyle _style;
	private readonly LabelLocation _location;
	#endregion

	#region Properties
	public string Text => _text;

	public int Offset => _offset;

	public LabelStyle Style => _style;

	public LabelLocation Location => _location;
	#endregion

	#region Constructors
	public PointLabel()
	{
		_text = string.Empty;
		_offset = 4;
		_location = LabelLocation.TopCenter;
		_style = LabelStyle.Default;
	}

	public PointLabel(string text, int offset, LabelStyle style, LabelLocation location)
	{
		_text = text;
		_offset = offset;
		_style = style;
		_location = location;
	}
	#endregion
}