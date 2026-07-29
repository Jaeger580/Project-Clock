using UnityEngine;

public class TheFesteringController : MonoBehaviour
{
    // Object to move
    [SerializeField]
    private GameObject rootObject;
    private Transform rootTransform;

    private Transform playerTransform;
    [SerializeField]
    private float speed = 1;

    private Animator animator;
    [SerializeField]
    private Transform portalTransform;
    [SerializeField]
    private float triggerRange = 10.0f;
    private LayerMask layerMask;
    public bool isActive;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        layerMask = LayerMask.GetMask("Default", "Player");

        rootTransform = rootObject.GetComponent<Transform>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        // Check if the player is in front of the portal
        if (!isActive) 
        {
            RaycastHit hit;

            Debug.DrawRay(portalTransform.position, portalTransform.TransformDirection(Vector3.down) * triggerRange, Color.yellow);


            if (Physics.Raycast(portalTransform.position, portalTransform.TransformDirection(Vector3.down), out hit, triggerRange, layerMask)) 
            {
                if (hit.collider.tag.Equals("Player")) 
                {
                    animator.SetBool("In Range", true);
                }
            }
            else 
            {
            }
        }
        // Once triggered, root body should move towards the player
        if (isActive) 
        {
            var lookTarget = new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z);

            rootTransform.LookAt(lookTarget);

            var targetPos = new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z);

            rootTransform.position = Vector3.MoveTowards(rootTransform.position, targetPos, speed * Time.deltaTime);
        }
    }
}
