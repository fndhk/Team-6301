using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance; // �̱��� �ν��Ͻ�

    // ���� �ִ� ��� Ÿ���� ������ ����Ʈ
    private List<Tower> allTowers = new List<Tower>();
    private bool isBuffActive = false; // ���� �ߺ� ���� �÷���
    private bool isAttackSpeedBuffActive = false;
    private bool isDamageBuffActive = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Ÿ���� ������ �� ����Ʈ�� �߰��ϴ� �Լ�
    public void RegisterTower(Tower tower)
    {
        if (!allTowers.Contains(tower))
        {
            allTowers.Add(tower);
        }
    }

    public void ApplyBuff(ItemData itemData)
    {
        // �������� effectType�� ���� �ٸ� ó���� ��
        switch (itemData.effectType)
        {
            case EffectType.AttackSpeed:
                StartCoroutine(AttackSpeedBuffCoroutine(itemData.effectValue, itemData.buffDuration));
                break;
            case EffectType.Damage:
                StartCoroutine(DamageBuffCoroutine(itemData.effectValue, itemData.buffDuration));
                break;
        }
    }
    private IEnumerator AttackSpeedBuffCoroutine(float multiplier, float duration)
    {
        if (isAttackSpeedBuffActive) yield break; // �ߺ� ���� ����

        isAttackSpeedBuffActive = true;
        foreach (Tower tower in allTowers) tower.SetFireRateMultiplier(multiplier);

        yield return new WaitForSeconds(duration);

        foreach (Tower tower in allTowers) tower.SetFireRateMultiplier(1f);
        isAttackSpeedBuffActive = false;
    }
    private IEnumerator DamageBuffCoroutine(float multiplier, float duration)
    {
        if (isDamageBuffActive) yield break; // �ߺ� ���� ����

        isDamageBuffActive = true;
        Debug.Log($"������ ���� ����! ({multiplier}��, {duration}��)");

        foreach (Tower tower in allTowers)
        {
            tower.SetDamageMultiplier(multiplier);
        }

        yield return new WaitForSeconds(duration);

        foreach (Tower tower in allTowers)
        {
            tower.SetDamageMultiplier(1f);
        }

        Debug.Log("������ ���� ����.");
        isDamageBuffActive = false;
    }

    // ������ ��� �� ȣ��� ���� ���� �Լ�
    public void ApplyAttackSpeedBuff(float multiplier, float duration)
    {
        // �̹� �ٸ� ���� �ӵ� ������ Ȱ��ȭ�Ǿ� �ִٸ� �ߺ� ���� ����
        if (isBuffActive)
        {
            Debug.Log("�̹� ���� �ӵ� ������ Ȱ��ȭ�Ǿ� �ֽ��ϴ�.");
            return;
        }
        StartCoroutine(AttackSpeedBuffCoroutine(multiplier, duration));
    }
    /*
    private IEnumerator AttackSpeedBuffCoroutine(float multiplier, float duration)
    {
        isBuffActive = true;
        Debug.Log($"���� �ӵ� ���� ����! ({multiplier}��, {duration}��)");

        // ��� Ÿ���� ���� �ӵ��� ������Ŵ
        foreach (Tower tower in allTowers)
        {
            tower.SetFireRateMultiplier(multiplier);
        }

        // ������ �ð�(duration)��ŭ ���
        yield return new WaitForSeconds(duration);

        // ��� Ÿ���� ���� �ӵ��� ������� �ǵ���
        foreach (Tower tower in allTowers)
        {
            tower.SetFireRateMultiplier(1f); // 1������� �ǵ���
        }

        Debug.Log("���� �ӵ� ���� ����.");
        isBuffActive = false;
    }
    */
}