using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using UnityEditor.Build;
using UnityEngine;
using Wolfram.NETLink;

public class WolframKernel : MonoBehaviour
{
    public string KernelPath = "";
    private static WolframKernel instance;
    public static WolframKernel Instance
    {
        get { return instance; }
    }
    private IKernelLink kernelLink;
    public static IKernelLink GetKernelLink()
    {
        return instance.kernelLink;
    }
    public IKernelLink KernelLink
    {
        get { return kernelLink; }
    }
    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        Debug.Log("Launch Wolfram Kernel.");
        if (KernelPath == "")
            kernelLink = MathLinkFactory.CreateKernelLink();
        else
        {
            kernelLink = MathLinkFactory.CreateKernelLink(new string[]{ "-linkmode", "launch", "-linkname", KernelPath + " -wstp"});
        }
        if (kernelLink != null)
        {
            kernelLink.WaitAndDiscardAnswer();
            if (!TestKernel())
                goto Fail;
            else
                goto Success;
        }

    Fail:
        Debug.LogError("Fail to launch Wolfam Kernel.");
        return;
    Success:
        DontDestroyOnLoad(gameObject);
        return;
    }
    private bool TestKernel()
    {
        kernelLink.Evaluate("1+2");
        kernelLink.WaitForAnswer();
        if (3 == kernelLink.GetInteger())
            return true;
        else
            return false;
    }
    private void OnDestroy()
    {
        Debug.Log("Close Wolfram Kernel.");
        kernelLink.Close();
    }
}
