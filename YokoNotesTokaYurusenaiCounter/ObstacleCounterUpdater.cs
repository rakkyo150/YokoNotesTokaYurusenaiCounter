using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
using YokoNotesTokaYurusenaiCounter.Configuration;
using Zenject;

namespace YokoNotesTokaYurusenaiCounter
{
    public class ObstacleCounterUpdater: MonoBehaviour
    {
        private YokoNotesTokaYurusenaiCounter yokoNotesTokaYurusenaiCounter;
        private PlayerHeadAndObstacleInteraction playerHeadAndObstacleInteraction;

        private bool previousFramePlayerHeadIsInObstacle = false;

        [Inject]
        public void Constructor(YokoNotesTokaYurusenaiCounter yokoNotesTokaYurusenaiCounter, PlayerHeadAndObstacleInteraction playerHeadAndObstacleInteraction)
        {
            this.yokoNotesTokaYurusenaiCounter = yokoNotesTokaYurusenaiCounter;
            this.playerHeadAndObstacleInteraction = playerHeadAndObstacleInteraction;
        }

        private void LateUpdate()
        {
            if (this.playerHeadAndObstacleInteraction.playerHeadIsInObstacle)
            {
                if(PluginConfig.Instance.ObstacleTimeType == ObstacleTimeTypeEnum.Second)
                {
                    yokoNotesTokaYurusenaiCounter.AddObstacleDamageDuration(Time.deltaTime);
                }
                else
                {
                    yokoNotesTokaYurusenaiCounter.AddObstacleDamageDuration();
                }
            }
            
            if(!PlayerHeadIsInNewObstacle()) return;

            yokoNotesTokaYurusenaiCounter.AddObstacleDamageCount();
        }

        private bool PlayerHeadIsInNewObstacle()
        {
            if (!this.playerHeadAndObstacleInteraction.playerHeadIsInObstacle)
            {
                if (!previousFramePlayerHeadIsInObstacle) return false;

                previousFramePlayerHeadIsInObstacle = false;
                return false;
            }

            if (previousFramePlayerHeadIsInObstacle) return false;

            previousFramePlayerHeadIsInObstacle = true;
            return true;
        }
    }
}
