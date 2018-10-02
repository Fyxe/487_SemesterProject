using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUpgradePlayer : Interactable
{
    [Header("Settings")]
    public float timeEnterPlayer = 0.25f;
    public float timeFlash = 0.15f;

    [Header("References")]
    public bool isControlled = false;
    public bool isMoving = false;
    public ControllerMultiPlayer playerCurrent;
    public Transform playerPosition;
    [Space]
    public Image imageToChangeToPlayersColor;
    public Image imageCost;
    public TextMeshProUGUI textCost;
    public TextMeshProUGUI textUpgradeName;
    public TextMeshProUGUI textUpgradeLevel;
    public Transform transformCanvas;

    Coroutine coroutineFlash;

    int m_cost = 0;
    public int cost
    {
        get
        {
            return m_cost;
        }
        set
        {
            m_cost = value;
            textCost.text = cost.ToString();
            if (playerCurrent.pointsCurrent >= cost)
            {
                imageCost.color = Color.green;
            }
            else
            {
                imageCost.color = Color.red;
            }
        }
    }

    public int indexJoystick
    {
        get
        {
            if (playerCurrent == null)
            {
                return 0;
            }
            else
            {
                return playerCurrent.indexJoystick;
            }
        }
    }

    bool axisInUseLeft = false;
    bool axisInUseRight = true;

    int currentDisplay = 0;

    void Awake()
    {
        transformCanvas.localScale = Vector3.one;
        DisplayBuy(-1);
    }

    void Update()
    {
        if (LevelManager.instance.isPlaying && isControlled)
        {
            if (Input.GetAxis("J" + indexJoystick.ToString() + "_Axis0Horizontal") < 0)
            {
                if (!axisInUseLeft)
                {
                    axisInUseLeft = true;
                    IterateLeft();
                }
            }
            else
            {
                axisInUseLeft = false;
            }

            if (Input.GetAxis("J" + indexJoystick.ToString() + "_Axis0Horizontal") > 0)
            {
                if (!axisInUseRight)
                {
                    axisInUseRight = true;
                    IterateRight();
                }
            }
            else
            {
                axisInUseRight = false;
            }

            if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 0"))
            {
                BuyCurrent();
            }

            if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 1"))
            {
                Exit();
            }
        }
    }

    public override bool InteractWithPlayer(ControllerMultiPlayer player)
    {
        return Enter(player);
    }

    public bool Enter(ControllerMultiPlayer newPlayerCurrent)
    {
        if (isMoving || isControlled)
        {
            return false;
        }
        playerCurrent = newPlayerCurrent;
        playerCurrent.isInShop = true;
        StartCoroutine(MovePlayer());
        return true;
    }

    IEnumerator MovePlayer()
    {
        isMoving = true;
        Vector3 startPosition = playerCurrent.transform.position;
        Quaternion startRotation = playerCurrent.transform.rotation;

        float currentTime = 0f;
        while(currentTime < timeEnterPlayer)
        {
            if (!LevelManager.instance.isPlaying)
            {
                continue;
            }
            currentTime += Time.deltaTime;
            playerCurrent.transform.SetPositionAndRotation(
                Vector3.Lerp(startPosition, playerPosition.position, currentTime / timeEnterPlayer),
                Quaternion.Lerp(startRotation, playerPosition.rotation, currentTime / timeEnterPlayer));
            yield return null;
        }
        playerCurrent.transform.SetPositionAndRotation(playerPosition.position, playerPosition.rotation);
        isMoving = false;

        imageToChangeToPlayersColor.color = playerCurrent.colorPlayer;
        DisplayBuy(0);
        isControlled = true;
        transformCanvas.localScale = Vector3.one * 2f;
        yield break;
    }

    public void Exit()
    {
        if (coroutineFlash != null)
        {
            StopCoroutine(coroutineFlash);
        }
        playerCurrent.isInShop = false;
        playerCurrent = null;
        isControlled = false;
        transformCanvas.localScale = Vector3.one;
        DisplayBuy(-1);
    }

    void IterateLeft()
    {
        currentDisplay--;
        if (currentDisplay < 0)
        {
            currentDisplay = 2;
        }
        DisplayBuy(currentDisplay);
    }

    void IterateRight()
    {
        currentDisplay++;
        if (currentDisplay > 2)
        {
            currentDisplay = 0;
        }
        DisplayBuy(currentDisplay);
    }

    void DisplayBuy(int whichBuy)
    {
        currentDisplay = whichBuy;
        switch (currentDisplay)
        {
            case -1:
                textUpgradeLevel.text = "";
                textUpgradeName.text = "Upgrade Shop";
                textCost.text = "";
                imageToChangeToPlayersColor.color = Color.white;
                imageCost.color = Color.white;
                break;
            case 0:
                cost = ((int)playerCurrent.speedMoveCurrent + 1) * PointsManager.instance.pointsPerLevelSpeedMove;
                textUpgradeLevel.text = playerCurrent.speedMoveCurrent + "-->" + (playerCurrent.speedMoveCurrent + 1);
                textUpgradeName.text = "Upgrade Speed";
                break;
            case 1:
                cost = ((int)playerCurrent.damageBaseCurrent + 1) * PointsManager.instance.pointsPerLevelBaseDamage;
                textUpgradeLevel.text = playerCurrent.damageBaseCurrent + "-->" + (playerCurrent.damageBaseCurrent + 1);
                textUpgradeName.text = "Upgrade Damage";
                break;
            case 2:
                cost = ((int)playerCurrent.hpMax + 1) * PointsManager.instance.pointsPerLevelMaxHP;
                textUpgradeLevel.text = playerCurrent.hpMax + "-->" + (playerCurrent.hpMax + 1);
                textUpgradeName.text = "Upgrade Max Health";
                break;
            default:
                break;
        }
    }

    void BuyCurrent()
    {
        bool canBuy = playerCurrent.pointsCurrent >= cost;
        if (canBuy)
        {
            playerCurrent.pointsCurrent -= cost;

            switch (currentDisplay)
            {
                case 0:
                    playerCurrent.speedMoveCurrent++;
                    break;
                case 1:
                    playerCurrent.damageBaseCurrent++;
                    break;
                case 2:
                    playerCurrent.hpMax++;
                    break;
            }
        }

        DisplayBuy(currentDisplay);
        if (coroutineFlash != null)
        {
            StopCoroutine(coroutineFlash);
        }
        coroutineFlash = StartCoroutine(Flash(canBuy));
    }

    IEnumerator Flash(bool successful)
    {
        if (successful)
        {
            imageToChangeToPlayersColor.color = Color.green;
        }
        else
        {
            imageToChangeToPlayersColor.color = Color.red;
        }
        yield return new WaitForSeconds(timeFlash);
        imageToChangeToPlayersColor.color = playerCurrent.colorPlayer;
    }
}
