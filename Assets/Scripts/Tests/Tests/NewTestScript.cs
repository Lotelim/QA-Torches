using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{

    [Test]
    public void Torch_Starts_Unlit()
    {
        InteractableTorch torch = new InteractableTorch();
        Assert.IsFalse(torch.IsLit);
    }

    [Test]
    public void DoesLightingWork()
    {
        InteractableTorch torch = new InteractableTorch();
        torch.LightUp();
        Assert.IsTrue(torch.IsLit);
    }

    [Test]
    public void LightingTorchStayLit()
    {
        InteractableTorch torch = new InteractableTorch();
        torch.LightUp();
        torch.LightUp();
        Assert.IsTrue(torch.IsLit);
    }

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
