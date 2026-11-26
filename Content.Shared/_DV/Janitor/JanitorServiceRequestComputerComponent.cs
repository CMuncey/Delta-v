using Robust.Shared.Network;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._DV.Janitor.Components;

[RegisterComponent]
public sealed partial class JanitorServiceRequestComputerComponent : Component
{
    [DataField]
    public string ComputerName = "Janitor Service Request Computer";
}
