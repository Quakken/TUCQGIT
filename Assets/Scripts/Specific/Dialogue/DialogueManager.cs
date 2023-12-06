/*
 * Name: Quinn Farrell
 * Date: 12/5/2023
 * Desc: A class that manages dialogue between two or more characters. Allows full customization of dialogue scene, changing stuff like character portraits
 * and such on the fly.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public enum PortraitSides { LEFT, RIGHT };

    [System.Serializable]
    public struct DialogueFrame
    {
        [Header("Dialogue Info")]
        [Tooltip("The text to display")]
        public string text;
        [Header("Character Info")]
        [Tooltip("The name that will be displayed by the character")]
        public string characterName;
        [Tooltip("The sprite that will be used to represent the character")]
        public Sprite characterSprite;
        [Tooltip("The side of the dialogue box that the character will be displayed on")]
        public PortraitSides characterSide;
    }

    [Header("Dialogue Options")]
    [Tooltip("Delay between each letter of the dialogue being displayed.")]
    [SerializeField] float scrollSpeed = 0.1f;
    [Tooltip("The current dialogue frame")]
    [SerializeField] int currentFrame = 0;
    [Tooltip("All of the frames that make up the dialogue")]
    [SerializeField] DialogueFrame[] dialogueFrames;

    int currentChar = 0; // The current character of the text that is being revealed
    bool revealingText; // Whether or not text is currently being revealed
    bool textRevealed;

    [Header("Assets")]
    [Space]
    [Tooltip("The text box that will display the current speaker's name")]
    [SerializeField] TextMeshProUGUI charNameBox;
    [Tooltip("The text box that will contain the actual dialogue text")]
    [SerializeField] TextMeshProUGUI dialogueBox;
    [SerializeField] Image charSpriteLeft;
    [SerializeField] Image charSpriteRight;

    [Header("Events")]
    [Tooltip("Called when the text of a dialogue frame has been fully displayed")]
    [SerializeField] UnityEvent onTextDisplayed;
    [Tooltip("Called when the dialogue frame is advanced")]
    [SerializeField] UnityEvent onDialogueAdvance;
    [Tooltip("Called when the dialogue has been completed")]
    [SerializeField] UnityEvent onDialogueComplete;


    /*--------------------Unity Functions--------------------*/

    private void Start()
    {
        PlayDialogue(currentFrame);
    }

    private void Update()
    {
        // When the player presses any key
        if (Input.anyKeyDown)
        {
            if (revealingText)
            {
                // Stop revealing text
                revealingText = false;

                // Set the dialogue box text to the full thing
                dialogueBox.text = dialogueFrames[currentFrame].text;
            }
            if (textRevealed)
            {
                // Advance frame
                AdvanceFrame();
            }
        }
    }

    /*--------------------Dialogue Management Functions--------------------*/

    // Plays out a specific dialogue frame
    public void PlayDialogue(int frame)
    {
        // Change speaker name
        charNameBox.text = dialogueFrames[currentFrame].characterName;

        // Clear dialogue box
        dialogueBox.text = "";
        currentChar = 0;

        // Change & focus their portrait
        if (dialogueFrames[currentFrame].characterSide == PortraitSides.LEFT)
        {
            // Change their portrait
            charSpriteLeft.sprite = dialogueFrames[currentFrame].characterSprite;

            // Focus their portrait
            charSpriteLeft.GetComponent<Animator>().SetBool("Focused", true);

            // Unfocus other portrait
            charSpriteRight.GetComponent<Animator>().SetBool("Focused", false);
        }
        else
        {
            // Change their portrait
            charSpriteRight.sprite = dialogueFrames[currentFrame].characterSprite;

            // Focus their portrait
            charSpriteRight.GetComponent<Animator>().SetBool("Focused", true);

            // Unfocus other portrait
            charSpriteLeft.GetComponent<Animator>().SetBool("Focused", false);
        }

        // Start revealing dialogue
        revealingText = true;
        StartCoroutine(RevealText(dialogueFrames[currentFrame].text));
    }

    // Displays text in the dialogue box one character at a time until the full dialogue has been displayed
    public IEnumerator RevealText(string textToDisplay)
    {
        // Repeats until the final character has been diplayed
        while (currentChar < textToDisplay.Length && revealingText)
        {
            // Wait for the scroll speed
            yield return new WaitForSeconds(scrollSpeed);

            if (revealingText)
            {
                // Add the current character to the dialogue box text
                dialogueBox.text += textToDisplay[currentChar];
                currentChar++;
            }
        }

        textRevealed = true;
        revealingText = false;
        onTextDisplayed.Invoke();
    }

    /*--------------------Utility Functions--------------------*/

    // Advances the current dialogue frame
    public void AdvanceFrame()
    { 
        if (currentFrame < dialogueFrames.Length)
        {
            currentFrame++;
            PlayDialogue(currentFrame);
            onDialogueAdvance.Invoke();
        }
        else
        {
            onDialogueComplete.Invoke();
        }
    }
}
