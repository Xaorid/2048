using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private TileBoard _board;
    [SerializeField] private CanvasGroup _gameOver;

    void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        _gameOver.alpha = 0;
        _gameOver.interactable = false;
        _board.ClearBoard();
        _board.CreateNewTile();
        _board.CreateNewTile();
        _board.enabled = true;
    }

    public void GameOver()
    {
        _board.enabled = false;
        _gameOver.interactable = true;

        StartCoroutine(Fade(_gameOver, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        yield return new WaitForSeconds(delay);

        var elapsed = 0f;
        var duration = 0.5f;
        var from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}
