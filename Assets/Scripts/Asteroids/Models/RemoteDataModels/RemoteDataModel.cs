using System.Linq;
using PG.Asteroids.Models.DataModels;
using UniRx;
using Unity.VisualScripting;
using Zenject;

namespace PG.Asteroids.Models.RemoteDataModels
{
    public class RemoteDataModel
    {
        [Inject] private readonly StaticDataModel _staticDataModel;

        public UserData UserData;

        public void SeedUserData(UserData userData)
        {
            UserData = userData;
        }
    }
}

