using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MapGeneration;
using RainbowLights.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RainbowLights.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class RainbowLightsCommand : ICommand
    {
        public string Command => "rainbowlights";

        public string[] Aliases => new string[0];

        public string Description => "Disable/enable/change rainbowlights";

        public static bool ChangeStateOfLights(Player p, ArraySegment<string> arguments, out string response, bool state)
        {
            foreach (var light in FlickerableLightController.Instances)
                if (!light.TryGetComponent<RainbowLightController>(out RainbowLightController _))
                    light.gameObject.AddComponent<RainbowLightController>();

            List<RainbowLightController> lights = RainbowLightController.Instances;

            if (arguments.Count == 2)
            {
                switch (arguments.At(1).ToLower())
                {
                    case "lcz":
                    case "lightzone":
                        lights = RainbowLightController.Instances.Where(pr => pr.room.Zone == FacilityZone.LightContainment).ToList();
                        break;
                    case "hcz":
                    case "heavyzone":
                        lights = RainbowLightController.Instances.Where(pr => pr.room.Zone == FacilityZone.HeavyContainment).ToList();
                        break;
                    case "ez":
                    case "entrance":
                    case "entrancezone":
                        lights = RainbowLightController.Instances.Where(pr => pr.room.Zone == FacilityZone.Entrance).ToList();
                        break;
                    case "surface":
                        lights = RainbowLightController.Instances.Where(pr => pr.room.Zone == FacilityZone.Surface).ToList();
                        break;
                    case "other":
                        lights = RainbowLightController.Instances.Where(pr => pr.room.Zone == FacilityZone.Other).ToList();
                        break;
                    case "current":
                        lights = RoomIdUtils.RoomAtPosition(p.Position).GetComponentsInChildren<RainbowLightController>().ToList();
                        break;
                    default:
                        response = $"Syntax: rainbowlights {(state ? "enable" : "disable")} (hcz/lcz/ez/surface/current)";
                        return false;
                }
            }


            foreach (var rainbowLight in lights)
            {
                rainbowLight.SetState(state);
            }
            response = $"Rainbow light {(state ? "enabled" : "disabled")}.";
            return true;
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get((CommandSender)sender);
            if (!p.CheckPermission("rainbowlights"))
            {
                response = "No permission. ( missing permission: rainbowlights )";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "Syntax: rainbowlights enable/disable/settings";
                return false;
            }

            switch (arguments.At(0).ToLower())
            {
                case "true":
                case "on":
                case "enable":
                    return ChangeStateOfLights(p, arguments, out response, true);
                case "false":
                case "off":
                case "disable":
                    return ChangeStateOfLights(p, arguments, out response, false);
                case "settings":
                    if (arguments.Count == 4 || arguments.Count == 6)
                    {
                        if (float.TryParse(arguments.At(1), out float saturation))
                        {
                            if (float.TryParse(arguments.At(2), out float hueshiftspeed))
                            {
                                if (float.TryParse(arguments.At(3), out float value))
                                {
                                    List<RainbowLightController> lights = RainbowLightController.Instances;

                                    if (arguments.Count == 6)
                                    {
                                        switch (arguments.At(1).ToLower())
                                        {
                                            case "lcz":
                                            case "lightzone":
                                                lights = RainbowLightController.Instances.Where(pr => pr.room.Zone == FacilityZone.LightContainment).ToList();
                                                break;
                                            case "hcz":
                                            case "heavyzone":
                                                lights = RainbowLightController.Instances.Where(pr => pr.room.Zone == FacilityZone.HeavyContainment).ToList();
                                                break;
                                            case "ez":
                                            case "entrance":
                                            case "entrancezone":
                                                lights = RainbowLightController.Instances.Where(pr => pr.room.Zone == FacilityZone.Entrance).ToList();
                                                break;
                                            case "surface":
                                                lights = RainbowLightController.Instances.Where(pr => pr.room.Zone == FacilityZone.Surface).ToList();
                                                break;
                                            case "other":
                                                lights = RainbowLightController.Instances.Where(pr => pr.room.Zone == FacilityZone.Other).ToList();
                                                break;
                                            case "current":
                                                lights.AddRange(RoomIdUtils.RoomAtPosition(p.Position).GetComponentsInChildren<RainbowLightController>().ToList());
                                                break;
                                            default:
                                                response = $"Syntax: rainbowlights settings <saturation> <hueShiftSpeed> <value> (hcz/lcz/ez/surface/current)";
                                                return false;
                                        }
                                    }

                                    foreach (var rainbowLight in lights)
                                    {
                                        rainbowLight._saturation = saturation;
                                        rainbowLight._hueShiftSpeed = hueshiftspeed;
                                        rainbowLight._value = value;
                                    }
                                    response = $"Changed all rainbow lights settings to, saturation ({saturation}), HueShift Speed ({hueshiftspeed}), Value ({value})";
                                    return true;
                                }
                            }
                        }
                    }
                    response = "Syntax: rainbowlights settings <saturation> <hueShiftSpeed> <value>";
                    return false;
                default:
                    response = "Syntax: rainbowlights enable/disable/settings";
                    return false;
            }
        }
    }
}
