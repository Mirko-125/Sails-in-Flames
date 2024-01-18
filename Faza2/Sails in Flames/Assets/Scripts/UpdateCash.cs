using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCash : MonoBehaviour
{
    void Start()
    {
        FindFirstObjectByType<ShopLogic>().IsLoaded();        
    }

    void Update()
    {
        
    }
}
