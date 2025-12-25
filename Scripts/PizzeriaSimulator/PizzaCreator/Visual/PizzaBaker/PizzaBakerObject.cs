using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Root.Utils.Audio;
using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual.Baker
{
    class PizzaOnConveyer
    {
        public readonly PizzaObject NotBakedPizza;
        public readonly BakedPizzaObject BakedPizzaPrefab;
        public BakedPizzaObject BakedPizza { get; private set; }
        public Transform PizzaTransform { get; private set; }
        public bool IsBaked { get; private set; }
        public Action OnCompleteCallback { get; private set; }
        public PizzaOnConveyer(PizzaObject _notBakedPizza, BakedPizzaObject _bakedPizzaPrefab, Action _onCompleteCallback)
        {
            NotBakedPizza = _notBakedPizza;
            BakedPizzaPrefab = _bakedPizzaPrefab;
            PizzaTransform = NotBakedPizza.transform;
            OnCompleteCallback = _onCompleteCallback;
        }
        public void SetBakedPizza(BakedPizzaObject _bakedPizzaObject)
        {
            IsBaked = true;
            BakedPizza = _bakedPizzaObject;
            PizzaTransform = BakedPizza.transform;
        }
    }
    class PizzaBakerObject : MonoBehaviour
    {
        [SerializeField] AudioClip pizzaBakedSFX;
        [SerializeField] AudioClip pizzaMoveToCutSFX;
        [SerializeField] AudioSource bakerAudioSource;
        [SerializeField] PizzaCutViewBase pizzaCutView;
        [SerializeField] Transform startPizzaMovePoint;
        [SerializeField] Transform pizzaPreCutPoint;
        [SerializeField] Transform pizzaCutPoint;
        [SerializeField] Vector3 pizzaBakePos;
        [SerializeField] Vector3 endPizzaMovePos;
        [SerializeField] float pizzaMoveSpeed;
        [SerializeField] float pizzaMoveToCutDur;
        [SerializeField] float pizzaLength;
        [SerializeField] float spacingBetweenPizzas;

        List<PizzaOnConveyer> pizzasOnConveyer;
        public Vector3 StartPizzaPosition => startPizzaMovePoint.position;
        public void StartBaking(PizzaObject pizzaObject, BakedPizzaObject bakedPizzaPrefab, Action onBaked)
        {
            pizzaObject.transform.position = StartPizzaPosition;
            pizzasOnConveyer.Add(new PizzaOnConveyer(pizzaObject, bakedPizzaPrefab, onBaked));
        }
        private void Awake()
        {
            pizzasOnConveyer = new List<PizzaOnConveyer>();
        }
        private void Update()
        {
            if (pizzasOnConveyer.Count < 1) return;
            MovePizzasNegativeDirZ();
        }
        void MovePizzasNegativeDirZ()
        {
            float currentPizzaSpeed = pizzaMoveSpeed * Time.deltaTime;
            bool needToTranslatePizza = false; 
            for (int i = 0; i < pizzasOnConveyer.Count; i++)
            {
                float targetPosZ = endPizzaMovePos.z + ((pizzaLength + spacingBetweenPizzas) * i);
                if (pizzasOnConveyer[i].PizzaTransform.position.z > targetPosZ)
                {
                    pizzasOnConveyer[i].PizzaTransform.position -= new Vector3(0, 0, currentPizzaSpeed);
                    if (!pizzasOnConveyer[i].IsBaked && pizzasOnConveyer[i].PizzaTransform.position.z < pizzaBakePos.z) BakePizza(i); 
                }
                else
                {
                    if (i == 0 && !pizzaCutView.HasPizzaToCut)
                    {
                        needToTranslatePizza = true;
                        continue;
                    }
                    pizzasOnConveyer[i].PizzaTransform.position = new Vector3(endPizzaMovePos.x, endPizzaMovePos.y, targetPosZ);
                }
            }
            if(needToTranslatePizza) TranslatePizzaToCut();
        }
        void BakePizza(int pizzaIndex)
        {
            if (pizzasOnConveyer[pizzaIndex].IsBaked) return;
            bakerAudioSource.PlayOneShot(pizzaBakedSFX);
            BakedPizzaObject spawnedBakedPizza = Instantiate(pizzasOnConveyer[pizzaIndex].BakedPizzaPrefab, 
                pizzasOnConveyer[pizzaIndex].PizzaTransform.position, pizzasOnConveyer[pizzaIndex].BakedPizzaPrefab.transform.rotation);
            pizzasOnConveyer[pizzaIndex].SetBakedPizza(spawnedBakedPizza);
            Destroy(pizzasOnConveyer[pizzaIndex].NotBakedPizza.gameObject);
        }
        void TranslatePizzaToCut()
        {
            if (pizzasOnConveyer.Count < 1 || pizzasOnConveyer[0].BakedPizza == null) return;
            BakedPizzaObject currentBakedPizza = pizzasOnConveyer[0].BakedPizza;
            bakerAudioSource.PlayOneShot(pizzaMoveToCutSFX);
            DOTween.Sequence()
                .Append(pizzasOnConveyer[0].PizzaTransform.DOMove(pizzaPreCutPoint.transform.position, pizzaMoveToCutDur).SetEase(Ease.Linear))
                .Append(pizzasOnConveyer[0].PizzaTransform.DOMove(pizzaCutPoint.transform.position, pizzaMoveToCutDur).SetEase(Ease.Linear))
                .OnComplete(() => pizzaCutView.SetPizzaToCut(currentBakedPizza))
                .Play();
            pizzasOnConveyer[0].OnCompleteCallback?.Invoke();
            pizzasOnConveyer.RemoveAt(0);
        }
    }
}
