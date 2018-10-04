using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour
{


    [Header("References")]
    public Transform positionDrop;
    public AI attachedAI;
    public ControllerMultiPlayer attachedPlayer;
    [SerializeField] Ability m_abilityCurrent;
    public Ability abilityCurrent
    {
        get
        {
            return m_abilityCurrent;
        }
        set
        {
            m_abilityCurrent = value;
            if (controllerType == ControllerType.player)
            {
                if (m_abilityCurrent == null)
                {
                    attachedPlayer.attributes.abilityCurrentID = -1;
                }
                else
                {
                    attachedPlayer.attributes.abilityCurrentID = m_abilityCurrent.abilityID;
                }
            }
        }
    }

    [SerializeField] Transform m_positionAbility;
    public Transform positionAbility
    {
        get
        {
            if (controllerType == ControllerType.player)
            {
                return attachedPlayer.positionAbility;
            }
            else
            {
                return m_positionAbility;
            }
        }
    }
    [SerializeField] Transform m_positionThrow;
    public Transform positionThrow
    {
        get
        {
            if (controllerType == ControllerType.player)
            {
                return attachedPlayer.positionThrow;
            }
            else
            {
                return m_positionThrow;
            }
        }
    }
    [SerializeField] float m_forceThrow = 500f;
    public float forceThrow
    {
        get
        {
            if (controllerType == ControllerType.player)
            {
                return PlayerManager.instance.forceThrow;
            }
            else
            {
                return m_forceThrow;
            }
        }
    }

    public ControllerType controllerType = ControllerType.NONE;

    [Header("Prefabs")]
    [SerializeField] Ability m_prefabBaseAbility;
    public Ability prefabBaseAbility
    {
        get
        {
            if (controllerType == ControllerType.player)
            {
                return PlayerManager.instance.prefabBaseAbility;
            }
            else
            {
                return m_prefabBaseAbility;
            }
        }
    }

    public bool isHoldingBaseAbility
    {
        get
        {
            return abilityCurrent != null && abilityCurrent.abilityID == prefabBaseAbility.abilityID;
        }
    }

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        attachedAI = GetComponent<AI>();
        attachedPlayer = GetComponent<ControllerMultiPlayer>();

        if (attachedAI != null)
        {
            controllerType = ControllerType.ai;
        }
        if (attachedPlayer != null)
        {
            controllerType = ControllerType.player;
        }
    }

    public void Setup()
    {
        Initialize();
        if (controllerType == ControllerType.player)
        {
            Ability spawnCurrent = ProgressionManager.instance.GetAbilityByID(attachedPlayer.attributes.abilityCurrentID, true);
            if (spawnCurrent == null)
            {
                SetCurrentAbility(prefabBaseAbility, true);
            }
            else
            {
                SetCurrentAbility(spawnCurrent, true);
            }
        }
        else
        {
            SetCurrentAbility(prefabBaseAbility, true);            
        }
    }

    public bool AttemptAttack()
    {
        if (abilityCurrent == null)
        {
            return false;
        }
        else
        {
            return abilityCurrent.AttemptAttack();
        }
    }

    public bool AttemptAttackAlternate()
    {
        if (abilityCurrent == null)
        {
            return false;
        }
        else
        {
            return abilityCurrent.AttemptAttackAlternate();
        }
    }

    public void PickupAbility(Ability toPickup, bool isPrefab = false)
    {
        if (DropAbility())
        {
            SetCurrentAbility(toPickup, isPrefab);
        }
    }

    public bool DropAbility()
    {
        if (abilityCurrent.isInUse)
        {
            return false;
        }
        else
        {
            abilityCurrent.OnDrop();
            abilityCurrent.transform.SetParent(null);
            abilityCurrent.transform.position = positionThrow.position;
            abilityCurrent.rb.AddForce(positionThrow.forward * forceThrow);
            abilityCurrent.rb.AddTorque(Random.rotation.eulerAngles * forceThrow);
            return true;
        }
    }

    void SetCurrentAbility(Ability toSet, bool isPrefab = false)
    {
        if (toSet == null)
        {
            Debug.LogError("Null ability could not be set.");
            return;
        }

        if (isPrefab)
        {
            GameObject spawnedAbilityObject = Instantiate(toSet.gameObject);
            Ability spawnedAbility = spawnedAbilityObject.GetComponent<Ability>();
            abilityCurrent = spawnedAbility;
        }
        else
        {
            abilityCurrent = toSet;
        }

        abilityCurrent.transform.SetParent(positionAbility);
        abilityCurrent.transform.Reset();
        abilityCurrent.OnEquip(this);
    }

    public bool AddForceToAttachedEntity(Vector3 forceToAdd, ForceMode mode = ForceMode.Force)
    {
        switch (controllerType)
        {
            case ControllerType.ai:
                if (attachedAI.rb.isKinematic)
                {
                    return false;
                }
                else
                {
                    attachedAI.rb.AddForce(forceToAdd, mode);
                    return true;
                }
            case ControllerType.player:
                if (attachedPlayer.rb.isKinematic)
                {
                    return false;
                }
                else
                {
                    attachedPlayer.rb.AddForce(forceToAdd, mode);
                    return true;
                }
        }
        return false;
    }
}
