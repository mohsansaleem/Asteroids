using UniRx;

namespace PG.Asteroids.Models.MediatorModels
{
    public class GamePlayModel
    {
        public ReactiveProperty<int> Lives;
        public ReactiveProperty<bool> IsDead;
        public ReactiveProperty<int> Scores;

        public GamePlayModel()
        {
            Lives = new ReactiveProperty<int>(0);
            IsDead = new ReactiveProperty<bool>();
            Scores = new ReactiveProperty<int>();
        }
    }
}