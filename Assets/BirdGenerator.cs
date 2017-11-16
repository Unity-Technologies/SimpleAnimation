using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdGenerator : MonoBehaviour {
    public GameObject m_birdPrefab;
    public Transform[] m_spawnPositions;
    public float m_interval = 1f;
    float m_timer;
    float m_birdPower = 1.0f;

	void Update ()
    {
        m_timer += Time.deltaTime;

        if (m_timer < m_interval) return;

        m_timer = 0;
        int i = Random.Range(0, m_spawnPositions.Length);
        GameObject go = Instantiate(m_birdPrefab, m_spawnPositions[i].position, m_birdPrefab.transform.rotation);
        float p = Random.Range(0.5f, 2f);
        go.GetComponent<BirdController>().Power = m_birdPower;
        m_birdPower += 0.1f;
	}
}
