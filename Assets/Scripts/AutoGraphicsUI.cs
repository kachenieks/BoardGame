using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script lai automātiski izveidotu Graphics settings UI
/// Uzliec uz SettingsPanel un spied "Create Graphics UI" inspector
/// </summary>
public class AutoGraphicsUI : MonoBehaviour
{
    [Header("Graphics UI Parent")]
    public Transform graphicsUIParent; // Kur izveidot elementus
    
    [Header("Spacing")]
    public float verticalSpacing = 80f;
    public float startY = -200f; // Offset no augšas
    
#if UNITY_EDITOR
    [ContextMenu("Create Graphics UI")]
    public void CreateGraphicsUI()
    {
        if (graphicsUIParent == null)
        {
            Debug.LogError("❌ Graphics UI Parent nav piesķirts!");
            return;
        }
        
        float currentY = startY;
        
        // 1. Izveido Resolution Dropdown
        GameObject resolutionObj = CreateResolutionDropdown(currentY);
        currentY -= verticalSpacing;
        
        // 2. Izveido Fullscreen Toggle
        GameObject fullscreenObj = CreateFullscreenToggle(currentY);
        
        Debug.Log("✅ Graphics UI izveidots!");
        Debug.Log("⚠️ Tagad pievieno references SettingsManager:");
        Debug.Log("   - Resolution Dropdown → ResolutionDropdown");
        Debug.Log("   - Fullscreen Toggle → FullscreenToggle");
    }
    
    GameObject CreateResolutionDropdown(float yPos)
    {
        // Container
        GameObject container = new GameObject("ResolutionContainer");
        container.transform.SetParent(graphicsUIParent, false);
        
        RectTransform containerRt = container.AddComponent<RectTransform>();
        containerRt.anchorMin = new Vector2(0.5f, 1f);
        containerRt.anchorMax = new Vector2(0.5f, 1f);
        containerRt.pivot = new Vector2(0.5f, 1f);
        containerRt.anchoredPosition = new Vector2(0, yPos);
        containerRt.sizeDelta = new Vector2(500, 60);
        
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(container.transform, false);
        
        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = "Rezolūcija:";
        label.fontSize = 24;
        label.alignment = TextAlignmentOptions.MidlineLeft;
        
        RectTransform labelRt = labelObj.GetComponent<RectTransform>();
        labelRt.anchorMin = new Vector2(0, 0.5f);
        labelRt.anchorMax = new Vector2(0, 0.5f);
        labelRt.pivot = new Vector2(0, 0.5f);
        labelRt.anchoredPosition = new Vector2(0, 0);
        labelRt.sizeDelta = new Vector2(200, 40);
        
        // Dropdown
        GameObject dropdownObj = new GameObject("ResolutionDropdown");
        dropdownObj.transform.SetParent(container.transform, false);
        
        TMP_Dropdown dropdown = dropdownObj.AddComponent<TMP_Dropdown>();
        Image dropdownImage = dropdownObj.AddComponent<Image>();
        dropdownImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        
        RectTransform dropdownRt = dropdownObj.GetComponent<RectTransform>();
        dropdownRt.anchorMin = new Vector2(1, 0.5f);
        dropdownRt.anchorMax = new Vector2(1, 0.5f);
        dropdownRt.pivot = new Vector2(1, 0.5f);
        dropdownRt.anchoredPosition = new Vector2(0, 0);
        dropdownRt.sizeDelta = new Vector2(280, 40);
        
        // Label child for dropdown
        GameObject dropdownLabelObj = new GameObject("Label");
        dropdownLabelObj.transform.SetParent(dropdownObj.transform, false);
        
        TextMeshProUGUI dropdownLabel = dropdownLabelObj.AddComponent<TextMeshProUGUI>();
        dropdownLabel.text = "1920 x 1080";
        dropdownLabel.fontSize = 20;
        dropdownLabel.alignment = TextAlignmentOptions.MidlineLeft;
        
        RectTransform dropdownLabelRt = dropdownLabelObj.GetComponent<RectTransform>();
        dropdownLabelRt.anchorMin = Vector2.zero;
        dropdownLabelRt.anchorMax = Vector2.one;
        dropdownLabelRt.sizeDelta = new Vector2(-20, 0);
        dropdownLabelRt.offsetMin = new Vector2(10, 0);
        dropdownLabelRt.offsetMax = new Vector2(-10, 0);
        
        dropdown.captionText = dropdownLabel;
        
        Debug.Log("✅ Resolution Dropdown izveidots");
        return container;
    }
    
    GameObject CreateFullscreenToggle(float yPos)
    {
        // Container
        GameObject container = new GameObject("FullscreenContainer");
        container.transform.SetParent(graphicsUIParent, false);
        
        RectTransform containerRt = container.AddComponent<RectTransform>();
        containerRt.anchorMin = new Vector2(0.5f, 1f);
        containerRt.anchorMax = new Vector2(0.5f, 1f);
        containerRt.pivot = new Vector2(0.5f, 1f);
        containerRt.anchoredPosition = new Vector2(0, yPos);
        containerRt.sizeDelta = new Vector2(500, 60);
        
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(container.transform, false);
        
        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = "Pilnekrāns:";
        label.fontSize = 24;
        label.alignment = TextAlignmentOptions.MidlineLeft;
        
        RectTransform labelRt = labelObj.GetComponent<RectTransform>();
        labelRt.anchorMin = new Vector2(0, 0.5f);
        labelRt.anchorMax = new Vector2(0, 0.5f);
        labelRt.pivot = new Vector2(0, 0.5f);
        labelRt.anchoredPosition = new Vector2(0, 0);
        labelRt.sizeDelta = new Vector2(200, 40);
        
        // Toggle
        GameObject toggleObj = new GameObject("FullscreenToggle");
        toggleObj.transform.SetParent(container.transform, false);
        
        Toggle toggle = toggleObj.AddComponent<Toggle>();
        Image toggleBg = toggleObj.AddComponent<Image>();
        toggleBg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        
        RectTransform toggleRt = toggleObj.GetComponent<RectTransform>();
        toggleRt.anchorMin = new Vector2(1, 0.5f);
        toggleRt.anchorMax = new Vector2(1, 0.5f);
        toggleRt.pivot = new Vector2(1, 0.5f);
        toggleRt.anchoredPosition = new Vector2(0, 0);
        toggleRt.sizeDelta = new Vector2(60, 40);
        
        // Background (child)
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(toggleObj.transform, false);
        
        Image bg = bgObj.AddComponent<Image>();
        bg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        RectTransform bgRt = bgObj.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.sizeDelta = Vector2.zero;
        
        // Checkmark (child of Background)
        GameObject checkmarkObj = new GameObject("Checkmark");
        checkmarkObj.transform.SetParent(bgObj.transform, false);
        
        Image checkmark = checkmarkObj.AddComponent<Image>();
        checkmark.color = new Color(0.2f, 0.8f, 0.2f, 1f);
        
        RectTransform checkmarkRt = checkmarkObj.GetComponent<RectTransform>();
        checkmarkRt.anchorMin = Vector2.zero;
        checkmarkRt.anchorMax = Vector2.one;
        checkmarkRt.sizeDelta = new Vector2(-10, -10);
        
        toggle.targetGraphic = bg;
        toggle.graphic = checkmark;
        
        Debug.Log("✅ Fullscreen Toggle izveidots");
        return container;
    }
#endif
}