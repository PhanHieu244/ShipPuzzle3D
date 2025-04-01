namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class ParticleManager : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnParticlePlay, OnParticlePlay);
        }
    
        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnParticlePlay, OnParticlePlay);
    
        }
    
        private void OnParticlePlay(object value, object position, object rotation)
        {
            GameObject particle = Instantiate(Resources.Load((string)value) as GameObject, (Vector3)position, Quaternion.Euler((Vector3)rotation));
    
            Destroy(particle, 1f);
        }
    }
    
}
