using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaDelivery : MonoBehaviour
{
    void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            OrderManager.instance.DropOffPizza(transform.parent.GetComponent<House>());
        }
    }
}
