﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayloadActive : DoorActivate {

	private PayloadBehaviour m_PayloadBehaviour;

	void Start () {
		m_PayloadBehaviour = GameObject.FindObjectOfType<PayloadBehaviour>();
	}
	
	public override void Use () {
		base.Use();
		m_PayloadBehaviour.Activate();
	}
}
