using Cysharp.Threading.Tasks;

namespace Game.PizzeriaSimulator.Player.Input.Factory
{
    public interface IPlayerInputFactory 
    {
        public UniTask Prewarm();
        public IPlayerInput CreateInput();
    }
}
