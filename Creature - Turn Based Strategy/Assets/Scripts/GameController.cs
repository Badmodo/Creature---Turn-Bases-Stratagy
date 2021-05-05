using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//this state controller designates what happens in what state freeroam or while in battle
public enum GameState { Freeroam, Battle, Dialogue }
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController360 playerController360;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera freeroamCam;

    [SerializeField] GameObject player;

    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject unlockedPlanetScreen;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClipMenuChangeSelection;
    [SerializeField] AudioClip audioClipMenuBack;

    static GameState state;
    public static GameController Instance;
    public static GameState State => state;

    private void Awake()
    {
        Instance = this;
        ConditionsDB.Initilize();
    }

    private void Start()
    {
        //playerController3D.onEncounter += StartBattle;
        playerController360.onEncounter += StartBattle;
        battleSystem.BattleOver += EndBattle;

        DialogueManager.Instance.OnShowDialogue += () =>
        {
            state = GameState.Dialogue;
        };
        
        DialogueManager.Instance.OnCloseDialogue += () =>
        {
            if (state == GameState.Dialogue)
            {
                state = GameState.Freeroam;
            }
        };

        pauseScreen.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseScreen.activeInHierarchy)
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                pauseScreen.SetActive(true);
            }
        }

        if (state == GameState.Freeroam)
        {
            playerController360.Update();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialogue)
        {
            DialogueManager.Instance.HandleUpdate();
        }
    }

    //start battle state
    void StartBattle()
    {
        if (state != GameState.Battle) //Preventing from entering a battle when already in one. 
        {
            player.SetActive(false);

            state = GameState.Battle;
            battleSystem.gameObject.SetActive(true);
            freeroamCam.gameObject.SetActive(false);

            //used to return the player creatures
            //var playerTeam = playerController3D.GetComponent<CreatureTeam>();
            var playerTeam = playerController360.GetComponent<CreatureTeam>();
            var wildCreature = FindObjectOfType<ListOfCreaturesInArea>().GetComponent<ListOfCreaturesInArea>().GetRandomWildCreatures();

            //bug that the captured creature would not show up because it was in your team, in the grass... Odd
            var wildCreatureCopy = new Creature(wildCreature.Base, wildCreature.Level);
            battleSystem.StartBattle(playerTeam, wildCreatureCopy);
        }
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        if (state != GameState.Battle) //Preventing from entering a battle when already in one. 
        {
            //Player.SetActive(false);

            state = GameState.Battle;
            battleSystem.gameObject.SetActive(true);
            freeroamCam.gameObject.SetActive(false);

            //used to return the player creatures
            //var playerTeam = playerController3D.GetComponent<CreatureTeam>();
            var playerTeam = playerController360.GetComponent<CreatureTeam>();
            var trainerTeam = trainer.GetComponent<CreatureTeam>();

            battleSystem.StartTrainerBattle(playerTeam, trainerTeam);
        }
    }


    //start freeroam state
    void EndBattle(bool won)
    {
        player.SetActive(true);

        state = GameState.Freeroam;
        battleSystem.gameObject.SetActive(false);
        freeroamCam.gameObject.SetActive(true);

        if (unlockedPlanetScreen.activeInHierarchy)
        {
            StartCoroutine(UnlockedPlanetPopUp());
        }
    }

    private IEnumerator UnlockedPlanetPopUp()
    {
        yield return new WaitForSeconds(3);

        unlockedPlanetScreen.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseScreen.SetActive(false);
    }

    public void SaveGame()
    {
        SaveSystem.SavePlayer(playerController360);
    }

    public void QuitGame()
    {
        SaveSystem.SavePlayer(playerController360);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void MouseOverTeleportLocations()
    {
        audioSource.PlayOneShot(audioClipMenuChangeSelection);
    }
    public void MouseOverTeleportLocationsBack()
    {
        audioSource.PlayOneShot(audioClipMenuBack);
    }
}
