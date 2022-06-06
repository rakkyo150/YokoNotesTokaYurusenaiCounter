using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
