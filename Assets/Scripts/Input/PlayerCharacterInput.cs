using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerCharacterInput : CharacterInput
{
      void Update()
    {
        float movementInputX = Input.GetAxis("Horizontal");
        float movementInputY = Input.GetAxis("Vertical");
        MovementInput = new Vector3(movementInputX, 0, movementInputY).normalized; // unity uses z axis for "up/down", whatever, I don't care
    }
}