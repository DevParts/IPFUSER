using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(LocalizableTypeConverter))]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.FacetSR")]
[DisplayNameKey("ServerPerformanceName")]
[DisplayDescriptionKey("ServerPerformanceDesc")]
[CLSCompliant(false)]
public interface IServerPerformanceFacet : IDmfFacet
{
	[DisplayDescriptionKey("AffinityMaskDesc")]
	[DisplayNameKey("AffinityMaskName")]
	int AffinityMask { get; }

	[DisplayNameKey("Affinity64MaskName")]
	[DisplayDescriptionKey("Affinity64MaskDesc")]
	int Affinity64Mask { get; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	[DisplayNameKey("AffinityMaskIOName")]
	[DisplayDescriptionKey("AffinityMaskIODesc")]
	int AffinityIOMask { get; set; }

	[DisplayDescriptionKey("Affinity64IOMaskDesc")]
	[DisplayNameKey("Affinity64IOMaskName")]
	int Affinity64IOMask { get; }

	[DisplayNameKey("BlockedProcessThresholdName")]
	[DisplayDescriptionKey("ServerPerformanceDesc")]
	int BlockedProcessThreshold { get; set; }

	[DisplayDescriptionKey("DynamicLocksDesc")]
	[DisplayNameKey("DynamicLocksName")]
	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	int DynamicLocks { get; set; }

	[DisplayNameKey("LightweightPoolingEnabledName")]
	[DisplayDescriptionKey("LightweightPoolingEnabledDesc")]
	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	bool LightweightPoolingEnabled { get; set; }

	[DisplayDescriptionKey("ServerPerformanceDesc")]
	[DisplayNameKey("ServerPerformanceName")]
	int MaxDegreeOfParallelism { get; set; }

	[DisplayDescriptionKey("CostThresholdforParallelismDesc")]
	[DisplayNameKey("CostThresholdforParallelismName")]
	int CostThresholdForParallelism { get; set; }

	[DisplayNameKey("MaxWorkerThreadsName")]
	[DisplayDescriptionKey("MaxWorkerThreadsDesc")]
	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	int MaxWorkerThreads { get; set; }

	[DisplayNameKey("NetworkPacketSizeName")]
	[DisplayDescriptionKey("NetworkPacketSizeDesc")]
	int NetworkPacketSize { get; set; }

	[DisplayNameKey("OpenObjectsName")]
	[DisplayDescriptionKey("OpenObjectsDesc")]
	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	int OpenObjects { get; set; }
}
