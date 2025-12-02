namespace Microsoft.SqlServer.Management.Smo;

public enum XmlTypeKind
{
	Any = 1,
	AnySimple,
	Primitive,
	Simple,
	List,
	Union,
	ComplexSimple,
	Complex,
	Element,
	ModelGroup,
	ElementWildcard,
	Attribute,
	AttributeGroup,
	AttributeWildcard
}
