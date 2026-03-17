using System.Collections.Generic;
using UnityEngine;

public class AnomalyHandlerPassthrough : MonoBehaviour
{
    [SerializeField] private AnomalyHandler parentAnomaly;
    public AnomalyHandler ParentAnomaly => parentAnomaly;

    protected void Start()
    {
        if (parentAnomaly == null)
            if (!transform.parent.TryGetComponent(out parentAnomaly))
                print("ERR: Missing AnomalyHandler on parent, assign it and/or add one to the parent.");
    }
}
