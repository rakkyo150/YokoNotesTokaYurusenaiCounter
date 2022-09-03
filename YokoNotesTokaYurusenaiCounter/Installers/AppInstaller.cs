using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YokoNotesTokaYurusenaiCounter.UI;
using Zenject;

namespace YokoNotesTokaYurusenaiCounter.Installers
{
    internal class AppInstaller:Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<AssetLoader>().AsSingle();
        }
    }
}
