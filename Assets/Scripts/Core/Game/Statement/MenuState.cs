using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuState : State
{
    public static new MenuState Instance => (MenuState)State.Instance;

#if UNITY_EDITOR
    [SerializeField] private UnityEditor.SceneAsset WordLinkScene;
    [SerializeField] private UnityEditor.SceneAsset WordPickScene;
    [SerializeField] private UnityEditor.SceneAsset WordBlastScene;
#endif
    [SerializeField] private string targetSceneWordLink;
    [SerializeField] private string targetSceneWordPick;
    [SerializeField] private string targetSceneWordBlast;

    protected override void Awake()
    {
        if (UIModule.OpenCanvas<MainMenuCanvas>(out var mainMenuCanvas))
        {
            mainMenuCanvas.OpenPanel<MainMenuPanel>();
        }
    }

    public void LoadScene(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.link:
                LoadSceneWordLink();
                break;
            case SceneType.pick:
                LoadSceneWordPick();
                break;
            case SceneType.blast:
                LoadSceneWordBlast();
                break;
        }
    }

    void LoadSceneWordLink()
    {
        SceneManager.LoadScene(targetSceneWordLink); 
    }

    void LoadSceneWordPick()
    { 
        SceneManager.LoadScene(targetSceneWordPick);
    }

    void LoadSceneWordBlast()
    {
        SceneManager.LoadScene(targetSceneWordBlast); 
    }

    protected override void Start()
    {
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (WordLinkScene) targetSceneWordLink = WordLinkScene.name;
        if (WordPickScene) targetSceneWordPick = WordPickScene.name;
        if (WordBlastScene) targetSceneWordBlast = WordBlastScene.name;
    }
#endif

    public enum SceneType { link, pick, blast}
}
