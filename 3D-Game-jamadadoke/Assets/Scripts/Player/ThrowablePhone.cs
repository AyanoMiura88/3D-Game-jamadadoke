using UnityEngine;

// 投擲されたスマホ。NPCに当たるとダメージ＆低確率爆発。
[RequireComponent(typeof(Rigidbody))]
public class ThrowablePhone : MonoBehaviour
{
    [SerializeField] float explosionChance = 0.05f;
    [SerializeField] float explosionRadius = 4f;
    [SerializeField] GameObject explosionVfxPrefab;

    bool hit;

    void OnCollisionEnter(Collision col)
    {
        if (hit) return;

        var npc = col.gameObject.GetComponentInParent<NPCController>();
        if (npc != null)
        {
            hit = true;
            npc.TakeHit(col.contacts[0].point, (col.contacts[0].point - transform.position).normalized * 8f, isThrow: true);
            ScoreManager.Instance.AddThrowScore();

            if (Random.value < explosionChance)
                Explode();
            else
                Destroy(gameObject);
        }
        else if (!col.gameObject.CompareTag("Player"))
        {
            // 床や壁に当たったら消える
            Destroy(gameObject, 2f);
        }
    }

    void Explode()
    {
        if (explosionVfxPrefab)
            Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);

        Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var c in cols)
        {
            var npc = c.GetComponentInParent<NPCController>();
            if (npc != null)
            {
                Vector3 dir = (c.transform.position - transform.position).normalized;
                npc.TakeHit(c.transform.position, dir * 10f, isThrow: true);
                ScoreManager.Instance.AddChainScore();
            }
        }
        Destroy(gameObject);
    }
}
