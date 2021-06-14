using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void Test1()
    {
        GameObject gameObject = new GameObject("Proiectil");

        Assert.Throws<MissingComponentException>(
            () => gameObject.GetComponent< Rigidbody >().velocity = Vector3.zero
        );
    }

    [Test]
    public void Test2()
    {
        //Assert.AreEqual(GameService.Origin = new Vector3(0f,0f,0f));
        GameObject g = new GameObject();
        g.name = "tralala";
        g.AddComponent<Rigidbody>();

        Assert.AreEqual(g.GetComponent< Rigidbody >().velocity, Vector3.zero);
    }

    [Test]
    public void Test3()
    {
        //Assert.AreEqual(GameService.Origin = new Vector3(0f,0f,0f));
        GameObject g = new GameObject();
        g.name = "tralala";
        g.AddComponent<Rigidbody>();

        Assert.AreEqual(g.GetComponent< Rigidbody >().velocity, Vector3.zero);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
