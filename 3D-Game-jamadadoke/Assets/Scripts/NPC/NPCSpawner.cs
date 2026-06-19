using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] GameObject phoneUserPrefab;
    [SerializeField] GameObject innocentPrefab;
    [SerializeField] int maxPhoneUsers = 15;
    [SerializeField] int maxInnocents  = 10;
    [SerializeField] float spawnInterval = 2f;
    [SerializeField] float spawnRadius = 20f;

    float timer;

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = spawnInterval;

        TrySpawn(phoneUserPrefab, maxPhoneUsers, NPCController.NPCType.PhoneUser);
        TrySpawn(innocentPrefab,  maxInnocents,  NPCController.NPCType.Innocent);
    }

    void TrySpawn(GameObject prefab, int max, NPCController.NPCType type)
    {
        // 現在のシーン上のNPC数をカウント（シンプルに都度検索）
        var existing = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
        int count = 0;
        foreach (var n in existing)
            if (n.npcType == type) count++;

        if (count >= max) return;

        Vector3 point = RandomNavPoint();
        if (point == Vector3.zero) return;

        Instantiate(prefab, point, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
    }

    Vector3 RandomNavPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 rand = transform.position + Random.insideUnitSphere * spawnRadius;
            rand.y = transform.position.y;
            if (NavMesh.SamplePosition(rand, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                return hit.position;
        }
        return Vector3.zero;
    }
}
