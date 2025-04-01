namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using DG.Tweening;
    
    [DefaultExecutionOrder(-10)]
    public class TunnelManager : MonoBehaviour
    {
        public List<BusController> allBusInTunnel = new List<BusController>();
        private int initialBusCount, busCount;
        public TextMeshPro busCountText;
    
    
        private float rayDistance = 10f;
        public LayerMask busLayer;
        public Transform spawnPoint, frontPoint;
        private bool isReadyToSpawn;
    
        GameManager manager => GameManager.Instance;
    
        private void Awake()
        {
            EventManager.AddHandler(GameEvent.TunnelInitialSpawn, InitialSpawn);
        }
    
        private void Start()
        {
            RayController();
        }
    
        public void InitialSpawn()
        {
            int index = manager.allTunnels.IndexOf(this);
            initialBusCount = manager.busCountsInTunnels[index];
            busCount = initialBusCount;
    
            for (int i = 0; i < initialBusCount; i++)
            {
                GameObject bus = Instantiate(manager.busPrefab, spawnPoint.position, Quaternion.LookRotation(transform.right));
                BusController busController = bus.GetComponent<BusController>();
                busController.isInTunnel = true;
                busController.relatedTunnel = this;
                allBusInTunnel.Add(busController);
            }
        }
    
        public void BusSendFromTunnel()
        {
            if (busCount == 0)
                return;
    
            BusController nextBus = allBusInTunnel[0];
            Vector3 initialScale = nextBus.transform.localScale;
            nextBus.transform.localScale = Vector3.zero;
            nextBus.transform.position = transform.position;
            nextBus.transform.DOScale(initialScale, 1f);
            nextBus.transform.DOMove(frontPoint.position, 1f).OnComplete( () =>
            {
                nextBus.isInTunnel = false;
                allBusInTunnel.RemoveAt(0);
                });
    
        }
    
    
        private void Update()
        {
            busCount = allBusInTunnel.Count;
            busCountText.text = busCount.ToString();
    
            //RayController();
        }
    
        void RayController()
        {
            Vector3 direction = (frontPoint.position - transform.position + Vector3.up).normalized;
    
            RaycastHit hit;
            bool collision = Physics.Raycast(transform.position + Vector3.up, direction, out hit, rayDistance, busLayer);
    
            if (collision)
            {
                hit.collider.transform.GetComponent<BusController>().relatedTunnel = this;
            }
            else
            {
                BusSendFromTunnel();
            }
    
            Debug.DrawRay(transform.position + Vector3.up, direction * rayDistance, Color.red);
    
        }
    
        void OnEnable()
        {
            //EventManager.AddHandler(GameEvent.TunnelInitialSpawn, InitialSpawn);
        }
    
        void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.TunnelInitialSpawn, InitialSpawn);
        }
    }
    
}
