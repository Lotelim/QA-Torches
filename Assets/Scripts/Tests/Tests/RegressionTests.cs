using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RegressionTests
{

    [Test]
    public void LightingTwiceStaysLit()
    {
        var torch = new InteractableTorch();
        torch.Interact();
        torch.Interact();

        Assert.IsTrue(torch.IsLit);
    }

    [Test]
    public void LitTorchRemainsLit()
    {
        var torch = new InteractableTorch();

        torch.Interact();

        Assert.IsTrue(torch.IsLit);
    }
}
