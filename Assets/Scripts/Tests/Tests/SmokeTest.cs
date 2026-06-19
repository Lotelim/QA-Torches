using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SmokeTest
{
    [Test]
    public void CanTorchCreate()
    {
        InteractableTorch torch = new GameObject().AddComponent<InteractableTorch>();
        Assert.NotNull(torch);
    }

    [Test]
    public void TorchStartsUnlit()
    {
        InteractableTorch torch = new InteractableTorch();
        Assert.IsFalse(torch.IsLit);
    }

    [Test]
    public void TorchCanBeLit()
    {
        InteractableTorch torch = new InteractableTorch();
        torch.Interact();
        Assert.IsTrue(torch.IsLit);
    }
}
