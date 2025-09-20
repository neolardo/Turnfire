using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public ProjectileData ProjectileData;
    private Rigidbody2D _rb;
    private bool _isFired = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }


    //TODO: interface for things that can be fired?
    public void Fire(Vector2 aimDirection)
    {
        _isFired = true;
        gameObject.SetActive(true);
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(aimDirection * ProjectileData.ImpulseStrength, ForceMode2D.Impulse);
        StartCoroutine(ShowBulletUntilImpact());
    }

    private IEnumerator ShowBulletUntilImpact()
    {
        //TODO
        _isFired = false;
        yield return null;
        int seconds = 2;
        float elapsedSeconds = 0;
        while (elapsedSeconds < seconds && !_isFired)
        {
            elapsedSeconds += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
