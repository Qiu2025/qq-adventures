using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;

public class RewardTests
{
    private GameObject rewardObject;
    private Reward reward;
    private GameObject player;
    private GameObject scoreUI;
    private TextMeshProUGUI scoreText;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Physics2D.gravity = Vector2.zero;
        GameManager.score = 0;

        // Score UI
        scoreUI = new GameObject("Score");
        toDestroy.Add(scoreUI);
        scoreUI.tag = "Score";
        scoreText = scoreUI.AddComponent<TextMeshProUGUI>();
        scoreText.text = "Secrets: 0/3";

        // Player
        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        var playerCol = player.AddComponent<BoxCollider2D>();
        playerCol.isTrigger = false;
        var rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        player.transform.position = new Vector3(-2f, 0f, 0f);

        // Reward
        rewardObject = new GameObject("Reward");
        toDestroy.Add(rewardObject);
        var rewardCol = rewardObject.AddComponent<BoxCollider2D>();
        rewardCol.isTrigger = true;
        reward = rewardObject.AddComponent<Reward>();
        rewardObject.transform.position = Vector3.zero;

        yield return new WaitForFixedUpdate();
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        foreach (var obj in toDestroy)
        {
            if (obj != null) Object.Destroy(obj);
        }
        toDestroy.Clear();
        yield return null;
    }

    [UnityTest]
    public IEnumerator Reward_increases_score_and_destroys_itself_on_collision()
    {
        var rb = player.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(10f, 0f);
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(1, GameManager.score, "El puntaje debe incrementarse en 1 al recoger el reward");
        Assert.AreEqual("Secrets: 1/3", scoreText.text, "El texto del score debe actualizarse correctamente");
        Assert.IsTrue(rewardObject == null, "El objeto reward debe destruirse tras ser recogido");
    }
}
