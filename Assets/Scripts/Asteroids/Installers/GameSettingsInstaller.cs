using PG.Asteroids.Models.DataModels;
using UnityEngine;
using Zenject;

namespace Asteroids.Data
{ 
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Create/GameSettingsInstaller")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public MetaData MetaData;

        public override void InstallBindings()
        {
            Container.BindInstance(MetaData);
        }
    }
}
