using Exiled.API.Features;

namespace RainbowLights
{
    public class MainClass : Plugin<PluginConfig>
    {
        public override string Name { get; } = "RainbowLights";
        public override string Author { get; } = "Killers0992";
        public override string Prefix { get; } = "rainbowlights";

        public override void OnEnabled()
        {
            base.OnEnabled();
        }
    }
}
