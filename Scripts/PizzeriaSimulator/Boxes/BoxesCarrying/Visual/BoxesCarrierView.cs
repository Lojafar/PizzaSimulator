using Game.Root.Utils;
using Game.Root.Utils.Audio;

namespace Game.PizzeriaSimulator.Boxes.Carry.Visual
{
    class BoxesCarrierView : IBoxesCarrierView
    {
        BoxesCarrier boxesCarrier;
        public void Bind(BoxesCarrier _boxesCarrier)
        {
            boxesCarrier = _boxesCarrier;
            boxesCarrier.OnActionDenied += ShowMessage;
            boxesCarrier.OnBoxThrowed += OnBoxThrowed;
            boxesCarrier.OnBoxRemoved += OnBoxThrowed;
        }
        public void Dispose()
        {
            boxesCarrier.OnActionDenied -= ShowMessage;
            boxesCarrier.OnBoxThrowed -= OnBoxThrowed;
            boxesCarrier.OnBoxRemoved -= OnBoxThrowed;
        }
        void ShowMessage(string message) 
        {
            Toasts.ShowToast(message);
            AudioPlayer.PlaySFX("Wrong");
        }
        void OnBoxThrowed(uint boxID)
        {
            AudioPlayer.PlaySFX("Swoosh");
        }
    }
}
