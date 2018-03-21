﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Deck activeDeck;
    private Desk desk;
    private Areas areas;
    public Card activeCard;
    public static int activePlayerNumber;

    public GameObject deckObject;
    public GameObject deskObject;
    public GameObject areasObject;

    public Card activeShowingCard;

    public GameObject playerNameTextObject;
    public Text playerNameText;

    public GameObject player1Object;
    public GameObject player2Object;
    public Player player1;
    public Player player2;

    public GameObject score1Object;
    public GameObject score2Object;
    public Text score1Text;
    public Text score2Text;

    public GameObject buttonObject;
    public Button button;

    private static int state = 0;

    void Awake()
    {
        activeDeck = deckObject.GetComponent<Deck>();
        desk = deskObject.GetComponent<Desk>();
        areas = areasObject.GetComponent<Areas>();

        playerNameText = playerNameTextObject.GetComponent<Text>();
        score1Text = score1Object.GetComponent<Text>();
        score2Text = score2Object.GetComponent<Text>();

        button = buttonObject.GetComponent<Button>();

        player1 = player1Object.GetComponent<Player>();
        player2 = player2Object.GetComponent<Player>();

        activePlayerNumber = (int)PlayerNumber.PLAYER1;
    }    

    void Start()
    {
        player1.getDeck().buildDeck(11);
        player2.getDeck().buildDeck(11);
        player2.setDeckVisibility(false);
        activeDeck = player1.getDeck();
        
        if(player1.getDeck().cardsInDeck.Count > 0)
            activeCard = player1.getDeck().cardsInDeck[0];
        activeShowingCard = Instantiate(activeCard) as Card;
        activeShowingCard.transform.position = new Vector3(8.96f, 0, -0.1f);
        showActiveCard(false);
    }

    private enum Status{
        FREE,
        ACTIVE_CARD,
        BLOCKED
    };

    void Update()
    {
        // vector of actual mouse position
        Vector3 mouseRelativePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseRelativePosition.z = -0.1f;
        if (Input.GetMouseButtonDown(0) && state != ((int)Status.BLOCKED))
        {
            // if we click on deck collision
            if (areas.getDeckColliderBounds().Contains(mouseRelativePosition) && activeDeck.cardsInDeck.Count > 0)
            {
                foreach (Card c in activeDeck.getCards())
                {
                    // if we click on card
                    if (c.getBounds().Contains(mouseRelativePosition))
                    {
                        activeDeck.disactiveAllInDeck();
                        activeCard = c;
                        c.setActive(true);

                        showActiveCard(true);

                        activeCard.transform.position += new Vector3(0, 0.15f, 0);
                        state = (int)Status.ACTIVE_CARD;
                    }
                }
            }
            // click on card sword group
            else if (areas.getSwordColliderBounds().Contains(mouseRelativePosition))
            {
                if (state == (int)Status.ACTIVE_CARD)
                {
                    // TODO - system rozmieszczania kart w grupie
                    activeCard.setActive(false);
                    if (activeDeck.addCardToSwords(activeCard) == true)
                    {
                        activeDeck.disactiveAllInDeck();
                        state = (int)Status.FREE;
                        switchPlayer();
                    }
                }
            }
            else if (areas.getBowColliderBounds().Contains(mouseRelativePosition))
            {
                if (state == (int)Status.ACTIVE_CARD)
                {
                    // TODO - system rozmieszczania kart w grupie
                    activeCard.setActive(false);
                    if (activeDeck.addCardToBows(activeCard) == true)
                    {
                        activeDeck.disactiveAllInDeck();
                        state = (int)Status.FREE;
                        switchPlayer();
                    }
                }
            }
            else if (areas.getTrebuchetColliderBounds().Contains(mouseRelativePosition))
            {
                if (state == (int)Status.ACTIVE_CARD)
                {
                    // TODO - system rozmieszczania kart w grupie
                    activeCard.setActive(false);
                    // TODO - is it enough to have controll under card in list? Position controll
                    if (activeDeck.addCardToTrebuchets(activeCard) == true)
                    {
                        activeDeck.disactiveAllInDeck();
                        state = (int)Status.FREE;
                        switchPlayer();
                    }
                }
            }
            else
            {
                activeDeck.disactiveAllInDeck();
                activeCard = null;
                showActiveCard(false);
                state = (int)Status.FREE;
            }
        }
    }

    /// <summary>
    /// Resolve new posiotion of each card in pointet group
    /// </summary>
    /// <param name="group">group of cards that we want to reorganize. Int value</param>
    public void reorganizeGroup(int group)
    {
        if (group == (int)CardGroup.DECK)
        {
        }
        else if (group == (int)CardGroup.SWORD)
        {
        }
        else if (group == (int)CardGroup.BOW)
        {

        }
        else if (group == (int)CardGroup.TREBUCHET)
        {

        }
    }

    /// <summary>
    /// Defined type of card groups
    /// </summary>
    private enum CardGroup { DECK, SWORD, BOW, TREBUCHET };

    /// <summary>
    /// Defined name of players
    /// </summary>
    private enum PlayerNumber { PLAYER1 = 1, PLAYER2 };

    /// <summary>
    /// Switch player - update active deck
    /// </summary>
    private void switchPlayer()
    {
        state = (int)Status.BLOCKED;
        showActiveCard(false);
        StartCoroutine(Wait(1));
    }

    IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
        
        button.transform.position = new Vector3(0, 0, -1f);

        if (activePlayerNumber == (int)PlayerNumber.PLAYER1)
        {
            this.activeDeck = player2.getDeck();
            player1.setDeckVisibility(false);
            player2.setDeckVisibility(true);
            activePlayerNumber = (int)PlayerNumber.PLAYER2;
        }
        else if (activePlayerNumber == (int)PlayerNumber.PLAYER2)
        {
            this.activeDeck = player1.getDeck();
            player1.setDeckVisibility(true);
            player2.setDeckVisibility(false);
            activePlayerNumber = (int)PlayerNumber.PLAYER1;
        }

        button.GetComponentInChildren<Text>().text = "Player " + activePlayerNumber + ",\nDotknij aby kontynuować";
        playerNameText.text = "Player " + activePlayerNumber.ToString();
        player1.getDeck().flipGroupCards();
        player2.getDeck().flipGroupCards();
        Vector3 tempVector = score1Text.transform.position;
        score1Text.transform.position = score2Text.transform.position;
        score2Text.transform.position = tempVector;

        state = (int)Status.FREE;
    }

    private void showActiveCard(bool ifShow)
    {
        // TODO - usunąć operację modulo!
        if(ifShow)
            activeShowingCard.setBigFront(activeCard.getIndex() % 2 + 1);
        else
            activeShowingCard.setBigFront(0);
    }
}