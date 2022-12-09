using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager instance;

    public GameObject pizzaLights;

    public List<Order> pizzasWaitingToBeCollected;
    public List<Order> pizzasInInventory;

    private float timeOfLastOrder;
    private int orderNumber = 0;
    private int addressNumber = 0;
    private int numberOfHouses;
    private float shortestLifespan;
    private const float orderCooldown = 10f;

    public GameObject PizzaJoint;

    private List<House> houses = new List<House>();

    private void Awake(){
        pizzasInInventory = new List<Order>();
        instance = this;
    }

    private void Start(){
        House[] tempHouses = FindObjectsOfType<House>();
        numberOfHouses = tempHouses.Length;
        foreach(House h in tempHouses){
            h.houseNumber = addressNumber;
            addressNumber++;
            houses.Add(h);
        }
    }

    private void Update(){
        float timeUntilNextOrder = orderCooldown - (Time.timeSinceLevelLoad - timeOfLastOrder);
        MenuManager.instance.UpdateNextOrderInText(timeUntilNextOrder);

        if(Time.timeSinceLevelLoad - timeOfLastOrder > orderCooldown){
            GenerateOrder();
        }

        UpdateLifeSpans();
        UpdateHousesToDeliverTo();
        UpdatePizzaJoint();
    }

    private void GenerateOrder(){
        AudioManager.instance.Play("OrderPlaced");
        Order pizzaOrder = new Order();
        pizzaOrder.OrderNumber = orderNumber;
        pizzaOrder.Address = GetHouseNumber();
        pizzasWaitingToBeCollected.Add(pizzaOrder);
        orderNumber++;
        timeOfLastOrder = Time.timeSinceLevelLoad;
    }

    private int GetHouseNumber(){
        int houseNumber = Random.Range(0,numberOfHouses);
        return houseNumber;
    }

    private void UpdateLifeSpans(){
        foreach(Order o in pizzasWaitingToBeCollected){
            o.Lifespan += Time.deltaTime;
            if(o.Lifespan >= 10f){
                //GameManager.instance.Lose();
            }
        }
        foreach(Order o in pizzasInInventory){
            o.Lifespan += Time.deltaTime;
            if(o.Lifespan >= 10f){
                GameManager.instance.Lose();
            }
            if(o.Lifespan > shortestLifespan){
                shortestLifespan = o.Lifespan;
            }
        }
        MenuManager.instance.UpdateDecayInText(10f - shortestLifespan);
    }

    private void UpdateHousesToDeliverTo(){
        foreach(Order o in pizzasInInventory){
            //Visual indicator of houses that need deliveries
            houses[o.Address].houseLight.gameObject.SetActive(true);
        }
    }

    private void UpdatePizzaJoint(){
        //Visual indicator of pizza needing to be collected
        if(pizzasWaitingToBeCollected.Count != 0){
            pizzaLights.SetActive(true);
        }else{
            pizzaLights.SetActive(false);
        }
    }

    public void CollectOrders(){
        foreach(Order o in pizzasWaitingToBeCollected.ToArray()){
            AudioManager.instance.Play("PickupOrder");
            o.Lifespan = 0;
            pizzasInInventory.Add(o);
            pizzasWaitingToBeCollected.Remove(o);
        }
    }

    public void DropOffPizza(House house){
        foreach(Order o in pizzasInInventory.ToArray()){
            if(o.Address == house.houseNumber){ 
                AudioManager.instance.Play("DeliverOrder");
                pizzasInInventory.Remove(o);
                house.houseLight.gameObject.SetActive(false);
                MenuManager.instance.UpdateScoreText();
                shortestLifespan = 0f;
            }
        }
    }


}
