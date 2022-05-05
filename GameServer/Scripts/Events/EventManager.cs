using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    private static string _defaultImage = "iVBORw0KGgoAAAANSUhEUgAAACgAAAA7CAYAAAAJvMhYAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAADSElEQVRoQ+2aX2hOYRzH3y1mSljz58YMsYSWhlrIpFyMDTdqhTsudjFR5IorF7vQkpakJSUu3O+KUkJarZAryp+yC1EmI3/GfL/P7/u++9PYOef5nZryqW/b83t6PufX+55z3uc9W2FkZKQWeYek4QPSXJgAam1IPzKMJOEH8gl5hdxHepBG6QwU9iFZGEDKpaFnHsKDxXJeSgOFrA2SemnoOWmlaJ4ix5AdyOzYBlvUHz0Ue9MQ2+Bu9UfPEyu5UhPb4Hb1R88dK7kS/RZvVH95vIJDRXFMg6vkWGNDV156NDhDjss2dKUvtkHejJcjB5FvLDjTG9tg3lwtNrjXxtOOc8UGVyI/Q2l6cSo0SDCoQxojchHxpk3txQNZnznduIuUNiJRQLQ0KP0YRGqljweylqD144DUPkBYhSTdoE7FDWl9gfiB+aN4jcyX0heIT4dDZIe3uSbp/IF8UzhMdjqlygccoAzJerPnl6yZUsUD2QpknYYlUMtyoXxBVksRD2Rzkfc0g0UqBzDO0uBRLfcBwrPmDSxQmXV+3KXlIVImRTyQLUaGaBaVmuLcFSslhudrg5b7AGF3UBvDKgcwfmTlxFzSUh8g5IXxPaiNQU1xjlcwT/ak8Byu1nIfILwe1KO80RTnllgpMYe11AcI65GJ97gBTXO+yUqJ8L0wCIS9QT0eNlyn+fZQmRqu2RCkXkC4Nagnh2/7LiTp+ed7YRBI75l7UviMcOyF8ze4CV0orQ8Qtga1Dyek9QHCcsTrWctzpEJqHyA8FNQ+7JHWBwgrkBdBHc8taf2AtMPc0XB3s1ZaHyCcg7yl3YFuaf2A9Iy5o+GfNaqk9QHCauQj7Q4ckdYPSLvMHQ2/Y/g8tigCYQ3ylXYHtkjrB6Rpd8R/4pqUfkDKR28ejy74DuxE1iN+txfI8ngEbE/oBcb86NyM8FhThU98Z2lpbg32Sx/AuNPKibmgpbk1eFt6+vnRye1WGm5qeW4Nlg6A35utlIrcX8Eu6ekf+3U1Kce1PJcGPyPLpKf/WaimY3SLhkEeryA3u/wL/P4wSk+r2sutQfJLP7OQ+zkYS4/a+99gVv6tBrdZbVoxrsFKxGsvGAv/Megx0mHdFQq/AXRYH/1ZLHFjAAAAAElFTkSuQmCC";
    public List<ZombieInstance> _ordeList= new List<ZombieInstance>();
    private void Awake()
    {
        instance = this;
    }

    public void startOrde()
    {
        _ordeList = new List<ZombieInstance>();
        int safeZone = Random.Range(0, SafeZone._safeZoneList.Count);
        SafeZone _safezone = SafeZone._safeZoneList[safeZone];
       _safezone.isUnderAttack = true;
        ServerSend.ChatMessage($"<color=red> Una horda esta en camino hacia: <b>{_safezone._safeZoneName}!</b> </color>", _defaultImage);
        for (int i = 0; i < _safezone._safeZoneLevel; i++)
        {
            for (int j = 0; j <=  50; j++)
            {
                float _xAxis = UnityEngine.Random.Range(-20f, 20f);
                float _zAxis = UnityEngine.Random.Range(-20f, 20f);
                _xAxis = Random.Range(_xAxis, -_xAxis);
                _zAxis = Random.Range(_zAxis, -_zAxis);
                Vector3 _randomPosition = new Vector3(transform.position.x + _xAxis, 1f, transform.position.z + _zAxis);
                int randomSkin = Random.Range(2, 48);
                if (NetworkManager.instance.enemyPrefab[2].GetComponent<ZombieInstance>().canRespawn)
                {
                    int _zombieLevel = Random.Range(1, _safezone._safeZoneLevel);
                    bool canRun = false;
                    float runChance = Random.Range(0f, 1f);
                    if (runChance < .75)
                        canRun = true;
                    ZombieInstance _zombie = NetworkManager.instance.InstantiateEnemy(_randomPosition, randomSkin).GetComponent<ZombieInstance>();
                    _zombie.ZombieLevel = _zombieLevel;
                    _zombie.runnerZombie = canRun;
                    _zombie._attackZone = _safezone;
                    _zombie._zombieType = ZombieInstance.zombieTypes.OrdeZombie;
                    _ordeList.Add(_zombie);
                }
            }
        }
        ZombieInstance _zom = NetworkManager.instance.InstantiateEnemy(new Vector3(transform.position.x+ 20, 1f, transform.position.z + 20), 1).GetComponent<ZombieInstance>();
        _zom.ZombieLevel = 5;
        _zom.runnerZombie = true;
        _zom._attackZone = _safezone;
        _zom._zombieType = ZombieInstance.zombieTypes.GiantZombie;
    }
    // Start is called before the first frame update
    void Start()
    {
       // StartCoroutine(startEvent());
    }

    IEnumerator startEvent()
    {
        yield return new WaitForSeconds(1);
        startOrde();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
