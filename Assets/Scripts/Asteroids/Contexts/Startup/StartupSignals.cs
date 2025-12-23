using PG.Asteroids.Models.DataModels;
using Zenject;

namespace PG.Asteroids.Contexts.Startup
{
    public class LoadStaticDataSignal
    { }

    public class LoadUserDataSignal
    { }
    
    public class SaveUserDataSignal { }

    public class CreateUserDataSignal
    {
        public UserData UserData;
    }
}