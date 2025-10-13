using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    GoTo,
    PickUp,
    DropAt
}

[System.Serializable]
public class BotActionStep
{
    public ActionType actionType;
    public Transform targetPosition;
}

public class LevelController2 : MonoBehaviour
{
    public static LevelController2 Instance;
    public BotController bot;
    public List<BotActionStep> steps = new List<BotActionStep>();

    private bool isRunning = false;

    public GameObject panelWin;

    public void StartSequence()
    {
        if (!isRunning)
            StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        isRunning = true;

        foreach (var step in steps)
        {
            switch (step.actionType)
            {
                case ActionType.GoTo:
                    step.targetPosition = bot.target;
                    yield return bot.isStop = false;
                    //yield return StartCoroutine(bot.MoveTo(step.targetPosition.position));
                    //yield return StartCoroutine(bot.FollowTarget(step.targetPosition));
                    break;

                case ActionType.PickUp:
                    yield return StartCoroutine(bot.PickUp(step.targetPosition.position));
                    break;

                case ActionType.DropAt:
                    yield return StartCoroutine(bot.DropAt(step.targetPosition.position));
                    break;
            }

            yield return new WaitForSeconds(0.2f); // nghỉ nhẹ giữa các bước
        }

        Debug.Log("🤖 Bot: All actions completed!");
        isRunning = false;
    }
}
