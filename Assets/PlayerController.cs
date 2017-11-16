using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SimpleAnimation))]
public class PlayerController : MonoBehaviour
{
    Rigidbody2D m_rb;
    [SerializeField] float m_moveSpeed = 10f;
    [SerializeField] float m_jumpSpeed = 15f;
    [SerializeField] float m_damageDurationSeconds = 1f;
    bool m_isGrounded;
    /// <summary>ダメージを受けて固まっているか</summary>
    bool m_isFrozen;
    SimpleAnimation m_anim;
    Vector3 m_spawnPosition;

	void Start ()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_anim = GetComponent<SimpleAnimation>();
        m_spawnPosition = transform.position;
	}

    void Update ()
    {
        // ダメージを受けている時は何もしない
        if (m_isFrozen) return;

        // 入力を受け付けてキャラクターを動かす
        float h = Input.GetAxis("Horizontal");

        m_rb.velocity = new Vector2(h * m_moveSpeed, m_rb.velocity.y);

        if (m_isGrounded && (Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1")))
        {
            m_isGrounded = false;
            m_rb.velocity = Vector2.up * m_jumpSpeed;
        }

        // キャラクターの向きを調整する
        if (m_rb.velocity.x * transform.localScale.x < 0)
            transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, transform.localScale.z);

        // アニメーションを制御する
        if (m_isGrounded)
        {
            if (m_rb.velocity.x == 0)
                m_anim.Play("Footwork");
            else
                m_anim.Play("Move");
        }
        else
        {
            if (m_rb.velocity.y > 0)
                m_anim.Play("JumpUp");
            else if (m_rb.velocity.y < 0)
                m_anim.Play("JumpDown");
        }

        // 落ちたら元の位置に戻す
        if (transform.position.y < -10f)
            transform.position = m_spawnPosition;
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground") m_isGrounded = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            m_isFrozen = true;
            m_anim.Play("Damage");
            Invoke("RecoverFrozen", m_damageDurationSeconds);
        }
    }

    void RecoverFrozen()
    {
        m_isFrozen = false;
    }
}
