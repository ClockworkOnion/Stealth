using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAnimationSubscript : MonoBehaviour
{
    public void ExecutePunch() {
        GetComponentInParent<GuardAI>().PunchPlayer();
    }
}
