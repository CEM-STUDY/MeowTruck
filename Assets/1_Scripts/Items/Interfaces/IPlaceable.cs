using MeowTruck.Data;

namespace MeowTruck.Items
{
	public interface IPlaceable
	{
		PlacementDefinition Placement { get; }
	}
}
