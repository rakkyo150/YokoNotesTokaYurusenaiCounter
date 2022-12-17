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
        private readonly int defaultObstacleCount = 0;

        private readonly IYurusenai InitializedYurusenaiNoteMiss;
        private readonly IYurusenai InitializedYurusenaiBombSlash;
        private readonly YurusenaiObstacle InitializedYurusenaiObstacle;

        private IYurusenai updatedYurusenaiNoteMiss;
        private IYurusenai updatedYurusenaiBombSlash;
        private YurusenaiObstacle updatedYurusenaiObstacle;

        private readonly int initialNumberOfDigitForNoteImage;
        private int updatedNumberOfDigitForNoteImage;

        private readonly int initialNumberOfDigitForSquatImage;
        private int updatedNumberOfDigitForSquatImage;

        private ObstacleCounterUpdater obstacleDamageCount;

        public YokoNotesTokaYurusenaiCounter(DiContainer diContainer)
        {
            _container = diContainer;

            labelOffset = new Vector3(
                PluginConfig.Instance.OffsetX, PluginConfig.Instance.OffsetY, PluginConfig.Instance.OffsetZ
                );

            InitializedYurusenaiNoteMiss = new YurusenaiNoteMiss(defaultNoteCount, defaultNoteCount, defaultNoteCount);
            InitializedYurusenaiBombSlash = new YurusenaiBombSlash(defaultBombCount, defaultBombCount, defaultBombCount);
            InitializedYurusenaiObstacle = new YurusenaiObstacle(defaultObstacleCount, defaultObstacleCount, defaultObstacleCount);

            if (PluginConfig.Instance.SeparateSaber)
            {
                initialNumberOfDigitForNoteImage = 2;
            }
            else
            {
                initialNumberOfDigitForNoteImage = 1;
            }

            if(PluginConfig.Instance.ObstacleTimeType == ObstacleTimeTypeEnum.Frame ||
                (PluginConfig.Instance.ObstacleTimeType == ObstacleTimeTypeEnum.Second 
                && PluginConfig.Instance.ObstacleSecondPrecision == 0)
                )
            {
                initialNumberOfDigitForSquatImage = 2;
            }
            else
            {
                initialNumberOfDigitForSquatImage = 2 + PluginConfig.Instance.ObstacleSecondPrecision;
            }
        }

        public override void CounterInit()
        {
            updatedYurusenaiNoteMiss = InitializedYurusenaiNoteMiss;
            updatedYurusenaiBombSlash = InitializedYurusenaiBombSlash;
            updatedYurusenaiObstacle = InitializedYurusenaiObstacle;

            updatedNumberOfDigitForNoteImage = initialNumberOfDigitForNoteImage;
            updatedNumberOfDigitForSquatImage = initialNumberOfDigitForSquatImage;

            if (!IsYokoNoteIconDisabled())
            {
                // MonoBehaviourを継承しているのでInstantiateする必要がある
                _imageViewSetter = _container.InstantiateComponentOnNewGameObject<ImageViewSetter>("yokoNoteImageView");
                _imageViewSetter?.Initialize();
            }

            obstacleDamageCount = _container.InstantiateComponentOnNewGameObject<ObstacleCounterUpdater>("obstacleDamageCount");

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
            ConfirmNumberOfDigitForYokoNote();
        }

        public void OnNoteMiss(NoteData data)
        {
            if (!IsNoteYoko(data)) return;

            UpdateYurusenai(ref updatedYurusenaiNoteMiss, data);

            UpdateText();

            if (IsYokoNoteIconDisabled()) return;
            ConfirmNumberOfDigitForYokoNote();
        }

        private void ConfirmNumberOfDigitForYokoNote()
        {
            int newNumberOfDigit;
            int numberOfMove;

            if (PluginConfig.Instance.SeparateSaber)
            {
                newNumberOfDigit = (int)Math.Log10(int.Parse(updatedYurusenaiNoteMiss.LeftCount())) + (int)Math.Log10(int.Parse(updatedYurusenaiNoteMiss.RightCount())) + 2;
            }
            else
            {
                newNumberOfDigit = (int)Math.Log10(int.Parse(updatedYurusenaiNoteMiss.BothCount())) + 1;
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

            newNumberOfDigit = (int)Math.Log10(int.Parse(updatedYurusenaiObstacle.LeftCount())) 
                + (int)Math.Log10(float.Parse(updatedYurusenaiObstacle.RightCount().TrimEnd('s','f'))) 
                + initialNumberOfDigitForSquatImage;

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
            updatedYurusenaiObstacle = null;
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
            updatedYurusenaiObstacle = (YurusenaiObstacle)updatedYurusenaiObstacle.UpdateLeft();
            UpdateText();
            ConfirmNumberOfDigitForSquat();
        }

        internal void AddObstacleDamageDuration(float time)
        {
            updatedYurusenaiObstacle = (YurusenaiObstacle)updatedYurusenaiObstacle.UpdateRight(time);
            UpdateText();
            ConfirmNumberOfDigitForSquat();
        }

        internal void AddObstacleDamageDuration()
        {
            updatedYurusenaiObstacle = (YurusenaiObstacle)updatedYurusenaiObstacle.UpdateRight();
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

            if (IsYokoNoteIconDisabled()) return;
            _imageViewSetter?.SetTMPTransform(counter.transform);
            _imageViewSetter?.SetTMPTransform2(counter.transform);
        }

        private void UpdateText() => counter.text = MakeCountText();

        private string MakeCountText()
        {
            switch (PluginConfig.Instance.CounterType)
            {
                case CounterTypeEnum.Both:
                    return $"{YurusenaiText(updatedYurusenaiNoteMiss, "      ")}" +
                        $"\n{YurusenaiText(updatedYurusenaiBombSlash, PluginConfig.Instance.BombSlashIcon)}" +
                        $"\n{YurusenaiText(updatedYurusenaiObstacle, "      ")}";
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
