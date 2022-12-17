using YokoNotesTokaYurusenaiCounter.Interfaces;

namespace YokoNotesTokaYurusenaiCounter
{
    internal class YurusenaiBombSlash : IYurusenai
    {
        readonly private int bothCount;
        public string BothCount() => bothCount.ToString();

        readonly private int leftCount;
        public string LeftCount() => leftCount.ToString();

        readonly private int rightCount;
        public string RightCount() => rightCount.ToString();

        internal YurusenaiBombSlash(int both, int left, int right)
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

        public IYurusenai UpdateBoth()
        {
            return new YurusenaiBombSlash(bothCount + 1, leftCount, rightCount);
        }

        public IYurusenai UpdateLeft()
        {
            return new YurusenaiBombSlash(bothCount, leftCount + 1, rightCount);
        }

        public IYurusenai UpdateRight()
        {
            return new YurusenaiBombSlash(bothCount, leftCount, rightCount + 1);
        }
    }
}
