using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshTrailVisual : MonoBehaviour
{//TODO: OPTIMIZE THIS SCRIPT instantiating is super expensive, look into object pooling instead
    public static Action onAllowRewind; // Go back to the last afterimage position. 
    private float activeTime = 2f;
    private float meshDestroyDelay = 3f;
    private Transform lastPosition;
    private bool isLastPositionStable;

    [Header("Mesh Related")]
    private float meshRefreshRate = 0.1f;

    private bool isTrailActive;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    [SerializeField] private Transform positionToSpawn;
    public bool currentState;

    [Header("Shader related materials")]
    [SerializeField] private Material mat;

    GameObject[] generatedMeshPool;
    MeshRenderer[] cacheMesh;
    MeshFilter[] cacheFilter;
    Mesh[] mesh;
    // FOR combat mesh
    private SkinnedMeshRenderer[] skinnedMeshCombatRenderers;
    GameObject[] generatedCombatMeshPool;
    MeshRenderer[] cacheCombatMesh;
    MeshFilter[] cacheCombatFilter;
    Mesh[] Combatmesh;
    [SerializeField] private Material Combatmat;
    private void Awake()
    {
        if (skinnedMeshRenderers == null)
            skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshCombatRenderers == null)
            skinnedMeshCombatRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        // Debug.Log("L" + skinnedMeshRenderers.Length);
        PopulatePool();
        PopulateCombatPool();
    }

    

    private void Start()
    {
        if (skinnedMeshRenderers == null)
            skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshCombatRenderers == null)
            skinnedMeshCombatRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        PlayerDashingState.OnActivateMeshTrail += PlayerDashingState_OnActivateMeshTrail;
        PlayerBaseState.onMimicingCommands += PlayerBaseState_onMimicingCommands;

    }
    private void OnEnable()
    {
        PlayerCombatTimers.onCleanupDash += PlayerStateMachine_onCleanupDash;
     
    }

   

    private void PlayerStateMachine_onCleanupDash()
    {
      DeSpawnMeshesPostTimer();
    }

    private void PlayerBaseState_onMimicingCommands()
    {
        StartCoroutine(ActivateCombatTrail(activeTime));
    }

    private void PlayerDashingState_OnActivateMeshTrail(bool obj)
    {
        if(obj == true)
        {
            currentState = true;
        }
        else
        {
            currentState = false;
        }
    }
    private void Update()
    {
        ActivateMeshTrail();
    }

    public void ActivateMeshTrail()
    {
        if (currentState == true && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
        }
        
    }

    IEnumerator ActivateTrail(float activeTime)
    {
        while(activeTime > 0)
        {
            activeTime -= meshRefreshRate;

            SpawnMeshes();
            // Placing despawn outside the while loop will cause the meshes to still be active at the very end leaving a last known position, 
            // This idea can be used for stealth detection as well as a feature where you Bloop back to your last spot
            
            DeSpawnMeshes();
            yield return new WaitForSeconds(meshRefreshRate);
            RecordLastPosition();
        }
        isLastPositionStable = true;
       
        isTrailActive = false;
    }


    private void RecordLastPosition()
    {
        if (generatedMeshPool.Length > 0)
        {
            lastPosition = generatedMeshPool[generatedMeshPool.Length - 1].transform; // get the last known place mesh was spawned 
        }
    }


    public void DeSpawnMeshesPostTimer()
    {
        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
                generatedMeshPool[i].SetActive(false);
        }
    }
    private void DeSpawnMeshes()
    {
        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            float Timer = meshDestroyDelay;
            Timer -= Time.deltaTime;
            if(Timer <= 0)
            {
                generatedMeshPool[i].SetActive(false);
                
            }       
        }
        onAllowRewind?.Invoke();
    }

    private void SpawnMeshes()
    {
        // adding a small offset will make spawning look a bit smoother
        // since i'm no long creating, baking and destroying in real time its much faster to spawn in objects. 
        Vector3 spawnOffset;
        spawnOffset = positionToSpawn.position;
        spawnOffset.x -= 0.1f;
        spawnOffset.z -= 0.1f;
        for (int i = 0;i < skinnedMeshRenderers.Length; i++)
        {
            generatedMeshPool[i].transform.SetPositionAndRotation(spawnOffset, positionToSpawn.rotation);
            generatedMeshPool[i].SetActive(true);

            // using built in function in skinned mesh renderer to bake a new mesh from skinned mesh renderer ( takes a snapshot of mesh at that moment). 
            skinnedMeshRenderers[i].BakeMesh(mesh[i]);

            cacheFilter[i].mesh = mesh[i];
            cacheMesh[i].material = mat;
        }
    }

    private void OnDisable()
    {
        PlayerCombatTimers.onCleanupDash -= PlayerStateMachine_onCleanupDash;
        PlayerBaseState.onMimicingCommands -= PlayerBaseState_onMimicingCommands;
    }
    private void OnDestroy()
    {
        PlayerDashingState.OnActivateMeshTrail -= PlayerDashingState_OnActivateMeshTrail;
    }

    private void PopulatePool()
    {
        
        
        generatedMeshPool = new GameObject[skinnedMeshRenderers.Length];
        mesh = new Mesh[skinnedMeshRenderers.Length];
        cacheMesh = new MeshRenderer[skinnedMeshRenderers.Length];
        cacheFilter = new MeshFilter[skinnedMeshRenderers.Length];

        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {

            //Create Objects

            generatedMeshPool[i] = new GameObject();
            //meshComponents.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

            // Cache mesh components
             cacheMesh[i] = generatedMeshPool[i].AddComponent<MeshRenderer>();
             cacheFilter[i] = generatedMeshPool[i].AddComponent<MeshFilter>();
             mesh[i] = new Mesh();


            


            //Deactivate objects, they will each be turned on during coroutine then turned off. 

            generatedMeshPool[i].SetActive(false);
        }
        //
       // isTrailActive = false;
    }

    public Vector3 GetLastPosition() => lastPosition.position;

    public bool GetStatusOfLastPosition() => isLastPositionStable;

    public void SetStatusOfLastPosition(bool IsLastPositionStable) { isLastPositionStable = IsLastPositionStable; }



    //Combat mimic functions (basically the same thing)
    IEnumerator ActivateCombatTrail(float activeTime)
    {
        while (activeTime > 0)
        {
            activeTime -= 0.1f;

            SpawnCombatMeshs();
            // Placing despawn outside the while loop will cause the meshes to still be active at the very end leaving a last known position, 
            // This idea can be used for stealth detection as well as a feature where you Bloop back to your last spot


            yield return new WaitForSeconds(0.22f);
            RecordLastCombatPosition();
            
        }
        DeSpawnMeshesPostCombatTimer();
        DespawnCombatMeshes();
        // isLastPositionStable = true;
        //  isTrailActive = false;
    }
    private void RecordLastCombatPosition()
    {
        if (generatedCombatMeshPool.Length > 0)
        {
            lastPosition = generatedCombatMeshPool[generatedCombatMeshPool.Length - 1].transform; // get the last known place mesh was spawned 
        }
    }
    private void SpawnCombatMeshs()
    {
        Vector3 spawnOffset;
        spawnOffset = positionToSpawn.position;
        spawnOffset.x -= 0.1f;
        spawnOffset.z -= 0.1f;
        for (int i = 0; i < skinnedMeshCombatRenderers.Length; i++)
        {
            generatedCombatMeshPool[i].transform.SetPositionAndRotation(spawnOffset, positionToSpawn.rotation);
            generatedCombatMeshPool[i].SetActive(true);

            // using built in function in skinned mesh renderer to bake a new mesh from skinned mesh renderer ( takes a snapshot of mesh at that moment). 
            skinnedMeshCombatRenderers[i].BakeMesh(Combatmesh[i]);

            cacheCombatFilter[i].mesh = Combatmesh[i];
            cacheCombatMesh[i].material = Combatmat;
        }
    }

    private void DespawnCombatMeshes()
    {
        for (int i = 0; i < skinnedMeshCombatRenderers.Length; i++)
        {
            float Timer = 0.2f;
            Timer -= Time.deltaTime;
            if (Timer <= 0)
            {
                generatedCombatMeshPool[i].SetActive(false);

            }
        }
    }

    private void PopulateCombatPool()
    {
        generatedCombatMeshPool = new GameObject[skinnedMeshCombatRenderers.Length];
        Combatmesh = new Mesh[skinnedMeshCombatRenderers.Length];
        cacheCombatMesh = new MeshRenderer[skinnedMeshCombatRenderers.Length];
        cacheCombatFilter = new MeshFilter[skinnedMeshCombatRenderers.Length];

        for (int i = 0; i < skinnedMeshCombatRenderers.Length; i++)
        {

            //Create Objects

            generatedCombatMeshPool[i] = new GameObject();
            //meshComponents.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

            // Cache mesh components
            cacheCombatMesh[i] = generatedCombatMeshPool[i].AddComponent<MeshRenderer>();
            cacheCombatFilter[i] = generatedCombatMeshPool[i].AddComponent<MeshFilter>();
            Combatmesh[i] = new Mesh();





            //Deactivate objects, they will each be turned on during coroutine then turned off. 

            generatedCombatMeshPool[i].SetActive(false);
        }
    }
    public void DeSpawnMeshesPostCombatTimer()
    {
        for (int i = 0; i < skinnedMeshCombatRenderers.Length; i++)
        {
            generatedCombatMeshPool[i].SetActive(false);
        }
    }
}
