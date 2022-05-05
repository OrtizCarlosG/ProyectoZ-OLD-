using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderManager : MonoBehaviour
{
    // Start is called before the first frame update

    public List<ZombieInstance> _zombies = new List<ZombieInstance>();

    [SerializeField] public Transform[] _wanderPosition;
    private int lastJoinedZombie = 0;

    public void addZombie(ZombieInstance _zombie)
    {
        _zombies.Add(_zombie);
        _zombie.wanderID = lastJoinedZombie;
        lastJoinedZombie++;
    }
    public Transform getNextWanderPosition(ZombieInstance zombieID)
    {
        //if (!_zombies[zombieID.id])
        //     addZombie(zombieID);

        if (_zombies[zombieID.wanderID]._wanderPos >= _wanderPosition.Length - 1)
        {
            _zombies[zombieID.wanderID]._wanderPos = 0;
        }
        else
        {
            _zombies[zombieID.wanderID]._wanderPos = Random.Range(0, _wanderPosition.Length);
        }
        if (_zombies[zombieID.wanderID]._wanderPos < _wanderPosition.Length - 1)
        {
            return _wanderPosition[_zombies[zombieID.wanderID]._wanderPos];
        } else
        {
            return _wanderPosition[0];
        }
    }


   
}
