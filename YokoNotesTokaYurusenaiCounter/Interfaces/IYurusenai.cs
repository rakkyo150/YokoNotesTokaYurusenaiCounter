namespace YokoNotesTokaYurusenaiCounter.Interfaces
{
    internal interface IYurusenai
    {
        string BothCount();
        string LeftCount();
        string RightCount();

        IYurusenai UpdateBoth();

        IYurusenai UpdateLeft();

        IYurusenai UpdateRight();
    }
}
