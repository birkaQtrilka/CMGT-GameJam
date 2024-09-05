using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;
    public float lifetime = 10;
    public int damage = 1;
    bool shouldDestroy = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        lifetime -= Time.deltaTime;
        if (lifetime< 0 || shouldDestroy) Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        shouldDestroy = true;
        if (collision.collider.CompareTag("Player"))
        {
            Health health = collision.collider.GetComponent<Health>();
            if (health) health.TakeDamage(damage); 
        }    
    }
}
