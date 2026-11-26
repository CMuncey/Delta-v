using Content.Shared.DeviceLinking;
using Robust.Shared.Network;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._DV.Janitor.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class JanitorServiceLightComponent : Component
{
    [DataField(required: true)]
    public string Department = string.Empty;

    [DataField]
    public bool Request = false;

    [DataField]
    public ProtoId<SinkPortPrototype> OnPort = "On";

    [DataField]
    public ProtoId<SinkPortPrototype> OffPort = "Off";

    [DataField]
    public ProtoId<SinkPortPrototype> TogglePort = "Toggle";
}
