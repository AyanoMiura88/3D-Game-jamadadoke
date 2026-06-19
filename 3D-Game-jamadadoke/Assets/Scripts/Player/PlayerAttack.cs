using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("近接攻撃")]
    [SerializeField] float slapRange = 2f;
    [SerializeField] float slapRadius = 1f;
    [SerializeField] float slapCooldown = 0.4f;
    [SerializeField] float knockbackForce = 7f;

    [Header("投擲")]
    [SerializeField] GameObject phonePrefab;
    [SerializeField] Transform throwOrigin;
    [SerializeField] float throwForce = 20f;
    [SerializeField] float throwCooldown = 0.8f;

    [Header("カメラ参照")]
    [SerializeField] Camera mainCamera;

    float slapTimer;
    float throwTimer;

    // 手持ちスマホ（奪ったもの）
    int heldPhones;
    public int HeldPhones => heldPhones;

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;

        slapTimer  -= Time.deltaTime;
        throwTimer -= Time.deltaTime;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            TrySlap();

        if (Mouse.current.rightButton.wasPressedThisFrame && heldPhones > 0)
            TryThrow();
    }

    void TrySlap()
    {
        if (slapTimer > 0f) return;
        slapTimer = slapCooldown;

        Vector3 origin = mainCamera.transform.position;
        Vector3 dir    = mainCamera.transform.forward;

        // SphereCastで近くのNPCを捉える
        RaycastHit[] hits = Physics.SphereCastAll(origin, slapRadius, dir, slapRange);
        foreach (var h in hits)
        {
            var npc = h.collider.GetComponentInParent<NPCController>();
            if (npc == null) continue;

            Vector3 kb = (h.transform.position - transform.position).normalized * knockbackForce;
            kb.y = 3f;
            bool gotPhone = npc.TakeHit(h.point, kb, isThrow: false);

            if (gotPhone)
                heldPhones++;

            ScoreManager.Instance.AddHitScore();
            break; // 1回の叩きで1体
        }
    }

    void TryThrow()
    {
        if (throwTimer > 0f) return;
        throwTimer = throwCooldown;

        heldPhones--;

        var phone = Instantiate(phonePrefab, throwOrigin.position, Quaternion.identity);
        var rb = phone.GetComponent<Rigidbody>();
        rb.linearVelocity = mainCamera.transform.forward * throwForce;

        // 自分自身との衝突を無視
        Physics.IgnoreCollision(phone.GetComponent<Collider>(),
            GetComponent<Collider>());
    }

    // NPCが倒れた際に呼ばれる（スマホ自動取得）
    public void PickUpPhone()
    {
        heldPhones++;
    }
}
