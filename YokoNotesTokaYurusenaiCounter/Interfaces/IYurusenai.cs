namespace YokoNotesTokaYurusenaiCounter.Interfaces
{
    internal interface IYurusenai
    {
        int BothCount();
        int LeftCount();
        int RightCount();

        IYurusenai UpdateBothHand();

        IYurusenai UpdateLeftHand();

        IYurusenai UpdateRightHand();
    }
}
