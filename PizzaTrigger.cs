using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other){
        if(other.tag == "Player") OrderManager.instance.CollectOrders();
    }

    void OnTriggerStay(Collider other){
        if(other.tag == "Player") OrderManager.instance.CollectOrders();
    }
}
