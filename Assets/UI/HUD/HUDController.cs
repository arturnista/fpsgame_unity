﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

	private Text m_AmmoText;
	private Text m_HealthText;
	private Image m_HitIndicator;

	[SerializeField]
	private Color m_HitColor;
	private Color m_HitColorTransparent;
	private PlayerWeapon m_PlayerWeapon;
	private PlayerHealth m_PlayerHealth;

	private bool m_TakeHit = false;
	private float m_HitDuration = .5f;
	private float m_HitStartTime;

	void Awake () {
		m_AmmoText = transform.Find("AmmoText").GetComponent<Text>();
		m_HealthText = transform.Find("HealthText").GetComponent<Text>();
		m_HitIndicator = transform.Find("HitIndicator").GetComponent<Image>();
		m_HitColorTransparent = new Color(1f, 0f, 0f, 0f);

		m_PlayerWeapon = GameObject.FindObjectOfType<PlayerWeapon>();
		m_PlayerHealth = GameObject.FindObjectOfType<PlayerHealth>();
	}
	
	void Update () {
		m_AmmoText.text = m_PlayerWeapon.magazine + " / " + m_PlayerWeapon.ammo;
		m_HealthText.text = Mathf.RoundToInt(m_PlayerHealth.health).ToString();

		if(m_TakeHit) {
			float t = (Time.time - m_HitStartTime) / m_HitDuration;
			m_HitIndicator.color = Color.Lerp(m_HitColor, m_HitColorTransparent, t);
			if(t >= 1) m_TakeHit = false;
		}
	}

	public void TakeDamage() {
		m_TakeHit = true;

		Color col = m_HitColor;
		m_HitIndicator.color = col;
		m_HitStartTime = Time.time;
	}
}