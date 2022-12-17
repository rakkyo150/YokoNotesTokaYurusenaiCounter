using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YokoNotesTokaYurusenaiCounter.Configuration;
using YokoNotesTokaYurusenaiCounter.Interfaces;
using YokoNotesTokaYurusenaiCounter.UI;
using Zenject;

namespace YokoNotesTokaYurusenaiCounter
{
    public class ConfirmorOfNumberOfDigit
    {
        private readonly ImageViewSetter _imageViewSetter;
        
        public ConfirmorOfNumberOfDigit(ImageViewSetter imageViewSetter)
        {
            _imageViewSetter = imageViewSetter;
        }

        internal (int forNoteImage, int forSquatImage) GetInitialNumbersOfDigit()
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

            if (PluginConfig.Instance.IsTwoNumberInInitialObstacleCounter())
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

        internal int ConfirmNumberOfDigitForYokoNote(IYurusenai yurusenai,int initialNumberOfDigit, int updatedNumberOfDigit)
        {
            int newNumberOfDigit = int.MinValue;
            int numberOfMove = int.MinValue;

            if (PluginConfig.Instance.SeparateSaber)
            {
                newNumberOfDigit = (int)Math.Log10(int.Parse(yurusenai.LeftCount())) + (int)Math.Log10(int.Parse(yurusenai.RightCount())) + 2;
            }
            else
            {
                newNumberOfDigit = (int)Math.Log10(int.Parse(yurusenai.BothCount())) + 1;
            }

            if (updatedNumberOfDigit >= newNumberOfDigit) return updatedNumberOfDigit;

            numberOfMove = newNumberOfDigit - updatedNumberOfDigit;

            foreach (var _ in Enumerable.Range(1, numberOfMove))
            {
                _imageViewSetter.MoveYokoNoteLeft();
            }

            return newNumberOfDigit;
        }

        internal int ConfirmNumberOfDigitForSquat(IYurusenai yurusenai,int initialNumberOfDigit, int updatedNumberOfDigit)
        {
            int newNumberOfDigit = int.MinValue;
            int numberOfMove = int.MinValue;

            if (PluginConfig.Instance.IsObstacleTimeEnable)
            {
                newNumberOfDigit = (int)Math.Log10(int.Parse(yurusenai.LeftCount()))
                + (int)Math.Log10(float.Parse(yurusenai.RightCount().TrimEnd('s', 'f')))
                + initialNumberOfDigit;
            }
            else
            {
                newNumberOfDigit = (int)Math.Log10(int.Parse(yurusenai.LeftCount()))
                + initialNumberOfDigit;
            }

            if (updatedNumberOfDigit >= newNumberOfDigit) return updatedNumberOfDigit;

            numberOfMove = newNumberOfDigit - updatedNumberOfDigit;

            foreach (var _ in Enumerable.Range(1, numberOfMove))
            {
                _imageViewSetter.MoveSquatLeft();
            }

            return newNumberOfDigit;
        }
    }
}
