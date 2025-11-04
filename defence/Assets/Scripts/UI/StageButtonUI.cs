using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StageButtonUI : MonoBehaviour
{
    [Tooltip("버튼의 배경 이미지를 제어할 Image 컴포넌트입니다.")]
    public Image buttonBackground;

    public TextMeshProUGUI stageNameText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI stageNoText;
    public GameObject lockedOverlay;
}