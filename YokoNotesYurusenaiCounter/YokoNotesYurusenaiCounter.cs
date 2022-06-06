using CountersPlus.Counters.Custom;
using CountersPlus.Counters.Interfaces;
using TMPro;
using UnityEngine;
using YokoNotesYurusenaiCounter.Configuration;
using YokoNotesYurusenaiCounter.Interfaces;

namespace YokoNotesYurusenaiCounter
{
    public class YokoNotesYurusenaiCounter : BasicCustomCounter, INoteEventHandler
    {
        private readonly Vector3 labelOffset;

        private TMP_Text counter;

        private readonly int defaultNoteCount = 0;
        private readonly int defaultBombCount = 0;

        private readonly IYurusenai defaultYurusenaiNoteMiss;
        private readonly IYurusenai defaultYurusenaiBomb;

        private IYurusenai updatedYurusenaiNoteMiss;
        private IYurusenai updatedYurusenaiBomb;
        
        public YokoNotesYurusenaiCounter()
        {
            labelOffset = new Vector3(
                PluginConfig.Instance.OffsetX, PluginConfig.Instance.OffsetY, PluginConfig.Instance.OffsetZ
                );

            defaultYurusenaiNoteMiss = new YurusenaiNoteMiss(defaultNoteCount, defaultNoteCount, defaultNoteCount);
            defaultYurusenaiBomb = new YurusenaiBomb(defaultBombCount, defaultBombCount, defaultBombCount);
        }
        
        public override void CounterInit()
        {
            updatedYurusenaiNoteMiss = defaultYurusenaiNoteMiss;
            updatedYurusenaiBomb = defaultYurusenaiBomb;
            
            CreateLabel();
            CreateCounter();
        }
        

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            if (IsNoteBomb(data))
            {
                if (PluginConfig.Instance.CounterType == CounterTypeEnum.YokoNotesOnly) return;
                UpdateYurusenai(ref updatedYurusenaiBomb,info);

                UpdateText();

                return;
            }

            if (PluginConfig.Instance.CounterType == CounterTypeEnum.BombsOnly) return;
            
            if (!IsNoteYoko(data)) return;

            if (info.allIsOK) return;

            UpdateYurusenai(ref updatedYurusenaiNoteMiss, info);

            UpdateText();
        }

        public void OnNoteMiss(NoteData data)
        {
            if (!IsNoteYoko(data)) return;

            UpdateYurusenai(ref updatedYurusenaiNoteMiss, data);

            UpdateText();
        }

        public override void CounterDestroy() { }

        private void UpdateYurusenai(ref IYurusenai yurusenai, NoteData data)
        {
            yurusenai = yurusenai.UpdateBothHand();

            if (data.colorType==ColorType.ColorA)
            {
                yurusenai = yurusenai.UpdateLeftHand();
            }
            else if(data.colorType==ColorType.ColorB)
            {
                yurusenai = yurusenai.UpdateRightHand();
            }
        }

        private void UpdateYurusenai(ref IYurusenai yurusenai,NoteCutInfo info)
        {
            yurusenai = yurusenai.UpdateBothHand();

            if (info.saberType == SaberType.SaberA)
            {
                yurusenai = yurusenai.UpdateLeftHand();
                return;
            }

            yurusenai = yurusenai.UpdateRightHand();
        }

        private void CreateLabel()
        {
            if (!PluginConfig.Instance.IsLabelEnable) return;

            TMP_Text label = CanvasUtility.CreateTextFromSettings(Settings, labelOffset);
            label.text = PluginConfig.Instance.LabelName;
            label.fontSize = 3f;
        }

        private void CreateCounter()
        {
            TextAlignmentOptions counterAlign = TextAlignmentOptions.Top;

            counter = CanvasUtility.CreateTextFromSettings(Settings, labelOffset - Vector3.up * 0.2f);
            counter.lineSpacing = -26;
            counter.fontSize = 4f;
            counter.text = MakeCountText();
            counter.alignment = counterAlign;
        }

        private void UpdateText()=> counter.text = MakeCountText();

        private string MakeCountText()
        {
            switch (PluginConfig.Instance.CounterType)
            {
                case CounterTypeEnum.Both:
                    return $"{YurusenaiText(updatedYurusenaiNoteMiss, PluginConfig.Instance.YokoNoteMissIcon)}" +
                        $"\n{YurusenaiText(updatedYurusenaiBomb, PluginConfig.Instance.BombSlashIcon)}";
                case CounterTypeEnum.YokoNotesOnly:
                    return YurusenaiText(updatedYurusenaiNoteMiss, PluginConfig.Instance.YokoNoteMissIcon);
                default:
                    return YurusenaiText(updatedYurusenaiBomb, PluginConfig.Instance.BombSlashIcon);
            }
        }

        private string YurusenaiText(IYurusenai yurusenai,string icon)
        {
            if (PluginConfig.Instance.IsIconEnable)
            {
                if (PluginConfig.Instance.SeparateSaber)
                {
                    return $"{icon}  {yurusenai.LeftCount()}  {yurusenai.RightCount()}";
                }

                return $"{icon} {yurusenai.BothCount()}";
            }

            if (PluginConfig.Instance.SeparateSaber)
            {
                return $"{yurusenai.LeftCount()}  {yurusenai.RightCount()}";
            }

            return $"{yurusenai.BothCount()}";
        }

        private static bool IsNoteYoko(NoteData data)
        {
            return data.cutDirection == NoteCutDirection.Left || data.cutDirection == NoteCutDirection.Right;
        }

        private static bool IsNoteBomb(NoteData data)
        {
            return data.gameplayType == NoteData.GameplayType.Bomb;
        }
    }
}
