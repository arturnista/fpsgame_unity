﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerGunSpread {
	public int bullet;
	public Vector2 position;
}

public class PlayerGun : MonoBehaviour {

	[SerializeField]
	private int m_MagazineSize;
	[SerializeField]
	private int m_MaxAmmo;
	[SerializeField]
	private float m_ReloadTime;
	[SerializeField]
	private float m_Damage;

	[SerializeField]
	private bool m_IsAutomatic;

	[SerializeField]
	private float m_FireRate;
	private float m_FireDelay;

	[Header("Spread")]
	[SerializeField]
	private float m_DefaultSpread = .1f;
	[SerializeField]
	private float m_RecoverSpreadDelay = .2f;
	private float m_RecoverSpreadTime;
	private int m_CurrentSpreadBullet;
	[SerializeField]
	private List<PlayerGunSpread> m_SpreadList;
	private int m_CurrentSpread;

	private Vector3 m_OriginalPosition;
	private Vector3 m_OriginalEuler;

	private int m_CurrentMagazine;
	private int m_PlayerSpeed;

	private float m_NextShootTime;

	private bool m_IsShooting;
	private bool m_IsReloading;

	private Transform m_Head;
	private PlayerMovement m_PlayerMovement;
	private Animator m_Animator;
	private ParticleSystem m_Flash;

	void Awake () {
		m_Flash = GetComponentInChildren<ParticleSystem>();
		m_PlayerMovement = GetComponentInParent<PlayerMovement>();
		m_Animator = GetComponent<Animator>();

		m_FireDelay = 1 / m_FireRate;
		m_CurrentMagazine = m_MagazineSize;

		m_CurrentSpread = 0;
		m_CurrentSpreadBullet = 0;

		m_OriginalPosition = transform.localPosition;
		m_OriginalEuler = transform.localEulerAngles;
	}

	void Update() {
		m_PlayerSpeed = Mathf.RoundToInt(m_PlayerMovement.planeVelocity.sqrMagnitude);
		m_Animator.SetInteger("sqrSpeed", m_PlayerSpeed);
		m_Animator.SetBool("isGrounded", m_PlayerMovement.isGrounded);

		if(!m_IsShooting && m_CurrentSpreadBullet > 0) {
			m_RecoverSpreadTime += Time.deltaTime;
			if(m_RecoverSpreadTime > m_RecoverSpreadDelay) {
				m_RecoverSpreadTime = 0f;
				m_CurrentSpreadBullet--;
				if(m_CurrentSpread > 0 && m_SpreadList[m_CurrentSpread].bullet > m_CurrentSpreadBullet) m_CurrentSpread--;
			}
		}
		transform.localEulerAngles = m_OriginalEuler;
		transform.localPosition = m_OriginalPosition;
	}
	
	public void StartShooting (Transform head) {
		if(m_IsReloading) return;
		if(Time.time < m_NextShootTime) return;

		m_NextShootTime = Time.time + m_FireDelay;
		m_Head = head;

		m_RecoverSpreadTime = 0f;
		m_IsShooting = true;

		if(m_IsAutomatic) InvokeRepeating("Shoot", 0f, m_FireDelay);
		else Shoot();
	}
	
	public void StopShooting (bool reload = true) {
		if(m_IsReloading) return;
		m_RecoverSpreadTime = 0f;
		m_IsShooting = false;

		if(m_IsAutomatic) CancelInvoke();
		if(reload && m_CurrentMagazine <= 0) Reload();
	}

	public void Reload() {
		m_IsReloading = true;
		this.StopShooting(false);

		m_Animator.SetTrigger("reload");
		Invoke("FinishReload", m_ReloadTime);
	}

	public void Select() {
		transform.localPosition = m_OriginalPosition;
		transform.localEulerAngles = m_OriginalEuler;
	}

	public void Deselect() {
		this.StopShooting();
		m_IsReloading = false;
		CancelInvoke();
	}

	void FinishReload() {
		m_IsReloading = false;
		m_CurrentMagazine = m_MagazineSize;
		m_CurrentSpread = 0;
		m_CurrentSpreadBullet = 0;
	}

	void Shoot() {
		if(m_IsReloading) return;
		if(m_CurrentMagazine <= 0) {
			// Play empty sound
			return;
		}

		float cBullet = m_CurrentSpreadBullet;
		PlayerGunSpread cSpread = m_SpreadList[m_CurrentSpread];
		if(cSpread.bullet < cBullet) {
			m_CurrentSpread++;
			cSpread = m_SpreadList[m_CurrentSpread];
		}

		float cBulletOff = m_CurrentSpread > 0 ? m_SpreadList[m_CurrentSpread - 1].bullet : 0;
		cBullet -= cBulletOff;

		Vector2 lastPos = m_CurrentSpread > 0 ? m_SpreadList[m_CurrentSpread - 1].position : Vector2.zero;
		// Vector3 spreadOffset = Vector3.Slerp(lastPos, cSpread.position, cBullet / (cSpread.bullet - cBulletOff));
		Vector3 spreadOffset = Vector3.Slerp(lastPos, cSpread.position, cBullet / (cSpread.bullet - cBulletOff));

		// Play shot sound
		m_CurrentMagazine--;
		m_CurrentSpreadBullet++;
		m_Flash.Play();
		m_Animator.SetTrigger("fire");

		// Debug.DrawRay(m_Head.transform.position, m_Head.transform.forward * 10f, Color.red, 10f);

		float force = m_Damage;
		Vector3 dir = new Vector3(
			Random.Range(-m_DefaultSpread, m_DefaultSpread) + m_Head.transform.forward.x - (Mathf.Sign(m_Head.transform.forward.x) * m_Head.transform.forward.z * spreadOffset.x), 
			Random.Range(-m_DefaultSpread, m_DefaultSpread) + m_Head.transform.forward.y + spreadOffset.y, 
			Random.Range(-m_DefaultSpread, m_DefaultSpread) + m_Head.transform.forward.z - (Mathf.Sign(m_Head.transform.forward.z) * m_Head.transform.forward.x * spreadOffset.x)
		);
		RaycastHit[] hits = Physics.RaycastAll(m_Head.transform.position, dir);
		foreach(RaycastHit hit in hits) {
			MaterialType material = hit.transform.GetComponent<MaterialType>();
			if(material) {
				force = material.Impact(hit.point, hit.normal, force);
				if(force <= 0f) break;
			}
			// Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
		}
	}
}
