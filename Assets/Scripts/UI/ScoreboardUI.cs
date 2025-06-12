using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ScoreboardUI : MonoBehaviour
{
    [Header("按键切换显示")]
    public KeyCode toggleKey = KeyCode.Tab;

    [Header("请在此处拖入你希望承载排行榜的 Canvas")]
    public Canvas parentCanvas;        // ← 新增：在 Inspector 指定父 Canvas

    private Canvas           uiCanvas;
    private GameObject       panel;
    private GameObject       content;
    private bool             isShowing  = false;
    private List<GameObject> rowObjects = new List<GameObject>();

    void Awake()
    {
        Debug.Log("[ScoreboardUI] Awake(), 开始创建排行榜 UI");
        // 优先用 Inspector 指定的 Canvas，否则回退到查找
        uiCanvas = parentCanvas != null
            ? parentCanvas
            : FindObjectOfType<Canvas>();

        if (uiCanvas == null)
        {
            Debug.LogWarning("[ScoreboardUI] 未找到任何 Canvas，自动新建一个");
            var cGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            uiCanvas = cGO.GetComponent<Canvas>();
            uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = cGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        CreateUI();
        panel.SetActive(false);
    }

    void Start()
    {
        if (NetScoreManager.Instance != null)
            NetScoreManager.Instance.OnScoresUpdated += OnScoresUpdated;
        else
            Debug.LogError("ScoreboardUI: 找不到 NetScoreManager.Instance");
    }

    void OnDestroy()
    {
        if (NetScoreManager.Instance != null)
            NetScoreManager.Instance.OnScoresUpdated -= OnScoresUpdated;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey) && !isShowing)
        {
            isShowing = true;
            RefreshAndShow();
        }
        else if (Input.GetKeyUp(toggleKey) && isShowing)
        {
            isShowing = false;
            panel.SetActive(false);
        }
    }

    void OnScoresUpdated(Dictionary<int,int> _)
    {
        if (isShowing)
            RefreshAndShow();
    }

    void CreateUI()
    {
        // 1) 在 uiCanvas 下创建背景 Panel
        panel = new GameObject("ScoreboardPanel",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(VerticalLayoutGroup));
        panel.transform.SetParent(uiCanvas.transform, false);
        var rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.2f, 0.2f);
        rt.anchorMax = new Vector2(0.8f, 0.8f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0,0,0,0.6f);
        var vlg = panel.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = 10;
        vlg.padding = new RectOffset(20,20,20,20);

        // 2) 标题
        var header = new GameObject("Title", typeof(Text), typeof(LayoutElement));
        header.transform.SetParent(panel.transform, false);
        var ht = header.GetComponent<Text>();
        ht.text = "排行榜";
        ht.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        ht.fontSize = 36;
        ht.alignment = TextAnchor.MiddleCenter;
        ht.color = Color.white;
        header.GetComponent<LayoutElement>().preferredHeight = 50;

        // 3) ScrollView
        var sv = new GameObject("ScrollView",
            typeof(Image), typeof(Mask), typeof(ScrollRect));
        sv.transform.SetParent(panel.transform, false);
        var svRT = sv.GetComponent<RectTransform>();
        svRT.anchorMin = Vector2.zero;
        svRT.anchorMax = Vector2.one;
        svRT.offsetMin = new Vector2(0, -70);
        svRT.offsetMax = Vector2.zero;
        sv.GetComponent<Image>().color = new Color(1,1,1,0);
        sv.GetComponent<Mask>().showMaskGraphic = false;
        var scroll = sv.GetComponent<ScrollRect>();
        scroll.horizontal = false;

        // 4) Viewport
        var vp = new GameObject("Viewport", typeof(Image), typeof(Mask));
        vp.transform.SetParent(sv.transform, false);
        var vpRT = vp.GetComponent<RectTransform>();
        vpRT.anchorMin = Vector2.zero;
        vpRT.anchorMax = Vector2.one;
        vpRT.offsetMin = vpRT.offsetMax = Vector2.zero;
        vp.GetComponent<Image>().color = Color.clear;
        vp.GetComponent<Mask>().showMaskGraphic = false;
        scroll.viewport = vpRT;

        // 5) Content
        content = new GameObject("Content",
            typeof(VerticalLayoutGroup),
            typeof(ContentSizeFitter));
        content.transform.SetParent(vp.transform, false);
        var cRT = content.GetComponent<RectTransform>();
        cRT.anchorMin = new Vector2(0,1);
        cRT.anchorMax = Vector2.one;
        cRT.pivot = new Vector2(0.5f,1f);
        cRT.offsetMin = cRT.offsetMax = Vector2.zero;
        var cvlg = content.GetComponent<VerticalLayoutGroup>();
        cvlg.spacing = 5;
        content.GetComponent<ContentSizeFitter>()
            .verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scroll.content = cRT;

        // 6) 确保在最前
        panel.transform.SetAsLastSibling();
        panel.GetComponentInParent<Canvas>().sortingOrder = 100;
    }



    void RefreshAndShow()
    {
        panel.transform.SetAsLastSibling();
        rowObjects.ForEach(Destroy);
        rowObjects.Clear();

        var list = NetScoreManager.Instance.GetSortedScores();
        Debug.Log($"[ScoreboardUI] Tab pressed: {PhotonNetwork.PlayerList.Length} players, {list.Count} scores");

        foreach (var (nick, score) in list)
        {
            var row = new GameObject("Row", typeof(HorizontalLayoutGroup));
            row.transform.SetParent(content.transform, false);
            var hlg = row.GetComponent<HorizontalLayoutGroup>();
            hlg.spacing = 20;

            var nameGO = new GameObject("Name", typeof(Text));
            nameGO.transform.SetParent(row.transform, false);
            var t1 = nameGO.GetComponent<Text>();
            t1.text = nick;
            t1.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t1.fontSize = 24;
            t1.color = Color.white;

            var scoreGO = new GameObject("Score", typeof(Text));
            scoreGO.transform.SetParent(row.transform, false);
            var t2 = scoreGO.GetComponent<Text>();
            t2.text = score.ToString();
            t2.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t2.fontSize = 24;
            t2.color = Color.white;

            rowObjects.Add(row);
        }

        panel.SetActive(true);
    }

}
