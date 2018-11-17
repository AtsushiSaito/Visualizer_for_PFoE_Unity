using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

namespace ROSBridgeSharp
{
    public class PFoEOutputSubscriber : BaseSubscriber<ROSBridgeSharp.Messages.PFoEOutput>
    {
        public Text PFoEOutputLabel;
        ROSBridgeSharp.Messages.PFoEOutput data;

        private int n_particle = 1000;
        private int n_event = 0;
        private int mode_event = -1;

        private float Scale = 3.0f;
        private float CanvasWidth, CanvasHeight;
        private float ParticleFilterObjectDeltaX, ParticleFilterObjectDeltaY;
        private float GraphPSDeltaX, GraphPSDeltaY;

        private ParticleSystem.Particle[] Particle;

        public Canvas MainCanvas;
        public GameObject ParticleFilterObject;
        public ParticleSystem GraphParticleSystem;

        public Slider slider;

        private Vector2 ParticleSystemSize;

        protected override void Start(){
            base.Start();
            ParticleSystemSize = CalcSize();
            Debug.Log(ParticleSystemSize);
        }

       
        Vector2 CalcSize(){
            // Canvasのサイズを計算
            CanvasWidth = MainCanvas.pixelRect.width;
            CanvasHeight = MainCanvas.pixelRect.height;

            // ParticleFilterオブジェクトの相対位置を計算
            ParticleFilterObjectDeltaX = ParticleFilterObject.GetComponent<RectTransform>().sizeDelta.x;
            ParticleFilterObjectDeltaY = ParticleFilterObject.GetComponent<RectTransform>().sizeDelta.y;

            // ParticleSystemの相対位置を計算
            GraphPSDeltaX = GraphParticleSystem.GetComponent<RectTransform>().sizeDelta.x;
            GraphPSDeltaY = GraphParticleSystem.GetComponent<RectTransform>().sizeDelta.y;

            return new Vector2(CanvasWidth + ParticleFilterObjectDeltaX + GraphPSDeltaX, CanvasHeight + ParticleFilterObjectDeltaY + GraphPSDeltaY);
        }

        protected override void ReceiveHandler(ROSBridgeSharp.Messages.PFoEOutput message){
            data = message;
            isReceived = true;
        }

        internal void Update(){
            if (isReceived){
                PFoEOutputLabel.text = "Eta : " + data.eta.ToString() + "\n" +
                    "Estimate Event : " + (n_event - 1).ToString();
                MaxEvent();
                ParticleSystemSize = CalcSize();
                ProcessMessage();
            }
        }

        internal void ProcessMessage(){
            GraphParticleSystem.Emit(n_event);
            Particle = new ParticleSystem.Particle[n_event];

            GraphParticleSystem.GetParticles(Particle);
            GraphParticleSystem.Pause();

            int[] pe = countParticle(data.particles_pos);

            float Resolution = (ParticleSystemSize.x - 2) / (n_event - 1);

            for (int i = 0; i < n_event; i++){
                Particle[i].position = Vector3.Lerp(Particle[i].position, new Vector3(-(ParticleSystemSize.x * 0.5f) + (i * Resolution), pe[i] * 0.5f * Scale), 0.2f);
                Particle[i].startSize3D = Vector3.Lerp(Particle[i].startSize3D, new Vector2(Resolution*0.6f, pe[i] * Scale), 0.2f);

                if(mode_event == i){
                    Particle[i].startColor = Color.magenta;
                }else{
                    Particle[i].startColor = Color.white;
                }
            }
            GraphParticleSystem.SetParticles(Particle, n_event);
        }

        void Awake(){
            Application.targetFrameRate = 60;
        }

        internal int[] countParticle(int[] p)
        {
            int[] a = new int[n_event];
            int counter = 0;
            for (int i = 0; i < n_event; i++) { a[i] = 0; }
            for (int i = 0; i < n_particle; i++) {
                a[p[i]] += 1;
                if(counter < a[p[i]]){
                    counter = a[p[i]];
                    mode_event = p[i];
                }
            }
            return a;
        }

        internal void MaxEvent(){
            int maxValue = data.particles_pos.Max();
            if (n_event < maxValue){
                n_event = maxValue + 1;
            }
        }

        public void ChangeSlider(){
            Scale = slider.value;
            Debug.Log(slider.value);
        }
    }
}