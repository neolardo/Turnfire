using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.CharacterTag))
        {
            var c = collision.GetComponent<Character>();
            c.Kill();
        }
    }
}
