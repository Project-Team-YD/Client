using HSMLibrary.Generics;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobSystemManager : Singleton<JobSystemManager>
{

    //NativeArray
    public void Emptys()
    {
        JobMoveController jobMoveController = new JobMoveController();
        jobMoveController.SetNativeArray = new NativeArray<Vector3>(1, Allocator.TempJob);

        //JobHandle handle = jobMoveController.Schedule();
    }
}