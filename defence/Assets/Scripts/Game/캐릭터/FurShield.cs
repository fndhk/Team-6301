// ���� �̸�: FurShield.cs
using UnityEngine;

public class FurShield : MonoBehaviour
{
    private float gaugeCostPerHit;
    private float pushbackForce;
    private float pushbackDuration;

    // ��ų �����͸� �޾ƿͼ� �ʱ�ȭ�ϴ� �Լ�
    public void Initialize(float cost, float force, float duration)
    {
        this.gaugeCostPerHit = cost;
        this.pushbackForce = force;
        this.pushbackDuration = duration;
    }

    void Update()
    {
        // �� ������ ��ų �������� Ȯ��
        if (SkillManager.instance.GetCurrentGauge() <= 0)
        {
            Debug.Log("<color=gray>�� ���� ��Ȱ��ȭ.</color> �������� ��� �Ҹ��߽��ϴ�.");
            // �������� 0�� �Ǹ� �ڽ�(�� ������Ʈ)�� �ı��Ͽ� ��ų�� ����
            Destroy(this);
        }
    }

    /// <summary>
    /// CoreFacility�� ���ݹ޾��� �� ȣ��� �Լ�
    /// </summary>
    public void OnCoreHit(Enemy attacker)
    {
        // 1. ������ ������ ��ġ�� ȿ�� ����
        attacker.ApplyPushback(pushbackForce, pushbackDuration);

        // 2. SkillManager�� ������ �Ҹ�
        SkillManager.instance.ConsumeGauge(gaugeCostPerHit);
    }
}