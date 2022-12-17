using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace YokoNotesTokaYurusenaiCounter.Configuration
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual bool IsLabelEnable { get; set; } = true;
        public virtual string LabelName { get; set; } = "Yurusenai Counter";
        public virtual bool IsIconEnable { get; set; } = true;
        public virtual string BombSlashIcon { get; set; } = "💣";
        public virtual CounterTypeEnum CounterType { get; set; } = CounterTypeEnum.Both;
        public virtual ObstacleTimeTypeEnum ObstacleTimeType { get; set; }=ObstacleTimeTypeEnum.Second;
        public virtual int ObstacleSecondPrecision { get; set; } = 2;
        public virtual bool SeparateSaber { get; set; } = true;
        public virtual float OffsetX { get; set; } = 0; // Must be 'virtual' if you want BSIPA to detect a value change and save the config automatically.
        public virtual float OffsetY { get; set; } = 0;
        public virtual float OffsetZ { get; set; } = 0;
    }
}