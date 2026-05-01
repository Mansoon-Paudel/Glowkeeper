using System.Collections;
using UnityEngine;

public class Firetrap : MonoBehaviour
{
    [SerializeField] private float damage;
    [Header("Fire Trap Timers")]
    [SerializeField] private float animationDelay = 1f;
    [SerializeField] private float activateTime = 2f;

    private Animator anim;
    private SpriteRenderer spriteRend;
    private bool active;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(TrapLoop());
    }

    private IEnumerator TrapLoop()
    {
        while (true)
        {
            spriteRend.color = Color.darkSlateGray; 
            yield return new WaitForSeconds(animationDelay);
            spriteRend.color = Color.darkRed; 
            active = true;
            anim.SetBool("activated", true);
            yield return new WaitForSeconds(activateTime);
            active = false;
            anim.SetBool("activated", false);
            spriteRend.color = Color.black;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (active && collision.CompareTag("Player"))
        {
            collision.GetComponent<Health>().TakeDamage(damage);
        }
    }
}