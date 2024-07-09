using UnityEngine;
using UnityEngine.UI;

public class LockLevelSystem : MonoBehaviour
{
    [SerializeField] private Button[] levels;
    private static int _unlockedLevels;
    
    private void OnEnable()
    {
        LoadGameData();
        for (var i = 0; i < levels.Length; i++) levels[i].interactable = i < _unlockedLevels;
    }
    
    public static void LevelUp(int levelNumber)
    {
        if (_unlockedLevels == levelNumber)
        {
            _unlockedLevels++;
            SaveGameData();
        }
    }
   
    private static void SaveGameData()
    {
    
        PlayerPrefs.SetInt("UnlockedLevels", _unlockedLevels);
        PlayerPrefs.Save();
    }
    private void LoadGameData()
    {
        _unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);
    }
    
   
}