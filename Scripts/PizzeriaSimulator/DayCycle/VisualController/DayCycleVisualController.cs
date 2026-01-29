using Game.PizzeriaSimulator.DayCycle.Config;
using Game.PizzeriaSimulator.DayCycle.Manager;
using Game.Root.ServicesInterfaces;
using R3;
using UnityEngine;

namespace Game.PizzeriaSimulator.DayCycle.Visual
{
    class DayCycleVisualController : IInittable, ISceneDisposable
    {
        public int InitPriority => 10;
        readonly DayCycleManager dayCycleManager;
        readonly DayCycleConfig dayCycleConfig;
        readonly Light mainLight;
        readonly int skyColorPropId;
        readonly CompositeDisposable disposables;
        Material skyboxMaterial;

        int startDayMinutes;
        int dayDuration;
        int lastUpdateMinutes;
        int minutesAdd;
        const int minutesInHour = 60;
        const int updateDelay = 7;
        public DayCycleVisualController(DayCycleManager _dayCycleManager, DayCycleConfig _dayCycleConfig, Light _mainLight) 
        {
            dayCycleManager = _dayCycleManager;
            dayCycleConfig = _dayCycleConfig;
            mainLight = _mainLight;
            disposables = new CompositeDisposable();
            skyColorPropId = Shader.PropertyToID("_Tint");
        }
        public void Init()
        {
            skyboxMaterial = Object.Instantiate(RenderSettings.skybox);
            RenderSettings.skybox = skyboxMaterial;
            startDayMinutes = dayCycleConfig.StartHour * minutesInHour;
            dayDuration = dayCycleConfig.EndHour * minutesInHour - startDayMinutes;

            dayCycleManager.OnDayStarted += HandleDayStart;
            dayCycleManager.Hours.Subscribe(HandleHour).AddTo(disposables);
            dayCycleManager.Minutes.Subscribe(HandleMinutes).AddTo(disposables);
        }
        public void Dispose()
        {
            dayCycleManager.OnDayStarted -= HandleDayStart;
            disposables.Dispose();
        }
        void HandleDayStart()
        {
            lastUpdateMinutes = 0;
            HandleMinutes(dayCycleManager.Minutes.CurrentValue);
        }
        void HandleHour(int hour)
        {
            minutesAdd = hour * minutesInHour;
        }
        void HandleMinutes(int minutes)
        {
            if (minutes + minutesAdd - lastUpdateMinutes < updateDelay) return;
            lastUpdateMinutes = minutesAdd + minutes;
            float t = (float)(lastUpdateMinutes - startDayMinutes) / dayDuration;
            skyboxMaterial.SetColor(skyColorPropId, dayCycleConfig.SkyColorGradient.Evaluate(t));
            RenderSettings.ambientLight = dayCycleConfig.AmbientColorGradient.Evaluate(t);
            mainLight.color = dayCycleConfig.DirectionColorGradient.Evaluate(t);
        }
    }
}
