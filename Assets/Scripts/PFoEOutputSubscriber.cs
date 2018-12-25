using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using ROSBridgeSharp.Messages;

public class PFoEOutputSubscriber : MonoBehaviour
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
    public GameObject SafeArea;
    public GameObject ParticleFilterObject;
    public ParticleSystem GraphParticleSystem;

    public Slider slider;

    private Vector2 ParticleSystemSize;


    private void Awake()
    {
        RBSubscriber<PFoEOutput> s = new RBSubscriber<PFoEOutput>("/pfoe_out", Callback);
        ParticleSystemSize = CalcSize();
        Application.targetFrameRate = 60;
    }
    void Callback(PFoEOutput msg)
    {
        data = msg;
    }

    private void Update()
    {
        if (data != null)
        {
            PFoEOutputLabel.text = "Eta : " + data.eta.ToString() + "\n" +
                            "Estimate Event : " + (n_event - 1).ToString();
            MaxEvent();
            ParticleSystemSize = CalcSize();
            ProcessMessage();
        }
    }

    Vector2 CalcSize()
    {
        // Canvasのサイズを計算
        CanvasWidth = MainCanvas.pixelRect.width;
        CanvasHeight = MainCanvas.pixelRect.height;

        RectTransform rectTransform = SafeArea.GetComponent<RectTransform>();
        Vector2 amax = rectTransform.anchorMax;
        Vector2 amin = rectTransform.anchorMin;

        float AfterCanvasSizeWidth = (amax.x - amin.x) * CanvasWidth;
        float AfterCanvasSizeHeight = (amax.y - amin.y) * CanvasHeight;
        //Debug.Log(CanvasWidth + ":" + AfterCanvasSizeWidth);

        // ParticleFilterオブジェクトの相対位置を計算
        ParticleFilterObjectDeltaX = ParticleFilterObject.GetComponent<RectTransform>().sizeDelta.x;
        ParticleFilterObjectDeltaY = ParticleFilterObject.GetComponent<RectTransform>().sizeDelta.y;

        // ParticleSystemの相対位置を計算
        GraphPSDeltaX = GraphParticleSystem.GetComponent<RectTransform>().sizeDelta.x;
        GraphPSDeltaY = GraphParticleSystem.GetComponent<RectTransform>().sizeDelta.y;

        return new Vector2(AfterCanvasSizeWidth + ParticleFilterObjectDeltaX + GraphPSDeltaX, CanvasHeight + ParticleFilterObjectDeltaY + GraphPSDeltaY);
    }

    internal void ProcessMessage()
    {
        GraphParticleSystem.Emit(n_event);
        Particle = new ParticleSystem.Particle[n_event];

        GraphParticleSystem.GetParticles(Particle);
        GraphParticleSystem.Pause();

        int[] pe = countParticle(data.particles_pos);

        float Resolution = (ParticleSystemSize.x - 2) / (n_event - 1);

        for (int i = 0; i < n_event; i++)
        {
            Particle[i].position = UnityEngine.Vector3.Lerp(Particle[i].position, new UnityEngine.Vector3(-(ParticleSystemSize.x * 0.5f) + (i * Resolution), pe[i] * 0.5f * Scale), 0.2f);
            Particle[i].startSize3D = UnityEngine.Vector3.Lerp(Particle[i].startSize3D, new Vector2(Resolution * 0.6f, pe[i] * Scale), 0.2f);

            if (mode_event == i)
            {
                Particle[i].startColor = Color.magenta;
            }
            else
            {
                Particle[i].startColor = Color.white;
            }
        }
        GraphParticleSystem.SetParticles(Particle, n_event);
    }
    internal void MaxEvent()
    {
        int maxValue = data.particles_pos.Max();
        if (n_event < maxValue)
        {
            n_event = maxValue;
        }
    }

    public void ChangeSlider()
    {
        Scale = slider.value;
        Debug.Log(slider.value);
    }

    internal int[] countParticle(int[] p)
    {
        int[] a = new int[n_event + 1];
        int counter = 0;
        for (int i = 0; i < n_event + 1; i++) { a[i] = 0; }
        for (int i = 0; i < n_particle; i++)
        {
            a[p[i]] += 1;
            if (counter < a[p[i]])
            {
                counter = a[p[i]];
                mode_event = p[i];
            }
        }
        return a;
    }
}