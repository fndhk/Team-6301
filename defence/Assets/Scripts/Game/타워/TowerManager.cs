// ���� �̸�: TowerManager.cs (�ܼ� ���� ���� ����)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance;

    private List<Tower> allTowers = new List<Tower>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RegisterTower(Tower tower)
    {
        if (!allTowers.Contains(tower))
        {
            allTowers.Add(tower);
        }
    }

    // ������ �����͸� �޾� ������ �´� �ڷ�ƾ�� ��������ִ� ���Ҹ� ��
    public void ApplyBuff(ItemData itemData)
    {
        // ������ Ÿ���� ���� �ڷ�ƾ�� �ִٸ� ���� (���� Ŭ�� �� Ÿ�̸� ���� ����)
        if (itemData.effectType == EffectType.AttackSpeed)
        {
            StopCoroutine("AttackSpeedBuffCoroutine");
            StartCoroutine(AttackSpeedBuffCoroutine(itemData.effectValue, itemData.buffDuration));
        }
        else if (itemData.effectType == EffectType.Damage)
        {
            StopCoroutine("DamageBuffCoroutine");
            StartCoroutine(DamageBuffCoroutine(itemData.effectValue, itemData.buffDuration));
        }
    }

    // ���� �ӵ� ���� �ڷ�ƾ
    private IEnumerator AttackSpeedBuffCoroutine(float multiplier, float duration)
    {
        Debug.Log("���� �ӵ� ���� ����!");
        foreach (Tower tower in allTowers) if (tower != null) tower.SetFireRateMultiplier(multiplier);

        yield return new WaitForSeconds(duration);

        foreach (Tower tower in allTowers) if (tower != null) tower.SetFireRateMultiplier(1f);
        Debug.Log("���� �ӵ� ���� ����.");
    }

    // ������ ���� �ڷ�ƾ
    private IEnumerator DamageBuffCoroutine(float multiplier, float duration)
    {
        Debug.Log("������ ���� ����!");
        foreach (Tower tower in allTowers) if (tower != null) tower.SetDamageMultiplier(multiplier);

        yield return new WaitForSeconds(duration);

        foreach (Tower tower in allTowers) if (tower != null) tower.SetDamageMultiplier(1f);
        Debug.Log("������ ���� ����.");
    }
}