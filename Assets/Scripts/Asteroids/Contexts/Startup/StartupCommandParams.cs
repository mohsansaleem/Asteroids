using PG.Asteroids.Models.DataModels;

namespace PG.Asteroids.Contexts.Startup
{
    public class LoadStaticDataParams
    { }

    public class LoadUserDataParams
    { }

    public class SaveUserDataParams { }

    public class CreateUserDataParams
    {
        public UserData UserData;
    }
}