using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    [SerializeField] protected float displayTime = 1f;

    [SerializeField] LineRenderer lr;

    [SerializeField] float laserLength = 4f;
    [SerializeField] float laserTravel = 12f;

    [SyncVar]
    public MPlayerController.DamageInfo damage;
    [SerializeField] public int collisionDamage = 10;
    [SerializeField] ParticleSystem[] explodeParticles;
    AudioManager audioManager;
    [SerializeField] AudioTask explodeAudio;

    public void SetDamageSourceId(NetworkInstanceId _netId) {
        if(!isServer) {
            DebugHUD.Debugg("not server set dmg src id");
            return;
        }
        DebugHUD.Debugg("setting dmg ");
        damage = new MPlayerController.DamageInfo()
        {
            amount = collisionDamage,
            netId = _netId
        };
    }

    [SerializeField] public bool collisionDealsDamage;

    public struct BulletInfo
    {
        public Vector3 destination;
    }

    public BulletInfo info;


    private void Start() 
    {
        var dir = GetComponent<Rigidbody>().velocity;
        StartCoroutine(flashAndDie(dir.normalized)); // info.destination));
        audioManager = FindObjectOfType<AudioManager>();
    }
    /*
     * We could piggy-back on rb velocity to infer the destination
     * (But this seems needlessly baroque. just laser forward a fixed short distance)
     */
    protected virtual IEnumerator flashAndDie(Vector3 dir) 
    {
        
        var start = transform.position;

        float velocity = laserTravel / displayTime;
        int frames = (int)(displayTime / Time.deltaTime);

        var origin = start;

        for (int i = 1; i < frames; ++i) 
        {
            origin = start + dir * i * velocity;
            lr.SetPosition(0, origin);
            lr.SetPosition(1, origin + dir * laserLength);

            yield return new WaitForFixedUpdate();

            if ((i + 1) * velocity + laserLength > laserTravel) 
            {
                break;
            }
        }


        Destroy(gameObject); //TEST should destroy immediately
    }

    private void OnCollisionEnter(Collision collision) {

<<<<<<< HEAD
        if (collisionDealsDamage) {
            var health = collision.gameObject.GetComponent<Health>();
            if(health) {

=======
        if (collisionDealsDamage) 
        {
            var health = collision.gameObject.GetComponentInParent<Health>(); // collision.collider.GetComponent<Health>();
            if(health) 
            {
                DebugHUD.Debugg("BulletColl " + collision.collider.name);
>>>>>>> 0059ef355bb40a72a83387160265cb974b1cb821
                health.TakeDamage(damage);
                GetComponent<Collider>().enabled = false;
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<Renderer>().enabled = false;
<<<<<<< HEAD
                float liveTime = 0f;
                audioManager.play(explodeAudio);
                foreach(var ps in explodeParticles) {
                    liveTime = Mathf.Max(ps.main.duration, liveTime);
                    ps.Play();
=======
                if (explodeParticles) 
                {
                    explodeParticles.Play();
                    Destroy(gameObject, explodeParticles.main.duration);
                }
                else 
                {
                    Destroy(gameObject);
>>>>>>> 0059ef355bb40a72a83387160265cb974b1cb821
                }
                Destroy(gameObject, liveTime);
                
            }
        }

    }


}
