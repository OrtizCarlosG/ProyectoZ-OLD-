using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public class BackpackSlot
    {
        public int id;
        public string itemName;
        public int x;
        public int y;
    }

    public class TacticalRigSlot
    {
        public int id;
        public string itemName;
        public int idSlot;
    }

    public TacticalRigSlot[] _tacticalRigSlot;
    public BackpackSlot[] _backpackSlot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
