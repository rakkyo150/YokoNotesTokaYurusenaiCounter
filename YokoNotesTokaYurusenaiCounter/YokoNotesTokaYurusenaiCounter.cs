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
        private ImageViewSetter _imageViewSetter;
        private readonly DiContainer _container;

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

        public YokoNotesTokaYurusenaiCounter(DiContainer diContainer)
        {
            _container = diContainer;
            // MonoBehaviourを継承しているのでInstantiateする必要がある
            _imageViewSetter = _container.InstantiateComponentOnNewGameObject<ImageViewSetter>("yokoNoteTokaImageView");
        }

        public override void CounterInit()
        {
            labelOffset = new Vector3(
                PluginConfig.Instance.OffsetX, PluginConfig.Instance.OffsetY, PluginConfig.Instance.OffsetZ
                );
            
            (initialNumberOfDigitForNoteImage, initialNumberOfDigitForSquatImage) = GetInitialNumbersOfDigit();

            yurusenaiNoteMiss = new YurusenaiNoteMiss(defaultNoteCount, defaultNoteCount, defaultNoteCount); ;
            yurusenaiBombSlash = new YurusenaiBombSlash(defaultBombCount, defaultBombCount, defaultBombCount);
            yurusenaiObstacle = new YurusenaiObstacle(defaultObstacleCount, defaultObstacleCount, defaultObstacleCount);

            updatedNumberOfDigitForNoteImage = initialNumberOfDigitForNoteImage;
            updatedNumberOfDigitForSquatImage = initialNumberOfDigitForSquatImage;

            CreateLabel();
            CreateCounter();

            // Start Obstacle Dmage Tracking
            obstacleDamageCount = _container.InstantiateComponentOnNewGameObject<ObstacleCounterUpdater>("obstacleDamageCount");
        }

        private (int forNoteImage, int forSquatImage) GetInitialNumbersOfDigit()
        {
            int forNoteImage = int.MaxValue;
            int forSquatImage = int.MaxValue;
            
            if (PluginConfig.Instance.SeparateSaber)
            {
                forNoteImage = 2;
            }
            else
            {
                forNoteImage = 1;
            }

            if (IsTwoNumberInInitialObstacleCounter())
            {
                forSquatImage = 2;
                return (forNoteImage, forSquatImage);
            }

            if (!PluginConfig.Instance.IsObstacleTimeEnable)
            {
                forSquatImage = 1;
                return (forNoteImage, forSquatImage);
            }

            forSquatImage = 2 + PluginConfig.Instance.ObstacleSecondPrecision;
            return (forNoteImage, forSquatImage);
        }

        private static bool IsTwoNumberInInitialObstacleCounter()
        {
            return PluginConfig.Instance.IsObstacleTimeEnable &&
                            (PluginConfig.Instance.ObstacleTimeType == ObstacleTimeTypeEnum.Frame ||
                            (PluginConfig.Instance.ObstacleTimeType == ObstacleTimeTypeEnum.Second
                            && PluginConfig.Instance.ObstacleSecondPrecision == 0)
                            );
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

            if (IsYokoNoteIconDisabled()) return;
            ConfirmNumberOfDigitForYokoNote();
        }

        public void OnNoteMiss(NoteData data)
        {
            if (!IsNoteYoko(data)) return;

            UpdateYurusenai(ref yurusenaiNoteMiss, data);

            UpdateText();

            if (IsYokoNoteIconDisabled()) return;
            ConfirmNumberOfDigitForYokoNote();
        }

        private bool IsYokoNoteIconDisabled()
        {
            return !PluginConfig.Instance.IsIconEnable
                || PluginConfig.Instance.CounterType == CounterTypeEnum.BombsOnly
                || PluginConfig.Instance.CounterType == CounterTypeEnum.ObstaclesOnly
                || PluginConfig.Instance.CounterType == CounterTypeEnum.BombsAndObstacles;
        }

        private bool IsObstacleIconDisabled()
        {
            return !PluginConfig.Instance.IsIconEnable
                || PluginConfig.Instance.CounterType == CounterTypeEnum.BombsOnly
                || PluginConfig.Instance.CounterType == CounterTypeEnum.YokoNotesOnly
                || PluginConfig.Instance.CounterType == CounterTypeEnum.YokoNotesAndBombs;
        }

        private void ConfirmNumberOfDigitForYokoNote()
        {
            int newNumberOfDigit;
            int numberOfMove;

            if (PluginConfig.Instance.SeparateSaber)
            {
                newNumberOfDigit = (int)Math.Log10(int.Parse(yurusenaiNoteMiss.LeftCount())) + (int)Math.Log10(int.Parse(yurusenaiNoteMiss.RightCount())) + 2;
            }
            else
            {
                newNumberOfDigit = (int)Math.Log10(int.Parse(yurusenaiNoteMiss.BothCount())) + 1;
            }

            if (updatedNumberOfDigitForNoteImage >= newNumberOfDigit) return;

            numberOfMove = newNumberOfDigit - updatedNumberOfDigitForNoteImage;

            foreach (var _ in Enumerable.Range(1, numberOfMove))
            {
                _imageViewSetter.MoveYokoNoteLeft();
            }

            updatedNumberOfDigitForNoteImage = newNumberOfDigit;
        }

        private void ConfirmNumberOfDigitForSquat()
        {
            int newNumberOfDigit;
            int numberOfMove;

            if (PluginConfig.Instance.IsObstacleTimeEnable)
            {
                newNumberOfDigit = (int)Math.Log10(int.Parse(yurusenaiObstacle.LeftCount()))
                + (int)Math.Log10(float.Parse(yurusenaiObstacle.RightCount().TrimEnd('s', 'f')))
                + initialNumberOfDigitForSquatImage;
            }
            else
            {
                newNumberOfDigit = (int)Math.Log10(int.Parse(yurusenaiObstacle.LeftCount()))
                + initialNumberOfDigitForSquatImage;
            }

            if (updatedNumberOfDigitForSquatImage >= newNumberOfDigit) return;

            numberOfMove = newNumberOfDigit - updatedNumberOfDigitForSquatImage;

            foreach (var _ in Enumerable.Range(1, numberOfMove))
            {
                _imageViewSetter.MoveSquatLeft();
            }

            updatedNumberOfDigitForSquatImage = newNumberOfDigit;
        }

        public override void CounterDestroy() 
        {
            yurusenaiObstacle = null;
        }

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
            ConfirmNumberOfDigitForSquat();
        }

        internal void AddObstacleDamageDuration(float time)
        {
            yurusenaiObstacle = (YurusenaiObstacle)yurusenaiObstacle.UpdateRight(time);
            UpdateText();
            ConfirmNumberOfDigitForSquat();
        }

        internal void AddObstacleDamageDuration()
        {
            yurusenaiObstacle = (YurusenaiObstacle)yurusenaiObstacle.UpdateRight();
            UpdateText();
            ConfirmNumberOfDigitForSquat();
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
            
            CreateIcons();
        }

        private void CreateIcons()
        {
            if (!IsYokoNoteIconDisabled())
            {
                _imageViewSetter?.InitializeYokoNoteIcon();
                _imageViewSetter?.SetTMPTransformForYokoNote(counter.transform);
            }

            if (IsObstacleIconDisabled()) return;

            _imageViewSetter?.InitializeSquatIcon();
            _imageViewSetter?.SetTMPTransformForObstacle(counter.transform);
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
