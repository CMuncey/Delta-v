using Content.Shared._DV.Light.EntitySystems;
using Content.Shared._DV.Light.JanitorServiceLight;
using Content.Shared.DeviceLinking.Events;
using Content.Server.Pinpointer;

namespace Content.Server._DV.Light.EntitySystems;

public sealed class JanitorServiceLightSystem : SharedJanitorServiceLightSystem
{
    [Dependency] private readonly NavMapSystem _navMap = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<JanitorServiceLightComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<JanitorServiceLightComponent, SignalReceivedEvent>(OnSignalReceived);
        return;
    }

    private void OnComponentStartup(EntityUid uid, JanitorServiceLightComponent comp, ComponentStartup args)
    {
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
        if (ent.Comp.Request == request)
            return;

        ent.Comp.Request = request;
        Log.Info( "Set request to {$req} for light near {$loc}.", ent.Comp.Request, _navMap.GetNearestBeaconString( ent.Owner ) );
        return;
    }
}
