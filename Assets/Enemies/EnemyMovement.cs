using UnityEngine;

public class EnemyMovement : PhysicEntity {

	protected PlayerHealth m_Player;

	[SerializeField]
    protected bool m_IsFollowingPlayer = false;

    protected virtual void Awake() {
        base.Awake();
    }

    protected virtual void Start() {
		    m_Player = PlayerHealth.Instance;        
    }

    public virtual void TakeDamage(float damage, string name) {

    }
    
    public virtual void NoticePlayer() {
		m_IsFollowingPlayer = true;
	}
}
