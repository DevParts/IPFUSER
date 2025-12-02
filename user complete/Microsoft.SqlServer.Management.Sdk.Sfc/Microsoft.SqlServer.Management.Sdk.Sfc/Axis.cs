namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class Axis : AstNode
{
	internal enum AxisType
	{
		Ancestor,
		AncestorOrSelf,
		Attribute,
		Child,
		Descendant,
		DescendantOrSelf,
		Following,
		FollowingSibling,
		Namespace,
		Parent,
		Preceding,
		PrecedingSibling,
		Self,
		None
	}

	private static readonly string[] str = new string[13]
	{
		"Ancestor", "AncestorOrSelf", "Attribute", "Child", "Descendant", "DescendantOrSelf", "Following", "FollowingSibling", "Namespace", "Parent",
		"Preceding", "PrecedingSibling", "Self"
	};

	private AxisType _axistype;

	private AstNode _input;

	private string _urn;

	private string _prefix;

	private string _name;

	private XPathNodeType _nodetype;

	internal override QueryType TypeOfAst => QueryType.Axis;

	internal override RType ReturnType => RType.NodeSet;

	internal AstNode Input => _input;

	internal string Name => _name;

	internal AxisType TypeOfAxis => _axistype;

	internal override double DefaultPriority
	{
		get
		{
			if (_input != null)
			{
				return 0.5;
			}
			if (_axistype == AxisType.Child || _axistype == AxisType.Attribute)
			{
				if (_name != null && _name.Length != 0)
				{
					return 0.0;
				}
				if (_prefix != null && _prefix.Length != 0)
				{
					return -0.25;
				}
				return -0.5;
			}
			return 0.5;
		}
	}

	internal Axis(AxisType axistype, AstNode input, string urn, string prefix, string name, XPathNodeType nodetype)
	{
		_axistype = axistype;
		_input = input;
		_urn = urn;
		_prefix = prefix;
		_name = name;
		_nodetype = nodetype;
	}

	internal Axis(AxisType axistype, AstNode input)
	{
		_axistype = axistype;
		_input = input;
		_urn = string.Empty;
		_prefix = string.Empty;
		_name = string.Empty;
		_nodetype = XPathNodeType.All;
	}
}
