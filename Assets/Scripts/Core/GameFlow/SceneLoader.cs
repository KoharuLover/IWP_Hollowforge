using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private const string CombatSceneName = "CombatScene";
    private const string ShopSceneName = "ShopScene";

    public static void LoadCombatScene()
    {
        SceneManager.LoadScene(CombatSceneName);
    }

    public static void LoadShopScene()
    {
        SceneManager.LoadScene(ShopSceneName);
    }
}
