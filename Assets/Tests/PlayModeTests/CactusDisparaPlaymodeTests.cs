using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CactusDisparaPlaymodeTests
{
    private GameObject poolGO;
    private GameObject cactusGO;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Instancia el pool primero (para que Cactus pueda disparar)
        var poolPrefab = Resources.Load<GameObject>("BulletPool");
        Assert.NotNull(poolPrefab, "No encontré el prefab 'BulletPool' en Resources.");
        poolGO = Object.Instantiate(poolPrefab);

        // Instancia el cactus real
        var cactusPrefab = Resources.Load<GameObject>("Cactus");
        Assert.NotNull(cactusPrefab, "No encontré el prefab 'Cactus' en Resources.");
        cactusGO = Object.Instantiate(cactusPrefab);

        yield return null; // deja correr Awake/Start
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (cactusGO) Object.Destroy(cactusGO);
        if (poolGO) Object.Destroy(poolGO);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Start_DisparaUnaBola()
    {
        // Tras un frame, debería existir al menos una bola activa
        yield return null;

        var todas = Object.FindObjectsOfType<GameObject>(false);
        bool hayBolaActiva = false;
        foreach (var go in todas)
        {
            if (go.GetComponent<BolaPinchos>()  && go.activeInHierarchy)
            { hayBolaActiva = true; break; }
        }
        Assert.IsTrue(hayBolaActiva, "El Cactus debería disparar al iniciar.");
    }
}
