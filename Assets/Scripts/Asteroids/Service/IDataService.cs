
using Cysharp.Threading.Tasks;
using PG.Asteroids.Models.DataModels;
using UnityEngine;

namespace PG.Asteroids.Service
{
    public interface IDataService
    {
        UniTask<MetaData> GetMetaData();
        UniTask<UserData> SaveUserData(UserData userData);
        UniTask<UserData> GetUserData();
    }
}