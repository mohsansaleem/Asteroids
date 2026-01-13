using System.Linq;
using PG.Asteroids.Models.DataModels;
using Unity.VisualScripting;
using Zenject;

namespace PG.Asteroids.Models.RemoteDataModels
{
    public class RemoteDataModel
    {
        public UserData UserData;

        public void SeedUserData(UserData userData)
        {
            UserData = userData;
        }
    }
}

