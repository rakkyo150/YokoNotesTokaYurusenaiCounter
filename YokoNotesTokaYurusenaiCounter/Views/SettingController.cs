using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [UIValue("type")]
        public List<object> type = Enum.GetNames(typeof(CounterTypeEnum)).ToList<object>();

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

            PluginConfig.Instance.CounterType = CounterTypeEnum.Both;
        }
    }
}
