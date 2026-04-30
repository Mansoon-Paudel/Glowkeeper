using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private float direction;
    private bool hit;
    private float lifetime;

    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        if (hit)
        {
            Debug.Log("Current state: " + anim.GetCurrentAnimatorStateInfo(0).IsName("End"));
            return;
        }
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > 5) gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit: " + collision.gameObject.name);
        hit = true;
        boxCollider.enabled = false;
        anim.SetBool("End", true);
        Debug.Log("End bool set to: " + anim.GetBool("End"));
    }
   public void SetDirection(float _direction)
   {
       lifetime = 0;
       direction = _direction;
       gameObject.SetActive(true);
       hit = false;
       boxCollider.enabled = true;
       anim.SetBool("End", false);  

       float localScaleX = transform.localScale.x;
       if (Mathf.Sign(localScaleX) != _direction)
           localScaleX = -localScaleX;
       transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
   }
    private void Deactivate()
    {
        anim.SetBool("End", false);
        gameObject.SetActive(false);
    }
}