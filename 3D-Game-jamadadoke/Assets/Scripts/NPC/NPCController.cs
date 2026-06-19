using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class NPCController : MonoBehaviour
{
    public enum NPCType { PhoneUser, Innocent }

    [Header("設定")]
    [SerializeField] public NPCType npcType = NPCType.PhoneUser;
    [SerializeField] float wanderRadius = 10f;
    [SerializeField] float wanderInterval = 3f;

    [Header("状態")]
    [SerializeField] Renderer bodyRenderer;
    [SerializeField] Color phoneUserColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] Color innocentColor  = new Color(0.9f, 0.85f, 0.75f);
    [SerializeField] Color stunnedColor   = Color.yellow;

    NavMeshAgent agent;
    Rigidbody rb;
    bool isDead;
    bool isStunned;
    bool hasPhone;

    // 連鎖用：このNPCが倒れた衝撃で隣接NPCを巻き込む
    [SerializeField] float chainRadius = 2f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb    = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        hasPhone = (npcType == NPCType.PhoneUser);
    }

    void Start()
    {
        ApplyColor();
        StartCoroutine(WanderRoutine());
    }

    void ApplyColor()
    {
        if (bodyRenderer == null) return;
        bodyRenderer.material.color = npcType == NPCType.PhoneUser ? phoneUserColor : innocentColor;
    }

    IEnumerator WanderRoutine()
    {
        while (!isDead)
        {
            if (!isStunned && agent.enabled)
            {
                Vector3 dest = RandomNavPoint();
                agent.SetDestination(dest);
            }
            yield return new WaitForSeconds(wanderInterval + Random.Range(-0.5f, 0.5f));
        }
    }

    Vector3 RandomNavPoint()
    {
        Vector3 random = transform.position + Random.insideUnitSphere * wanderRadius;
        random.y = transform.position.y;
        if (NavMesh.SamplePosition(random, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            return hit.position;
        return transform.position;
    }

    // 攻撃を受けた。戻り値: スマホを落としたか
    public bool TakeHit(Vector3 hitPoint, Vector3 knockback, bool isThrow)
    {
        if (isDead) return false;

        bool droppedPhone = false;

        if (npcType == NPCType.Innocent)
        {
            GameManager.Instance.ReportInnocentKilled();
            StunAndKnockback(knockback);
            return false;
        }

        if (hasPhone && !isStunned)
        {
            droppedPhone = true;
            hasPhone = false;
            TriggerChain(knockback);
        }

        StunAndKnockback(knockback);
        return droppedPhone;
    }

    void StunAndKnockback(Vector3 force)
    {
        StopCoroutine(WanderRoutine());
        isStunned = true;
        agent.enabled = false;
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.VelocityChange);

        if (bodyRenderer) bodyRenderer.material.color = stunnedColor;

        StartCoroutine(RecoverRoutine());
    }

    IEnumerator RecoverRoutine()
    {
        yield return new WaitForSeconds(2f);
        if (isDead) yield break;

        rb.isKinematic = true;
        isStunned = false;

        // NavMesh上に位置を戻す
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            transform.position = hit.position;

        agent.enabled = true;
        ApplyColor();
        StartCoroutine(WanderRoutine());
    }

    // ドミノ倒し：周囲の歩きスマホを巻き込む
    void TriggerChain(Vector3 sourceForce)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, chainRadius);
        foreach (var c in cols)
        {
            if (c.gameObject == gameObject) continue;
            var other = c.GetComponentInParent<NPCController>();
            if (other != null && other.npcType == NPCType.PhoneUser && !other.isStunned)
            {
                Vector3 dir = (other.transform.position - transform.position).normalized;
                other.TakeHit(other.transform.position, dir * sourceForce.magnitude * 0.6f, isThrow: false);
                ScoreManager.Instance.AddChainScore();
            }
        }
    }

    public void SetDead()
    {
        isDead = true;
        agent.enabled = false;
        rb.isKinematic = false;
        Destroy(gameObject, 5f);
    }
}
