using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    [UnityTest]
    public IEnumerator CheckIfTorchIsLitWhenUnlit()
    {
        InteractableTorch torch = new GameObject().AddComponent<InteractableTorch>();
        Assert.IsTrue(torch.IsLit);
        yield return null;
    }

    [UnityTest]
    public IEnumerator CheckIfTorchIsLitWhenLit()
    {
        InteractableTorch torch = new GameObject().AddComponent<InteractableTorch>();
        torch.LightUp();
        Assert.IsTrue(torch.IsLit);
        yield return null;
    }
}
