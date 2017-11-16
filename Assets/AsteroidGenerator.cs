using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{
    public GameObject m_asteroidPrefab;
    public Transform[] m_spawnPositions;
    public float m_initialInterval = 1.0f;
    public float m_minimumInterval = 0.2f;
    public float m_IntervalStep = 0.03f;
    float m_timer;
    float m_interval;

	void Start ()
    {
        m_interval = m_initialInterval;
	}
	
	void Update ()
    {
        m_timer += Time.deltaTime;
        if (m_timer < m_interval) return;

        m_timer = 0f;
        int i = Random.Range(0, m_spawnPositions.Length);
        Instantiate(m_asteroidPrefab, m_spawnPositions[i].position, m_asteroidPrefab.transform.rotation);
        m_interval = Mathf.Max(m_minimumInterval, m_interval - m_IntervalStep);    
	}
}
