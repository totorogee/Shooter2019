using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.Translate(new Vector3(0, 2, 0) * Time.fixedDeltaTime);
    }
}
