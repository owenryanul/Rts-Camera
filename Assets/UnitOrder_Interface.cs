using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UnitOrder_Interface
{
    public void OnSelected();

    public void OnDeSelected();

    public void issueMoveOrder(Vector3 target);

    public void onDeath_DeSelect();
}
