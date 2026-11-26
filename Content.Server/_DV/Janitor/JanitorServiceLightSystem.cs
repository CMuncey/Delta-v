using Content.Shared._DV.Janitor.Components;
using Content.Shared._DV.Janitor.Constants;
using Content.Shared.DeviceLinking.Events;
using Content.Shared.DeviceNetwork;
using Content.Server.DeviceNetwork.Systems;
using Content.Shared.DeviceNetwork.Events;
using Content.Shared.DeviceNetwork.Components;

namespace Content.Server._DV.Janitor.Systems;

public sealed class JanitorServiceLightSystem : EntitySystem
{
    [Dependency] private readonly DeviceNetworkSystem _deviceNetworkSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<JanitorServiceLightComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<JanitorServiceLightComponent, SignalReceivedEvent>(OnSignalReceived);
        return;
    }

    private void OnComponentStartup(EntityUid uid, JanitorServiceLightComponent comp, ComponentStartup args)
    {
        if (!TryComp<DeviceNetworkComponent>(uid, out var netComp))
            return;

        // Send out a link command when the component is created so that any computers know we exist now
        var payload = new NetworkPayload
        {
            [DeviceNetworkConstants.Command] = JanitorServiceRequestConstants.Command.Link,
            [JanitorServiceRequestConstants.Department] = comp.Department,
        };

        _deviceNetworkSystem.QueuePacket(uid, null, payload, netComp.TransmitFrequency);
        return;
    }


    private void OnSignalReceived(Entity<JanitorServiceLightComponent> ent, ref SignalReceivedEvent args)
    {
        if (args.Port == ent.Comp.OffPort)
            SetRequestState(ent, false);
        else if (args.Port == ent.Comp.OnPort)
            SetRequestState(ent, true);
        else if (args.Port == ent.Comp.TogglePort)
            SetRequestState(ent, !ent.Comp.Request);
        return;
    }

    private void SetRequestState(Entity<JanitorServiceLightComponent> ent, bool request)
    {
        ent.Comp.Request = request;
        SendRequestMessage(ent);
        return;
    }

    private void SendRequestMessage(Entity<JanitorServiceLightComponent> ent)
    {
        if (!TryComp<DeviceNetworkComponent>(ent.Owner, out var netComp))
            return;

        var payload = new NetworkPayload
        {
            [DeviceNetworkConstants.Command] = JanitorServiceRequestConstants.Command.Request,
            [JanitorServiceRequestConstants.Department] = ent.Comp.Department,
            [JanitorServiceRequestConstants.Request] = ent.Comp.Request
        };

        _deviceNetworkSystem.QueuePacket(ent.Owner, null, payload, netComp.TransmitFrequency);
        return;
    }
}
