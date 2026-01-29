using Cysharp.Threading.Tasks;

namespace Game.Root.ServicesInterfaces
{
    public interface IPrewarmable
    {
        public UniTask Prewarm();
    }
}
