using System;
using UnityEngine;

public class CharacterModel
{
    private CharacterArmorManager _armorManager;
    private CharacterDefinition _definition;
    private int _health;
    public int Health
    {
        get
        {
            return _health;
        }
        private set
        {
            if (_health != value)
            {
                _health = value;
                HealthChanged?.Invoke(NormalizedHealth, Health);
            }
        }
    }

    public Team Team { get; private set; }
    public bool IsAlive => _health > 0;
    public float NormalizedHealth => _health / (float)_definition.MaxHealth;

    public event Action<float, int> HealthChanged;
    public event Action Died;
    public event Action<ArmorDefinition> Blocked;
    public event Action Hurt;
    public event Action Healed;

    public CharacterModel(CharacterDefinition characterDefinition, Team team, CharacterArmorManager armorManager)
    {
        Team = team;
        _definition = characterDefinition;
        _armorManager = armorManager;
    }


    public void Damage(int value)
    {
        if (_armorManager.IsProtected)
        {
            var armor = _armorManager.BlockAttack();
            Blocked?.Invoke(armor);
        }
        else
        {
            Hurt?.Invoke();
            Health = Mathf.Max(0, Health - value);
            if (!IsAlive)
            {
                Die();
            }
        }
    }

    public void Heal(int value)
    {
        Health = Mathf.Min(Health + value, _definition.MaxHealth);
        Healed?.Invoke();
    }

    public void Kill()
    {
        Damage(Health);
    }

    private void Die()
    {
        Died?.Invoke();
    }

}
