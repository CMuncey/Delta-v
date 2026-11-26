using Content.Shared._DV.Janitor.Components;
using Content.Shared._DV.Janitor.Constants;
using Content.Shared._DV.Pager;
using Content.Shared.DeviceNetwork;
using Content.Server.DeviceNetwork.Systems;
using Content.Shared.DeviceNetwork.Events;
using Content.Shared.DeviceNetwork.Components;
using Robust.Shared.Timing;

namespace Content.Server._DV.Janitor.Systems;

public sealed class JanitorServiceRequestComputerSystem : EntitySystem
{
    [Dependency] private readonly DeviceNetworkSystem _deviceNetworkSystem = default!;
    [Dependency] private readonly PageSenderSystem _pageSender = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    public sealed partial class RequestEntry
    {
        public bool Request = false;
        public TimeSpan? LastAlert = null;
    }

    // This must be stored in the system so that all computers work with the same data
    private Dictionary<string, RequestEntry> Requests = new();

    // To avoid alert spam, wait two minutes between alerts
    private TimeSpan AlertTimeout = TimeSpan.FromSeconds( 120 );

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<JanitorServiceRequestComputerComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<JanitorServiceRequestComputerComponent, DeviceNetworkPacketEvent>(OnPacketRecv);
        SubscribeLocalEvent<JanitorServiceRequestComputerComponent, PageSenderNameEvent>(OnPagerGetName);
        return;
    }

    private void OnComponentStartup(EntityUid uid, JanitorServiceRequestComputerComponent comp, ComponentStartup args)
    {
        return;
    }

    private void OnPacketRecv(EntityUid uid, JanitorServiceRequestComputerComponent comp, DeviceNetworkPacketEvent args)
    {
        if (!args.Data.TryGetValue(DeviceNetworkConstants.Command, out JanitorServiceRequestConstants.Command? command))
            return;

        switch (command)
        {
            case JanitorServiceRequestConstants.Command.Link:
            {
                if (!args.Data.TryGetValue(JanitorServiceRequestConstants.Department, out string? department))
                    break;

                if (!Requests.ContainsKey(department))
                    Requests.Add(department, new RequestEntry());
                break;
            }

            case JanitorServiceRequestConstants.Command.Request:
            {
                if (!args.Data.TryGetValue(JanitorServiceRequestConstants.Department, out string? department))
                    break;

                if (!args.Data.TryGetValue(JanitorServiceRequestConstants.Request, out bool request))
                    break;
                
                // If we somehow don't know about this department, add it
                if (!Requests.ContainsKey(department))
                    Requests.Add(department, new RequestEntry());

                // If LastAlert is null, we've never had an alert so send one
                bool shouldAlert = true;

                TimeSpan debugTimeSpanThing = TimeSpan.Zero;

                // If CurTime - LastAlertTime is more than 2 minutes, send an alert
                if (Requests[department].LastAlert is { } lastAlert )
                {
                    shouldAlert = (_gameTiming.CurTime.Subtract(lastAlert) >= AlertTimeout);
                    debugTimeSpanThing = AlertTimeout.Subtract(_gameTiming.CurTime.Subtract(lastAlert));
                }

                Requests[department].Request = request;
                shouldAlert = true;
                if (shouldAlert && request == true)
                    SendPage(uid, department);
                break;
            }
        }

        return;
    }

    private void SendPage(EntityUid uid, string department)
    {
        string msg = String.Format( "Beep boop, The {0} department is requesting janitorial services.", department );
        _pageSender.Notify(uid, msg);
        Requests[department].LastAlert = _gameTiming.CurTime;
        return;
    }

    private void OnPagerGetName(Entity<JanitorServiceRequestComputerComponent> ent, ref PageSenderNameEvent args)
    {
        args.Name = ent.Comp.ComputerName;
    }
}
