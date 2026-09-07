using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public GameObject interactOutline;
   

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    [SerializeField] private PlayerMovement player;
    [SerializeField] private InteractionDetector id;

    [SerializeField] private PauseMenu menu;
    public bool canInteract()
    {
        return !isDialogueActive;
    }

    public void interact()
    {
        if (dialogueData == null || PauseMenu.isGamePaused && !isDialogueActive) return;
        if (isDialogueActive) 
        {
            NextLine();
        }
        else
        {
            //startdialogue
            StartDialogue();
        }
    }
    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);
        PlayerMovement.canInput = false;
        
        StartCoroutine(TypeLine());
        //Typeline
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else
        {
            dialogueIndex++;
            if (dialogueIndex < dialogueData.dialogueLines.Length)

            {
                // if another line, type next line
                StartCoroutine(TypeLine());
            }

            else
            {
                EndDialogue();
                //end dialogue
            }
        }
    }
        IEnumerator TypeLine()
        {
            isTyping = true;
            dialogueText.SetText("");

            foreach(char letter in dialogueData.dialogueLines[dialogueIndex])
            {
                dialogueText.text += letter;
                yield return new WaitForSecondsRealtime(dialogueData.typingSpeed);
            }
            isTyping = false;
            if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex]) 
            {
                yield return new WaitForSecondsRealtime(dialogueData.autoProgressDelay);
                //display next line
                NextLine();
            }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        StopAllCoroutines();
        
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        PlayerMovement.canInput = true;
        id.interactableInRange = null;
        id.interactableObject=null;
        id.interactableInRangeDist = 999999.9999f;
    }

    public void outline()
    {
        interactOutline.SetActive(true);
    }
    public void unoutline()
    {
        interactOutline.SetActive(false);
    }
}
