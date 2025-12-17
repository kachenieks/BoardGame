using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Uzliec uz PauseMenuPanel, SettingsPanel un LeaderboardPanel
/// Nodrošina ka UI strādā ar Time.timeScale = 0
/// </summary>
public class PauseUIFixer : MonoBehaviour
{
    void OnEnable()
    {
        FixAllAnimators();
        FixAllScrollRects();
        
        Debug.Log($"✅ PauseUIFixer aktivizēts uz {gameObject.name}");
    }
    
    void FixAllAnimators()
    {
        // Visi animatori uz Unscaled Time
        Animator[] animators = GetComponentsInChildren<Animator>(true);
        
        foreach (Animator anim in animators)
        {
            if (anim.updateMode != AnimatorUpdateMode.UnscaledTime)
            {
                anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                Debug.Log($"🔧 {anim.name} animator → UnscaledTime");
            }
        }
    }
    
    void FixAllScrollRects()
    {
        // Visi scroll rects strādā ar unscaled time
        ScrollRect[] scrolls = GetComponentsInChildren<ScrollRect>(true);
        
        foreach (ScrollRect scroll in scrolls)
        {
            scroll.scrollSensitivity = 10f; // Palielina sensitivity
            Debug.Log($"🔧 {scroll.name} scroll sensitivity uzstādīta");
        }
    }
    
    void Update()
    {
        // Ja ir pauze, force enable visus UI elementus
        if (Time.timeScale == 0f)
        {
            // Nodrošina ka visi buttoni ir interactable
            Button[] buttons = GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                if (!btn.interactable && btn.gameObject.activeInHierarchy)
                {
                    btn.interactable = true;
                }
            }
            
            // Nodrošina ka visi slideri ir interactable
            Slider[] sliders = GetComponentsInChildren<Slider>(true);
            foreach (Slider slider in sliders)
            {
                if (!slider.interactable && slider.gameObject.activeInHierarchy)
                {
                    slider.interactable = true;
                }
            }
            
            // Nodrošina ka visi toggle ir interactable
            Toggle[] toggles = GetComponentsInChildren<Toggle>(true);
            foreach (Toggle toggle in toggles)
            {
                if (!toggle.interactable && toggle.gameObject.activeInHierarchy)
                {
                    toggle.interactable = true;
                }
            }
        }
    }
}