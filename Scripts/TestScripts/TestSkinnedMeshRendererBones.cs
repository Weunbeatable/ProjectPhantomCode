    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkinnedMeshRendererBones : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRendererPrefab;
    [SerializeField] private SkinnedMeshRenderer oritinalSkinnedMeshRenderer;
    [SerializeField] private Transform rootBone;
    void Start()
    {
        SkinnedMeshRenderer spawnedSkinnedMeshRenderer = Instantiate(skinnedMeshRendererPrefab, transform);
        spawnedSkinnedMeshRenderer.bones = oritinalSkinnedMeshRenderer.bones;
        spawnedSkinnedMeshRenderer.rootBone = rootBone;

        foreach(Transform bone in oritinalSkinnedMeshRenderer.bones)
        {
            Debug.Log(bone);
        }
    }

    
}
