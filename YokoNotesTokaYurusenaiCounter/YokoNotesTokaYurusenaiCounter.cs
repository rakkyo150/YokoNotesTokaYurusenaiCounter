using BeatSaberMarkupLanguage;
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
        private readonly DiContainer _container;
        private readonly ImageViewSetter _imageViewSetter;
        private readonly ConfirmorOfNumberOfDigit _confirmorOfNumberOfDigit;

        private Vector3 labelOffset;

        private TMP_Text counter;

        private int defaultNoteCount = 0;
        private int defaultBombCount = 0;
        private int defaultObstacleCount = 0;

        private IYurusenai yurusenaiNoteMiss;
        private IYurusenai yurusenaiBombSlash;
        private YurusenaiObstacle yurusenaiObstacle;

        private int initialNumberOfDigitForNoteImage;
        private int updatedNumberOfDigitForNoteImage;

        private int initialNumberOfDigitForSquatImage;
        private int updatedNumberOfDigitForSquatImage;

        private ObstacleCounterUpdater obstacleDamageCount;

        public YokoNotesTokaYurusenaiCounter(DiContainer diContainer,ImageViewSetter imageViewSetter, ConfirmorOfNumberOfDigit confirmorOfNumberOfDigit)
        {
            _container = diContainer;
            _imageViewSetter = imageViewSetter;
            _confirmorOfNumberOfDigit = confirmorOfNumberOfDigit;
        }

        public override void CounterInit()
        {
            labelOffset = new Vector3(
                PluginConfig.Instance.OffsetX, PluginConfig.Instance.OffsetY, PluginConfig.Instance.OffsetZ
                );
            
            (initialNumberOfDigitForNoteImage, initialNumberOfDigitForSquatImage) 
                = _confirmorOfNumberOfDigit.GetInitialNumbersOfDigit();

            yurusenaiNoteMiss = new YurusenaiNoteMiss(defaultNoteCount, defaultNoteCount, defaultNoteCount); ;
            yurusenaiBombSlash = new YurusenaiBombSlash(defaultBombCount, defaultBombCount, defaultBombCount);
            yurusenaiObstacle = new YurusenaiObstacle(defaultObstacleCount, defaultObstacleCount, defaultObstacleCount);

            updatedNumberOfDigitForNoteImage = initialNumberOfDigitForNoteImage;
            updatedNumberOfDigitForSquatImage = initialNumberOfDigitForSquatImage;

            CreateLabel();
            CreateCounter();

            // MonoBehaviourを継承しているのでInstantiateする必要がある
            // Start Obstacle Damage Tracking
            obstacleDamageCount = _container.InstantiateComponentOnNewGameObject<ObstacleCounterUpdater>("obstacleDamageCount");
        }

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            if (IsNoteBomb(data))
            {
                if (PluginConfig.Instance.CounterType == CounterTypeEnum.YokoNotesOnly) return;
                UpdateYurusenai(ref yurusenaiBombSlash, info);

                UpdateText();

                return;
            }

            if (PluginConfig.Instance.CounterType == CounterTypeEnum.BombsOnly) return;

            if (!IsNoteYoko(data)) return;

            if (info.allIsOK) return;

            UpdateYurusenai(ref yurusenaiNoteMiss, info);

            UpdateText();

            if (PluginConfig.Instance.IsYokoNoteIconDisabled()) return;

            updatedNumberOfDigitForNoteImage = _confirmorOfNumberOfDigit.ConfirmNumberOfDigitForYokoNote(
                yurusenaiNoteMiss,initialNumberOfDigitForNoteImage,updatedNumberOfDigitForNoteImage);
        }

        public void OnNoteMiss(NoteData data)
        {
            if (!IsNoteYoko(data)) return;

            UpdateYurusenai(ref yurusenaiNoteMiss, data);

            UpdateText();

            if (PluginConfig.Instance.IsYokoNoteIconDisabled()) return;

            updatedNumberOfDigitForNoteImage = _confirmorOfNumberOfDigit.ConfirmNumberOfDigitForYokoNote(
                yurusenaiNoteMiss, initialNumberOfDigitForNoteImage, updatedNumberOfDigitForNoteImage);
        }

        public override void CounterDestroy(){ }

        private void UpdateYurusenai(ref IYurusenai yurusenai, NoteData data)
        {
            yurusenai = yurusenai.UpdateBoth();

            if (data.colorType == ColorType.ColorA)
            {
                yurusenai = yurusenai.UpdateLeft();
            }
            else if (data.colorType == ColorType.ColorB)
            {
                yurusenai = yurusenai.UpdateRight();
            }
        }

        private void UpdateYurusenai(ref IYurusenai yurusenai, NoteCutInfo info)
        {
            yurusenai = yurusenai.UpdateBoth();

            if (info.saberType == SaberType.SaberA)
            {
                yurusenai = yurusenai.UpdateLeft();
                return;
            }

            yurusenai = yurusenai.UpdateRight();
        }

        internal void AddObstacleDamageCount()
        {
            yurusenaiObstacle = (YurusenaiObstacle)yurusenaiObstacle.UpdateLeft();
            UpdateText();
            updatedNumberOfDigitForSquatImage = _confirmorOfNumberOfDigit.ConfirmNumberOfDigitForSquat(
                yurusenaiObstacle,initialNumberOfDigitForSquatImage,updatedNumberOfDigitForSquatImage);
        }

        internal void AddObstacleDamageDuration(float time)
        {
            yurusenaiObstacle = (YurusenaiObstacle)yurusenaiObstacle.UpdateRight(time);
            UpdateText();
            updatedNumberOfDigitForSquatImage = _confirmorOfNumberOfDigit.ConfirmNumberOfDigitForSquat(
                yurusenaiObstacle, initialNumberOfDigitForSquatImage, updatedNumberOfDigitForSquatImage);
        }

        internal void AddObstacleDamageDuration()
        {
            yurusenaiObstacle = (YurusenaiObstacle)yurusenaiObstacle.UpdateRight();
            UpdateText();
            updatedNumberOfDigitForSquatImage = _confirmorOfNumberOfDigit.ConfirmNumberOfDigitForSquat(
                yurusenaiObstacle, initialNumberOfDigitForSquatImage, updatedNumberOfDigitForSquatImage);
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
            
            _imageViewSetter.CreateIcons(counter);
        }

        private void UpdateText() => counter.text = MakeCountText();

        private string MakeCountText()
        {
            switch (PluginConfig.Instance.CounterType)
            {
                case CounterTypeEnum.All:
                    return $"{YurusenaiText(yurusenaiNoteMiss, "      ")}" +
                        $"\n{YurusenaiText(yurusenaiBombSlash, PluginConfig.Instance.BombSlashIcon)}" +
                        $"\n{YurusenaiText(yurusenaiObstacle, "      ")}";
                case CounterTypeEnum.YokoNotesAndBombs:
                    return $"{YurusenaiText(yurusenaiNoteMiss, "      ")}" +
                        $"\n{YurusenaiText(yurusenaiBombSlash, PluginConfig.Instance.BombSlashIcon)}";
                case CounterTypeEnum.YokoNotesAndObstacles:
                    return $"{YurusenaiText(yurusenaiNoteMiss, "      ")}" +
                        $"\n{YurusenaiText(yurusenaiObstacle, "      ")}";
                case CounterTypeEnum.BombsAndObstacles:
                    return $"\n{YurusenaiText(yurusenaiBombSlash, PluginConfig.Instance.BombSlashIcon)}" +
                        $"\n{YurusenaiText(yurusenaiObstacle, "      ")}";
                case CounterTypeEnum.YokoNotesOnly:
                    return YurusenaiText(yurusenaiNoteMiss, "      ");
                case CounterTypeEnum.BombsOnly:
                    return YurusenaiText(yurusenaiBombSlash, PluginConfig.Instance.BombSlashIcon);
                default:
                    return YurusenaiText(yurusenaiObstacle, "      ");
            }
        }

        private string YurusenaiText(IYurusenai yurusenai, string icon)
        {
            if (PluginConfig.Instance.IsIconEnable)
            {
                if (yurusenai.GetType() != typeof(YurusenaiObstacle))
                {
                    if (PluginConfig.Instance.SeparateSaber)
                    {
                        return $"{icon}  {yurusenai.LeftCount()}  {yurusenai.RightCount()}";
                    }

                    return $"{icon} {yurusenai.BothCount()}";
                }

                if (PluginConfig.Instance.IsObstacleTimeEnable)
                {
                    return $"{icon}  {yurusenai.LeftCount()}  {yurusenai.RightCount()}";
                }

                return $"{icon}  {yurusenai.LeftCount()}";
            }

            if (yurusenai.GetType() != typeof(YurusenaiObstacle))
            {
                if (PluginConfig.Instance.SeparateSaber)
                {
                    return $"{yurusenai.LeftCount()}  {yurusenai.RightCount()}";
                }

                return $"{yurusenai.BothCount()}";
            }

            if (PluginConfig.Instance.IsObstacleTimeEnable)
            {
                return $"{yurusenai.LeftCount()}  {yurusenai.RightCount()}";
            }

            return $"{yurusenai.LeftCount()}";
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
