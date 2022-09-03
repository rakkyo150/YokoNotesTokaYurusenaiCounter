using CountersPlus.Counters.Custom;
using CountersPlus.Counters.Interfaces;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using YokoNotesTokaYurusenaiCounter.Configuration;
using YokoNotesTokaYurusenaiCounter.Interfaces;
using YokoNotesTokaYurusenaiCounter.UI;
using Zenject;

namespace YokoNotesTokaYurusenaiCounter
{
    public class YokoNotesTokaYurusenaiCounter : BasicCustomCounter, INoteEventHandler
    {
        private ImageViewSetter _imageViewSetter;
        private readonly DiContainer _container;
        
        private readonly Vector3 labelOffset;

        private TMP_Text counter;

        private readonly int defaultNoteCount = 0;
        private readonly int defaultBombCount = 0;

        private readonly IYurusenai InitializedYurusenaiNoteMiss;
        private readonly IYurusenai InitializedYurusenaiBombSlash;

        private IYurusenai updatedYurusenaiNoteMiss;
        private IYurusenai updatedYurusenaiBombSlash;

        private readonly int initialNumberOfDigitForNoteImage;
        private int updatedNumberOfDigitForNoteImage;

        public YokoNotesTokaYurusenaiCounter(DiContainer diContainer)
        {
            _container = diContainer;
            
            labelOffset = new Vector3(
                PluginConfig.Instance.OffsetX, PluginConfig.Instance.OffsetY, PluginConfig.Instance.OffsetZ
                );

            InitializedYurusenaiNoteMiss = new YurusenaiNoteMiss(defaultNoteCount, defaultNoteCount, defaultNoteCount);
            InitializedYurusenaiBombSlash = new YurusenaiBombSlash(defaultBombCount, defaultBombCount, defaultBombCount);

            if (PluginConfig.Instance.SeparateSaber)
            {
                initialNumberOfDigitForNoteImage = 2;
            }
            else
            {
                initialNumberOfDigitForNoteImage = 1;
            }
        }

        public override void CounterInit()
        {
            updatedYurusenaiNoteMiss = InitializedYurusenaiNoteMiss;
            updatedYurusenaiBombSlash = InitializedYurusenaiBombSlash;

            updatedNumberOfDigitForNoteImage = initialNumberOfDigitForNoteImage;

            if (!IsYokoNoteIconDisabled())
            {
                // MonoBehaviourを継承しているのでInstantiateする必要がある
                _imageViewSetter = _container.InstantiateComponentOnNewGameObject<ImageViewSetter>("yokoNoteImageView");
                _imageViewSetter?.Initialize();
            }

            CreateLabel();
            CreateCounter();
        }

        private static bool IsYokoNoteIconDisabled()
        {
            return !PluginConfig.Instance.IsIconEnable || PluginConfig.Instance.CounterType == CounterTypeEnum.BombsOnly;
        }

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            if (IsNoteBomb(data))
            {
                if (PluginConfig.Instance.CounterType == CounterTypeEnum.YokoNotesOnly) return;
                UpdateYurusenai(ref updatedYurusenaiBombSlash, info);

                UpdateText();

                return;
            }

            if (PluginConfig.Instance.CounterType == CounterTypeEnum.BombsOnly) return;

            if (!IsNoteYoko(data)) return;

            if (info.allIsOK) return;

            UpdateYurusenai(ref updatedYurusenaiNoteMiss, info);

            UpdateText();

            if (IsYokoNoteIconDisabled()) return;           
            ConfirmNumberOfDigit();
        }

        public void OnNoteMiss(NoteData data)
        {
            if (!IsNoteYoko(data)) return;

            UpdateYurusenai(ref updatedYurusenaiNoteMiss, data);

            UpdateText();

            if(IsYokoNoteIconDisabled()) return;          
            ConfirmNumberOfDigit();
        }

        private void ConfirmNumberOfDigit()
        {
            int newNumberOfDigit;
            int numberOfMove;
            
            if (PluginConfig.Instance.SeparateSaber)
            {
                newNumberOfDigit = (int)Math.Log10(updatedYurusenaiNoteMiss.LeftCount()) + (int)Math.Log10(updatedYurusenaiNoteMiss.RightCount()) + 2;
            }
            else
            {
                newNumberOfDigit = (int)Math.Log10(updatedYurusenaiNoteMiss.BothCount()) + 1;
            }

            if (updatedNumberOfDigitForNoteImage >= newNumberOfDigit) return;

            numberOfMove = newNumberOfDigit - updatedNumberOfDigitForNoteImage;
            
            foreach (var _ in Enumerable.Range(1, numberOfMove))
            {
                _imageViewSetter.MoveLeft();
            }
            
            updatedNumberOfDigitForNoteImage = newNumberOfDigit;
        }

        public override void CounterDestroy() { }

        private void UpdateYurusenai(ref IYurusenai yurusenai, NoteData data)
        {
            yurusenai = yurusenai.UpdateBothHand();

            if (data.colorType == ColorType.ColorA)
            {
                yurusenai = yurusenai.UpdateLeftHand();
            }
            else if (data.colorType == ColorType.ColorB)
            {
                yurusenai = yurusenai.UpdateRightHand();
            }
        }

        private void UpdateYurusenai(ref IYurusenai yurusenai, NoteCutInfo info)
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

            if (IsYokoNoteIconDisabled()) return;
            _imageViewSetter?.SetTMPTransform(counter.transform);
        }

        private void UpdateText() => counter.text = MakeCountText();

        private string MakeCountText()
        {
            switch (PluginConfig.Instance.CounterType)
            {
                case CounterTypeEnum.Both:
                    return $"{YurusenaiText(updatedYurusenaiNoteMiss, "      ")}" +
                        $"\n{YurusenaiText(updatedYurusenaiBombSlash, PluginConfig.Instance.BombSlashIcon)}";
                case CounterTypeEnum.YokoNotesOnly:
                    return YurusenaiText(updatedYurusenaiNoteMiss, "      ");
                default:
                    return YurusenaiText(updatedYurusenaiBombSlash, PluginConfig.Instance.BombSlashIcon);
            }
        }

        private string YurusenaiText(IYurusenai yurusenai, string icon)
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
