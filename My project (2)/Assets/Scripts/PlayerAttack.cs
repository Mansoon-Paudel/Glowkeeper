using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;

    private static readonly int AttackHash = Animator.StringToHash("attack");
    private Animator _anim;
    private PlayerMovement _playerMovement;
    private float _cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.isPressed && _cooldownTimer > attackCooldown && _playerMovement.CanAttack())
            Attack();

        _cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        _anim.SetTrigger(AttackHash);
        _cooldownTimer = 0;

        fireballs[FindFireball()].transform.position = firePoint.position;
        fireballs[FindFireball()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }
    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}