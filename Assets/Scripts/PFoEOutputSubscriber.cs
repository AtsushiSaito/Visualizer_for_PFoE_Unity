using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using ROSBridgeSharp.Messages;

public class PFoEOutputSubscriber : MonoBehaviour
{
    ROSBridgeSharp.Messages.PFoEOutput data;

    private int N_Particle = 1000;
    private int EventLength = 0;
    private int ModeEvent = -1;

    private float Scale = 3.0f;
    private float CanvasWidth, CanvasHeight;
    private float ParticleFilterObjectDeltaX, ParticleFilterObjectDeltaY;
    private float GraphPSDeltaX, GraphPSDeltaY;
    private Vector2 ParticleSystemSize;
    private ParticleSystem.Particle[] Particles;

    private int[] CountedParticleArray;

    public Text PFoEOutputLabel;
    public Canvas MainCanvas;
    public GameObject SafeArea;
    public GameObject ParticleFilterObject;
    public ParticleSystem GraphParticleSystem;
    public Slider ParticleVolumeSlider;

    private void Awake()
    {
        // サブスクライブを定義
        RBSubscriber<PFoEOutput> s = new RBSubscriber<PFoEOutput>("/pfoe_out", Callback);
        ViewSizeCalculation();
        Application.targetFrameRate = 60;
    }

    private void Callback(PFoEOutput msg)
    {
        data = msg;
    }

    private void Update()
    {
        if (data != null)
        {
            // ラベルの更新
            PFoEOutputLabel.text = "Eta : " + data.eta.ToString() + "\n";
            PFoEOutputLabel.text += "Estimate Event : " + (EventLength - 1).ToString();

            // 各計算
            CheckMaxEvent();
            ViewSizeCalculation();
            ProcessMessage();
        }
    }

    private void ViewSizeCalculation()
    {
        // Canvasのサイズを計算
        CanvasWidth = MainCanvas.pixelRect.width;
        CanvasHeight = MainCanvas.pixelRect.height;

        //セーフエリアのオブジェクトを取得して、アンカーポイントを取得
        RectTransform rectTransform = SafeArea.GetComponent<RectTransform>();
        Vector2 amax = rectTransform.anchorMax;
        Vector2 amin = rectTransform.anchorMin;

        //取得したアンカーポイントから実際の表示領域を計算
        float AfterCanvasSizeWidth = (amax.x - amin.x) * CanvasWidth;
        float AfterCanvasSizeHeight = (amax.y - amin.y) * CanvasHeight;

        // ParticleFilterオブジェクトの相対位置を計算
        ParticleFilterObjectDeltaX = ParticleFilterObject.GetComponent<RectTransform>().sizeDelta.x;
        ParticleFilterObjectDeltaY = ParticleFilterObject.GetComponent<RectTransform>().sizeDelta.y;

        // ParticleSystemの相対位置を計算
        GraphPSDeltaX = GraphParticleSystem.GetComponent<RectTransform>().sizeDelta.x;
        GraphPSDeltaY = GraphParticleSystem.GetComponent<RectTransform>().sizeDelta.y;

        // パーティクルシステムを表示する領域を設定
        ParticleSystemSize.x = AfterCanvasSizeWidth + ParticleFilterObjectDeltaX + GraphPSDeltaX;
        ParticleSystemSize.y = CanvasHeight + ParticleFilterObjectDeltaY + GraphPSDeltaY;
    }

    private void ProcessMessage()
    {
        // パーティクルの放出
        GraphParticleSystem.Emit(EventLength);
        // EventLength個のパーティクルを生成
        Particles = new ParticleSystem.Particle[EventLength];
        // パーティクルを取り出す
        GraphParticleSystem.GetParticles(Particles);
        // パーティクルの動作を停止させる
        GraphParticleSystem.Pause();

        // イベントの出現頻度を計算
        CountParticle();

        // グラフの解像度(棒の幅)
        float Resolution = (ParticleSystemSize.x - 2) / (EventLength - 1);

        for (int i = 0; i < EventLength; i++)
        {
            // 変化後の座標を計算
            Vector2 AfterChangePosition = new UnityEngine.Vector3(-(ParticleSystemSize.x * 0.5f) + (i * Resolution), CountedParticleArray[i] * 0.5f * Scale);
            Particles[i].position = UnityEngine.Vector3.Lerp(Particles[i].position, AfterChangePosition, 0.2f);

            // 変化後のスケールを計算
            Vector2 AfterChangeSize = new Vector2(Resolution * 0.6f, CountedParticleArray[i] * Scale);
            Particles[i].startSize3D = UnityEngine.Vector3.Lerp(Particles[i].startSize3D, AfterChangeSize, 0.2f);

            if (ModeEvent == i)
            {
                // モードだけマゼンタで色付け
                Particles[i].startColor = Color.magenta;
            }
            else
            {
                // モード以外は白で色付け
                Particles[i].startColor = Color.white;
            }
        }
        GraphParticleSystem.SetParticles(Particles, EventLength);
    }

    // 最大イベントの計算
    private void CheckMaxEvent()
    {
        int maxEvent = data.particles_pos.Max();
        if (EventLength < maxEvent)
        {
            // イベントの長さを更新
            EventLength = maxEvent;
            // イベントの長さ+1で配列初期化
            CountedParticleArray = new int[EventLength + 1];
        }
    }

    // グラフのボリューム調整
    public void ChangeSlider()
    {
        Scale = ParticleVolumeSlider.value;
    }

    // イベントの出現頻度を計算
    private void CountParticle()
    {
        int counter = 0;
        for (int i = 0; i < EventLength + 1; i++) { CountedParticleArray[i] = 0; }
        for (int i = 0; i < N_Particle; i++)
        {
            CountedParticleArray[data.particles_pos[i]] += 1;
            if (counter < CountedParticleArray[data.particles_pos[i]])
            {
                counter = CountedParticleArray[data.particles_pos[i]];
                ModeEvent = data.particles_pos[i];
            }
        }
    }
}