﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour {

	[SerializeField]
	private float m_Damage;

	[SerializeField]
	private bool m_IsAutomatic;

	[SerializeField]
	private float m_FireRate;
	private float m_FireDelay;

	private Transform m_Head;
	private PlayerMovement m_PlayerMovement;
	private Animator m_Animator;
	private ParticleSystem m_Flash;

	void Awake () {
		m_Flash = GetComponentInChildren<ParticleSystem>();
		m_PlayerMovement = GetComponentInParent<PlayerMovement>();
		m_Animator = GetComponent<Animator>();

		m_FireDelay = 1 / m_FireRate;
	}

	void Update() {
		m_Animator.SetInteger("speed", Mathf.RoundToInt(m_PlayerMovement.planeVelocity.sqrMagnitude));
	}
	
	public void StartShoting (Transform head) {
		m_Head = head;
		if(m_IsAutomatic) InvokeRepeating("Shot", 0f, m_FireDelay);
		else Shot();
	}
	
	public void StopShoting () {
		CancelInvoke();
	}

	void Shot() {
		m_Flash.Play();

		Debug.DrawRay(m_Head.transform.position, m_Head.transform.forward * 10f, Color.red, 10f);

		float force = m_Damage;
		RaycastHit[] hits = Physics.RaycastAll(m_Head.transform.position, m_Head.transform.forward);
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
