 using UnityEngine;
 using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

[RequireComponent (typeof (Animator))]
 [RequireComponent (typeof (UnityEngine.AI.NavMeshAgent))]
 
 public class ZombieInstance : MonoBehaviour {


    public static int maxEnemies = 1;

    public Dictionary<int, float> attackedBy = new Dictionary<int, float>();
    public static int nextEnemyId = 1;

    [SerializeField, Range(0, 48)] public int _zombieSkin;
    public int id;
    public int ZombieLevel = 1;
    public float hp = 100.0f, damage = 20.25f, bodieRemovalTime = 10.0f, crawlSpeed = 0.5f, moveSpeed = 1.0f, runSpeed = 5.0f, fieldOfView = 45.0f, viewDistance = 5.0f, playerSearchInterval = 1.0f, minChase = 5.0f, maxChase = 10.0f, minWander = 5.0f, maxWander = 20.0f, _meleeDistance = 3.0f, _runDistance = 5f, _attackCooldown = 1f;
    public RuntimeAnimatorController[] animator;
    /*
     * Health points of this zombie.
     * Damage of the attacks of this zombie.
     * Time to wait for deleting the dead bodie.
     * GLOBAL SPEED OF THE ZOMBIE I RECOMMEND 2 FOR WALKERS AND 7 FOR RUNNERS, DEPENDS ON YOUR GAME
     * FIELD OF VIEW OF THE ZOMBIE
     * VIEW DISTANCE OF THE ZOMBIE
     * INTERVAL USED TO CHECK IF THE ZOMBIE IS LOOKING AT THE PLAYER, JUST TO PREVENT OVERUSE OF RESOURCES
     SET 0 IF YOU DON'T CARE OR KEEP IT BETWEEN 1.0F AND 0.5F, RECOMMENDED
     *Min time to chase the player
     *Max time to chase the player (will be random between these 2 values)
     *Min time to wander around
     *Max time to wander around (works the same way)
    */
    public string playerTag = "Player", bodiesTag = "DeadPlayer";
    /*
     * SET HERE THE TAG TO IDENTIFY YOUR PLAYER
     * YOUR PLAYER DEAD BODIES
    */
    public bool runnerZombie = false, eatBodies = false, canCrawl = false, canRespawn = true, canRot = true;
    private bool canRun = false;
    public bool canDismemberment = true;

    [HideInInspector] public bool isCrawler = false, isRotten = false;
    [HideInInspector] public bool hasHead = true, hasShoulderL = true, hasShoulderR = true, hasElbowL = true, hasElbowR = true, hasUpperLegL = true, hasUpperLegR = true, hasLowerLegL = true, hasLowerLegR = true;
    public enum zombieTypes
    {
        AcidZombie,
        GiantZombie,
        OrdeZombie,
        NormalZombie
    }
    public int _key;
    public zombieTypes _zombieType;
     /*
      * CAN YOUR ZOMBIE RUN?
      * WILL IT EAT THE DEAD PLAYER BODIES?
     */
     public Transform zombieHead = null;//Pivot point to use as reference, use it as if it were the eyes of the zombie.
     public LayerMask checkLayers;//Layers to check when searching for the player (after the check interval)
 
     /*Zombie sounds AnimationStates
      * 0 = Sound on wandering, idle or whatever.
      * 1 = Sound for chasing player.
      * 2 = Sound for losing player or extras.
      * 3 = Sound while eating.
      * Length of 4.
     */

    [SerializeField] private float _attackDistance;
    [SerializeField] private float _coolDown;
    [SerializeField] private Transform _shootOrigin;
    [SerializeField] private Transform bulletPrefab;
    private float _lastAttack;

   [HideInInspector] public int _wanderPos = 0;
   [HideInInspector] public Transform wanderPosition;
   [HideInInspector] public int wanderID = 0;
    [HideInInspector] public ZombieSpot _spot;
    /*
     * If you want to set a new clip, you must change the length of the array and edit the inspector script.
     * You can find the needed line by searching "AnimationStates", on the script ZombieUI.cs
    */
    private bool playerChase = false, wandering = false, eatingBodie = false;
     /*
      * Will be true when the zombie is chasing the player, then the code will randomize when will stop doing it.
     */
     private float lastCheck = -10.0f, lastChaseInterval = -10.0f, lastWander = -10.0f;
     /*
     *Last time we checked the player's position.
     *Last or next time we chased the player before losing him.
     *Last time we set a wander position
     */
     private UnityEngine.AI.NavMeshAgent agent;//This zombie's nav mesh agent
     private Animator Anim;//This zombie's animator
    [HideInInspector] public Transform player = null;//Player position/transform
    [HideInInspector] public WanderManager wanderManager;
     private Vector3 lastKnownPos = Vector3.zero;//Last known player position
     private AudioSource SNDSource;//zombieHead should always have 2 audioSources

    [HideInInspector]
    
    public int _animatorVariant = 0;
    public float _behaviorCooldown;
    float _lastBehavior;
    bool doingBehavior = false;
    public float _rotLevel = 0f;
    public bool _isOrdeZombie = false;
    public Transform _nearObject;
    public SafeZone _attackZone;
    private bool isInCooldown = false;


     void Start () {

        id = nextEnemyId;
        nextEnemyId++;
        //enemies.Add(id, this);
         ZombieSpot._zombies.Add(this);

        hp = hp * ZombieLevel;

        //THIS IS USED TO GET THE LOCAL NAV MESH AGENT, wanderManager AND ANIMATOR OF THIS ZOMBIE
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
         Anim = GetComponent<Animator>();
        _animatorVariant = Random.Range(0, animator.Length);
        Anim.runtimeAnimatorController = animator[_animatorVariant];
        wanderManager = GameObject.FindWithTag("wanderManager").GetComponent<WanderManager>();
        //SET THE MAIN VALUES
        agent.speed = moveSpeed;
        if (canRun)
            agent.speed = runSpeed;
         agent.acceleration = moveSpeed * 40;
         agent.angularSpeed = 999;
         resetZombie();
        wanderManager.addZombie(this);
        _lastBehavior = Time.time;
        ServerSend.SpawnEnemy(this);
    }
    private void FixedUpdate()
    {
        _key = 1;
        if (Anim.GetBool("isChasing"))
        {
            _key = 2;
        } else if (Anim.GetBool("isRunning"))
        {
            _key = 3;
        } else if (Anim.GetBool("isEating"))
        {
            _key = 4;
        } else if (Anim.GetBool("doAttack"))
        {
            _key = 5;
        } else if (Anim.GetBool("isCrawling"))
        {
            _key = 6;
        } else if (Anim.GetBool("StartCrawl"))
        {
            _key = 7;
        }
        if (!_zombieType.Equals(zombieTypes.OrdeZombie))
        {
            if ((Time.time - _lastBehavior > 1 / _behaviorCooldown) && wandering && !isCrawler)
            {
                float probability = Random.Range(0f, 1f);
                if (probability < .3f)
                {
                    StartCoroutine(doBehavior());
                }
                _lastBehavior = Time.time;
            }
        }
        if (canRot)
            if (_rotLevel < 100)
            _rotLevel +=  0.001f;
        if (_rotLevel > 75 && !isRotten)
        {
            isRotten = true;
            ServerSend.EnemyRotten(this);
        }
        ServerSend.EnemyPosition(this);
        if (_zombieType.Equals(zombieTypes.OrdeZombie))
        {
            _nearObject = _attackZone.getZoneObjects()[0].transform;
        }
        if (_zombieType.Equals(zombieTypes.OrdeZombie) && _nearObject)
        {
            if (!playerChase)
                doWanderFunctions();
            agent.SetDestination(_nearObject.transform.position);
            agent.isStopped = false;
            if (!runnerZombie)
            {
                Anim.SetBool("isIdle", false);
                Anim.SetBool("isChasing", true);
                Anim.SetBool("isRunning", false);
            } else
            {
                Anim.SetBool("isIdle", false);
                Anim.SetBool("isChasing", false);
                Anim.SetBool("isRunning", true);
                agent.speed = runSpeed;
            }
        }
    }

        void Update(){
        /* IN CASE YOU CHANGE YOUR PLAYER GAMEOBJECT OR THE ZOMBIE IS SPAWNINg */
        //if(Anim.GetCurrentAnimatorStateInfo(0).IsName("Spawn"))
        //  return;

        if (player == null)
        {
            if (eatingBodie)
                resetZombie();
            GameObject newPlayer = GameObject.FindWithTag(playerTag);
            GameObject bodySearch = GameObject.FindWithTag(bodiesTag);
            if (newPlayer != null && newPlayer.name.Contains("Clone"))
            {
                //There is a bug in unity that i couldn't fix, so i added the clone thing, so yeah...
                player = newPlayer.transform;
            }
            else if (eatBodies && bodySearch != null)
            {
                player = bodySearch.transform;
                eatingBodie = true;
                followBodie();
            }
            else
            {
                // wandering = true;
                doWanderFunctions();

                return;//DON'T DO ANYTHING UNTIL WE HAVE THE PLAYER GAMEOBJECT, GO BACK, ABOOORRTTT!!!!
            }
        }
 
         /* ACTUAL ZOMBIE CODE */
 
         if(Time.time - lastCheck > playerSearchInterval){//SEARCH FOR THE PLAYER AFTER INTERVAL
             checkView();
            lastCheck = Time.time;
         } 

        if (doingBehavior)
        {
            agent.isStopped = true;
        }
        if (!eatingBodie){
             /* PLAYER SEARCH ALGORITHMS */
             if(playerChase && (Anim.GetBool("isChasing") || Anim.GetBool("isRunning"))){
                 if(Time.time > lastChaseInterval){
                     gotoLastKnown();
                 }else{
                     chasePlayer();
                 }
             }
 
             //SET THE ATTACK AND RESET IT
             AnimatorStateInfo state = Anim.GetCurrentAnimatorStateInfo(0);
            float _distance = Vector3.Distance(transform.position, player.position);
            if (_zombieType.Equals(zombieTypes.AcidZombie) && !state.IsName("Attack") && playerChase)
            {
                if (_distance > 5.0f && _distance <= _attackDistance)
                {
                    if (Time.time - _lastAttack > 1 / _coolDown)
                    {

                        _lastAttack = Time.time;

                        Anim.Play("Spitter", -1, 0f);

                        StartCoroutine(SpitterAttack());

                        Anim.SetBool("isIdle", false);
                        Anim.SetBool("isChasing", false);
                        playerChase = false;
                    }
                }
            }
            if (_zombieType.Equals(zombieTypes.GiantZombie) && ! state.IsName("Attack") && playerChase)
            {
                if (_distance > 15.0f && _distance <= _attackDistance)
                {
                    if (Time.time - _lastAttack > 1 / _coolDown)
                    {
                        _lastAttack = Time.time;

                        Anim.SetTrigger("doThrow");
                        Anim.SetBool("isIdle", false);
                        Anim.SetBool("isChasing", false);
                        Anim.SetBool("isRunning", false);
                        StartCoroutine(RockAttack());

                        playerChase = false;

                    }
                } else if (_distance < 15.0f && _distance >= 5f)
                {
                    if (Time.time - _lastAttack > 1 / _coolDown)
                    {
                        _lastAttack = Time.time;

                        Anim.SetBool("doCharge", true);
                        Anim.SetBool("isIdle", false);
                        Anim.SetBool("isChasing", false);
                        Anim.SetBool("isRunning", false);
                        ServerSend.EnemySpecialAttack(this, _shootOrigin.transform.position, _shootOrigin.transform.rotation, 0f, "Charger", player.transform.position);
                        StartCoroutine(ChargeAttack(5f));

                    }
                }
            }
            if (runnerZombie && !isCrawler)
            {
                if (_zombieType.Equals(zombieTypes.GiantZombie))
                {
                    if (!Anim.GetBool("doCharge"))
                    {
                        if (_distance > _runDistance)
                        {
                            canRun = true;
                            chasePlayer();
                        }
                        else
                        {
                            canRun = false;
                            chasePlayer();
                        }
                    }
                } else
                {
                    if (_distance > _runDistance)
                    {
                        canRun = true;
                        chasePlayer();
                    }
                    else
                    {
                        canRun = false;
                        chasePlayer();
                    }
                }
            }
            if (_zombieType.Equals(zombieTypes.GiantZombie))
            {
                if (Anim.GetBool("doCharge"))
                {
                    agent.speed = 8f;
                }
            }
            if (!state.IsName("Attack")  && playerChase && (Vector3.Distance(transform.position, player.position) < _meleeDistance && !isInCooldown)){//READY TO ATTACK!
                 Anim.SetTrigger("doAttack");
                 Anim.SetBool("isIdle", false);
                 Anim.SetBool("isChasing", false);
                 Anim.SetBool("isRunning", false);
                 Anim.SetBool("isCrawling", false);
                 playerChase = false;
                 agent.isStopped = true;
             }
            if (!state.IsName("Attack") && wandering && !playerChase && reachedPathEnd())
            {
                Anim.SetBool("isIdle", true);
                Anim.SetBool("isChasing", false);
                Anim.SetBool("isRunning", false);
                Anim.SetBool("isCrawling", false);
                agent.isStopped = true;
            }
            if (state.IsName("Attack") && state.normalizedTime > 0.9f){
                 chasePlayer();
                 Attack();//HERE WE CALL THE PLAYER'S DAMAGE
             }
            if (state.IsName("Throw") && state.normalizedTime > 0.90f)
            {
                chasePlayer();
            }
        }
        else{
             /* EAT BODIE ALGORITHMS */
             if(reachedPathEnd()){//This means we are in the right position to eat the bodie.
                 startEating();
             }else{
                 followBodie();//In case or explosions or stuff like that.
             }
         }
 
         //MAKE THE ZOMBIE WANDER AROUND THE MAP
         if(wandering){
             if(reachedPathEnd()){
                 resetZombie();
             }
             if(Time.time > lastWander){//IF WE ARE READY TO CHOOSE A NEW POSITION
                if (wanderManager)
                {
                    if (_wanderPos > wanderManager._wanderPosition.Length - 1)
                    {
                        wanderPosition = wanderManager._wanderPosition[_wanderPos];
                        setNewWanderPos(wanderPosition.position);
                        _wanderPos++;
                    }
                }
            }
         }
    }
 
     //Just to make this code prettier, simplify with functions...
     void checkView(){
         RaycastHit hit = new RaycastHit();
         Vector3 checkPosition = player.position - zombieHead.position;
         if(Vector3.Angle(checkPosition, zombieHead.forward) < fieldOfView){ //Check if player is inside the field of view
             if (Physics.Raycast(zombieHead.position, checkPosition, out hit, viewDistance)) {
                if (hit.collider.tag == playerTag){//do this..
                     chasePlayer();
                     lastChaseInterval = Time.time + Random.Range(minChase, maxChase);
                 }
             }
         }else if(!meleeDistance()){
             chasePlayer();
             lastChaseInterval = Time.time + UnityEngine.Random.Range(minChase, maxChase);
         }
     }
 
     void gotoLastKnown(){
         Anim.SetBool("isChasing", true);

        if (isCrawler && canCrawl)
            Anim.SetBool("isCrawling", true);

        if (canRun && !isCrawler)
             Anim.SetBool("isRunning", true);
 
         Anim.SetBool("isIdle", false);
         playerChase = true;
         agent.SetDestination(lastKnownPos);
         agent.isStopped = false;
        agent.stoppingDistance = 1;
         wandering = true;
         eatingBodie = false;
     }
 
     void chasePlayer(){

        if (player)
        {
            agent.stoppingDistance = 0;
            Anim.SetBool("isChasing", true);
            agent.speed = moveSpeed - _rotLevel/200f;
            if (isCrawler && canCrawl)
            {
                Anim.SetBool("isCrawling", true);
                Anim.SetBool("isRunning", false);
                Anim.SetBool("isChasing", false);
            }

            if (canRun && !isCrawler)
            {
                Anim.SetBool("isRunning", true);
                Anim.SetBool("isChasing", false);
                agent.speed = runSpeed - _rotLevel / 200f;
            }

            Anim.SetBool("isIdle", false);
            playerChase = true;
            agent.SetDestination(player.position);
            lastKnownPos = player.position;
            agent.isStopped = false;
            wandering = false;
            eatingBodie = false;
        } else
        {
            wandering = true;
            doWanderFunctions();
        }
         //playSound(audioClips[1], false, false, true, true);
     }
 
     void stopChase(){
         Anim.SetBool("isChasing", false);
 
         if(canRun)
             Anim.SetBool("isRunning", false);
 
         Anim.SetBool("isIdle", true);
         playerChase = false;
         agent.isStopped = true;
         wandering = false;
         eatingBodie = false;
     }
 
     void resetZombie(){
         Anim.SetBool("isIdle", true);
         Anim.SetBool("isChasing", false);
         Anim.SetBool("isEating", false);

        if (isCrawler && canCrawl)
            Anim.SetBool("isCrawling", false);

        if (canRun)
             Anim.SetBool("isRunning", false);
 
         playerChase = false;
         agent.isStopped = true;

         wandering = true;
         eatingBodie = false;
        agent.stoppingDistance = 1f;
         //playSound(audioClips[0], false, false, true, true);
     }
 
     void startEating(){
         Anim.SetBool("isChasing", false);;

        if (isCrawler && canCrawl)
            Anim.SetBool("isCrawling", false);

        if (canRun)
             Anim.SetBool("isRunning", false);
 
         Anim.SetBool("isIdle", false);
         Anim.SetBool("isEating", true);
 
         playerChase = false;
         agent.SetDestination(player.position);//Just to keep track of it, ignore this.
        agent.isStopped = true;//Won't actually follow the bodie, let's store that for later.
         wandering = false;
         eatingBodie = true;
        // playSound(audioClips[3], true, true, false, true);
     }
 
     void followBodie(){
         Anim.SetBool("isChasing", true);

        if (isCrawler && canCrawl)
            Anim.SetBool("isCrawling", true);

        if (canRun)
             Anim.SetBool("isRunning", true);
 
         Anim.SetBool("isIdle", false);
         Anim.SetBool("isEating", false);
         playerChase = false;
         agent.SetDestination(player.position);//In this case "player" will be a dead bodie.
         agent.isStopped = false;//Lets follow it, to prevent mistakes.
         wandering = false;
         eatingBodie = true;
         SNDSource.Stop();
     }
 
     void setNewWanderPos(Vector3 targetPos){
        if (!_isOrdeZombie)
        {
            Anim.SetBool("isIdle", false);
            Anim.SetBool("isChasing", true);

            if (isCrawler && canCrawl)
                Anim.SetBool("isCrawling", true);

            if (canRun)
                Anim.SetBool("isRunning", true);

            playerChase = false;
            agent.SetDestination(targetPos);
            agent.isStopped = false;
            lastWander = Time.time + UnityEngine.Random.Range(minWander, maxWander);
        }
         //playSound(audioClips[2], false, false, true, true);
     }
 
 
     //ONLY WHEN THE PLAYER AND DEAD BODY VARIABLE ARE NULL
     void doWanderFunctions(){
        wandering = true;
        playerChase = false;
        //MAKE THE ZOMBIE WANDER AROUND THE MAP EXACTLY LIKE THE UPDATE SYSTEM, JUST TO MAKE EVERYTHING EASIER
        if (wandering)
        {
            if (reachedPathEnd())
            {
                resetZombie();
            }
            if (Time.time > lastWander)
            {//IF WE ARE READY TO CHOOSE A NEW POSITION
                if (wanderManager)
                {
                    if (_wanderPos > wanderManager._wanderPosition.Length - 1)
                    {
                        wanderPosition = wanderManager._wanderPosition[_wanderPos];
                        setNewWanderPos(wanderPosition.position);
                        _wanderPos++;
                    }
                }
            }
        }
    }
 
     void Attack(){
         if(!meleeDistance())//DON'T DO ANYTHING IF WE ARE NOT AT A MELEE DISTANCE TO THE PLAYER
             return;
        if (isInCooldown)
            return;
         agent.updateRotation = false;
         transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(agent.destination - transform.position, transform.up), 7.0f *Time.deltaTime);
         agent.updateRotation = true;
         Player _player = player.transform.gameObject.GetComponent<Player>();
        float calcDamage = damage + (10 * (ZombieLevel)) - (_rotLevel/10f);
        _player.TakeDamage(calcDamage);
        Debug.Log($"{_player.name} damage taken {calcDamage} by attacker id {id} current health: {_player.health}");
        isInCooldown = true;
        StartCoroutine(attackCooldown());
     }

    IEnumerator attackCooldown()
    {
        yield return new WaitForSeconds(_attackCooldown);
        isInCooldown = false;
    }
 
     //Check if agent reached the player
     bool reachedPathEnd (){
         if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance){
             if (!agent.hasPath || agent.velocity.sqrMagnitude == 0.0f){
                 return true;
             }
             return true;
         }
         return false;
     }
 
     bool meleeDistance(){
         if(Vector3.Distance(transform.position, player.position) < _meleeDistance)
        {
             return true;
         }
 
         return false;
     }

    public void playCrawl()
    {
        Anim.SetBool("StartCrawl", true);
        Anim.SetBool("isChasing", false);
        agent.speed = crawlSpeed;
    }

    public void TakeDamage(float _damage, Player _attacker)
    {
        hp -= _damage;
        if (player != null)
        {
            player = _attacker.transform;
            chasePlayer();
            float damageadd;
            if (attackedBy.TryGetValue(_attacker.id, out damageadd))
            {
                attackedBy[_attacker.id] = damageadd + _damage;
            } else
            {
                attackedBy.Add(_attacker.id, _damage);
            }
        }
        if (hp <= 0f)
        {
            wanderManager._zombies.Remove(this);
            ServerSend.SendKillFeed($"{_attacker.name} mato a un zombie con <color=red>{_attacker._currentSlot._weaponName}</color>");
            hp = 0f;
            ZombieSpot._zombies.Remove(this);
            if (_spot)
                _spot.inSpot--;
            sendExperience();
            GC.Collect();
            Destroy(gameObject);
        }
        ServerSend.EnemyHealth(this);
    }

    public void TakeDamage(float _damage, Guard _guard)
    {

        if (_guard)
        {
            hp -= _damage;
            if (hp <= 0)
            {
                wanderManager._zombies.Remove(this);
                hp = 0f;
                ZombieSpot._zombies.Remove(this);
                if (_spot)
                    _spot.inSpot--;
                GC.Collect();
                Destroy(gameObject);
            }
        }
        ServerSend.EnemyHealth(this);
    }

    public void sendExperience()
    {
        foreach (KeyValuePair<int, float> player in attackedBy)
        {
            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    if (_client.id == player.Key)
                    {
                        _client.player._experience += player.Value;
                        ServerSend.NotifyPlayer(player.Key, 1, $"Recibiste <color=red>{(int) player.Value}</color> de experiencia.");
                    }
                }
            }
        }
    }

    private IEnumerator SpitterAttack()
    {
        ServerSend.EnemySpecialAttack(this, _shootOrigin.transform.position, _shootOrigin.transform.rotation, 1.18f, _zombieType.ToString(), player.transform.position);

        yield return new WaitForSeconds(1.18f);

        Vector3 aim = (player.transform.position - _shootOrigin.transform.position).normalized;
        var bullet = (Transform)Instantiate(
                           bulletPrefab,
                          _shootOrigin.transform.position,
                           Quaternion.LookRotation(player.transform.position));

        bullet.LookAt(player);
        bullet.GetComponent<Rigidbody>().velocity =
                        aim * 25;

    }

    private IEnumerator doBehavior()
    {
        float randomBehavior = UnityEngine.Random.Range(0f, 1f);
        float waitTime = 2.46f;
        if (randomBehavior < .5f)
        {
            _key = 8;
            Anim.Play("Scream", -1, 0f);
        }
        else
        {
            _key = 9;
            Anim.Play("Agonize", -1, 0f);
            waitTime = 11.42f;
        }
        doingBehavior = true;
        float moveSpeed = agent.speed;
        agent.speed = 0f;
        yield return new WaitForSeconds(waitTime);
        doingBehavior = false;
        agent.isStopped = false;
        agent.speed = moveSpeed;
    }
    private IEnumerator RockAttack()
    {
        var bullet = (Transform)Instantiate(
                           bulletPrefab,
                          _shootOrigin.transform.position,
                           Quaternion.LookRotation(player.transform.position));
        bullet.SetParent(_shootOrigin);
        Vector3 _playerPos = new Vector3(player.position.x, player.position.y + 2, player.position.z);
        agent.isStopped = true;
        ServerSend.EnemySpecialAttack(this, _shootOrigin.transform.position, _shootOrigin.transform.rotation, 1f, _zombieType.ToString(), _playerPos);

        yield return new WaitForSeconds(1f);
        agent.isStopped = false;
         _playerPos = new Vector3(player.position.x, player.position.y + 2, player.position.z);
        Vector3 aim = (_playerPos - bullet.position).normalized;
        float _distance = Vector3.Distance(transform.position, _playerPos);
        bullet.transform.gameObject.AddComponent<Rigidbody>();
        bullet.SetParent(null);

        bullet.LookAt(player);
        bullet.GetComponent<Rigidbody>().velocity =
                        aim * (_distance + 8);

    }

    private IEnumerator ChargeAttack(float chargeDuration)
    {
        agent.speed = 8f;
        yield return new WaitForSeconds(chargeDuration);
        Anim.SetBool("doCharge", false);
        chasePlayer();
    }

    public void setBodyStatus(string bodyName)
    {
        if (bodyName.Equals("Head"))
            hasHead = false;
        else if (bodyName.Equals("ShoulderL"))
            hasShoulderL = false;
        else if (bodyName.Equals("Elbow_L"))
            hasElbowL = false;
        else if (bodyName.Equals("ShoulderR"))
            hasShoulderR = false;
        else if (bodyName.Equals("Elbow_R"))
            hasElbowR = false;
        else if (bodyName.Equals("UpperLegL"))
            hasUpperLegL = false;
        else if (bodyName.Equals("LowerLegL"))
            hasLowerLegL = false;
        else if (bodyName.Equals("UpperLegR"))
            hasUpperLegR = false;
        else if (bodyName.Equals("LowerLegR"))
            hasLowerLegR = false;
    }    
}