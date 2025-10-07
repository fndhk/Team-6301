using System.Collections.Generic; // List를 사용하기 위해 필요합니다.

// [System.Serializable] 어트리뷰트는 이 클래스의 내용을
// 파일로 저장하거나 다른 형식으로 변환(직렬화)할 수 있게 만들어줍니다.
[System.Serializable]
public class GameData
{
    // 저장할 데이터 항목들
    public string nickname;
    public int playerLevel;
    public int currentStage;
    public List<string> items;
    public string lastSaveTime;

    // 게임을 처음 시작할 때 사용할 기본값 설정 (생성자)
    public GameData()
    {
        this.nickname = "";
        this.playerLevel = 1;
        this.currentStage = 1;
        this.items = new List<string>();
        this.lastSaveTime = "";
    }
}