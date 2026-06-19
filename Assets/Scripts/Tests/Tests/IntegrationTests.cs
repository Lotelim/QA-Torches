using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IntegrationTests
{

    [UnityTest]
    public IEnumerator PlayerLightsTorch()
    {
        PlayerManager player = new PlayerManager();
        InteractableTorch torch = new InteractableTorch();

        torch.Interact();

        yield return null;

        Assert.IsTrue(torch.IsLit);
    }
}
