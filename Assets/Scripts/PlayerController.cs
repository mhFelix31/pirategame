using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private ShipControl shipControl;
    public bool canShoot;
    public float nextShootTimer;
    public Ship ship;


    void Awake()
    {
        shipControl = new ShipControl();
        ship = GetComponent<Ship>();
    }

    // Update is called once per frame
    void OnEnable()
    {
        shipControl.Enable();
        shipControl.Actions.ShootForward.performed += ShootForward;
        shipControl.Actions.ShootSideways.performed += ShootSideways;
        
    }


    void FixedUpdate()
    {
        Movement();
        Timers();
    }

    void Timers(){
        if (nextShootTimer > 0)nextShootTimer -= Time.deltaTime;
        else canShoot = true;
    }
    void Movement(){
        if (shipControl.Movement.Forward.ReadValue<float>() != 0)ship.MoveForward();

        if (shipControl.Movement.Rotation.ReadValue<float>() != 0)ship.Rotate(shipControl.Movement.Rotation.ReadValue<float>());
    }

    void ShootForward(InputAction.CallbackContext context){
        if (canShoot){
            ship.Shoot(ship.cannonsForward);
            nextShootTimer = ship.attackRate;
            canShoot = false;
        }
    }
    void ShootSideways(InputAction.CallbackContext context){
        if (canShoot){
            ship.Shoot(ship.cannonsLeft);
            nextShootTimer = ship.attackRate;
            canShoot = false;
        }
    }

}
