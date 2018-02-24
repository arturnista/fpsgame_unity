﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
	
	private Transform m_Head;
	private List<PlayerGun> m_Guns;
	private PlayerGun m_CurrentGun;

	public int magazine {
		get {
			return m_CurrentGun.currentMagazine;
		}
	}
	public int ammo {
		get {
			return m_CurrentGun.currentAmmo;
		}
	}

	private int m_CurrentWeapon;

	void Awake () {
		m_Head = transform.Find("Head");

		m_Guns = new List<PlayerGun>();
		foreach(Transform child in m_Head) {
			PlayerGun g = child.GetComponent<PlayerGun>();
			if(g) m_Guns.Add(g);
		}
	}

	void Start() {
		SelectWeapon(0, true);		
	}
	
	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			m_CurrentGun.StartShooting(m_Head);
		} else if(Input.GetMouseButtonUp(0)) {
			m_CurrentGun.StopShooting();
		}

		if(Input.GetKeyDown(KeyCode.R)) {
			m_CurrentGun.Reload();
		}

		if(Input.GetKeyDown(KeyCode.Alpha1)) {
			SelectWeapon(0);
		} else if(Input.GetKeyDown(KeyCode.Alpha2)) {
			SelectWeapon(1);
		} else if(Input.GetKeyDown(KeyCode.Alpha3)) {
			SelectWeapon(2);
		}
	}

	void SelectWeapon(int index, bool force = false) {
		if(!force && m_CurrentWeapon == index) return;

		if(m_CurrentGun) m_CurrentGun.Deselect();

		m_Guns[m_CurrentWeapon].gameObject.SetActive(false);
		m_CurrentWeapon = index;
		m_Guns[m_CurrentWeapon].gameObject.SetActive(true);

		m_CurrentGun = m_Guns[m_CurrentWeapon];
		m_CurrentGun.Select();
	}

	public int GiveAmmo(string gunName, int amount) {
		PlayerGun gun = m_Guns.Find(x => x.gunName == gunName);
		if(gun == null) {
			Debug.LogWarning("Giving ammo to unknown gun " + gunName);
			return amount;
		}

		return gun.GiveAmmo(amount);
	}
}