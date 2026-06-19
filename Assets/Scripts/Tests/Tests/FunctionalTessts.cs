using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FunctionalTessts
{
    [Test]
    public void CheckIfTorchLitWhenFar()
    {
        PlayerManager player = new GameObject().AddComponent<PlayerManager>();
        InteractableTorch torch = new InteractableTorch();
        torch.Interact();
        player.transform.position = new Vector3(100, 100, 100);
        Assert.IsTrue(torch.IsLit);
    }
}
