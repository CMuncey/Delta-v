using Content.Shared.DeviceLinking;
using Robust.Shared.Network;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._DV.Light.JanitorServiceLight;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class JanitorServiceLightComponent : Component
{
    [DataField, Required(true)]
    public string Department = string.Empty;

    [DataField, AutoNetworkedField]
    public bool Request = false;

    [DataField]
    public ProtoId<SinkPortPrototype> OnPort = "On";

    [DataField]
    public ProtoId<SinkPortPrototype> OffPort = "Off";

    [DataField]
    public ProtoId<SinkPortPrototype> TogglePort = "Toggle";
}

[Serializable]
public sealed class JanitorServiceLightRequestEvent : EventArgs;
