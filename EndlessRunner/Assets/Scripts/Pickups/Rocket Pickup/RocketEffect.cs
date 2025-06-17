using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/RocketPickup")]
public class RocketEffect : PowerUp_Effect
{

    public override void ApplyEffect(GameObject target, MonoBehaviour coroutineHost)
    {
        coroutineHost.StartCoroutine(PlayEffect(target));

        if (particleSystemPrefab != null && Camera.main != null)
        {

            ParticleSystem pEffect = Instantiate(particleSystemPrefab, Camera.main.transform.position, Quaternion.identity);

            Vector3 zOffset = Camera.main.transform.forward * -2f;
            pEffect.transform.SetParent(Camera.main.transform);
            pEffect.transform.localPosition = zOffset;
            pEffect.Play();


            float duration = particleSystemPrefab.duration + particleSystemPrefab.startLifetime;
            float destroyDelay = duration + 10f;

            coroutineHost.StartCoroutine(StopParticle(pEffect, duration, destroyDelay));
        }


    }
    

    private IEnumerator StopParticle(ParticleSystem pEffect, float stopDelay, float destroyDelay)
    {
        yield return new WaitForSeconds(stopDelay);
        pEffect.Stop();

        yield return new WaitForSeconds(destroyDelay - stopDelay);
        Destroy(pEffect.gameObject);
    }

    private IEnumerator PlayEffect(GameObject target)
    {
        float setTime = 0;
        
        target.GetComponent<PlayerController>().maxSpeed = 150;
        target.GetComponent<PlayerDeath>().playerImmune = true;
        target.GetComponent<PlayerController>()?.LeftExhaustFlame.Play();
        target.GetComponent<PlayerController>()?.RightExhaustFlame.Play();
        target.GetComponent<PlayerController>().Shield.SetActive(true);


        while (setTime < this.duration)
        {
            setTime += Time.deltaTime;
            yield return null;

        }

        target.GetComponent<PlayerController>().maxSpeed = 100;
        target.GetComponent<PlayerDeath>().playerImmune = false;
        target.GetComponent<PlayerController>().LeftExhaustFlame?.Stop();
        target.GetComponent<PlayerController>().RightExhaustFlame?.Stop();
        target.GetComponent<PlayerController>().Shield.SetActive(false);


    }
}


