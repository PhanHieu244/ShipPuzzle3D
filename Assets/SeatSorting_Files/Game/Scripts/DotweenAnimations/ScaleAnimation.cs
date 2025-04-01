namespace EKStudio
{
    using UnityEngine;
    using DG.Tweening;
    
    public enum ScaleDirection { ScaleUp, ScaleDown }
    public enum ScaleAxis { X, Y, Z, All }
    public enum LoopTypeOption { Restart, Yoyo, Incremental }
    
    public class ScaleAnimation : MonoBehaviour
    {
        public ScaleDirection ScaleDirection = ScaleDirection.ScaleUp;
        public ScaleAxis ScaleAxis = ScaleAxis.All;
        public Ease Ease = Ease.Linear;
        public float Duration = 1f;
        public float ScaleFactor = 1.1f;
        public float Delay = 0f;
        public int LoopCount = -1; // Sonsuz d�ng� varsay�lan olarak
        public LoopTypeOption LoopTypeOption = LoopTypeOption.Yoyo; // Varsay�lan olarak Yoyo d�ng�s�
    
        public AnimationCurve ScaleCurve = AnimationCurve.Linear(0, 1, 1, 1); // Curve Editor ile ayarlanabilir
    
        private Vector3 originalScale;
    
        private void Start()
        {
            originalScale = transform.localScale;
    
            switch (ScaleDirection)
            {
                case ScaleDirection.ScaleUp:
                    ScaleUp();
                    break;
                case ScaleDirection.ScaleDown:
                    ScaleDown();
                    break;
                default:
                    break;
            }
        }
    
        void ScaleUp()
        {
            ApplyScaling(ScaleFactor);
        }
    
        void ScaleDown()
        {
            ApplyScaling(1f / ScaleFactor);
        }
    
        void ApplyScaling(float factor)
        {
            Vector3 targetScale = originalScale;
    
            if (ScaleAxis == ScaleAxis.All || ScaleAxis == ScaleAxis.X)
            {
                targetScale.x *= factor;
            }
            if (ScaleAxis == ScaleAxis.All || ScaleAxis == ScaleAxis.Y)
            {
                targetScale.y *= factor;
            }
            if (ScaleAxis == ScaleAxis.All || ScaleAxis == ScaleAxis.Z)
            {
                targetScale.z *= factor;
            }
    
            transform.DOScale(targetScale, Duration).SetDelay(Delay)
                .SetEase(Ease)
                .SetLoops(LoopCount, (LoopType)LoopTypeOption)
                .OnUpdate(() =>
                {
                    float curveValue = ScaleCurve.Evaluate((transform.localScale - originalScale).magnitude / (targetScale - originalScale).magnitude);
                    transform.localScale = Vector3.LerpUnclamped(originalScale, targetScale, curveValue);
                });
        }
    }
    
}
