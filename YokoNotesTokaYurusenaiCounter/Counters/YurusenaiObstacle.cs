using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YokoNotesTokaYurusenaiCounter.Configuration;
using YokoNotesTokaYurusenaiCounter.Interfaces;

namespace YokoNotesTokaYurusenaiCounter
{
    internal class YurusenaiObstacle : IYurusenai
    {
        readonly float bothCount;
        public string BothCount() => bothCount.ToString();

        readonly int leftCount;
        public string LeftCount() => leftCount.ToString();

        readonly float rightCount;
        public string RightCount()
        {
            if(PluginConfig.Instance.ObstacleTimeType == ObstacleTimeTypeEnum.Second)
            {
                return rightCount.ToString($"F{PluginConfig.Instance.ObstacleSecondPrecision}") + "s";
            }
            
            return rightCount.ToString("F0") + "f";
        }

        internal YurusenaiObstacle(float both, int left, float right)
        {
            if (both < 0 || left < 0 || right < 0)
            {
                Plugin.Log.Warn("Invalid argument, so all is substituded 0");
                bothCount = 0;
                leftCount = 0;
                rightCount = 0;
            }

            bothCount = both;
            leftCount = left;
            rightCount = right;
        }

        // Not used
        public IYurusenai UpdateBoth()
        {
            return new YurusenaiObstacle(float.MaxValue, int.MaxValue, float.MaxValue);
        }

        public IYurusenai UpdateLeft()
        {
            return new YurusenaiObstacle(bothCount, leftCount + 1, rightCount);
        }

        public IYurusenai UpdateRight()
        {
            return new YurusenaiObstacle(bothCount, leftCount, rightCount + 1);
        }

        public IYurusenai UpdateRight(float time)
        {
            return new YurusenaiObstacle(bothCount, leftCount, rightCount + time);
        }
    }
}
