using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using YokoNotesTokaYurusenaiCounter.Configuration;

namespace YokoNotesTokaYurusenaiCounter.Views
{
    // MonoBehaviourは使えない
    public class SettingController
    {
        [UIValue("IsLabelEnable")]
        public bool IsLabelEnable
        {
            get => PluginConfig.Instance.IsLabelEnable;
            set
            {
                PluginConfig.Instance.IsLabelEnable = value;
            }
        }

        [UIValue("IsIconEnable")]
        public bool IsIconEnable
        {
            get => PluginConfig.Instance.IsIconEnable;
            set
            {
                PluginConfig.Instance.IsIconEnable = value;
            }
        }

        [UIValue("CounterType")]
        public string CounterType
        {
            get => PluginConfig.Instance.CounterType.ToString();
            set
            {
                SetCounterType(value);
            }
        }

        [UIValue("counter-type")]
        public List<object> counterType = Enum.GetNames(typeof(CounterTypeEnum)).ToList<object>();

        [UIValue("IsObstacleTimeEnable")]
        public bool IsObstacleTimeEnable
        {
            get => PluginConfig.Instance.IsObstacleTimeEnable;
            set
            {
                PluginConfig.Instance.IsObstacleTimeEnable = value;
            }
        }
        
        [UIValue("ObstacleTimeType")]
        public string ObstacleTime
        {
            get => PluginConfig.Instance.ObstacleTimeType.ToString();
            set
            {
                SetObstacleTimeType(value);
            }
        }

        [UIValue("obstacle-time-type")]
        public List<object> obstacleTimeType = Enum.GetNames(typeof(ObstacleTimeTypeEnum)).ToList<object>();

        [UIValue("ObstacleSecondPrecision")]
        public int ObstacleSecondPrecision
        {
            get => PluginConfig.Instance.ObstacleSecondPrecision;
            set
            {
                PluginConfig.Instance.ObstacleSecondPrecision = value;
            }
        }
        
        [UIValue("SeparateSaber")]
        public bool SeparateSaber
        {
            get => PluginConfig.Instance.SeparateSaber;
            set
            {
                PluginConfig.Instance.SeparateSaber = value;
            }
        }

        [UIValue("OffsetX")]
        public float OffsetX
        {
            get => PluginConfig.Instance.OffsetX;
            set
            {
                PluginConfig.Instance.OffsetX = value;
            }
        }

        [UIValue("OffsetY")]
        public float OffsetY
        {
            get => PluginConfig.Instance.OffsetY;
            set
            {
                PluginConfig.Instance.OffsetY = value;
            }
        }

        [UIValue("OffsetZ")]
        public float OffsetZ
        {
            get => PluginConfig.Instance.OffsetZ;
            set
            {
                PluginConfig.Instance.OffsetZ = value;
            }
        }

        private static void SetCounterType(string value)
        {
            if (Enum.TryParse(value, out CounterTypeEnum result))
            {
                PluginConfig.Instance.CounterType = result;
                return;
            }

            PluginConfig.Instance.CounterType = CounterTypeEnum.All;
        }

        private static void SetObstacleTimeType(string value)
        {
            if (Enum.TryParse(value, out ObstacleTimeTypeEnum result))
            {
                PluginConfig.Instance.ObstacleTimeType = result;
                return;
            }

            PluginConfig.Instance.ObstacleTimeType = ObstacleTimeTypeEnum.Second;
        }
    }
}
