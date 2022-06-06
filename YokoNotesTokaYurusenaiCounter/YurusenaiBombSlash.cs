using YokoNotesTokaYurusenaiCounter.Interfaces;

namespace YokoNotesTokaYurusenaiCounter
{
    internal class YurusenaiBombSlash : IYurusenai
    {
        readonly private int bothCount;
        public int BothCount() => bothCount;

        readonly private int leftCount;
        public int LeftCount() => leftCount;

        readonly private int rightCount;
        public int RightCount() => rightCount;

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

        public IYurusenai UpdateBothHand()
        {
            return new YurusenaiBombSlash(bothCount + 1, leftCount, rightCount);
        }

        public IYurusenai UpdateLeftHand()
        {
            return new YurusenaiBombSlash(bothCount, leftCount + 1, rightCount);
        }

        public IYurusenai UpdateRightHand()
        {
            return new YurusenaiBombSlash(bothCount, leftCount, rightCount + 1);
        }
    }
}
