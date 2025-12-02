using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace NLog.Config;

/// <summary>
/// Attribute used to mark the required parameters for targets,
/// layout targets and filters.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
[MeansImplicitUse]
[Obsolete("Instead perform relevant config validation in InitializeTarget / InitializeLayout. Marked obsolete with NLog v6.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class RequiredParameterAttribute : Attribute
{
}
